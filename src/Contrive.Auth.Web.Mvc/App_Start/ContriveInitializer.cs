using Contrive.Auth.EntityFramework;
using Contrive.Auth.Web.Mvc.App_Start;
using WebActivator;

[assembly: PostApplicationStartMethod(typeof (ContriveInitializer), "PostStart")]

namespace Contrive.Auth.Web.Mvc.App_Start
{
  public static class ContriveInitializer
  {
    public static void PostStart()
    {
      ContriveContextInitializer.Initialize();
    }
  }
}