using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MunicipalServicesMvc.Models;
using MunicipalServicesMvc.Services;

namespace MunicipalServicesMvc.Controllers
{
    public class IssuesController : Controller
    {
        private readonly IssueStore _store;
        private readonly IWebHostEnvironment _env;

        public IssuesController(IssueStore store, IWebHostEnvironment env)
        {
            _store = store; _env = env;
        }

        [HttpGet]
        public IActionResult Create() => View(new IssueCreateViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IssueCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var issue = new Issue
            {
                Location = vm.Location.Trim(),
                Category = vm.Category.Trim(),
                Description = vm.Description.Trim(),
                Status = "Received",
                CreatedAt = DateTime.UtcNow
            };

            // Save to our custom DS (assigns Id)
            _store.Add(issue);

            // Save uploads to wwwroot/uploads/{id}/ and record in custom AttachmentList
            if (vm.Files != null && vm.Files.Count > 0)
            {
                var folder = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", issue.Id.ToString());
                Directory.CreateDirectory(folder);

                for (int i = 0; i < vm.Files.Count; i++)
                {
                    var f = vm.Files[i];
                    if (f.Length <= 0) continue;
                    var safeName = Path.GetFileName(f.FileName);
                    var full = Path.Combine(folder, safeName);
                    using (var fs = new FileStream(full, FileMode.Create))
                    {
                        await f.CopyToAsync(fs);
                    }
                    issue.Attachments.Add($"/uploads/{issue.Id}/{safeName}");
                }
            }

            return RedirectToAction(nameof(Details), new { id = issue.Id });
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var issue = _store.Get(id);
            if (issue == null) return NotFound();
            return View(issue);
        }
    }
}
