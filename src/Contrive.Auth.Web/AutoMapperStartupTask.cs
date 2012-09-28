using System.Web.Security;
using AutoMapper;
using Contrive.Common;

namespace Contrive.Auth.Web
{
  public class AutoMapperStartupTask : IStartupTask
  {
    public void OnStartup()
    {
      Mapper.CreateMap<UserCreateStatus, MembershipCreateStatus>();
    }
  }
}