using System;
using System.Collections.Generic;
using System.Text;
using WPF_Soundboard.Audio.Cache;
using WPF_Soundboard.Clips;

namespace WPF_Soundboard.Audio.Clips.ClipPlayerHandlers
{
    class StoppingClipPlayerHandler : IClipPlayerHandler
    {
        private SoundInfo soundInfo;
        private ClipPlayer clipPlayer;
        private bool playing = false;


        public StoppingClipPlayerHandler(SoundInfo soundInfo)
        {
            this.soundInfo = soundInfo;
            if (soundInfo.Cached)
            {
                ClipCache.CacheClip(soundInfo);
            }
        }

        public void Play()
        {
            if (playing)
            {
                Stop();
                return;
            }
            clipPlayer = soundInfo.Cached ? new CachedClipPlayer(soundInfo) as ClipPlayer : new LiveClipPlayer(soundInfo) as ClipPlayer;
            clipPlayer.HasEnded += Player_HasEnded;
            AudioHandler.Play(clipPlayer);
            playing = true;
        }

        private void Player_HasEnded(object sender, EventArgs e)
        {
            playing = false;
            (sender as ClipPlayer).Dispose();
        }
        public void Stop()
        {
            if (playing)
                AudioHandler.Stop(clipPlayer);
        }
    }
}
