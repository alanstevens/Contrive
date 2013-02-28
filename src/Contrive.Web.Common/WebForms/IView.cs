using System;

namespace Contrive.Web.Common.WebForms
{
  public interface IView : IDisposable
  {
    event EventHandler PreLoad;

    event EventHandler InitialGet;

    event EventHandler PostBack;

    event EventHandler LoadComplete;

    event EventHandler PreRender;

    event EventHandler Unload;

    event EventHandler DataBinding;

    event EventHandler Error;
  }
}