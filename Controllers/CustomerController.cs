using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PagingExample.Context;
using PagingExample.Filter;
using PagingExample.Helpers;
using PagingExample.Services;
using PagingExample.Wrappers;

namespace PagingExample
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private IApplicationDbContext _context;
        private IUriService _uriService;
        public CustomerController(IApplicationDbContext context, IUriService uriService)
        {
            this._context = context;
            this._uriService = uriService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return Ok(customer.Id);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _context.Customers
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToListAsync();
            var totalRecords = await _context.Customers.CountAsync();
            var pagedReponse = PaginationHelper.CreatePagedReponse<Customer>(pagedData, validFilter, totalRecords, _uriService, route);

            return Ok(pagedReponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _context.Customers.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (customer == null) return NotFound();
            return Ok(new Response<Customer>(customer));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Customers.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (student == null) return NotFound();
            _context.Customers.Remove(student);
            await _context.SaveChangesAsync();
            return Ok(student.Id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Customer customerUpdate)
        {
            var student = _context.Customers.Where(a => a.Id == id).FirstOrDefault();
            if (student == null) return NotFound();
            else
            {
                student.FirstName = customerUpdate.FirstName;
                student.LastName = customerUpdate.LastName;
                await _context.SaveChangesAsync();
                return Ok(student.Id);
            }
        }
    }
}