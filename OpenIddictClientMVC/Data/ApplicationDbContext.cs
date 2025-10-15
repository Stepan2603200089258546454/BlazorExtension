using Microsoft.EntityFrameworkCore;

namespace OpenIddictClientMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}
