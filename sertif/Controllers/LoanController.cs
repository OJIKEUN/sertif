using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sertif.Data;
using sertif.Models;
using sertif.ViewModels;
using System.Security.Claims;

[Authorize]
public class LoanController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<LoanController> _logger;

    public LoanController(AppDbContext context, ILogger<LoanController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Menampilkan daftar alat yang bisa dipinjam
    public async Task<IActionResult> Index()
    {
        var equipments = await _context.Equipments
            .Select(e => new LoanViewModel
            {
                EquipmentId = e.EquipmentId,
                Name = e.Name,
                Description = e.Description,
                QuantityAvailable = e.QuantityAvailable
            })
            .ToListAsync();

        return View(equipments);
    }

    // Menampilkan form untuk peminjaman
    public async Task<IActionResult> Loan(int id)
    {
        var equipment = await _context.Equipments.FindAsync(id);
        if (equipment == null || equipment.QuantityAvailable <= 0)
        {
            return NotFound();
        }

        var model = new LoanViewModel
        {
            EquipmentId = equipment.EquipmentId,
            Name = equipment.Name,
            Description = equipment.Description,
            QuantityAvailable = equipment.QuantityAvailable,
            LoanDate = DateTime.Now
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Loan(LoanViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogError("User ID claim is missing or null.");
                return BadRequest("User ID is required to perform this action.");
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogError("User ID claim could not be parsed to an integer.");
                return BadRequest("Invalid User ID.");
            }

            var equipment = await _context.Equipments.FindAsync(model.EquipmentId);
            if (equipment == null || equipment.QuantityAvailable < model.QuantityBorrowed)
            {
                return NotFound("Equipment not found or insufficient quantity available.");
            }

            var loan = new Loan
            {
                EquipmentId = model.EquipmentId,
                UserId = userId,
                Reason = model.Reason,
                QuantityBorrowed = model.QuantityBorrowed,
                LoanDate = model.LoanDate,
                Status = LoanStatus.PendingApproval
            };

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            return RedirectToAction("MyLoans");
        }

        return View(model);
    }

    // Menampilkan daftar peminjaman saya
    public async Task<IActionResult> MyLoans()
    {
        var userIdClaim = User.FindFirstValue("UserId");
        if (string.IsNullOrEmpty(userIdClaim))
        {
            _logger.LogError("User ID claim is missing or null.");
            return BadRequest("User ID is required to perform this action.");
        }

        if (!int.TryParse(userIdClaim, out int userId))
        {
            _logger.LogError("User ID claim could not be parsed to an integer.");
            return BadRequest("Invalid User ID.");
        }

        var loans = await _context.Loans
            .Where(l => l.UserId == userId)
            .Include(l => l.Equipment)
            .ToListAsync();

        return View(loans);
    }

    // Metode Return
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(int id)
    {
        var loan = await _context.Loans.Include(l => l.Equipment).FirstOrDefaultAsync(l => l.LoanId == id);
        if (loan == null)
        {
            return NotFound();
        }

        if (loan.Status != LoanStatus.Approved)
        {
            return BadRequest("Only approved loans can be returned.");
        }

        loan.Status = LoanStatus.Returned;
        loan.ReturnDate = DateTime.Now;
        loan.Equipment.QuantityAvailable += loan.QuantityBorrowed;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(MyLoans));
    }
}

