using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace WPF_Soundboard.Audio.Outputs
{
    interface IOutput : IWavePlayer
    {
        public WaveFormat WaveFormat { get; }
    }
}
