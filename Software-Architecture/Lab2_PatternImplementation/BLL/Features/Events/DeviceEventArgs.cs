using System;

namespace HardwareSim.BLL.Features.Events
{
    public class HardwareEventArgs : EventArgs
    {
        //Event Arguments
        public string Message { get; }
        public HardwareEventArgs(string message)
        {
            Message = message;
        }
    }
}