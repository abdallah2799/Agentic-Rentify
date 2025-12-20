using Serilog;
using Scalar.AspNetCore;
using Hangfire;
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

    // builder.Services.AddTransient<Agentic_Rentify.Api.Middleware.GlobalExceptionHandlerMiddleware>(); // Middleware registered via UseMiddleware does not need DI registration if strictly conventional.
    // Removed to fix RequestDelegate resolution error.
    builder.Services.AddControllers();

    // 3. دعم الـ OpenApi (الأساسي لـ Scalar)
    builder.Services.AddOpenApi();
    
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

    // 4. تفعيل الـ Scalar في الـ Development
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi(); // ده بيطلع الـ /openapi/v1.json
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("Agentic Rentify API")
                   .WithTheme(ScalarTheme.Moon)
                   .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });
    }
    app.UseSerilogRequestLogging();
    app.UseMiddleware<Agentic_Rentify.Api.Middleware.GlobalExceptionHandlerMiddleware>(); // Use Middleware
    app.UseHangfireDashboard();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseCors("AllowAll");
    app.UseAuthorization();
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