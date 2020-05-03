using System;
using System.Collections.Generic;
using System.Text;

namespace WPF_Soundboard.Audio.Clips.ClipPlayerHandlers
{
    interface IClipPlayerHandler
    {
        public void Play();
        public void Stop();
    }
}
