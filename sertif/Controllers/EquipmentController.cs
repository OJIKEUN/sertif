using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sertif.Data;
using sertif.Models;
using System.Linq;
using System.Threading.Tasks;

namespace sertif.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EquipmentController> _logger;

        public EquipmentController(AppDbContext context, ILogger<EquipmentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Equipment
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetData()
        {
            var equipments = _context.Equipments
                .Select(equipment => new
                {
                    equipment.EquipmentId,
                    equipment.Name,
                    equipment.Description,
                    equipment.QuantityAvailable
                })
                .ToList();

            return Json(new { rows = equipments });
        }

        // GET: Equipment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipments
                .FirstOrDefaultAsync(m => m.EquipmentId == id);

            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        // GET: Equipment/Create
        [Authorize(Roles = "Laboran")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Laboran")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EquipmentId,Name,Description,QuantityAvailable")] Equipment equipment)
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogError($"ModelState Error: {state.Key}: {error.ErrorMessage}");
                    }
                }
                return View(equipment);
            }

            try
            {
                _context.Add(equipment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Account updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating equipment: {ex.Message}");
                return View(equipment);
            }
        }

        // GET: Equipment/Edit/5
        [Authorize(Roles = "Laboran")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipments.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }
            return View(equipment);
        }

        // POST: Equipment/Edit/5
        [Authorize(Roles = "Laboran")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EquipmentId,Name,Description,QuantityAvailable")] Equipment equipment)
        {
            // Logging untuk memastikan id dan EquipmentId yang diterima
            _logger.LogInformation($"Received id: {id}, EquipmentId from model: {equipment.EquipmentId}");

            if (id != equipment.EquipmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipment);
                    TempData["SuccessMessage"] = "Account updated successfully!";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipmentExists(equipment.EquipmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(equipment);
        }


        // POST: Equipment/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipment = await _context.Equipments.FindAsync(id);
            if (equipment != null)
            {
                _context.Equipments.Remove(equipment);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        private bool EquipmentExists(int id)
        {
            return _context.Equipments.Any(e => e.EquipmentId == id);
        }
    }
}
