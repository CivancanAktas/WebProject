namespace JobApp.Controllers
{
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using JobApp.Data;
	using JobApp.Models;
	using Microsoft.EntityFrameworkCore;

	[Authorize(Roles = "Employer")]
	public class EmployerProfileController : Controller
	{
		private readonly JobAppIdentityContext _context;
		private readonly UserManager<IdentityUser> _userManager;

		public EmployerProfileController(JobAppIdentityContext context, UserManager<IdentityUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		// GET: Profile
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Challenge();

			// Try find employer by contact email or username
			var employer = await _context.Employers!.FirstOrDefaultAsync(e => e.ContactEmail == user.Email || e.CompanyName == user.UserName);
			if (employer == null)
			{
				return RedirectToAction("RegisterEmployer", "LoginPage");
			}

			return View("~/Views/ProfilePage/EmployerProfile.cshtml", employer);
		}

		// GET: Profile/Edit
		public async Task<IActionResult> Edit()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Challenge();

			var employer = await _context.Employers!.FirstOrDefaultAsync(e => e.ContactEmail == user.Email || e.CompanyName == user.UserName);
			if (employer == null) return RedirectToAction("Index");

			return View(employer);
		}

		// POST: Profile/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Address,ContactEmail,CompanyName")] Employer model)
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
				if (!_context.Employers!.Any(e => e.Id == model.Id)) return NotFound();
				throw;
			}

			return RedirectToAction(nameof(Index));
		}
	}
}

