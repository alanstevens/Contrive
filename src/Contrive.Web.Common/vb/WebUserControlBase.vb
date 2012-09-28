Imports System
Imports System.Web
Imports amm_services.intranet.alcoa.com.Common
Imports amm_services.intranet.alcoa.com.Abstractions

Public MustInherit Class WebUserControlBase(Of V As IWebUI, T As ControllerBase(Of V))
  Inherits System.Web.UI.UserControl
  Implements IWebUI

#Region " Web Form Designer Generated Code "

  'This call is required by the Web Form Designer.
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

  End Sub

  'NOTE: The following placeholder declaration is required by the Web Form Designer.
  'Do not delete or move it.
  Private designerPlaceholderDeclaration As System.Object

  Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
    'CODEGEN: This method call is required by the Web Form Designer
    'Do not modify it using the code editor.
    InitializeComponent()
  End Sub

#End Region

  Protected WithEvents _controller As T

  Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

    MyBase.OnInit(e)

    _controller = IoC.Resolve(Of T)("view", Me)

  End Sub

  Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

    RaiseEvent PreLoad(Me, e)

    MyBase.OnLoad(e)

    If IsPostBack() Then

      RaiseEvent PostBack(Me, e)

    Else

      RaiseEvent InitialGet(Me, e)

    End If

    RaiseEvent LoadComplete(Me, e)

  End Sub

  Public Overrides Sub Dispose()

    If Not _controller Is Nothing Then

      _controller.Dispose()

    End If

    MyBase.Dispose()

  End Sub

  Public Event PreLoad(ByVal sender As Object, ByVal e As EventArgs) Implements IWebUI.PreLoad

  Public Event InitialGet(ByVal sender As Object, ByVal e As EventArgs) Implements IWebUI.InitialGet

  Public Event PostBack(ByVal sender As Object, ByVal e As EventArgs) Implements IWebUI.PostBack

  Public Event LoadComplete(ByVal sender As Object, ByVal e As EventArgs) Implements IWebUI.LoadComplete

  Public Shadows Event PreRender(ByVal sender As Object, ByVal e As EventArgs) Implements IWebUI.PreRender

  Public Shadows Event DataBinding(ByVal sender As Object, ByVal e As EventArgs) Implements IWebUI.DataBinding

  Public Shadows Event Unload(ByVal sender As Object, ByVal e As EventArgs) Implements IWebUI.Unload

  Public Shadows Event [Error](ByVal sender As Object, ByVal e As EventArgs) Implements IWebUI.Error

  Private Sub Base_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.PreRender
    RaiseEvent PreRender(Me, e)
  End Sub
  Private Sub Base_DataBinding(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.DataBinding
    RaiseEvent DataBinding(Me, e)
  End Sub
  Private Sub Base_Unload(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Unload
    RaiseEvent Unload(Me, e)
  End Sub
  Private Sub Base_Error(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Error
    RaiseEvent Error(Me, e)
  End Sub

  Private Sub OnAccessViolation(ByVal sender As Object, ByVal e As EventArgs) Handles _controller.AccessViolation
    Response.Redirect("/Includes/Logon.asp")
  End Sub

End Class
