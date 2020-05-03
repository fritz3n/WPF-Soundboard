using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace WPF_Soundboard.Audio.Outputs
{
    class AsioOutput : IOutput
    {
        private AsioOut output;

        public AsioOutput(Dictionary<string, object> config)
        {
            output = new AsioOut(config["DriverName"] as string)
            {
                ChannelOffset = (int)config["StartChannel"],
            };

            //TODO Implement automatic SampleRate discovery, when NAudio supports this
            int sampleRate = 44100;
            int channels = (int)config["Channels"];
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels); // We prefer IEEE Float to avoid an extra conversion between mixer and Output
        }


        public float Volume { get => ((IWavePlayer)output).Volume; set => ((IWavePlayer)output).Volume = value; }

        public PlaybackState PlaybackState => ((IWavePlayer)output).PlaybackState;

        public WaveFormat WaveFormat { get; }

        public event EventHandler<StoppedEventArgs> PlaybackStopped
        {
            add
            {
                ((IWavePlayer)output).PlaybackStopped += value;
            }

            remove
            {
                ((IWavePlayer)output).PlaybackStopped -= value;
            }
        }

        public void Dispose() => ((IWavePlayer)output).Dispose();
        public void Init(IWaveProvider waveProvider) => ((IWavePlayer)output).Init(waveProvider);
        public void Pause() => ((IWavePlayer)output).Pause();
        public void Play() => ((IWavePlayer)output).Play();
        public void Stop() => ((IWavePlayer)output).Stop();
    }
}
