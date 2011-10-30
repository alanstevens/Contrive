using Contrive.EntityFramework;

[assembly: WebActivator.PostApplicationStartMethod(
    typeof(Contrive.App_Start.ContriveInitializer), "PostStart")]

namespace Contrive.App_Start
{
  public static class ContriveInitializer
  {
    public static void PostStart()
    {
      new ContextInitialization().Execute();
    }
  }
}