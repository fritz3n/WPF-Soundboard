using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Soundboard.Clips;

namespace WPF_Soundboard.Audio.Cache
{
    class CachedClip
    {
        private readonly SoundInfo info;
        private float[] AudioData { get; set; }
        public WaveFormat WaveFormat { get; private set; }
        public CachedClip(SoundInfo info)
        {
            this.info = info;
        }

        public void Cache()
        {
            using (AudioFileReader audioFileReader = new AudioFileReader(info.Path))
            {
                ISampleProvider provider = audioFileReader;

                if (info.Start != 0 || info.Length != 0)
                {
                    provider = new OffsetSampleProvider(provider)
                    {
                        SkipOver = TimeSpan.FromSeconds(info.Start),
                        Take = info.Length == 0 ? TimeSpan.Zero : TimeSpan.FromSeconds(info.Length)
                    };
                }

                WaveFormat = provider.WaveFormat;
                List<float> wholeFile = new List<float>((int)(audioFileReader.Length / 4));
                float[] readBuffer = new float[provider.WaveFormat.SampleRate * provider.WaveFormat.Channels];
                int samplesRead;
                while ((samplesRead = provider.Read(readBuffer, 0, readBuffer.Length)) > 0)
                {
                    wholeFile.AddRange(readBuffer.Take(samplesRead));
                }
                AudioData = wholeFile.ToArray();
            }
        }

        public SampleProvider GetSampleProvider() => new SampleProvider(this);

        public class SampleProvider : ISampleProvider
        {
            private readonly CachedClip parent;
            private long position = 0;
            public SampleProvider(CachedClip parent)
            {
                this.parent = parent;
            }

            public bool Ended => position >= parent.AudioData.Length;

            public WaveFormat WaveFormat => parent.WaveFormat;

            public int Read(float[] buffer, int offset, int count)
            {
                long availableSamples = parent.AudioData.Length - position;
                long samplesToCopy = Math.Min(availableSamples, count);
                Buffer.BlockCopy(parent.AudioData, (int)position * 4, buffer, offset * 4, (int)samplesToCopy * 4);
                position += samplesToCopy;
                return (int)samplesToCopy;
            }
        }
    }
}
