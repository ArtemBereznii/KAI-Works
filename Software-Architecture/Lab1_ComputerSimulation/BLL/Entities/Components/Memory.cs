using System.Collections.Generic;
using System.Linq;

namespace HardwareSim.BLL.Entities.Components
{
    public class Memory
    {
        public List<AppSoftware> InstalledSoftware { get; set; } = new();

        public void Install(AppSoftware software)
        {
            if (!IsInstalled(software.Name))
            {
                InstalledSoftware.Add(software);
            }
        }

        public bool Uninstall(string softwareName)
        {
            var software = GetSoftware(softwareName);
            if (software != null)
            {
                InstalledSoftware.Remove(software);
                return true;
            }
            return false;
        }

        public bool IsInstalled(string softwareName)
        {
            return InstalledSoftware.Any(s => s.Name.ToLower() == softwareName.ToLower());
        }

        public AppSoftware? GetSoftware(string softwareName)
        {
            return InstalledSoftware.FirstOrDefault(s => s.Name.ToLower() == softwareName.ToLower());
        }
    }
}