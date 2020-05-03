using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPF_Soundboard.Audio;
using WPF_Soundboard.Clips;

namespace WPF_Soundboard
{
    static class Serializer
    {
        public static string Path { get; set; } = "config.json";
        public static string AudioPath { get; set; } = "audio.json";
        private static bool deserializing = false;



        static Dictionary<ClipPageList, Timer> saveTimers = new Dictionary<ClipPageList, Timer>();



        public static ClipPageList GetClipPages(string path = null, bool autoSave = true)
        {
            path ??= System.IO.Path.Combine(GetAssemblyPath(), Path);

            if (!File.Exists(path))
            {
                ClipPageList list = new ClipPageList();
                if (autoSave)
                    list.OnChanged += Pages_OnChanged;
                return list;
            }

            string json = File.ReadAllText(path);

            if (string.IsNullOrEmpty(json))
            {
                ClipPageList list = new ClipPageList();
                if (autoSave)
                    list.OnChanged += Pages_OnChanged;
                return list;
            }

            deserializing = true;

            ClipPageList pages = JsonConvert.DeserializeObject<ClipPageList>(json);



            if (autoSave)
                pages.OnChanged += Pages_OnChanged;

            deserializing = false;
            return pages;
        }


        public static AudioConfig GetAudioConfig()
        {
            if (!File.Exists(AudioPath))
            {
                AudioConfig config = AudioConfig.GetDefault();
                SaveAudioConfig(config);
                return config;
            }

            string json = File.ReadAllText(System.IO.Path.Combine(GetAssemblyPath(), AudioPath));

            return JsonConvert.DeserializeObject<AudioConfig>(json);
        }

        public static void SaveClipPages(ClipPageList clipPages, string path = null)
        {

            path ??= System.IO.Path.Combine(GetAssemblyPath(), Path);

#if DEBUG
            string json = JsonConvert.SerializeObject(clipPages, Formatting.Indented);
#else
            string json = JsonConvert.SerializeObject(clipPages);
#endif

            File.WriteAllText(path, json);
        }

        private static void Pages_OnChanged(object sender, EventArgs e)
        {
            if (deserializing)
                return;

            if (!saveTimers.ContainsKey(sender as ClipPageList))
                saveTimers.Add(sender as ClipPageList, new Timer(SavePages, sender, 500, Timeout.Infinite));
        }

        private static void SavePages(object stateInfo)
        {
            saveTimers[stateInfo as ClipPageList].Dispose();
            saveTimers.Remove(stateInfo as ClipPageList);
            SaveClipPages(stateInfo as ClipPageList);
        }

        public static void SaveAudioConfig(AudioConfig config)
        {
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);

            File.WriteAllText(System.IO.Path.Combine(GetAssemblyPath(), AudioPath), json);
        }

        private static string GetAssemblyPath() => System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    }
}
