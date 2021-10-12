using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application.WriteOperations;

namespace PaymentGateway.Application
{
   public static class DependencyInjectionExtensions
    {
        public static IServiceCollection RegisterBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<EnrollCustomerOperation>();
            services.AddTransient<CreateAccountOperation>();
            services.AddTransient<DepositMoneyOperation>();
            services.AddTransient<WithdrawMoneyOperation>();
            services.AddTransient<CreateProductOperation>();
            services.AddTransient<PurchaseProductOperation>();

            services.AddSingleton<Data.Database>();

            services.AddSingleton(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var options = new AccountOptions
                {
                    InitialBalance = config.GetValue("AccountOptions:InitialBalance", 0)
                };
                return options;
            });


            return services;
        }
    }
}