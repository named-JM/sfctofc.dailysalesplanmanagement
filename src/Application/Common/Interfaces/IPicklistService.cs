using SFCTOFC.DailySalesPlanManagement.Application.Features.PicklistSets.DTOs;

namespace SFCTOFC.DailySalesPlanManagement.Application.Common.Interfaces;

public interface IPicklistService
{
    List<PicklistSetDto> DataSource { get; }
    event Func<Task>? OnChange;
    Task InitializeAsync();
    Task RefreshAsync();
}