
namespace JobApp.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Authorization;
    using JobApp.Data;
    using JobApp.Models;
        
    
    using Microsoft.AspNetCore.Identity;

    // Controller that handles job postings: listing, details, CRUD and applicant management
public class JobPostController : Controller
    {
        private readonly JobAppContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public JobPostController(JobAppContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: JobPost
        // Index: list jobs with filtering/paging and view context for user role and current page
public async Task<IActionResult> Index(string jobSearchString, string jobType, string location, int? year, int page = 1, int pageSize = 5)
        {
            var jobsQuery = _context.JobDetails
                .Include(j => j.Employer)
                .Include(j => j.Employees)
                .AsQueryable();

            if (!string.IsNullOrEmpty(jobSearchString))
            {
                jobsQuery = jobsQuery.Where(j => j.Title!.Contains(jobSearchString) || j.Description!.Contains(jobSearchString) || j.Company!.Contains(jobSearchString));
            }

            ViewData["JobSearchString"] = jobSearchString;

            // years for dropdown
            ViewData["Years"] = await _context.JobDetails
                .Select(j => j.PostedDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();
            ViewData["Year"] = year;
            if (year.HasValue)
                jobsQuery = jobsQuery.Where(j => j.PostedDate.Year == year.Value);

            // job types and locations for dropdowns
            ViewData["JobTypes"] = await _context.JobDetails.Select(j => j.JobType).Distinct().OrderBy(t => t).ToListAsync();
            ViewData["Locations"] = await _context.JobDetails.Select(j => j.Location).Distinct().OrderBy(l => l).ToListAsync();
            ViewData["JobType"] = jobType;
            ViewData["Location"] = location;

            if (!string.IsNullOrEmpty(jobType))
                jobsQuery = jobsQuery.Where(j => j.JobType == jobType);

            if (!string.IsNullOrEmpty(location))
                jobsQuery = jobsQuery.Where(j => j.Location == location);

            var totalNumberOfJobs = await jobsQuery.CountAsync();
            var jobs = await jobsQuery
                .OrderBy(j => j.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // include current user info for view logic (owner vs applicant visibility)
            var identityUser = await _userManager.GetUserAsync(User);
            ViewData["CurrentUserEmail"] = identityUser?.Email;
            ViewData["IsEmployer"] = User.IsInRole("Employer");

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalNumberOfJobs / (double)pageSize);

            return View(jobs);
        }

        // GET: JobPost/Details/5
        [Authorize]
        // Details: show job information; employers see applicants when owner
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var job = await _context.JobDetails
                .Include(j => j.Employer)
                .Include(j => j.Employees)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null) return NotFound();

            // If current user is Employer and owner of this job, show details view with applicants
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser != null && User.IsInRole("Employer") && job.Employer != null)
            {
                var isOwner = job.Employer.ContactEmail == identityUser.Email || job.Employer.CompanyName == identityUser.UserName;
                if (isOwner)
                {
                    return View("DetailsWithApplicants", job);
                }
            }

            // If current user is Employee, tell the view whether they have already applied
            if (identityUser != null && User.IsInRole("Employee"))
            {
                var employee = await _context.Employees.Include(e => e.AppliedJobs).FirstOrDefaultAsync(e => e.Email == identityUser.Email);
                ViewData["HasApplied"] = employee != null && employee.AppliedJobs != null && employee.AppliedJobs.Any(j => j.Id == id);
            }

            // Default details view for other users
            return View(job);
        }

        // GET: JobPost/Applicants/5
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Applicants(int? id)
        {
            if (id == null) return NotFound();
            var job = await _context.JobDetails.Include(j => j.Employer).Include(j => j.Employees).FirstOrDefaultAsync(j => j.Id == id);
            if (job == null) return NotFound();

            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return Unauthorized();

            var isOwner = job.Employer != null && (job.Employer.ContactEmail == identityUser.Email || job.Employer.CompanyName == identityUser.UserName);
            if (!isOwner) return Forbid();

            var applicants = job.Employees ?? new List<Employee>();
            return View(applicants);
        }


        // GET: JobPost/Post
        [Authorize(Roles = "Employer")]
        public IActionResult Post()
        {
            ViewData["EmployerId"] = new SelectList(_context.Employers.OrderBy(e => e.CompanyName), "Id", "CompanyName");
            return View(); // Post.cshtml view'ı döner
        }

        // GET: JobPost/Create
        [Authorize(Roles = "Employer")]
        // Display Create form for employers to add a new job
public IActionResult Create()
        {
            ViewData["EmployerId"] = new SelectList(_context.Employers.OrderBy(e => e.CompanyName), "Id", "CompanyName");
            return View();
        }

        // POST: JobPost/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Company,Location,Salary,JobType,PostedDate")] JobDetails job, int? employerId)
        {
            if (ModelState.IsValid)
            {
                if (employerId.HasValue)
                {
                    var emp = await _context.Employers.FindAsync(employerId.Value);
                    job.Employer = emp;
                }
                job.PostedDate = job.PostedDate == default ? DateTime.Now : job.PostedDate;
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployerId"] = new SelectList(_context.Employers.OrderBy(e => e.CompanyName), "Id", "CompanyName", employerId);
            return View(job);
        }

        // GET: JobPost/Edit/5
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var job = await _context.JobDetails.Include(j => j.Employer).FirstOrDefaultAsync(j => j.Id == id);
            if (job == null) return NotFound();

            // Only the employer who owns the job (or Admin via role) may edit
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return Challenge();

            var isOwner = job.Employer != null && (job.Employer.ContactEmail == identityUser.Email || job.Employer.CompanyName == identityUser.UserName);
            if (!isOwner && !User.IsInRole("Admin")) return Forbid();

            ViewData["EmployerId"] = new SelectList(_context.Employers.OrderBy(e => e.CompanyName), "Id", "CompanyName", job.Employer?.Id);
            return View(job);
        }

        // POST: JobPost/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Company,Location,Salary,JobType,PostedDate")] JobDetails job, int? employerId)
        {
            if (id != job.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Verify the current user owns this job before applying updates
                    var existing = await _context.JobDetails.Include(j => j.Employer).FirstOrDefaultAsync(j => j.Id == id);
                    var identityUser = await _userManager.GetUserAsync(User);
                    if (identityUser == null) return Challenge();

                    var isOwner = existing != null && existing.Employer != null && (existing.Employer.ContactEmail == identityUser.Email || existing.Employer.CompanyName == identityUser.UserName);
                    if (!isOwner && !User.IsInRole("Admin")) return Forbid();

                    if (employerId.HasValue)
                    {
                        var emp = await _context.Employers.FindAsync(employerId.Value);
                        job.Employer = emp;
                    }
                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployerId"] = new SelectList(_context.Employers.OrderBy(e => e.CompanyName), "Id", "CompanyName", employerId);
            return View(job);
        }

        // GET: JobPost/Delete/5
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var job = await _context.JobDetails
                .Include(j => j.Employer)
                .FirstOrDefaultAsync(j => j.Id == id);
            if (job == null) return NotFound();

            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return Challenge();

            // Only allow the employer who owns this job or Admin to access Delete
            var isOwner = job.Employer != null && (job.Employer.ContactEmail == identityUser.Email || job.Employer.CompanyName == identityUser.UserName);
            if (!isOwner && !User.IsInRole("Admin")) return Forbid();

            return View(job);
        }

        // POST: JobPost/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer")]
        // DeleteConfirmed: remove job from database (only owner or Admin)
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var job = await _context.JobDetails.Include(j => j.Employer).FirstOrDefaultAsync(j => j.Id == id);
            if (job == null) return NotFound();

            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return Challenge();

            var isOwner = job.Employer != null && (job.Employer.ContactEmail == identityUser.Email || job.Employer.CompanyName == identityUser.UserName);
            if (!isOwner && !User.IsInRole("Admin")) return Forbid();

            _context.JobDetails.Remove(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobExists(int id)
        {
            return _context.JobDetails.Any(e => e.Id == id);
        }
    }
}