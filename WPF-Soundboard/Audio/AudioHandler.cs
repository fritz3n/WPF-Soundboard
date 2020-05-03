using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Soundboard;
using WPF_Soundboard.Audio.Conversion;
using WPF_Soundboard.Audio.Outputs;
using WPF_Soundboard.Clips;

namespace WPF_Soundboard.Audio
{
    static class AudioHandler
    {
        static List<ClipPlayer> clipPlayers = new List<ClipPlayer>();
        static MixingSampleProvider audioMixer;
        static MixingSampleProvider clipMixer;
        static List<WasapiProvider> providers = new List<WasapiProvider>();

        private static VolumeSampleProvider OverallVolumeProvider;
        private static VolumeSampleProvider ClipVolumeProvider;

        static bool Initialized = false;
        private static IOutput output;
        private static AudioConfig config = Serializer.GetAudioConfig();
        private static WaveFormat mixerFormat;

        public static AudioConfig Config { get => config; set { config = value; Serializer.SaveAudioConfig(value); Initialize(); } }
        public static float ClipVolume { get => ClipVolumeProvider?.Volume ?? 1; set { if (ClipVolumeProvider != null) ClipVolumeProvider.Volume = value; } }

        public static void Initialize()
        {
            if (Initialized)
                Dispose();

            if (config.OutType == AudioConfig.OutputType.WASAPI)
                output = new WasapiOutput(config.OutputParameters);
            else
                output = new AsioOutput(config.OutputParameters);

            mixerFormat = output.WaveFormat;

            bool differentMixerFormat = false;

            if (mixerFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                differentMixerFormat = true;
                mixerFormat = WaveFormat.CreateIeeeFloatWaveFormat(mixerFormat.SampleRate, mixerFormat.Channels);
            }


            clipMixer = new MixingSampleProvider(mixerFormat);
            clipMixer.MixerInputEnded += ClipMixer_MixerInputEnded;
            ClipVolumeProvider = new VolumeSampleProvider(clipMixer)
            {
                Volume = 1
            };


            audioMixer = new MixingSampleProvider(mixerFormat);
            audioMixer.MixerInputEnded += AudioMixer_MixerInputEnded;
            audioMixer.AddMixerInput(new SmoothCutoffSampleProvider(ClipVolumeProvider));


            foreach (string id in config.WasapiIns)
            {
                try
                {
                    WasapiProvider input = new WasapiProvider(id);
                    providers.Add(input);
                    audioMixer.AddMixerInput(new ConvertingSampleProvider(input, mixerFormat));
                }
                catch (Exception) { }
            }

            OverallVolumeProvider = new VolumeSampleProvider(audioMixer)
            {
                Volume = config.Volume
            };

            IWaveProvider mixerProvider;
            if (differentMixerFormat)
            {
                mixerProvider = new PCMConversionWaveProvider(OverallVolumeProvider, output.WaveFormat.BitsPerSample / 8);
            }
            else
            {
                mixerProvider = OverallVolumeProvider.ToWaveProvider();
            }

            output.Init(mixerProvider);

            foreach (WasapiProvider provider in providers)
                provider.StartRecording();

            output.Play();
            Initialized = true;
        }

        private static void ClipMixer_MixerInputEnded(object sender, SampleProviderEventArgs e)
        {
            ClipPlayer clipPlayer = (e.SampleProvider as ClipPlayer);
            clipPlayer?.InvokeHasEnded();
            clipPlayers.Remove(clipPlayer);
        }

        private static void AudioMixer_MixerInputEnded(object sender, SampleProviderEventArgs e) =>
            //TODO Error Handling
            throw new Exception();

        public static void Play(ClipPlayer player)
        {
            if (!Initialized)
                throw new InvalidOperationException("AudioHandler is not initialized!");

            if (player.SoundInfo.SinglePlay)
                StopAllSingleplay(player.SoundInfo);

            player.Init(mixerFormat);

            clipPlayers.Add(player);
            clipMixer.AddMixerInput(player);
        }

        public static void Stop(ClipPlayer player)
        {
            if (!Initialized)
                throw new InvalidOperationException("AudioHandler is not initialized!");

            clipMixer.RemoveMixerInput(player);
            clipPlayers.Remove(player);
            player.InvokeHasEnded();
        }

        public static void StopAll()
        {
            if (!Initialized)
                throw new InvalidOperationException("AudioHandler is not initialized!");

            foreach (ClipPlayer player in clipPlayers)
            {
                clipMixer.RemoveMixerInput(player);
                player.InvokeHasEnded();
            }
            clipPlayers.Clear();
        }

        static void StopAllSingleplay(SoundInfo requestor)
        {
            if (!Initialized)
                throw new InvalidOperationException("AudioHandler is not initialized!");

            List<ClipPlayer> remove = new List<ClipPlayer>();

            foreach (ClipPlayer player in clipPlayers)
            {
                if (player.SoundInfo != requestor && !player.SoundInfo.SinglePlayImmunity)
                {
                    clipMixer.RemoveMixerInput(player);
                    remove.Add(player);
                    player.InvokeHasEnded();
                }
            }
            foreach (ClipPlayer player in remove)
            {
                clipPlayers.Remove(player);
            }
        }

        public static void Dispose()
        {
            if (Initialized)
                StopAll();

            (output as IDisposable)?.Dispose();
            foreach (WasapiProvider provider in providers)
            {
                provider?.StopRecording();
                provider?.Dispose();
            }
            providers.Clear();
            Initialized = false;
        }
    }

    public class ConfigUnavailableException : Exception
    {
        public ConfigUnavailableException(string message) : base(message)
        {
        }

        public ConfigUnavailableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ConfigUnavailableException()
        {
        }
    }

    public class InvalidOutputException : Exception
    {
        public InvalidOutputException(string message) : base(message)
        {
        }

        public InvalidOutputException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidOutputException()
        {
        }
    }
}
