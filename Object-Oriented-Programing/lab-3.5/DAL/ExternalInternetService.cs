using Core.Interfaces;

namespace DAL.Services
{
    // Справжня реалізація сервісу
    public class ExternalInternetService : IInternetService
    {
        public bool IsConnected()
        {
            // У реальному житті тут була б перевірка
            return true;
        }
    }
}