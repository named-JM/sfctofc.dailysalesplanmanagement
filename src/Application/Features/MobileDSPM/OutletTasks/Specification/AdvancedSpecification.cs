using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFCTOFC.DailySalesPlanManagement.Application.Features.MobileDSPM.OutletTasks.Specification;
public class AdvancedSpecificationMBLOutletTasks : Specification<MBLOutletTasks>
{
    public AdvancedSpecificationMBLOutletTasks(AdvancedFilterMBLOutletTasks filter)
    {
        DateTime today = DateTime.UtcNow;
        //var todayrange = today.GetDateRange("TODAY", filter.CurrentUser.LocalTimeOffset);
        //var last30daysrange = today.GetDateRange("LAST_30_DAYS", filter.CurrentUser.LocalTimeOffset);
        //Query.Where(x => x.TaskName.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword))
        //     .Where(x => x.CreatedAt >= filter.StartDate && x.CreatedAt <= filter.EndDate, filter.StartDate != null && filter.EndDate != null)
        //     .Where(x => x.CreatedByUserId == filter.CurrentUser.UserId, filter.ListView == ListView.My)
        //     .Where(x => x.CreatedAt >= todayrange.Start && x.CreatedAt < todayrange.End.AddDays(1), filter.ListView == ListView.TODAY)
        //     .Where(x => x.CreatedAt >= last30daysrange.Start, filter.ListView == ListView.LAST_30_DAYS);
    }
}
