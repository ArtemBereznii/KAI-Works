using System.Collections.Generic;

namespace HardwareSim.BLL.Entities.Components
{
    public class Memory
    {
        private readonly List<string> _installedSoftware = new();

        public void Install(string softwareName)
        {
            if (!_installedSoftware.Contains(softwareName))
                _installedSoftware.Add(softwareName);
        }

        public bool IsInstalled(string softwareName)
        {
            return _installedSoftware.Contains(softwareName);
        }
    }
}