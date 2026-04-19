using System;

namespace HardwareSim.BLL.Events
{
    public class HardwareEventArgs : EventArgs
    {
        public string Message { get; }
        public HardwareEventArgs(string message)
        {
            Message = message;
        }
    }
}