using FluentValidation;
using GeniusStoreERP.Application.Behaviors;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Users.Commands.CreateUser;
using GeniusStoreERP.Domain.Entities;
using GeniusStoreERP.Infrastructure.Data;
using GeniusStoreERP.UI.ViewModels;
using GeniusStoreERP.UI.Views;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace GeniusStoreERP.UI
{
    public partial class App : System.Windows.Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = default!;
        private IConfiguration? configuration;



        protected override async void OnStartup(StartupEventArgs e)
        {
            configuration = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .Build();


            var services = new ServiceCollection();

            services.AddLogging();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            // Identity registration
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // MediatR registration
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly));

            services.AddValidatorsFromAssembly(typeof(CreateUserCommand).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddSingleton<LoginView>();
            services.AddTransient<LoginViewModel>();

            services.AddSingleton<MainView>();
            services.AddTransient<MainViewModel>();

            services.AddTransient<DashboardView>();
            services.AddTransient<DashboardViewModel>();


            // 7. بناء الـ ServiceProvider النهائي
            ServiceProvider = services.BuildServiceProvider();

            await InitializeDatabaseAsync();

            var mainWindow = ServiceProvider.GetRequiredService<LoginView>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        private async Task InitializeDatabaseAsync()
        {
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            var services = scope.ServiceProvider;
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (await userManager.FindByNameAsync("admin") == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    FullName = "Administrator",

                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
