using Microsoft.AspNetCore.Hosting;

namespace Sensor.Api.Host
{
    public interface IWebServiceHost
    {
        void Run();
        IWebHost GetWebHost();
    }
}