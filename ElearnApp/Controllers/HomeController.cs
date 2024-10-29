using ElearnApp.Data;
using ElearnApp.Models;
using ElearnApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElearnApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Slider> sliders = await _context.Sliders.ToListAsync();
            List<Course> courses = await _context.Courses.Include(m=>m.Images).Include(m=>m.Category).ToListAsync();

            HomeVM model = new() 
            {
                Sliders = sliders,
                Courses = courses
            };
            return View(model);
        }
    }
}
