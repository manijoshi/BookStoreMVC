using BookStore.Application.MappingProfiles;

namespace BookStore.Web
{
    public static class ServiceExtensions
    {
        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ProductProfile>();
            }, typeof(Program).Assembly);
        }
    }
}
