using Serilog;
using Scalar.AspNetCore;
using Hangfire;
using Hangfire.Common;
using Agentic_Rentify.Infragentic;

// 1. إعداد الـ Logger المبدئي
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Agentic Rentify API (Core .NET 10)...");

    var builder = WebApplication.CreateBuilder(args);

    // 2. ربط Serilog بالـ Host
    builder.Host.UseSerilog((context, services, configuration) => configuration
     .ReadFrom.Configuration(context.Configuration) // كدة هيقرأ الـ Console من الـ JSON مرة واحدة
     .ReadFrom.Services(services)
     .Enrich.FromLogContext());

    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddInfragenticServices(builder.Configuration);
    builder.Services.AddHostedService<Agentic_Rentify.Api.Services.VectorSyncHostedService>();

    // builder.Services.AddTransient<Agentic_Rentify.Api.Middleware.GlobalExceptionHandlerMiddleware>(); // Middleware registered via UseMiddleware does not need DI registration if strictly conventional.
    // Removed to fix RequestDelegate resolution error.
    builder.Services.AddControllers();

    // 3. دعم الـ Swagger/Swashbuckle للـ API Documentation
    // For MVC controllers with Swashbuckle, we don't need AddEndpointsApiExplorer()
    
    builder.Services.AddSwaggerGen(options =>
    {
        // Add document info - using anonymous type for compatibility
        options.SwaggerDoc("v1", new()
        {
            Title = "Agentic Rentify API",
            Version = "v1",
            Description = """
Comprehensive REST API for an AI-powered travel booking platform with intelligent discovery, semantic search, and automated booking workflows.

## Developer Guide

### Authentication
- **JWT Tokens**: Include `Authorization: Bearer {token}` header for protected endpoints
- Obtain tokens via `/api/Auth/login` or `/api/Auth/register`
- User context is automatically extracted for personalized AI responses

### Stripe Payment Flow
1. Create a booking via `POST /api/Bookings`
2. Response includes `sessionUrl` - redirect user to this Stripe Checkout page
3. After payment completion, Stripe redirects to your success URL
4. Use `/api/Bookings/{id}/confirm` webhook to mark booking as paid
5. Handle cancellation via `POST /api/Bookings/{id}/cancel`

### AI Chat Integration
- The `/api/Chat` endpoint uses Semantic Kernel with function calling
- AI automatically invokes tools (trip discovery, bookings) based on user intent
- **UiHint**: Response may contain `uiHint` field suggesting UI actions:
  - `show_trips` - Display trip search results
  - `redirect_payment` - Navigate to Stripe checkout
  - `booking_confirmed` - Show confirmation screen
- **Payload**: Contains structured data corresponding to the hint

### Semantic Search
- Vector-powered search across Trips, Attractions, Hotels, Cars
- Automatically synced with Qdrant on every create/update/delete
- Use natural language queries like "romantic quiet beaches"

### Endpoint Groups
- **AI Operations**: Chat, Semantic Discovery
- **Catalog**: Trips, Attractions, Hotels, Cars
- **Booking System**: Create, Confirm, Cancel bookings
- **Admin**: Vector sync, Health checks
"""
        });

        // Include XML comments from Api project
        var apiXmlFile = Path.Combine(AppContext.BaseDirectory, "Api.xml");
        if (File.Exists(apiXmlFile))
        {
            options.IncludeXmlComments(apiXmlFile, includeControllerXmlComments: true);
        }

        // Include XML comments from Application project (if available in output)
        var appXmlFile = Path.Combine(AppContext.BaseDirectory, "Application.xml");
        if (File.Exists(appXmlFile))
        {
            options.IncludeXmlComments(appXmlFile);
        }

        // Include XML comments from Core project (if available in output)
        var coreXmlFile = Path.Combine(AppContext.BaseDirectory, "Core.xml");
        if (File.Exists(coreXmlFile))
        {
            options.IncludeXmlComments(coreXmlFile);
        }

        // Include all ApiExplorer groups in the single v1 document so grouped controllers still appear
        options.DocInclusionPredicate((_, __) => true);
    });
    
    // Configure JSON serializer
    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.WriteIndented = true;
    });
    
    // CORS Configuration
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy => 
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // Configure Hangfire Recurring Jobs (using service-based API, not static)
    using (var scope = app.Services.CreateScope())
    {
        var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
        var imageCleanupService = scope.ServiceProvider.GetRequiredService<Agentic_Rentify.Application.Interfaces.IImageCleanupService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        // Schedule hourly image cleanup job
        recurringJobManager.AddOrUpdate(
            recurringJobId: "image-cleanup-hourly",
            methodCall: () => imageCleanupService.CleanupOrphanedImagesAsync(),
            cronExpression: Cron.Hourly(),
            options: new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            }
        );

        logger.LogInformation("Hangfire recurring job 'image-cleanup-hourly' scheduled to run every hour (UTC).");
    }

    app.UseSerilogRequestLogging();
    app.UseMiddleware<Agentic_Rentify.Api.Middleware.GlobalExceptionHandlerMiddleware>(); // Use Middleware
    app.UseHangfireDashboard();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseCors("AllowAll");
    app.UseAuthorization();

    // 4. Enable Swagger/Swashbuckle in Development
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger(options =>
        {
            // مسار توليد ملف الـ JSON بواسطة Swashbuckle
            options.RouteTemplate = "swagger/{documentName}/swagger.json";
        });

        app.MapScalarApiReference(options =>
        {
            // في النسخ الحديثة، يتم استخدام هذا التعديل لربط المستند
            options.OpenApiRoutePattern = "/swagger/v1/swagger.json";

            // إعدادات إضافية اختيارية
            options.Title = "Agentic Rentify API Reference";
        });
    }

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The application failed to start correctly.");
}
finally
{
    Log.CloseAndFlush();
}