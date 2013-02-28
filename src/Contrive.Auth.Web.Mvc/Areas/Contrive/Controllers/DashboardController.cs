using System.Web.Mvc;
using Contrive.Auth.Membership;
using Contrive.Auth.Web.Mvc.Areas.Contrive.Models;

namespace Contrive.Auth.Web.Mvc.Areas.Contrive.Controllers
{
  [Authorize(Roles = "Admin")]
  public class DashboardController : Controller
  {
    public DashboardController(IRoleService roleService, IUserService userService)
    {
      _roleService = roleService;
      _userService = userService;
    }

    readonly IRoleService _roleService;
    readonly IUserService _userService;

    public virtual ActionResult Index()
    {
      var viewModel = new DashboardViewModel();
      //viewModel.TotalUserCount = _userService.GetAllUsers().Count.ToString();
      //viewModel.TotalUsersOnlineCount = _userService.GetNumberOfUsersOnline().ToString();
      //viewModel.TotalRolesCount = _roleService.GetAllRoles().Length.ToString();

      return View(viewModel);
    }
  }
}