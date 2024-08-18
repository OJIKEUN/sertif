using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sertif.Data;
using sertif.Models;
using sertif.ViewModels;
using System.Security.Claims;

[Authorize]
public class ProfileController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProfileController> _logger;
    public ProfileController(AppDbContext context, ILogger<ProfileController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirstValue("UserId"));
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        var viewModel = new ProfileViewModel
        {
            UserId = user.UserId,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role
        };

        if (user.Role == UserRole.Student)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
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
            var laboran = await _context.Laborans.FirstOrDefaultAsync(l => l.UserId == userId);
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
    public async Task<IActionResult> Update(ProfileViewModel model)
    {
        _logger.LogInformation("POST Update method called.");

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is invalid.");
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    _logger.LogError($"ModelState Error: {state.Key}: {error.ErrorMessage}");
                }
            }
            return View("Index", model);
        }

        var user = await _context.Users.FindAsync(model.UserId);
        if (user == null)
        {
            _logger.LogWarning("User not found.");
            return NotFound();
        }

        // Perbarui data umum pengguna
        user.FullName = model.FullName;

        if (user.Role == UserRole.Student)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == model.UserId);
            if (student == null)
            {
                student = new Student { UserId = model.UserId };
                _context.Students.Add(student);
                await _context.SaveChangesAsync(); // Save changes to generate StudentId
            }

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
        else if (user.Role == UserRole.Laboran)
        {
            var laboran = await _context.Laborans.FirstOrDefaultAsync(l => l.UserId == model.UserId);
            if (laboran == null)
            {
                laboran = new Laboran { UserId = model.UserId };
                _context.Laborans.Add(laboran);
                await _context.SaveChangesAsync(); // Save changes to generate LaboranId
            }

            laboran.NIP = model.NIP;
            laboran.Gender = model.Gender;
            laboran.Religion = model.Religion;
            laboran.PlaceOfBirth = model.PlaceOfBirth;
            laboran.DateOfBirth = model.DateOfBirth;

            _context.Laborans.Update(laboran);
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Profile updated successfully!";
        // Reload data to ensure the updated data is displayed
        return RedirectToAction(nameof(Index));
    }
}