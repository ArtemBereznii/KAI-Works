using HardwareSim.BLL.Entities.Components;

namespace HardwareSim.BLL.Services
{
    public class SoftwareManager
    {
        // Checks all prerequisites for operation execution
        public bool ValidatePrerequisites(string softwareName, Memory memory, bool hasNetwork, bool hasAudio, out string error)
        {
            if (!memory.IsInstalled(softwareName))
            {
                error = $"Software '{softwareName}' is not installed.";
                return false;
            }

            // Simulating software requirements based on naming for demo purposes
            if (softwareName.ToLower().Contains("online") && !hasNetwork)
            {
                error = $"'{softwareName}' requires an active network connection.";
                return false;
            }

            if (softwareName.ToLower().Contains("music") || softwareName.ToLower().Contains("video"))
            {
                if (!hasAudio)
                {
                    error = $"'{softwareName}' requires speakers or headphones connected.";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }
    }
}