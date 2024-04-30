using AutoMapper;
using Vulnerabilities.Api.Models;

namespace Vulnerabilities.Api.MappingProfiles;
public class VulnProfile : Profile
{
    public VulnProfile()
    {
        CreateMap<VulnerabilityDTO, Vulnerability>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}