using DeConzToMqtt.Domain.DeConz.Dtos;
using DeConzToMqtt.Domain.DeConz.Dtos.Configuration;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace DeConzToMqtt.Domain.DeConz.Apis
{
    public interface IDeConzConfigurationApi
    {
        // TODO https://github.com/reactiveui/refit/blob/main/Refit/ApiResponse.cs
        [Get("/{apiKey}/config")]
        Task<Configuration> GetConfigurationAsync(string apiKey, CancellationToken cancellationToken);

        [Post("/")]
        Task<SuccesResult<ApiKey>> CreateApiKeyAsync(CreateApiKeyRequest request);

        [Delete("/{apiKey}/config/whitelist/{apiKey2}")]
        Task DeleteApiKeyAsync(string apiKey, string apiKey2);
    }
}