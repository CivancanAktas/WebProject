using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using JobApp.Data;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Collections.Generic;

namespace JobApp.Controllers
{
    [Authorize(Roles = "Employee")]

    // Controller for managing job applications by employees
    public class AppliedJobsController : Controller
    {
        private readonly JobAppContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AppliedJobsController(JobAppContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Show the logged-in employee's applied jobs
public async Task<IActionResult> AppliedJobs()
        {
            // Resolve the current authenticated user's email via UserManager for reliability
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return Unauthorized();
            var userEmail = identityUser.Email;
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var employee = await _context.Employees
                .Include(e => e.AppliedJobs!)
                    .ThenInclude(j => j.Employer)
                .FirstOrDefaultAsync(e => e.Email == userEmail);
            if (employee == null) return Unauthorized();
            var jobs = employee.AppliedJobs ?? new List<JobApp.Models.JobDetails>();
            return View(jobs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Apply: add job to current user's applied jobs list
        public async Task<IActionResult> Apply(int id, string? returnUrl)
        {
            // Use UserManager to get the current user's email to find the employee record
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return Unauthorized();
            var userEmail = identityUser.Email;
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var employee = await _context.Employees.Include(e => e.AppliedJobs).FirstOrDefaultAsync(e => e.Email == userEmail);
            if (employee == null) return Unauthorized();
            var job = await _context.JobDetails.Include(j => j.Employer).FirstOrDefaultAsync(j => j.Id == id);
            if (job == null) return NotFound();
            if (employee.AppliedJobs == null)
                employee.AppliedJobs = new List<JobApp.Models.JobDetails>();
            if (!employee.AppliedJobs.Any(j => j.Id == id))
            {
                employee.AppliedJobs.Add(job);
                await _context.SaveChangesAsync();
            }
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("AppliedJobs");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // CancelApply: remove job from current user's applied jobs list
        public async Task<IActionResult> CancelApply(int id, string? returnUrl)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return Unauthorized();
            var userEmail = identityUser.Email;
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var employee = await _context.Employees.Include(e => e.AppliedJobs).FirstOrDefaultAsync(e => e.Email == userEmail);
            if (employee == null) return Unauthorized();
            var job = await _context.JobDetails.FirstOrDefaultAsync(j => j.Id == id);
            if (job == null) return NotFound();

            if (employee.AppliedJobs != null && employee.AppliedJobs.Any(j => j.Id == id))
            {
                var existing = employee.AppliedJobs.First(j => j.Id == id);
                employee.AppliedJobs.Remove(existing);
                await _context.SaveChangesAsync();
            }
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("AppliedJobs");
        }
    }
}
