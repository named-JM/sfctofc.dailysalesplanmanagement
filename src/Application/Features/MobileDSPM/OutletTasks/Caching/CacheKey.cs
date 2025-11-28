using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFCTOFC.DailySalesPlanManagement.Application.Features.MobileDSPM.OutletTasks.Caching;
public static class MBLOutletTasksCacheKey
{
    public const string GetAllCacheKey = "all-MBLOutletTasks";
    public static string GetMBLOutletTasksByIdCacheKey(int id)
    {
        return $"GetMBLOutletTasksById,{id}";
    }
    public static string GetPaginationCacheKey(string parameters)
    {
        return $"MBLOutletTasksWithPaginationQuery,{parameters}";
    }
    public static IEnumerable<string>? Tags => new string[] { "MBLOutletTasks" };
    public static void Refresh()
    {
        FusionCacheFactory.RemoveByTags(Tags);
    }
}
