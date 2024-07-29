using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Hangfire.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Hangfire services.
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
IBackgroundJobClient backgroundJobs = app.Services.GetService<IBackgroundJobClient>();

app.UseStaticFiles();
app.UseHangfireDashboard();
backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));



app.UseHttpsRedirection();

app.UseRouting(); // Add routing middleware here
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Map controller routes
    endpoints.MapHangfireDashboard(); // Map Hangfire Dashboard routes
});

app.Run();
