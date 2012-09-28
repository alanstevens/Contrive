using Contrive.EntityFramework;
using Contrive.Sample.App_Start;
using WebActivator;

[assembly: PostApplicationStartMethod(typeof (ContriveInitializer), "PostStart")]

namespace Contrive.Sample.App_Start
{
  public static class ContriveInitializer
  {
    public static void PostStart()
    {
      ContriveContextInitializer.Initialize();
    }
  }
}