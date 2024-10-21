using BookStore.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BookStore.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category() { Id = 1, Name = "Romance" },
                new Category() { Id = 2, Name = "Detective" },
                new Category() { Id = 3, Name = "Tragedy" }
             );

            modelBuilder.Entity<Product>().HasData(
                new Product()
                {
                    Id = 1,
                    Title = "12 Rules for Life",
                    ISBN = "9780345816023",
                    Description = "<p>12 Rules for Life offers a deeply rewarding antidote to the chaos in our lives: eternal truths applied to our modern problems.</p>",
                    Author = "Mark",
                    Price = 120,
                    CategoryId = 1,
                    ProductImage = "\\images\\products\\4dca90aa-7e9c-4756-bf6b-c020a1728d9b.jpg"
                },
                new Product()
                {
                    Id = 2,
                    Title = "Kanban",
                    ISBN = "9780984521401",
                    Description = "<p>Optimize the effectiveness of your business, to produce fit-for-purpose products and services that delight your customers, making them loyal to your brand and increasing your share, revenues and margins.</p>",
                    Author = "Mark",
                    Price = 120,
                    CategoryId = 2,
                    ProductImage = "\\images\\products\\6626ca37-6e94-4a28-ae57-742ba0ec23e6.jpg"
                }

            );
        }
    }
}
