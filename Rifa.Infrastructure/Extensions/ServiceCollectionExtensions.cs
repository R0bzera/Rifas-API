using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rifa.Application.Interfaces;
using Rifa.Application.Services;
using Rifa.Infrastructure.Config;
using Rifa.Infrastructure.ExternalsCalls;
using Rifa.Infrastructure.Repository;

namespace Rifa.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RifaDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IRifaRepository, RifaRepository>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<ICotaRepository, CotaRepository>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IRifaService, RifaService>();
            services.AddScoped<IPedidoService, PedidoService>();
            services.AddScoped<ICotaService, CotaService>();

            services.AddScoped<IPaymentService, MercadoPagoPaymentService>();
            services.AddScoped<IPaymentStatusService, PaymentStatusService>();
            services.AddScoped<IPaymentOrderService, PaymentOrderService>();
            services.AddScoped<IPaymentOrderRepository, PaymentOrderRepository>();

            return services;
        }
    }
}
