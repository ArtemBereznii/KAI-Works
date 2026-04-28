using HardwareSim.BLL.Entities.Base;
using HardwareSim.BLL.Entities.Components;

namespace HardwareSim.BLL.Features.SoftwareManagement
{
    public static class SoftwareFactory
    {
        public static AppSoftware CreateApp(string name, bool isIntensive, bool reqAudio, bool reqNet, DevicePlatform platform)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("App name cannot be empty.");
            }

            return new AppSoftware
            {
                Name = name,
                IsIntensive = isIntensive,
                RequiresAudio = reqAudio,
                RequiresNetwork = reqNet,
                SupportedPlatform = platform
            };
        }
    }
}