using ElearnApp.Data;
using ElearnApp.Models;
using ElearnApp.ViewModels.Admin.Slider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace ElearnApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sliders.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest();

            Slider slider = await _context.Sliders.FirstOrDefaultAsync(x => x.Id == id);

            if (slider == null) return NotFound();

            return View(slider);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider slider)
        {
            if(ModelState.IsValid)
            {
                return View();
            }
            
            string fileName = Guid.NewGuid().ToString() + "_" + slider.Photo.FileName;

            string path = Path.Combine(_env.WebRootPath,"assets/images",fileName);
            
            using(FileStream stream = new(path, FileMode.Create))
            {
                await slider.Photo.CopyToAsync(stream);
            }

            await _context.Sliders.AddAsync(new Slider { Title = slider.Title, Description = slider.Description, Image = fileName});
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var slider = await _context.Sliders.FindAsync(id);

            string existPath = Path.Combine(_env.WebRootPath, "assets/images", slider.Image);

            DeleteFile(existPath);

            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
			if (id is null) return BadRequest();

			Slider slider = await _context.Sliders.FirstOrDefaultAsync(x => x.Id == id);

			if (slider == null) return NotFound();

            return View(new SliderEditVM { Image = slider.Image, Title = slider.Title, Description = slider.Description,Id = slider.Id});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int? id, SliderEditVM request)
		{
			if (id is null) return BadRequest();

			Slider slider = await _context.Sliders.FirstOrDefaultAsync(x => x.Id == id);

			if (slider == null) return NotFound();

			if (request.Photo != null)
			{
				string existPath = Path.Combine(_env.WebRootPath, "assets/images", slider.Image);
				DeleteFile(existPath);

				string newFileName = Guid.NewGuid().ToString() + "_" + request.Photo.FileName;
				string newPath = Path.Combine(_env.WebRootPath, "assets/images", newFileName);

				using (FileStream stream = new(newPath, FileMode.Create))
				{
					await request.Photo.CopyToAsync(stream);
				}

				slider.Image = newFileName;
			}


			slider.Title = request.Title;
			slider.Description = request.Description;

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}


		private void DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
    }
}
