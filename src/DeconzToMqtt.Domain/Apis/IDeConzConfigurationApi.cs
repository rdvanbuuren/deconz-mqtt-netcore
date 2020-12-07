using DeConzToMqtt.Domain.DeConz.Dtos;
using DeConzToMqtt.Domain.DeConz.Dtos.Configuration;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace DeConzToMqtt.Domain.DeConz.Apis
{
    public interface IDeConzConfigurationApi
    {
        [Get("/{apiKey}/config")]
        Task<Configuration> GetConfiguration(string apiKey, CancellationToken cancellationToken);

        [Post("/")]
        Task<SuccesResult<ApiKey>> CreateApiKey(CreateApiKeyRequest request);

        [Delete("/{apiKey}/config/whitelist/{apiKey2}")]
        Task DeleteApiKey(string apiKey, string apiKey2);
    }
}