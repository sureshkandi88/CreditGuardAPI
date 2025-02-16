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
    public class GroupController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GroupController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Group>> CreateGroup([FromBody] Group group)
        {
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, group);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroups()
        {
            return await _context.Groups
                .Include(g => g.CustomerGroups)
                .Include(g => g.ActiveCustomers)
                .Include(g => g.Debts)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> GetGroup(int id)
        {
            var group = await _context.Groups
                .Include(g => g.CustomerGroups)
                .Include(g => g.ActiveCustomers)
                .Include(g => g.Debts)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                return NotFound();
            }

            return group;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, [FromBody] Group updatedGroup)
        {
            if (id != updatedGroup.Id)
            {
                return BadRequest();
            }

            _context.Entry(updatedGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{groupId}/add-customer/{customerId}")]
        public async Task<IActionResult> AddCustomerToGroup(int groupId, int customerId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            var customer = await _context.Customers.FindAsync(customerId);

            if (group == null || customer == null)
            {
                return NotFound();
            }

            // Check if customer is already in the group
            var existingMapping = await _context.CustomerGroups
                .FirstOrDefaultAsync(cg => cg.GroupId == groupId && cg.CustomerId == customerId);

            if (existingMapping != null)
            {
                return Conflict("Customer is already in this group");
            }

            // Create new customer group mapping
            var customerGroup = new CustomerGroup
            {
                GroupId = groupId,
                CustomerId = customerId,
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.CustomerGroups.Add(customerGroup);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{groupId}/remove-customer/{customerId}")]
        public async Task<IActionResult> RemoveCustomerFromGroup(int groupId, int customerId)
        {
            var customerGroup = await _context.CustomerGroups
                .FirstOrDefaultAsync(cg => cg.GroupId == groupId && cg.CustomerId == customerId);

            if (customerGroup == null)
            {
                return NotFound();
            }

            _context.CustomerGroups.Remove(customerGroup);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}
