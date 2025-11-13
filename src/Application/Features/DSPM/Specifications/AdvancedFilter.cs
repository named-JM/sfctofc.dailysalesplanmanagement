

using DocumentFormat.OpenXml;

namespace SFCTOFC.DailySalesPlanManagement.Application.Features.DSPM.Specifications;

public class AdvancedFilterSalesmanTracker : PaginationFilter
{
    public int Id { get; set; }
    public string? AspNetUserId { get; set; }
    public string? Name { get; set; }
    public DateTime? Date { get; set; }
    public decimal? ActualSales { get; set; }
    public decimal? TargetSales { get; set; }
    public int? ActualStoreVisited { get; set; }
    public int? TargetStoreVisited { get; set; }
    public ListView ListView { get; set; } = ListView.All;
    public UserProfile? CurrentUser { get; set; }
}
