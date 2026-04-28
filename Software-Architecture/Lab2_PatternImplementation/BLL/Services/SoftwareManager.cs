using HardwareSim.BLL.Entities.Components;

namespace HardwareSim.BLL.Services
{
    public class SoftwareManager
    {
        public bool ValidatePrerequisites(AppSoftware software, bool hasNetwork, bool hasAudio, out string error)
        {
            if (software.RequiresNetwork && !hasNetwork)
            {
                error = $"'{software.Name}' requires an active network connection.";
                return false;
            }

            if (software.RequiresAudio && !hasAudio)
            {
                error = $"'{software.Name}' requires speakers or headphones connected.";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}