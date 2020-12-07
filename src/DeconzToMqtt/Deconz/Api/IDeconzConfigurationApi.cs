using DeconzToMqtt.Deconz.Api.Requests;
using DeconzToMqtt.Deconz.Api.Responses;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace DeconzToMqtt.Deconz.Api
{
    public interface IDeconzConfigurationApi
    {
        [Get("/{apiKey}/config")]
        Task<Configuration> GetConfiguration(string apiKey, CancellationToken cancellationToken);

        [Post("/")]
        Task<Succes<ApiKey>> CreateApiKey(CreateApiKeyRequest request);

        [Delete("/{apiKey}/config/whitelist/{apiKey2}")]
        Task DeleteApiKey(string apiKey, string apiKey2);
    }
}