using SFCTOFC.DailySalesPlanManagement.Application.Features.Identity.DTOs;

namespace SFCTOFC.DailySalesPlanManagement.Application.Common.Interfaces.Identity;

public interface IRoleService
{
    List<ApplicationRoleDto> DataSource { get; }
    event Func<Task>? OnChange;
    Task InitializeAsync();
    Task  RefreshAsync();
}