using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CreditGuardAPI.Data;
using CreditGuardAPI.Models;

namespace CreditGuardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DebtController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DebtController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("record")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<Debt>> RecordDebt([FromBody] Debt debt)
        {
            // Validate Group exists
            var group = await _context.Groups.FindAsync(debt.GroupId);
            if (group == null)
            {
                return BadRequest("Invalid Group");
            }

            // Set initial status and dates
            debt.Status = DebtStatus.Active;
            debt.StartDate = DateTime.UtcNow;
            debt.RemainingAmount = debt.TotalAmount;

            // Calculate EMI details
            if (debt.TotalEMICount <= 0)
            {
                return BadRequest("Total EMI count must be greater than zero");
            }
            debt.EMIAmount = Math.Round(debt.TotalAmount / debt.TotalEMICount, 2);

            // Calculate end date based on EMI count (assuming monthly EMIs)
            debt.EndDate = debt.StartDate.Value.AddMonths(debt.TotalEMICount);

            _context.Debts.Add(debt);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDebt), new { id = debt.Id }, debt);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Debt>> GetDebt(int id)
        {
            var debt = await _context.Debts
                .Include(d => d.Group)
                .Include(d => d.EmiTransactions)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (debt == null)
            {
                return NotFound();
            }

            return debt;
        }

        [HttpPost("emi-payment")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<EmiTransaction>> RecordEMIPayment([FromBody] EmiTransaction emiPayment)
        {
            // Validate Debt
            var debt = await _context.Debts.FindAsync(emiPayment.DebtId);
            if (debt == null)
            {
                return BadRequest("Invalid Debt");
            }

            // Validate Customer
            var customer = await _context.Customers.FindAsync(emiPayment.CustomerId);
            if (customer == null)
            {
                return BadRequest("Invalid Customer");
            }

            // Validate Group
            var group = await _context.Groups.FindAsync(emiPayment.GroupId);
            if (group == null)
            {
                return BadRequest("Invalid Group");
            }

            // Set payment details
            emiPayment.DueDate = debt.StartDate.Value.AddMonths(emiPayment.EMINumber);
            emiPayment.PaidDate = DateTime.UtcNow;
            emiPayment.Status = EmiTransactionStatus.Paid;
            emiPayment.PaidAmount = emiPayment.EMIAmount;

            // Update debt remaining amount
            debt.RemainingAmount -= emiPayment.EMIAmount;
            if (debt.RemainingAmount <= 0)
            {
                debt.Status = DebtStatus.Completed;
            }

            _context.EmiTransactions.Add(emiPayment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEMITransaction), new { id = emiPayment.Id }, emiPayment);
        }

        [HttpGet("emi/{id}")]
        public async Task<ActionResult<EmiTransaction>> GetEMITransaction(int id)
        {
            var emiTransaction = await _context.EmiTransactions
                .Include(et => et.Debt)
                .Include(et => et.Customer)
                .Include(et => et.Group)
                .FirstOrDefaultAsync(et => et.Id == id);

            if (emiTransaction == null)
            {
                return NotFound();
            }

            return emiTransaction;
        }

        [HttpGet("group/{groupId}/total-due")]
        public async Task<ActionResult<decimal>> GetGroupTotalDueAmount(int groupId)
        {
            var totalDueAmount = await _context.Debts
                .Where(d => d.GroupId == groupId && d.Status != DebtStatus.Completed)
                .SumAsync(d => d.RemainingAmount);

            return Ok(totalDueAmount);
        }
    }
}
