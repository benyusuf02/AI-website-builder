namespace YDeveloper.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Core services
            services.AddScoped<YDeveloper.Services.ICacheService, YDeveloper.Services.CacheService>();
            services.AddScoped(typeof(YDeveloper.Repositories.IRepository<>), typeof(YDeveloper.Repositories.Repository<>));
            services.AddScoped<YDeveloper.Repositories.IUnitOfWork, YDeveloper.Repositories.UnitOfWork>();

            return services;
        }

        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.SetIsOriginAllowed(_ => true)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            return services;
        }
    }
}
