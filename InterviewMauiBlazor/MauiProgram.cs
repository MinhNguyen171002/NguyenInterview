using InterviewMauiBlazor.Database;
using InterviewMauiBlazor.Database.Repositories;
using InterviewMauiBlazor.Mapping;
using InterviewMauiBlazor.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Syncfusion.Blazor;
using Syncfusion.Licensing;
using System;
using static InterviewMauiBlazor.Components.Pages.TransactionManager;

namespace InterviewMauiBlazor
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // loi sercet // 
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(
                "Ngo9BigBOggjHTQxAR8/V1NMaF5cXmBCfEx0R3xbf1x1ZFFMZF1bQHdPMyBoS35Rc0ViW3xedXRSRWFUUUV0");
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSyncfusionBlazor();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDb");
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }, ServiceLifetime.Scoped);

            builder.Services.AddIdentityCore<IdentityUser>()
            .AddEntityFrameworkStores<ApplicationDbContext>();


            builder.Services.AddScoped<IOrderRepositories, OrderRepository>();
            builder.Services.AddScoped<IProductRepositories, ProductRepository>();
            builder.Services.AddScoped<ITransactionRepositories, TransactionRepository>();

            builder.Services.AddScoped<ITransactionServices, TransactionServices>();

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();
            }


            return builder.Build();
        }
    }
}
