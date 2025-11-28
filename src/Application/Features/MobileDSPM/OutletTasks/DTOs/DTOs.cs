using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SFCTOFC.DailySalesPlanManagement.Domain.Entities;

namespace SFCTOFC.DailySalesPlanManagement.Application.Features.MobileDSPM.OutletTasks.DTOs;


public class MBLOutletTasksDto
{
    public int Id { get; set; }
    public int SalesmanDailyPlanId { get; set; }
    public int CreatedByUserId { get; set; }
    public int? AssignedToUserId { get; set; }
    public string? TaskName { get; set; }
    public string? Notes { get; set; }
    public string? CompletedRemarks { get; set; }
    public bool? IsCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<MBLOutletTasks, MBLOutletTasksDto>().ReverseMap();
            CreateMap<MBLOutletTasksDto, MBLOutletTasks>(MemberList.None);
        }
    }


}



