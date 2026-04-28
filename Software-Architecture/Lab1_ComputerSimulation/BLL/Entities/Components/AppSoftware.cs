using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HardwareSim.BLL.Entities.Base;

namespace HardwareSim.BLL.Entities.Components
{
    public class AppSoftware
    {
        public string Name { get; set; } = string.Empty;
        public bool IsIntensive { get; set; }
        public bool RequiresAudio { get; set; }
        public bool RequiresNetwork { get; set; }

        public DevicePlatform SupportedPlatform { get; set; } = DevicePlatform.Universal;

        public AppSoftware() { }
    }
}
