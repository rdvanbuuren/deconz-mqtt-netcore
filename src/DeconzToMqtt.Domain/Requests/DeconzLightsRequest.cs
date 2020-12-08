using DeConzToMqtt.Domain.DeConz.Dtos.Lights;
using MediatR;
using System.Collections.Generic;

namespace DeConzToMqtt.Domain.DeConz.Requests
{
    public class DeConzLightsRequest : IRequest<ICollection<Light>>
    {
    }
}