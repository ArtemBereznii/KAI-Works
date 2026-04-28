using HardwareSim.BLL.Features.PowerManagement;
using HardwareSim.BLL.Features.Store;
using HardwareSim.DAL.Repositories;

namespace HardwareSim.PL
{
    class Program
    {
        static void Main(string[] args)
        {
            var deviceRepo = new JsonHardwareRepo();
            var catalogRepo = new JsonCatalogRepo();

            var powerService = new PowerService();
            var storeService = new StoreService(catalogRepo);

            var engine = new SimulationEngine(deviceRepo, powerService, storeService);

            engine.Start();
        }
    }
}