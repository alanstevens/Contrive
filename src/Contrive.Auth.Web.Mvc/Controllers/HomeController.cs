﻿using System.Web.Mvc;

namespace Contrive.Auth.Web.Mvc.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      ViewBag.Message = "Welcome to ASP.NET MVC!";

      return View();
    }

    public ActionResult About()
    {
      return View();
    }
  }
}