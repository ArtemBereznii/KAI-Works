namespace HardwareSim.BLL.Abstractions
{
    public interface IDevice
    {
        bool IsConnectedToGrid { get; }
        void ExecuteOperation(string softwareName, double durationHours);
    }
}