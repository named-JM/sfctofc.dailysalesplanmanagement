using SFCTOFC.DailySalesPlanManagement.Application.Features.Tenants.DTOs;

namespace SFCTOFC.DailySalesPlanManagement.Application.Common.Interfaces.MultiTenant;

public interface ITenantService
{
    List<TenantDto> DataSource { get; }
    event Func<Task>? OnChange;
    Task InitializeAsync();
    Task RefreshAsync();
}