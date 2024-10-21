using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces.Repository;
using BookStore.Infrastructure.Data;

namespace BookStore.Infrastructure.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository {
        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
        }
        public void Update(ApplicationUser applicationUser) {
            _context.ApplicationUsers.Update(applicationUser);
        }
    }
}
