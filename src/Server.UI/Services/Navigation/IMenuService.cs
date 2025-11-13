using SFCTOFC.DailySalesPlanManagement.Server.UI.Models.NavigationMenu;

namespace SFCTOFC.DailySalesPlanManagement.Server.UI.Services.Navigation;

public interface IMenuService
{
    IEnumerable<MenuSectionModel> Features { get; }
 
}

