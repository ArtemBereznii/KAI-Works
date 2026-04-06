using HardwareSim.DAL.Repositories;

namespace HardwareSim.PL
{
    class Program
    {
        static void Main(string[] args)
        {
            var mockRepo = new MockHardwareRepo();
            var demo = new DemoRunner(mockRepo);

            // Run the simulation
            demo.RunDemo();

            ConsoleManager.Pause();
        }
    }
}