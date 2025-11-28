using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using SFCTOFC.DailySalesPlanManagement.Application.Features.MobileDSPM.OutletTasks.Caching;
using SFCTOFC.DailySalesPlanManagement.Application.Features.MobileDSPM.OutletTasks.DTOs;
using SFCTOFC.DailySalesPlanManagement.Application.Features.Products.Caching;
using SFCTOFC.DailySalesPlanManagement.Application.Features.Products.DTOs;
using SFCTOFC.DailySalesPlanManagement.Domain.Entities;

namespace SFCTOFC.DailySalesPlanManagement.Application.Features.MobileDSPM.OutletTasks.Commands.AddEdit;
public class AddEditOutletTasksCommand : ICacheInvalidatorRequest<Result<int>>
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
    public string CacheKey => MBLOutletTasksCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => ProductCacheKey.Tags;
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<MBLOutletTasksDto, AddEditOutletTasksCommand>(MemberList.None);
            CreateMap<AddEditOutletTasksCommand, MBLOutletTasks>(MemberList.None);
        }
    }
}

public class AddEditOutletTasksCommandHandler : IRequestHandler<AddEditOutletTasksCommand, Result<int>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;

    public AddEditOutletTasksCommandHandler(
        IApplicationDbContextFactory dbContextFactory,
        IMapper mapper
    )
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(AddEditOutletTasksCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        if (request.Id > 0)
        {
            var item = await db.MBLOutletTask.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (item == null) return await Result<int>.FailureAsync($"Product with id: [{request.Id}] not found.");
            item = _mapper.Map(request, item);
            await db.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
            var item = _mapper.Map<MBLOutletTasks>(request);
            db.MBLOutletTask.Add(item);
            await db.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
    }
}