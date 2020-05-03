
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Soundboard.Audio
{
    struct AudioConfig
    {
        public OutputType OutType { get; set; }
        public Dictionary<string, object> OutputParameters { get; set; }

        public string[] WasapiIns { get; set; }
        public float Volume { get; set; }


        public static Dictionary<string, object> GetDefaultOuptutParameters(OutputType type) => type switch
        {
            OutputType.WASAPI => new Dictionary<string, object> { { "OutputName", "<default>" } },
            OutputType.ASIO => new Dictionary<string, object> { { "DriverName", "<unknown>" }, { "StartChannel", 0 }, { "Channels", 2 } }
        };


        public static AudioConfig GetDefault(OutputType type = OutputType.WASAPI)
        {
            if (type == OutputType.ASIO)
            {
                return new AudioConfig()
                {
                    OutType = OutputType.ASIO,
                    OutputParameters = new Dictionary<string, object> { { "DriverName", "<unknown>" }, { "StartChannel", 0 }, { "Channels", 2 } },
                    WasapiIns = Array.Empty<string>(),
                    Volume = 1
                };
            }
            else
            {
                return new AudioConfig()
                {
                    OutType = OutputType.WASAPI,
                    OutputParameters = new Dictionary<string, object> { { "OutputName", "<default>" } },
                    WasapiIns = Array.Empty<string>(),
                    Volume = 1
                };
            }
        }

        public enum OutputType
        {
            WASAPI,
            ASIO
        }
    }
}
