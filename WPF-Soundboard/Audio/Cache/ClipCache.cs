using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Soundboard.Audio.Cache;
using WPF_Soundboard.Clips;

namespace WPF_Soundboard.Audio.Cache
{
    static class ClipCache
    {
        private static Dictionary<SoundInfo, CachedClip> cachedClips = new Dictionary<SoundInfo, CachedClip>();

        public static void CacheClip(SoundInfo info)
        {
            if (cachedClips.ContainsKey(info))
                return;

            CachedClip clip;

            cachedClips.Add(info, (clip = new CachedClip(info)));
            Task.Run(clip.Cache);
        }

        public static CachedClip GetCachedClip(SoundInfo info) => cachedClips[info];

        public static void Release(SoundInfo info) => cachedClips.Remove(info);
    }
}
