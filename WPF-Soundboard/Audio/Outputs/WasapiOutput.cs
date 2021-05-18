using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace WPF_Soundboard.Audio.Outputs
{
	class WasapiOutput : IOutput
	{
		private WasapiOut output;

		public WasapiOutput(Dictionary<string, object> config)
		{
			string OutputName = config["OutputName"] as string;

			if (string.IsNullOrWhiteSpace(OutputName))
				OutputName = "<default>";

			MMDevice device = null;

			if (OutputName == "<default>")
			{
				device = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
			}
			else
			{
				foreach (MMDevice dev in new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
				{
					if (dev.ID == OutputName)
					{
						device = dev;
						break;
					}
				}
			}

			if (device == null)
				throw new InvalidOutputException(OutputName + " not found");

			output = new WasapiOut(device, AudioClientShareMode.Shared, false, 50);
		}

		public float Volume { get => ((IWavePlayer)output).Volume; set => ((IWavePlayer)output).Volume = value; }

		public PlaybackState PlaybackState => ((IWavePlayer)output).PlaybackState;

		public WaveFormat WaveFormat => output.OutputWaveFormat;

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
