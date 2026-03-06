using System.Threading.Tasks;

namespace wixi.Business.Abstract
{
    public interface ISmtpHealthCheck
    {
        Task<(bool IsHealthy, string Message)> CheckAsync();
    }
}

