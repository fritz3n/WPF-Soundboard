using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace WPF_Soundboard.Audio
{
    class SmoothCutoffSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider provider;
        private readonly bool doStop;
        float lastValue = 0f;

        /// <summary>
        /// Smooths the cutoff of another Sampleprovider if it stops outputting to avoid a 'click'
        /// </summary>
        /// <param name="provider">The SampleProvider from wich to read</param>
        /// <param name="neverStop">Wether to stop playback if the Sampleprovider stops playback</param>
        public SmoothCutoffSampleProvider(ISampleProvider provider, bool doStop = false)
        {
            this.provider = provider;
            this.doStop = doStop;
        }

        public WaveFormat WaveFormat => provider.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            int read = provider.Read(buffer, offset, count);
            if (read > 0)
                lastValue = buffer[offset + read - 1];
            else if (doStop && lastValue < 0.001f)
                return 0;

            if (read != count)
            {
                for (; read < count; read++)
                {
                    // Output drops from 1 to 0.001 in roughly 7000 samples
                    //TODO Handle seperate channels seperately
                    buffer[read] = lastValue *= 0.999f;
                }
            }

            return read;
        }
    }
}
