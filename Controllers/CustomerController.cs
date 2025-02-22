using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CreditGuardAPI.Data;
using CreditGuardAPI.Models;
using CreditGuardAPI.Dtos;
using System.IO;

namespace CreditGuardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CustomerController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer([FromForm] CustomerDto customerDto)
        {
            // Validate unique Aadhaar number
            if (await _context.Customers.AnyAsync(c => c.AadhaarNumber == customerDto.AadhaarNumber))
            {
                return Conflict("A customer with this Aadhaar number already exists.");
            }

            // Create customer from DTO
            var customer = new Customer
            {
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
                AadhaarNumber = customerDto.AadhaarNumber,
                PhoneNumber = customerDto.PhoneNumber,
                Street = customerDto.Address.Street,
                City = customerDto.Address.City,
                State = customerDto.Address.State,
                PinCode = customerDto.Address.PinCode
            };

            // Handle file uploads
            if (customerDto.ProfilePhoto != null)
            {
                customer.ProfilePhotoPath = await SaveFile(customerDto.ProfilePhoto, "ProfilePhotos");
            }

            if (customerDto.AadhaarPhoto != null)
            {
                customer.AadhaarPhotoPath = await SaveFile(customerDto.AadhaarPhoto, "AadhaarPhotos");
            }

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        private async Task<string> SaveFile(IFormFile file, string folderName)
        {
            // Ensure the directory exists
            var uploadFolder = Path.Combine(_environment.WebRootPath, folderName);
            Directory.CreateDirectory(uploadFolder);

            // Generate unique filename
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadFolder, uniqueFileName);

            // Save the file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return relative path
            return Path.Combine(folderName, uniqueFileName);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<Customer>>> GetCustomers(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            Thread.Sleep(3000);
            var totalCustomers = await _context.Customers.CountAsync();
            var customers = await _context.Customers
                .Include(c => c.CustomerGroups)
                .Include(c => c.ActiveGroup)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Customer>
            {
                Data = customers,
                Total = totalCustomers,
                Page = page,
                PageSize = pageSize
            };
        }

        public class PaginatedResponse<T>
        {
            public List<T> Data { get; set; }
            public int Total { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.CustomerGroups)
                .Include(c => c.ActiveGroup)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer updatedCustomer)
        {
            if (id != updatedCustomer.Id)
            {
                return BadRequest();
            }

            // Check if Aadhaar number is unique (excluding current customer)
            if (await _context.Customers.AnyAsync(c => c.AadhaarNumber == updatedCustomer.AadhaarNumber && c.Id != id))
            {
                return Conflict("A customer with this Aadhaar number already exists.");
            }

            _context.Entry(updatedCustomer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
