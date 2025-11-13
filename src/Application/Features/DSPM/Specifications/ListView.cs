using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFCTOFC.DailySalesPlanManagement.Application.Features.DSPM.Specifications;
public enum ListView
{
    [Description("All")] All,
    [Description("My Products")] My,
    [Description("Created Today")] TODAY,
    [Description("Created within the last 30 days")]
    LAST_30_DAYS
}