using HardwareSim.DAL.Repositories;

namespace HardwareSim.PL
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the data access layer
            var mockRepo = new JsonHardwareRepo();

            // Pass the data to the simulation engine
            var engine = new SimulationEngine(mockRepo);

            // Start the interactive console loop
            engine.Start();
        }
    }
}