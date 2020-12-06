using System.Collections.ObjectModel;
using System.Linq;

namespace DeconzToMqtt.Deconz.Api.Responses
{
    public class Succes<TResult> : Collection<InternalSuccess<TResult>>
        where TResult : class
    {
        public TResult Result => this.FirstOrDefault()?.Success;
    }

    public sealed class InternalSuccess<TResult>
    {
        public TResult Success { get; set; }
    }
}