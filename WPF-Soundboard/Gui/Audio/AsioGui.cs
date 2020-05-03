using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace WPF_Soundboard.Gui.Audio
{
    static class AsioGui
    {
        public static bool Supported => AsioOut.isSupported();

        public static string[] GetDrivers() => AsioOut.GetDriverNames();

        public static string[] GetChannels(string driver)
        {
            using AsioOut tempOut = new AsioOut(driver);
            string[] channels = new string[tempOut.DriverOutputChannelCount];

            for (int i = 0; i < tempOut.DriverOutputChannelCount; i++)
            {
                channels[i] = tempOut.AsioOutputChannelName(i);
            }

            return channels;
        }
    }
}
