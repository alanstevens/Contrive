Imports System
Imports System.Web

Public Interface IWebUI
  Inherits IDisposable

  Event PreLoad(ByVal sender As Object, ByVal e As EventArgs)

  Event InitialGet(ByVal sender As Object, ByVal e As EventArgs)

  Event PostBack(ByVal sender As Object, ByVal e As EventArgs)

  Event LoadComplete(ByVal sender As Object, ByVal e As EventArgs)

  Event PreRender(ByVal sender As Object, ByVal e As EventArgs)

  Event Unload(ByVal sender As Object, ByVal e As EventArgs)

  Event DataBinding(ByVal sender As Object, ByVal e As EventArgs)

  Event [Error](ByVal sender As Object, ByVal e As EventArgs)

End Interface