namespace JobApp.Controllers
{
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using JobApp.Data;
	using JobApp.Models;
	using Microsoft.EntityFrameworkCore;

	[Authorize(Roles = "Employee")]
	public class EmployeeProfileController : Controller
	{
		private readonly JobAppIdentityContext _context;
		private readonly UserManager<IdentityUser> _userManager;

		public EmployeeProfileController(JobAppIdentityContext context, UserManager<IdentityUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		// GET: Profile
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Challenge();

			// Try find employee by email
			var employee = await _context.Employees!.FirstOrDefaultAsync(e => e.Email == user.Email);
			if (employee == null)
			{
				// No employee record yet - offer to create one
				return RedirectToAction("RegisterEmployee", "LoginPage");
			}

			return View("~/Views/ProfilePage/EmployeeProfile.cshtml", employee);
		}

		// GET: Profile/Edit
		public async Task<IActionResult> Edit()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Challenge();

			var employee = await _context.Employees!.FirstOrDefaultAsync(e => e.Email == user.Email);
			if (employee == null) return RedirectToAction("Index");

			return View(employee);
		}

		// POST: Profile/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber")] Employee model)
		{
			if (id != model.Id) return NotFound();
			if (!ModelState.IsValid) return View(model);

			try
			{
				_context.Update(model);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!_context.Employees!.Any(e => e.Id == model.Id)) return NotFound();
				throw;
			}

			return RedirectToAction(nameof(Index));
		}
	}
}

