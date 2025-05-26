using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.MAUI.Example.Services.Proxy.Core
{
    public interface IWeatherServiceProxy
    {
        Task<IEnumerable<WeatherForecast>?> GetWeatherForecastAsync(CancellationToken token = default);
    }
}
