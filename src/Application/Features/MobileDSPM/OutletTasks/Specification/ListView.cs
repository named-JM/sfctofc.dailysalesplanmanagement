using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFCTOFC.DailySalesPlanManagement.Application.Features.MobileDSPM.OutletTasks.Specification;
public enum ListView
{
    [Description("All")] All,
    [Description("My Tasks")] My,
    [Description("Created Today")] TODAY,
    [Description("Created within the last 30 days")]
    LAST_30_DAYS
}
