using DualBuffer.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DualBuffer
{
   

    public class NetworkDbContext : DbContext
    {
        public DbSet<Call> calls { get; set; }

        public NetworkDbContext(DbContextOptions<NetworkDbContext> options) : base(options)
        {
        }
    }
}
