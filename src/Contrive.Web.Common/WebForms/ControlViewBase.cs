using System;
using System.Web.UI;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Web.Common.WebForms
{
  public abstract class ControlViewBase<V, T> : UserControl, IView where V : IView where T : PresenterBase<V>
  {
    protected T _presenter;

    public override void Dispose()
    {
      if ((_presenter != null)) _presenter.Dispose();

      base.Dispose();
    }

    public event EventHandler PreLoad;

    public event EventHandler InitialGet;

    public event EventHandler PostBack;

    public event EventHandler LoadComplete;

    protected override void OnInit(EventArgs e)
    {
      _presenter = ServiceLocator.Current.GetInstance<T>();

      base.OnInit(e);
    }

    protected override void OnLoad(EventArgs e)
    {
      if (PreLoad != null) PreLoad(this, e);

      base.OnLoad(e);

      if (IsPostBack)
      {
        if (PostBack != null) PostBack(this, e);
      }
      else if (InitialGet != null) InitialGet(this, e);

      if (LoadComplete != null) LoadComplete(this, e);
    }
  }
}