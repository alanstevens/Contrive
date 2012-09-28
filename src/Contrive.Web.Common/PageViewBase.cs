using System;
using System.Web.UI;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Web.Common
{
  public abstract class PageViewBase<V, T> : Page, IView where V : IView where T : PresenterBase<V>
  {
    protected T _presenter;

    public override void Dispose()
    {
      if ((_presenter != null)) _presenter.Dispose();

      base.Dispose();
    }

    public event EventHandler InitialGet;

    public event EventHandler PostBack;

    protected override void OnInit(EventArgs e)
    {
      base.OnInit(e);

      _presenter = ServiceLocator.Current.GetInstance<T>();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      if (IsPostBack)
      {
        if (PostBack != null) PostBack(this, e);
      }
      else if (InitialGet != null) InitialGet(this, e);
    }
  }
}