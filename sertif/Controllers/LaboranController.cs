using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sertif.Data;
using sertif.Models;

namespace sertif.Controllers
{
    [Authorize(Roles = "Laboran")]
    public class LaboranController : Controller
    {
        private readonly AppDbContext _context;

        public LaboranController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> UserLoans()
        {
            var loans = await _context.Loans
                .Include(l => l.User)
                .Include(l => l.Equipment)
                .ToListAsync();

            return View(loans);
        }

        // Menampilkan permintaan peminjaman untuk approval
        public async Task<IActionResult> Approvals()
        {
            var loans = await _context.Loans
                .Where(l => l.Status == LoanStatus.PendingApproval)
                .Include(l => l.Equipment)
                .Include(l => l.User)
                .ToListAsync();

            return View(loans);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var loan = await _context.Loans.Include(l => l.Equipment).FirstOrDefaultAsync(l => l.LoanId == id);
            if (loan == null)
            {
                return NotFound();
            }

            loan.Status = LoanStatus.Approved;
            loan.Equipment.QuantityAvailable -= loan.QuantityBorrowed;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Approvals));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var loan = await _context.Loans.FirstOrDefaultAsync(l => l.LoanId == id);
            if (loan == null)
            {
                return NotFound();
            }

            loan.Status = LoanStatus.Rejected;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Approvals));
        }
    }
}
