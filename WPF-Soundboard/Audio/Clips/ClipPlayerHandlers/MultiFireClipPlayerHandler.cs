using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Soundboard.Audio.Cache;
using WPF_Soundboard.Clips;

namespace WPF_Soundboard.Audio.Clips.ClipPlayerHandlers
{
    class MultiFireClipPlayerHandler : IClipPlayerHandler
    {
        private readonly SoundInfo soundInfo;
        List<ClipPlayer> clipPlayers = new List<ClipPlayer>();
        public MultiFireClipPlayerHandler(SoundInfo soundInfo)
        {
            this.soundInfo = soundInfo;
            if (soundInfo.Cached)
            {
                ClipCache.CacheClip(soundInfo);
            }
        }

        public void Play()
        {
            ClipPlayer player = soundInfo.Cached ? new CachedClipPlayer(soundInfo) as ClipPlayer : new LiveClipPlayer(soundInfo) as ClipPlayer;
            player.HasEnded += Player_HasEnded;
            clipPlayers.Add(player);
            AudioHandler.Play(player);
        }

        private void Player_HasEnded(object sender, EventArgs e)
        {
            (sender as ClipPlayer).Dispose();
            clipPlayers.Remove(sender as ClipPlayer);
        }

        public void Stop()
        {
            foreach (ClipPlayer clipPlayer in clipPlayers)
            {
                AudioHandler.Stop(clipPlayer);
            }
        }
    }
}
