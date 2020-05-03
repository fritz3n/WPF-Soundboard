using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Soundboard.Audio.Conversion
{
    class ConvertingSampleProvider : ISampleProvider, IDisposable
    {
        ISampleProvider provider;
        private MediaFoundationResampler resampler = null;

        public ConvertingSampleProvider(IWaveProvider waveProvider, WaveFormat targetFormat)
        {

            if (waveProvider.WaveFormat.SampleRate != targetFormat.SampleRate)
            {
                if (provider.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
                    provider = ((resampler = new MediaFoundationResampler(waveProvider, targetFormat)) as MediaFoundationResampler).ToSampleProvider();
                else
                    provider = ((resampler = new MediaFoundationResampler(new Wave16ToFloatProvider(waveProvider), targetFormat)) as MediaFoundationResampler).ToSampleProvider();
            }
            else
            {
                provider = waveProvider.ToSampleProvider();
            }

            if (provider.WaveFormat.Channels != targetFormat.Channels)
            {
                if (targetFormat.Channels == 1 & provider.WaveFormat.Channels > 1)
                    provider = provider.ToMono();
                else if (targetFormat.Channels == 2)
                    provider = provider.ToStereo();
                else
                    throw new InvalidWaveFormatException($"Couldn´t find a suitable conversion from {provider.WaveFormat.Channels} to {targetFormat.Channels} Channels");
            }
        }

        public WaveFormat WaveFormat => provider.WaveFormat;

        public void Dispose() => resampler?.Dispose();
        public int Read(float[] buffer, int offset, int count) => provider.Read(buffer, offset, count);
    }
}
