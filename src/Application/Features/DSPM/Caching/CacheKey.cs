
namespace SFCTOFC.DailySalesPlanManagement.Application.Features.DSPM.Caching;

#region OUTLETS
public static class OutletsCacheKey
{
    public const string GetAllCacheKey = "all-Outlets";
    public static string GetOutletByIdCacheKey(int id)
    {
        return $"GetOutletById,{id}";
    }
    public static string GetPaginationCacheKey(string parameters)
    {
        return $"OutletsWithPaginationQuery,{parameters}";
    }
    public static IEnumerable<string>? Tags => new string[] { "outlet" };
    public static void Refresh()
    {
        FusionCacheFactory.RemoveByTags(Tags);
    }
}
#endregion

#region SALESMANTRACKER
public static class SalesmanTracketCacheKey
{
    public const string GetAllCacheKey = "all-SalesmanTracket";
    public static string GetSalesmanTrackerByIdCacheKey(int id)
    {
        return $"GetSalesmanTrackerById,{id}";
    }
    public static string GetPaginationCacheKey(string parameters)
    {
        return $"SalesmanTrackerWithPaginationQuery,{parameters}";
    }
    public static IEnumerable<string>? Tags => new string[] { "SalesmanTracker" };
    public static void Refresh()
    {
        FusionCacheFactory.RemoveByTags(Tags);
    }
}
#endregion

