using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Soundboard.Clips;

namespace WPF_Soundboard.Audio
{
    class LiveClipPlayer : ClipPlayer
    {
        private AudioFileReader reader;
        IDisposable resampler = null;


        public LiveClipPlayer(SoundInfo soundInfo) : base(soundInfo)
        {
        }

        public override bool Ended => (reader?.Position ?? 0) >= (reader?.Length ?? 0);

        public override void Dispose()
        {
            reader.Dispose();
            resampler?.Dispose();
        }

        public override void Init(WaveFormat waveformat)
        {
            reader = new AudioFileReader(SoundInfo.Path);
            provider = reader;

            //TODO Use ConvertingSampleProvider

            if (reader.WaveFormat.SampleRate != waveformat.SampleRate)
            {
                if (reader.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
                    provider = ((resampler = new MediaFoundationResampler(reader, waveformat)) as MediaFoundationResampler).ToSampleProvider();
                else
                    provider = ((resampler = new MediaFoundationResampler(new Wave16ToFloatProvider(reader), waveformat)) as MediaFoundationResampler).ToSampleProvider();
            }
            else if (reader.WaveFormat.Channels != waveformat.Channels)
            {
                if (waveformat.Channels == 1 & reader.WaveFormat.Channels > 1)
                    provider = provider.ToMono();
                else if (waveformat.Channels == 2)
                    provider = provider.ToStereo();
                else
                    throw new InvalidWaveFormatException($"Couldn´t find a suitable converiosn from {reader.WaveFormat.Channels} to {waveformat.Channels} Channels");
            }

            if (SoundInfo.Start != 0 || SoundInfo.Length != 0)
            {
                provider = new OffsetSampleProvider(provider)
                {
                    SkipOver = TimeSpan.FromSeconds(SoundInfo.Start),
                    Take = SoundInfo.Length == 0 ? TimeSpan.Zero : TimeSpan.FromSeconds(SoundInfo.Length)
                };
            }

            provider = new VolumeSampleProvider(provider) { Volume = SoundInfo.VolumeModifier };
        }
    }
}
