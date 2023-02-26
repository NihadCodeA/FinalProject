using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace FinalProject.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SliderController : Controller
    {
        private readonly Database _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public SliderController(Database context, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int page=1)
        {
            //------------------------------
            ViewData["PageName"] = "Sliders";
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (user == null) return NotFound();
            ViewData["User"] = user;
            //------------------------------
            var query = _context.Sliders.AsQueryable();
            var pagenatedSliders = PaginatedList<Slider>.Create(query, 15, page);
            return View(pagenatedSliders);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //------------------------------
            ViewData["PageName"] = "Sliders";
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (user == null) return NotFound();
            ViewData["User"] = user;
            //------------------------------
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Slider slider)
        {
            //------------------------------
            ViewData["PageName"] = "Sliders";
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (user == null) return NotFound();
            ViewData["User"] = user;
            //------------------------------
            if (!ModelState.IsValid) return View();
            if (slider.ImageFile != null)
            {
                if (slider.ImageFile.Length > 3145728)
                {
                    ModelState.AddModelError("ImageFile", "Şəklin ölçüsü 3mb-dən çox ola bilməz!");
                    return View();
                }
            slider.Image = FileManager.SaveFile(_env.WebRootPath, "uploads/sliders", slider.ImageFile);
            }
            else
            {
                ModelState.AddModelError("ImageFile","Şəkil yükləmək məcburidi!");
                return View();
            }
            _context.Sliders.Add(slider);
            _context.SaveChanges();
            return RedirectToAction("Index","Slider",new { area="manage"});
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //------------------------------
            ViewData["PageName"] = "Sliders";
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (user == null) return NotFound();
            ViewData["User"] = user;
            //------------------------------
            Slider slider = _context.Sliders.Find(id);
            if (slider == null) View("Error");
            return View(slider);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Slider slider)
        {
            //------------------------------
            ViewData["PageName"] = "Sliders";
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (user == null) return NotFound();
            ViewData["User"] = user;
            //------------------------------
            Slider existSlider = _context.Sliders.Find(slider.Id);
            if (slider == null) View("Error");
            if (!ModelState.IsValid) return View();
            if (slider.ImageFile != null)
            {
                FileManager.DeleteFile(_env.WebRootPath, "uploads/sliders", existSlider.Image);
                if (slider.ImageFile.Length > 3145728)
                {
                    ModelState.AddModelError("PosterImgFile", "Şəklin ölçüsü 3mb-dən çox ola bilməz!");
                    return View();
                }
                existSlider.Image = FileManager.SaveFile(_env.WebRootPath, "uploads/sliders", slider.ImageFile);
            }
            existSlider.Order = slider.Order;
            existSlider.RedirectUrl = slider.RedirectUrl;
            existSlider.RedirectTextEn = slider.RedirectTextEn;
            existSlider.RedirectTextAz = slider.RedirectTextAz;
            existSlider.HeaderTextEn = slider.HeaderTextEn;
            existSlider.HeaderTextAz = slider.HeaderTextAz;
            existSlider.DetailEn=slider.DetailEn;
            existSlider.DetailAz = slider.DetailAz;
            existSlider.MessageEn = slider.MessageEn;
            existSlider.MessageAz = slider.MessageAz; 
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            //------------------------------
            ViewData["PageName"] = "Sliders";
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (user == null) return NotFound();
            ViewData["User"] = user;
            //------------------------------

            Slider slider = _context.Sliders.Find(id);
            if (slider == null) NotFound(); //404
            if (slider.Image != null)
            {
                FileManager.DeleteFile(_env.WebRootPath, "uploads/sliders", slider.Image);
            }
            _context.Sliders.Remove(slider);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
