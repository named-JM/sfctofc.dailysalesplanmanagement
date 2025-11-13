using ActualLab.Fusion;
using SFCTOFC.DailySalesPlanManagement.Application.Common.Interfaces.Identity;

namespace SFCTOFC.DailySalesPlanManagement.Application.Features.Fusion;

public interface IUserSessionTracker: IComputeService
{
    Task AddUserSession(string pageComponent, UserContext userContext, CancellationToken cancellationToken = default);
    Task RemoveUserSession(string pageComponent,string userId,  CancellationToken cancellationToken = default);
    Task RemoveAllSessions(string userId, CancellationToken cancellationToken = default);
   
    [ComputeMethod]
    Task<List<UserContext>> GetUserSessions(string pageComponent,CancellationToken cancellationToken = default);
}
