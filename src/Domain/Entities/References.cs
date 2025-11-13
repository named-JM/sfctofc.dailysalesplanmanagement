using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFCTOFC.DailySalesPlanManagement.Domain.Common.Entities;

namespace SFCTOFC.DailySalesPlanManagement.Domain.Entities;
public class Brands : BaseAuditableEntity
{
    public int Id { get; set; }
    public string? BrandCode { get; set; }
    public string? BrandName { get; set; }

}