Imports System
Imports System.Web
Imports System.Collections.Specialized
Imports amm_services.intranet.alcoa.com.Abstractions

Public MustInherit Class ControllerBase(Of T As IWebUI)
  Implements IDisposable

  Protected ReadOnly _view As T

  Protected ReadOnly _security As ISecurityService

  Protected ReadOnly _logger As ILoggingService

  Protected ReadOnly _props As IServerProperties

  Private Sub SetupView()

    AddHandler _view.PreLoad, AddressOf UI_PreLoad
    AddHandler _view.InitialGet, AddressOf UI_InitialGet
    AddHandler _view.PostBack, AddressOf UI_PostBack
    AddHandler _view.LoadComplete, AddressOf UI_LoadComplete
    AddHandler _view.PreRender, AddressOf UI_PreRender
    AddHandler _view.DataBinding, AddressOf UI_DataBinding
    AddHandler _view.Unload, AddressOf UI_Unload
    AddHandler _view.Error, AddressOf UI_Error

  End Sub

  Protected Sub New(ByVal view As T, _
    ByVal props As IServerProperties, _
    ByVal sec As ISecurityService, _
    ByVal logger As ILoggingService)

    _view = view

    SetupView()

    _props = props

    _security = sec

    _logger = logger

    _logger.BeginRequest()

  End Sub

#Region "Event Handlers"

  Private Sub UI_PreLoad(ByVal sender As Object, ByVal e As EventArgs) 'Handles _ui.PreLoad
    OnPreLoad(sender, e)
  End Sub
  Private Sub UI_InitialGet(ByVal sender As Object, ByVal e As EventArgs) 'Handles _ui.InitialGet
    OnInitialGet(sender, e)
  End Sub
  Private Sub UI_PostBack(ByVal sender As Object, ByVal e As EventArgs) 'Handles _ui.PostBack
    OnPostBack(sender, e)
  End Sub
  Private Sub UI_LoadComplete(ByVal sender As Object, ByVal e As EventArgs) 'Handles _ui.LoadComplete
    OnLoadComplete(sender, e)
  End Sub
  Private Sub UI_PreRender(ByVal sender As Object, ByVal e As EventArgs) 'Handles _ui.PreRender
    OnPreRender(sender, e)
  End Sub
  Private Sub UI_DataBinding(ByVal sender As Object, ByVal e As EventArgs) 'Handles _ui.DataBinding
    OnDataBinding(sender, e)
  End Sub
  Private Sub UI_Unload(ByVal sender As Object, ByVal e As EventArgs) 'Handles _ui.Unload
    OnUnload(sender, e)
    _logger.EndRequest()
  End Sub
  Private Sub UI_Error(ByVal sender As Object, ByVal e As EventArgs) 'Handles _ui.Error
    OnError(sender, e)
  End Sub

#End Region

  Public Overridable Sub OnPreLoad(ByVal sender As Object, ByVal e As EventArgs)
  End Sub
  Public Overridable Sub OnInitialGet(ByVal sender As Object, ByVal e As EventArgs)
  End Sub
  Public Overridable Sub OnPostBack(ByVal sender As Object, ByVal e As EventArgs)
  End Sub
  Public Overridable Sub OnLoadComplete(ByVal sender As Object, ByVal e As EventArgs)
  End Sub
  Public Overridable Sub OnPreRender(ByVal sender As Object, ByVal e As EventArgs)
  End Sub
  Public Overridable Sub OnDataBinding(ByVal sender As Object, ByVal e As EventArgs)
  End Sub
  Public Overridable Sub OnUnload(ByVal sender As Object, ByVal e As EventArgs)
  End Sub
  Public Overridable Sub OnDisposed(ByVal sender As Object, ByVal e As EventArgs)
  End Sub
  Public Overridable Sub OnError(ByVal sender As Object, ByVal e As EventArgs)
  End Sub

  Public MustOverride Sub Dispose() Implements IDisposable.Dispose

  Protected Sub OnAccessViolation(ByVal e As EventArgs)
    RaiseEvent AccessViolation(Me, e)
  End Sub

  Public Event AccessViolation(ByVal sender As Object, ByVal e As EventArgs)

End Class
