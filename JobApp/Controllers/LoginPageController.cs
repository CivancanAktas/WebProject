using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using JobApp.Models;

namespace JobApp.Controllers;

public class LoginPageController : Controller
{
    public IActionResult LoginPage()
    {
        return View();
    }

    public IActionResult RegisterEmployee()
    {
        return View();
    }

    public IActionResult RegisterEmployer()
    {
        return View();
    }
}
