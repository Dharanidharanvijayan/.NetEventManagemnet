using EventManagement.Data;  // Make sure to include this
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // Include this for Swagger


var builder = WebApplication.CreateBuilder(args);

// 1. Register services to the service container here, BEFORE builder.Build()
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers
builder.Services.AddControllers();   // Register AddControllers here before Build

// 2. Add Swagger services
builder.Services.AddEndpointsApiExplorer(); // Necessary for minimal APIs
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Event Management API", Version = "v1" });
});

// 2. Build the application
var app = builder.Build();

// 3. Configure the HTTP request pipeline (middleware)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Enable Swagger and Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Management API V1");
    c.RoutePrefix = "swagger"; // Makes Swagger UI accessible at /swagger
});

// Map controllers
app.MapControllers();
app.UseDeveloperExceptionPage();

app.Run();
