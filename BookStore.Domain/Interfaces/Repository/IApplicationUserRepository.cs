using BookStore.Domain.Entities;

namespace BookStore.Domain.Interfaces.Repository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        public void Update(ApplicationUser applicationUser);
    }
}
