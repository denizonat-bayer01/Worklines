using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using wixi.Business.Abstract;
using wixi.Core.Utilities.Security.Protection;

namespace wixi.Business.Concrete
{
    public sealed class SmtpHealthCheck : ISmtpHealthCheck
    {
        private readonly ISmtpSettingsService _settingsService;
        private readonly ISecretProtector _protector;

        public SmtpHealthCheck(ISmtpSettingsService settingsService, ISecretProtector protector)
        {
            _settingsService = settingsService;
            _protector = protector;
        }

        public async Task<(bool IsHealthy, string Message)> CheckAsync()
        {
            try
            {
                var settings = await _settingsService.GetAsync();
                if (settings == null)
                    return (false, "SMTP settings not configured");

                using var client = new TcpClient();
                await client.ConnectAsync(settings.Host, settings.Port);
                if (!client.Connected)
                    return (false, $"Cannot connect to {settings.Host}:{settings.Port}");

                return (true, $"SMTP {settings.Host}:{settings.Port} reachable");
            }
            catch (Exception ex)
            {
                return (false, $"SMTP health check failed: {ex.Message}");
            }
        }
    }
}

