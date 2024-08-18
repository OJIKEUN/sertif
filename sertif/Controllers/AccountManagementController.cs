using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sertif.Data;
using sertif.Models;
using sertif.ViewModels;
using System.Threading.Tasks;
using System.Linq;

[Authorize(Roles = "Laboran")] // Misalnya hanya Laboran yang bisa mengakses halaman ini
public class AccountManagementController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<AccountManagementController> _logger;

    public AccountManagementController(AppDbContext context, ILogger<AccountManagementController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GetData()
    {
        var users = _context.Users
            .Select(user => new
            {
                user.UserId,
                user.Username,
                user.FullName,
                Role = user.Role.ToString(),
                NIS = user.Role == UserRole.Student ? _context.Students.FirstOrDefault(s => s.UserId == user.UserId).NIS : null,
                NIP = user.Role == UserRole.Laboran ? _context.Laborans.FirstOrDefault(l => l.UserId == user.UserId).NIP : null,
                Class = user.Role == UserRole.Student ? _context.Students.FirstOrDefault(s => s.UserId == user.UserId).Class : null,
                Gender = user.Role == UserRole.Student
                        ? _context.Students.FirstOrDefault(s => s.UserId == user.UserId).Gender
                        : _context.Laborans.FirstOrDefault(l => l.UserId == user.UserId).Gender,
                Religion = user.Role == UserRole.Student
                        ? _context.Students.FirstOrDefault(s => s.UserId == user.UserId).Religion
                        : _context.Laborans.FirstOrDefault(l => l.UserId == user.UserId).Religion,
                PlaceOfBirth = user.Role == UserRole.Student
                        ? _context.Students.FirstOrDefault(s => s.UserId == user.UserId).PlaceOfBirth
                        : _context.Laborans.FirstOrDefault(l => l.UserId == user.UserId).PlaceOfBirth,
                DateOfBirth = user.Role == UserRole.Student
                    ? _context.Students.FirstOrDefault(s => s.UserId == user.UserId).DateOfBirth.HasValue
                        ? _context.Students.FirstOrDefault(s => s.UserId == user.UserId).DateOfBirth.Value.ToString("yyyy-MM-dd")
                        : null
                    : _context.Laborans.FirstOrDefault(l => l.UserId == user.UserId).DateOfBirth.HasValue
                        ? _context.Laborans.FirstOrDefault(l => l.UserId == user.UserId).DateOfBirth.Value.ToString("yyyy-MM-dd")
                        : null,
                HomeAddress = user.Role == UserRole.Student ? _context.Students.FirstOrDefault(s => s.UserId == user.UserId).HomeAddress : null,
                FatherName = user.Role == UserRole.Student ? _context.Students.FirstOrDefault(s => s.UserId == user.UserId).FatherName : null,
                MotherName = user.Role == UserRole.Student ? _context.Students.FirstOrDefault(s => s.UserId == user.UserId).MotherName : null
            })
            .ToList();

        return Json(new { rows = users });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var viewModel = new ManageAccountViewModel
        {
            UserId = user.UserId,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role
        };

        if (user.Role == UserRole.Student)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.UserId);
            if (student != null)
            {
                viewModel.StudentId = student.StudentId;
                viewModel.NIS = student.NIS;
                viewModel.Class = student.Class;
                viewModel.Gender = student.Gender;
                viewModel.Religion = student.Religion;
                viewModel.PlaceOfBirth = student.PlaceOfBirth;
                viewModel.DateOfBirth = student.DateOfBirth;
                viewModel.HomeAddress = student.HomeAddress;
                viewModel.FatherName = student.FatherName;
                viewModel.MotherName = student.MotherName;
            }
        }
        else if (user.Role == UserRole.Laboran)
        {
            var laboran = await _context.Laborans.FirstOrDefaultAsync(l => l.UserId == user.UserId);
            if (laboran != null)
            {
                viewModel.LaboranId = laboran.LaboranId;
                viewModel.NIP = laboran.NIP;
                viewModel.Gender = laboran.Gender;
                viewModel.Religion = laboran.Religion;
                viewModel.PlaceOfBirth = laboran.PlaceOfBirth;
                viewModel.DateOfBirth = laboran.DateOfBirth;
            }
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ManageAccountViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _context.Users.FindAsync(model.UserId);
        if (user == null)
        {
            return NotFound();
        }

        // Update user data
        user.FullName = model.FullName;

        if (user.Role == UserRole.Student)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == model.UserId);
            if (student != null)
            {
                student.NIS = model.NIS;
                student.Class = model.Class;
                student.Gender = model.Gender;
                student.Religion = model.Religion;
                student.PlaceOfBirth = model.PlaceOfBirth;
                student.DateOfBirth = model.DateOfBirth;
                student.HomeAddress = model.HomeAddress;
                student.FatherName = model.FatherName;
                student.MotherName = model.MotherName;
                _context.Students.Update(student);
            }
        }
        else if (user.Role == UserRole.Laboran)
        {
            var laboran = await _context.Laborans.FirstOrDefaultAsync(l => l.UserId == model.UserId);
            if (laboran != null)
            {
                laboran.NIP = model.NIP;
                laboran.Gender = model.Gender;
                laboran.Religion = model.Religion;
                laboran.PlaceOfBirth = model.PlaceOfBirth;
                laboran.DateOfBirth = model.DateOfBirth;
                _context.Laborans.Update(laboran);
            }
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Account updated successfully!";
        return RedirectToAction(nameof(Index)); // Redirect ke Index
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            // Jika pengguna adalah Student atau Laboran, hapus data terkait
            if (user.Role == UserRole.Student)
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.UserId);
                if (student != null)
                {
                    _context.Students.Remove(student);
                }
            }
            else if (user.Role == UserRole.Laboran)
            {
                var laboran = await _context.Laborans.FirstOrDefaultAsync(l => l.UserId == user.UserId);
                if (laboran != null)
                {
                    _context.Laborans.Remove(laboran);
                }
            }

            // Hapus data User
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        return Json(new { success = false });
    }

    private bool UserExists(int id)
    {
        return _context.Users.Any(e => e.UserId == id);
    }
}
