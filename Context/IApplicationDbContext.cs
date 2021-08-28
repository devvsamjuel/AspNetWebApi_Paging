using Microsoft.EntityFrameworkCore;
using PagingExample.Models;
using System.Threading.Tasks;

namespace PagingExample.Context
{
    public interface IApplicationDbContext
    {
        DbSet<Student> Students { get; set; }
        DbSet<Customer> Customers { get; set; }

        Task<int> SaveChangesAsync();
    }
}