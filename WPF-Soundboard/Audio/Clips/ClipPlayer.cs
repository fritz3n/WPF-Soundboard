using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WPF_Soundboard.Audio.Cache;
using WPF_Soundboard.Clips;

namespace WPF_Soundboard.Audio
{
    abstract class ClipPlayer : ISampleProvider
    {
        public SoundInfo SoundInfo { get; private set; }
        protected ISampleProvider provider;

        public abstract bool Ended { get; }
        public event EventHandler HasEnded;

        public ClipPlayer(SoundInfo soundInfo)
        {
            SoundInfo = soundInfo;
        }

        public static ClipPlayer GetClipPlayer(SoundInfo info)
        {
            if (info.Cached)
            {
                return new CachedClipPlayer(info);
            }
            else
            {
                return new LiveClipPlayer(info);
            }
        }

        public void InvokeHasEnded() => HasEnded?.Invoke(this, EventArgs.Empty);

        public abstract void Init(WaveFormat waveformat);

        public WaveFormat WaveFormat => provider.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            if (provider == null)
                throw new InvalidOperationException("Clipplayer is not initialized or already disposed");

            return provider.Read(buffer, offset, count);
        }

        public abstract void Dispose();
    }

    public class InvalidWaveFormatException : Exception
    {
        public InvalidWaveFormatException(string message) : base(message)
        {
        }

        public InvalidWaveFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
