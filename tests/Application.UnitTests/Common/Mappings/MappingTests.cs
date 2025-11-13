using System;
using System.Reflection;
using System.Runtime.Serialization;
using AutoMapper;
using SFCTOFC.DailySalesPlanManagement.Application.Common.Interfaces;
using SFCTOFC.DailySalesPlanManagement.Application.Features.AuditTrails.DTOs;
using SFCTOFC.DailySalesPlanManagement.Application.Features.Contacts.DTOs;
using SFCTOFC.DailySalesPlanManagement.Application.Features.Documents.DTOs;
using SFCTOFC.DailySalesPlanManagement.Application.Features.Identity.DTOs;
using SFCTOFC.DailySalesPlanManagement.Application.Features.PicklistSets.DTOs;
using SFCTOFC.DailySalesPlanManagement.Application.Features.Products.DTOs;
using SFCTOFC.DailySalesPlanManagement.Application.Features.SystemLogs.DTOs;
using SFCTOFC.DailySalesPlanManagement.Application.Features.Tenants.DTOs;
using SFCTOFC.DailySalesPlanManagement.Domain.Entities;
using SFCTOFC.DailySalesPlanManagement.Domain.Identity;
using NUnit.Framework;

namespace SFCTOFC.DailySalesPlanManagement.Application.UnitTests.Common.Mappings;
public class MappingTests
{
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;

    public MappingTests()
    {
        _configuration = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetAssembly(typeof(IApplicationDbContext))));
        _mapper = _configuration.CreateMapper();
    }

    [Test]
    public void ShouldHaveValidConfiguration()
    {
        _configuration.AssertConfigurationIsValid();
    }

    [Test]
    [TestCase(typeof(Document), typeof(DocumentDto), true)]
    [TestCase(typeof(Tenant), typeof(TenantDto), true)]
    [TestCase(typeof(Product), typeof(ProductDto), true)]
    [TestCase(typeof(Contact), typeof(ContactDto), true)]
    [TestCase(typeof(PicklistSet), typeof(PicklistSetDto), true)]
    [TestCase(typeof(ApplicationUser), typeof(ApplicationUserDto), false)]
    [TestCase(typeof(ApplicationRole), typeof(ApplicationRoleDto), false)]
    [TestCase(typeof(SystemLog), typeof(SystemLogDto), false)]
    [TestCase(typeof(AuditTrail), typeof(AuditTrailDto), false)]

    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination, bool inverseMap = false)
    {
        var instance = GetInstanceOf(source);

        _mapper.Map(instance, source, destination);

        if (inverseMap)
        {
            ShouldSupportMappingFromSourceToDestination(destination, source, false);
        }
    }

    private object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type);

        // Type without parameterless constructor
        return FormatterServices.GetUninitializedObject(type);
    }
}
