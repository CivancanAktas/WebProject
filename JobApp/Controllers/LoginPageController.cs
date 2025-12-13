using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using JobApp.Models;
using JobApp.ViewModels;
using JobApp.Data;
using Microsoft.EntityFrameworkCore;

namespace JobApp.Controllers;

// Controller that handles authentication and registration flows
public class LoginPageController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JobAppIdentityContext _appContext;

    public LoginPageController(SignInManager<IdentityUser> signInManager,
                               UserManager<IdentityUser> userManager,
                               JobAppIdentityContext appContext)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _appContext = appContext;
    }

    public IActionResult LoginPage()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login(string? ReturnUrl)
    {
        // ReturnUrl keeps the url of the action we are coming from, after login action, we want to return where we came from
        ViewData["ReturnUrl"] = ReturnUrl;
        return View("LoginPage");
    }

    [HttpPost]
    // Login POST: verifies credentials and signs in the user, redirecting to ReturnUrl if present
    public async Task<IActionResult> Login(LoginViewModel model, string? ReturnUrl)
    {

        if (!ModelState.IsValid)
            return View("LoginPage", model);

        if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
        {
            ModelState.AddModelError(string.Empty, "Username and password are required.");
            return View("LoginPage", model);
        }

        // Allow users to login using either username OR email
        var user = await _userManager.FindByNameAsync(model.UserName ?? "");
        if (user == null)
        {
            user = await _userManager.FindByEmailAsync(model.UserName ?? "");
        }
        if (user != null)
        {
            var result = await _signInManager.PasswordSignInAsync(user, model.Password ?? "", false, false);
            if (result.Succeeded)
            {
                // Check role and redirect accordingly
                if (await _userManager.IsInRoleAsync(user, "Employee"))
                {
                    var employeeExists = await _appContext.Employees!.AnyAsync(e => e.Email == user.Email);
                    if (employeeExists)
                        return RedirectToAction("Index", "EmployeeProfile");
                    else
                        return RedirectToAction("Access"); // ProfileNotFound yerine Access'e yönlendir
                }

                if (await _userManager.IsInRoleAsync(user, "Employer"))
                {
                    var employerExists = await _appContext.Employers!.AnyAsync(e => e.ContactEmail == user.Email || e.CompanyName == user.UserName);
                    if (employerExists)
                        return RedirectToAction("Index", "EmployerProfile");
                    else
                        return RedirectToAction("Access"); // ProfileNotFound yerine Access'e yönlendir
                }

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    return RedirectToAction("Index", "JobPost");

                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    return Redirect(ReturnUrl);
                return RedirectToAction("Index", "Home");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account locked. Try again later.");
            }
            else if (result.RequiresTwoFactor)
            {
                return RedirectToAction("LoginWith2fa", new { ReturnUrl });
            }
        }

        // Eğer AccesDenied view'ına yönlendirmek istersen:
        // return RedirectToAction("AccesDenied");
        ModelState.AddModelError(string.Empty, "Invalid username or password.");
        return View("LoginPage", model);
    }
    
    [HttpGet]
    public IActionResult RegisterEmployee()
    {
        return View();
    }

    [HttpPost]
    // RegisterEmployee: create Identity user, assign Employee role and sign in
    public async Task<IActionResult> RegisterEmployee(RegisterEmployeeViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
        {
            ModelState.AddModelError(string.Empty, "Email and password are required.");
            return View(model);
        }

        var user = new IdentityUser
        {
            // Use the supplied username for login and keep the email separately
            UserName = model.UserName ?? model.Email,
            Email = model.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password ?? "");
        if (result.Succeeded)
        {
            // Assign Employee role
            await _userManager.AddToRoleAsync(user, "Employee");

            // Save Employee to database
            var employee = new Employee
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = ""
            };
            _appContext.Employees!.Add(employee);
            await _appContext.SaveChangesAsync();

            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index", "EmployeeProfile"); // Kayıttan sonra profile'a yönlendir
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    [HttpGet]
    public IActionResult RegisterEmployer()
    {
        return View();
    }

    [HttpPost]
    // RegisterEmployer: create Identity user, assign Employer role and sign in
    public async Task<IActionResult> RegisterEmployer(RegisterEmployerViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (string.IsNullOrEmpty(model.CompanyName) || string.IsNullOrEmpty(model.ContactEmail) || string.IsNullOrEmpty(model.Password))
        {
            ModelState.AddModelError(string.Empty, "Company name, email and password are required.");
            return View(model);
        }

        var user = new IdentityUser
        {
            UserName = model.CompanyName,
            Email = model.ContactEmail,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password ?? "");
        if (result.Succeeded)
        {
            // Assign Employer role
            await _userManager.AddToRoleAsync(user, "Employer");

            // Save Employer to database
            var employer = new Employer
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                ContactEmail = model.ContactEmail,
                CompanyName = model.CompanyName,
                Address = ""
            };
            _appContext.Employers!.Add(employer);
            await _appContext.SaveChangesAsync();

            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index", "EmployerProfile"); // Kayıttan sonra profile'a yönlendir
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    [HttpGet]
    [ActionName("AccesDenied")]
    public IActionResult AccesDenied()
    {
        return View("AccesDenied");
    }

    // Logout: sign the user out and redirect to home
public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}