
using DocumentFormat.OpenXml.Bibliography;
using SFCTOFC.DailySalesPlanManagement.Application.Features.DSPM.Caching;
using SFCTOFC.DailySalesPlanManagement.Application.Features.DSPM.DTOs;
using SFCTOFC.DailySalesPlanManagement.Application.Features.DSPM.Specifications;
using SFCTOFC.DailySalesPlanManagement.Application.Features.Identity.DTOs;

namespace SFCTOFC.DailySalesPlanManagement.Application.Features.DSPM.Queries.Pagination;
public class PaginationQuerySalesmanTracker : AdvancedFilterSalesmanTracker, IRequest<PaginatedData<SalesmanTrackerDto>>
{
    public AdvancedSpecificationSalesmanTracker Specification => new(this);
    public string CacheKey => SalesmanTracketCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => SalesmanTracketCacheKey.Tags;

    // the currently logged in user
    public override string ToString()
    {
        return
            $"CurrentUser:{CurrentUser?.UserId},ListView:{ListView},Search:{Keyword},Name:{Name},Date:{Date},SortDirection:{SortDirection},OrderBy:{OrderBy},{PageNumber},{PageSize}";
    }
}

public class PaginationQuerySalesmanTrackerHandler : IRequestHandler<PaginationQuerySalesmanTracker, PaginatedData<SalesmanTrackerDto>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;
    public PaginationQuerySalesmanTrackerHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper )
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<PaginatedData<SalesmanTrackerDto>> Handle(PaginationQuerySalesmanTracker request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var query = from slm in db.SalesmanTracker.AsQueryable()
                    select new SalesmanTrackerDto
                    {
                        Id = slm.Id,
                        AspNetUserId = slm.AspNetUserId,
                        Name = slm.Name,
                        Date = slm.Date,
                        ActualSales = slm.ActualSales,
                        TargetSales = slm.TargetSales,
                        ActualStoreVisited = slm.ActualStoreVisited,
                        TargetStoreVisited = slm.TargetStoreVisited
                    };

        // Apply sorting
        query = query.OrderBy($"{request.OrderBy} {request.SortDirection}");
        // Count total before applying pagination
        var totalCount = await query.CountAsync(cancellationToken);
        // Apply pagination
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedData<SalesmanTrackerDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
