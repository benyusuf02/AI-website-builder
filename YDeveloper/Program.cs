using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Services;
using YDeveloper.Models;
using YDeveloper.Middleware;
using Hangfire;
using YDeveloper.Services.Background;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using QuestPDF.Infrastructure;
using Serilog;
using YDeveloper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

// Set QuestPDF License
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<YDeveloperContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Identity with ApplicationUser and Roles
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.SignIn.RequireConfirmedAccount = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<YDeveloperContext>()
.AddEntityFrameworkStores<YDeveloperContext>()
.AddDefaultTokenProviders();

// Redis Cache
// Redis Cache (Disabled for local dev ease, enabling Memory Cache)
/*
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "YDeveloper_";
});
*/
builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "DEFAULT_KEY_VERY_LONG_.................."))
        };
    });

// 2.1 Session & Security Configuration
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    // Check security stamp every 30 minutes to enforce global logout / role changes
    options.ValidationInterval = TimeSpan.FromMinutes(30);
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Idle timeout
    options.SlidingExpiration = true; // Extend valid time on activity
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// 3. MVC and Services
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource));
    });

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddRazorPages();

// Session Configuration (Onboarding için gerekli)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Response Compression (Gzip, Brotli)
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "YDeveloper API", Version = "v1" });
    // Add JWT Definition
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
// 4. Hangfire (Background Jobs)
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString));

builder.Services.AddHangfireServer();

// 5. Core Services & Repositories
builder.Services.AddScoped(typeof(YDeveloper.Repositories.IRepository<>), typeof(YDeveloper.Repositories.Repository<>));
builder.Services.AddScoped<YDeveloper.Repositories.IUnitOfWork, YDeveloper.Repositories.UnitOfWork>();
builder.Services.AddScoped<YDeveloper.Services.ICacheService, YDeveloper.Services.CacheService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(_ => true) // Allow any origin (subdomains)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for SignalR
    });
});
builder.Services.AddSignalR();
builder.Services.AddHttpClient<GeminiService>(client => { client.Timeout = TimeSpan.FromMinutes(5); });
builder.Services.AddHttpClient<IDomainService, NamecheapService>();

// 6. Application Services (Refactored)
builder.Services.AddScoped<IPaymentService, PaymentService>();
// builder.Services.AddScoped<IEmailService, EmailService>(); // Old SMTP
builder.Services.AddScoped<IEmailService, AwsSesEmailService>(); // New AWS SES
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, AwsSesEmailService>();
builder.Services.AddHttpClient<IReCaptchaService, ReCaptchaService>(); // ReCaptcha Service
builder.Services.AddScoped<IServerInfrastructureService, ServerInfrastructureService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPageService, PageService>(); // New Page Service
builder.Services.AddScoped<RecurringBillingService>(); // Background Worker
builder.Services.AddScoped<RecurringBillingService>(); // Background Worker
builder.Services.AddScoped<IAuditService, AuditService>(); // Audit Logging Service
builder.Services.AddScoped<IInvoiceService, InvoiceService>(); // Invoice Gen
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>(); // Proration Logi
builder.Services.AddScoped<ISystemSettingService, SystemSettingService>(); // Branding & Settings
builder.Services.AddScoped<IImageOptimizationService, ImageOptimizationService>(); // Image Optimization WebP

// 6.5 Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Global Limiter (100 requests per minute per IP)
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));

    // Specific Auth Limiter (Stricter: 5 req/min)
    options.AddFixedWindowLimiter("Auth", options =>
    {
        options.AutoReplenishment = true;
        options.PermitLimit = 5;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueLimit = 0;
    });
});

// 6.6 Performance Services
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});
builder.Services.AddResponseCaching();

// 7. AWS Services (Manual Registration to bypass Default Credential Chain)
var awsAccessKey = builder.Configuration["AWS:AccessKey"];
var awsSecretKey = builder.Configuration["AWS:SecretKey"];
var awsRegionStr = builder.Configuration["AWS:Region"];
var awsRegion = Amazon.RegionEndpoint.GetBySystemName(awsRegionStr);

// Validations
if (string.IsNullOrEmpty(awsAccessKey) || string.IsNullOrEmpty(awsSecretKey) || awsRegion == null)
{
    Console.WriteLine("[CRITICAL] AWS Config is missing in appsettings.json.");
}

// Register S3 Client
builder.Services.AddSingleton<Amazon.S3.IAmazonS3>(sp =>
{
    return new Amazon.S3.AmazonS3Client(awsAccessKey, awsSecretKey, awsRegion);
});

// Register SES Client
builder.Services.AddSingleton<Amazon.SimpleEmail.IAmazonSimpleEmailService>(sp =>
{
    return new Amazon.SimpleEmail.AmazonSimpleEmailServiceClient(awsAccessKey, awsSecretKey, awsRegion);
});

builder.Services.AddScoped<IS3Service, S3Service>();

// 4. Background Services
builder.Services.AddHostedService<YDeveloper.Services.EntegrasyonWorker>();

// Backup Service Registration (Injectable + Hosted)
builder.Services.AddSingleton<YDeveloper.Services.BackupService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<YDeveloper.Services.BackupService>());
builder.Services.AddSingleton<IBackupService>(sp => sp.GetRequiredService<YDeveloper.Services.BackupService>());

// 5. HttpContext & Security
builder.Services.AddHttpContextAccessor();

// 6. Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<YDeveloper.HealthChecks.DatabaseHealthCheck>("database")
    .AddCheck<YDeveloper.HealthChecks.RedisHealthCheck>("cache");

// AI Service
builder.Services.AddScoped<YDeveloper.Services.Ai.IAiCommandService, YDeveloper.Services.Ai.AiCommandService>();

var app = builder.Build();

// ... (existing code) ...

// IMPORTANT: Domain Routing Middleware BEFORE UseRouting
app.UseResponseCompression(); // Compress responses (Gzip/Brotli)
app.UseResponseCaching(); // Enable Public Caching

// Static Files with Cache Control
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Cache static files for 7 days
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=604800");
    }
});

app.UseCors();
app.UseMiddleware<SaaS_RoutingMiddleware>();
app.UseMiddleware<YDeveloper.Middleware.MaintenanceMiddleware>(); // Add Maintenance Middleware here

// Configure Forwarded Headers for Reverse Proxy (Caddy/Nginx)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

app.UseSerilogRequestLogging(); // Enable Serilog Request Logging (Must be early)

// API Error Handling
app.UseMiddleware<YDeveloper.Middleware.ApiExceptionMiddleware>();

// Security Headers (CSP, XSS, etc.)
app.UseMiddleware<YDeveloper.Middleware.SecurityHeadersMiddleware>();

app.UseRateLimiter(); // Apply Rate Limiting

// GLOBAL EXCEPTION HANDLING - Tüm yakalanmamış hatalar burada kontrol altına alınır
app.UseMiddleware<YDeveloper.Middleware.GlobalExceptionMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Handle 404s specifically
    app.UseStatusCodePagesWithReExecute("/Home/NotFound");
    app.UseHsts();
}
else
{
    // Swagger in Dev
    app.UseSwagger();
    app.UseSwaggerUI();

    // Even in dev, it's nice to see the 404 page for testing
    app.UseStatusCodePagesWithReExecute("/Home/NotFound");
}


app.UseRouting();

// HTTPS Redirect (Production security)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

// Session middleware (UseAuthentication öncesinde olmalı)
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Middleware: Admin IP Restriction
app.UseMiddleware<YDeveloper.Middleware.AdminSafeListMiddleware>();

// Admin area routing
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// Default routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapFallbackToController("Index", "Home");
app.MapRazorPages();
app.MapHub<YDeveloper.Hubs.ChatHub>("/chatHub");

// Health Check Endpoint
app.MapHealthChecks("/health");

// Schedule Recurring Jobs using IRecurringJobManager service
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate<RecurringBillingService>(
        "daily-renewal-check",
        service => service.ProcessDailyRenewals(),
        Cron.Daily(3)); // Run daily at 03:00 AM
}

// Seed Content
await YDeveloper.Data.ContentSeeder.SeedAsync(app);

app.Run();