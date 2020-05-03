using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace WPF_Soundboard.Gui.Audio
{
    static class WasapiGui
    {
        private static MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
        public static Dictionary<string, string> GetOutputs()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                { "<default>", "Default"}
            };

            foreach (MMDevice dev in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                dict.Add(dev.ID, dev.FriendlyName);
            }

            return dict;
        }

        public static Dictionary<string, string> GetInputs()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                { "<default>", "In: Default"},
                { "<defaultLoopback>", "Out: Default"}
            };

            foreach (MMDevice dev in enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active))
            {
                dict.Add(dev.ID, (dev.DataFlow == DataFlow.Capture ? "In: " : "Out: ") + dev.FriendlyName);
            }

            return dict;
        }
    }
}
