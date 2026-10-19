using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using QrPayment.Domain.Interfaces;
using QrPayment.Infrastructure.Persistence;
using QrPayment.Infrastructure.Repositories;

namespace QrPayment.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection") ?? "Data Source=qr-payment.db"));

            services.AddScoped<ITransactionRepository, TransactionRepository>();

            return services;
        }
    }
}
