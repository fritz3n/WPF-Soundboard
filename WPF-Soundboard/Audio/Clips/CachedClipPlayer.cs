using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Soundboard.Audio.Cache;
using WPF_Soundboard.Clips;

namespace WPF_Soundboard.Audio
{
    class CachedClipPlayer : ClipPlayer
    {
        CachedClip.SampleProvider cachedClipProvider;
        IDisposable resampler = null;

        public CachedClipPlayer(SoundInfo soundInfo) : base(soundInfo)
        {
        }

        public override bool Ended => cachedClipProvider?.Ended ?? false;

        public override void Dispose() => resampler?.Dispose();
        public override void Init(WaveFormat waveformat)
        {
            //TODO Use ConvertingSampleProvider

            provider = cachedClipProvider = ClipCache.GetCachedClip(SoundInfo).GetSampleProvider();

            if (provider.WaveFormat.SampleRate != waveformat.SampleRate)
            {
                if (provider.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
                    provider = ((resampler = new MediaFoundationResampler(provider.ToWaveProvider(), waveformat)) as MediaFoundationResampler).ToSampleProvider();
                else
                    provider = ((resampler = new MediaFoundationResampler(new Wave16ToFloatProvider(provider.ToWaveProvider()), waveformat)) as MediaFoundationResampler).ToSampleProvider();
            }
            else if (provider.WaveFormat.Channels != waveformat.Channels)
            {
                if (waveformat.Channels == 1 & provider.WaveFormat.Channels > 1)
                    provider = provider.ToMono();
                else if (waveformat.Channels == 2)
                    provider = provider.ToStereo();
                else
                    throw new InvalidWaveFormatException($"Couldn´t find a suitable conversion from {provider.WaveFormat.Channels} to {waveformat.Channels} Channels");
            }

            provider = new VolumeSampleProvider(provider) { Volume = SoundInfo.VolumeModifier };
        }
    }
}
