using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CreditGuardAPI.Data;
using CreditGuardAPI.Models;
using CreditGuardAPI.Dtos;
using System.IO;
using CreditGuardAPI.Services;

namespace CreditGuardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStaticFileService _staticFileService;

        public CustomerController(ApplicationDbContext context, IStaticFileService staticFileService)
        {
            _context = context;
            _staticFileService = staticFileService;
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
                Street = customerDto.Street,
                CityName = customerDto.CityName,
                State = customerDto.State,
                
                Location = customerDto.Location
            };

            // Handle file uploads
            if (customerDto.ProfilePhoto != null)
            {                
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await customerDto.ProfilePhoto.CopyToAsync(memoryStream);
                        customer.ProfilePictureId = await _staticFileService.SaveFileAsync(
                            memoryStream.ToArray(),
                            customerDto.ProfilePhoto.FileName,
                            customerDto.ProfilePhoto.ContentType,
                            "Customer",
                            customer.Id
                        );
                    }
                }
                catch (FileSizeLimitExceededException ex)
                {
                    return BadRequest($"Profile photo: {ex.Message}");
                }
            }

            if (customerDto.AadhaarPhoto != null)
            {                
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await customerDto.AadhaarPhoto.CopyToAsync(memoryStream);
                        customer.AadhaarPictureId = await _staticFileService.SaveFileAsync(
                            memoryStream.ToArray(),
                            customerDto.AadhaarPhoto.FileName,
                            customerDto.AadhaarPhoto.ContentType,
                            "Customer",
                            customer.Id
                        );
                    }
                }
                catch (FileSizeLimitExceededException ex)
                {
                    return BadRequest($"Aadhaar photo: {ex.Message}");
                }
            }

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<Customer>>> GetCustomers(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try{
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
            catch (Exception ex){
                return BadRequest(ex.Message);
            }
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
        public async Task<IActionResult> UpdateCustomer(int id, [FromForm] CustomerDto customerDto)
        {
            var existingCustomer = await _context.Customers.FindAsync(id);
            if (existingCustomer == null)
            {
                return NotFound();
            }

            // Update properties from DTO
            existingCustomer.FirstName = customerDto.FirstName;
            existingCustomer.LastName = customerDto.LastName;
            existingCustomer.AadhaarNumber = customerDto.AadhaarNumber;
            existingCustomer.PhoneNumber = customerDto.PhoneNumber;
            existingCustomer.Street = customerDto.Street;
            existingCustomer.CityName = customerDto.CityName;
            existingCustomer.State = customerDto.State;
            existingCustomer.Location = customerDto.Location;

            // Handle file upload updates
            if (customerDto.ProfilePhoto != null)
            {                
                try
                {
                    // Delete old profile picture if exists
                    if (existingCustomer.ProfilePictureId.HasValue)
                    {
                        await _staticFileService.DeleteFileAsync(existingCustomer.ProfilePictureId.Value);
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await customerDto.ProfilePhoto.CopyToAsync(memoryStream);
                        existingCustomer.ProfilePictureId = await _staticFileService.SaveFileAsync(
                            memoryStream.ToArray(),
                            customerDto.ProfilePhoto.FileName,
                            customerDto.ProfilePhoto.ContentType,
                            "Customer",
                            existingCustomer.Id
                        );
                    }
                }
                catch (FileSizeLimitExceededException ex)
                {
                    return BadRequest($"Profile photo: {ex.Message}");
                }
            }
            if (customerDto.AadhaarPhoto != null)
            {                
                try
                {
                    // Delete old Aadhaar picture if exists
                    if (existingCustomer.AadhaarPictureId.HasValue)
                    {
                        await _staticFileService.DeleteFileAsync(existingCustomer.AadhaarPictureId.Value);
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await customerDto.AadhaarPhoto.CopyToAsync(memoryStream);
                        existingCustomer.AadhaarPictureId = await _staticFileService.SaveFileAsync(
                            memoryStream.ToArray(),
                            customerDto.AadhaarPhoto.FileName,
                            customerDto.AadhaarPhoto.ContentType,
                            "Customer",
                            existingCustomer.Id
                        );
                    }
                }
                catch (FileSizeLimitExceededException ex)
                {
                    return BadRequest($"Aadhaar photo: {ex.Message}");
                }
            }

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
