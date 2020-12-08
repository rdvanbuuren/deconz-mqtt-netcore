using DeConzToMqtt.Domain.DeConz.Dtos.Lights;
using Refit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DeConzToMqtt.Domain.DeConz.Apis
{
    public interface IDeConzLightsApi
    {
        [Get("/{apiKey}/lights")]
        Task<IDictionary<string, Light>> GetLightsAsync(string apiKey, CancellationToken cancellationToken);
    }
}