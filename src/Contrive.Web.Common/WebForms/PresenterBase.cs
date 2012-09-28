using System;
using Contrive.Common;

namespace Contrive.Web.Common.WebForms
{
  public abstract class PresenterBase<T> : DisposableBase where T : IView
  {
    protected PresenterBase(T view)
    {
      _view = view;

      BindToView();
    }

    public delegate void AccessViolationEventHandler(object sender, EventArgs e);

    protected readonly T _view;

    void BindToView()
    {
      EventHandler viewOnPreLoad = (o, args) => OnPreLoad();
      _view.PreLoad += viewOnPreLoad;
      _view.InitialGet += (o, args) => OnInitialGet();
      _view.PostBack += (o, args) => OnPostBack();
      _view.LoadComplete += (o, args) => OnLoadComplete();
      _view.PreRender += (o, args) => OnPreRender();
      _view.DataBinding += (o, args) => OnDataBinding();
      _view.Unload += (o, args) => OnUnload();
      _view.Error += (o, args) => OnError();
    }

    public virtual void OnPreLoad() {}

    public virtual void OnInitialGet() {}

    public virtual void OnPostBack() {}

    public virtual void OnLoadComplete() {}

    public virtual void OnPreRender() {}

    public virtual void OnDataBinding() {}

    public virtual void OnUnload() {}

    public virtual void OnError() {}

    protected void OnAccessViolation(EventArgs e)
    {
      if (AccessViolation != null) AccessViolation(this, e);
    }

    public event AccessViolationEventHandler AccessViolation;
  }
}