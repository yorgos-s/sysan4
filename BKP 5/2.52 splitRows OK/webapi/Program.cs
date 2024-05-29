using webapi.Services;  // Assuming EmailService is in the webapi.Services namespace
using webapi.Services.cs;   //email

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", builder =>
    {
        builder.WithOrigins("https://localhost:3000") // Allow your React app's origin
               .AllowAnyMethod() // Allows all methods
               .AllowAnyHeader() // Allows all headers
               .AllowCredentials(); // Allow credentials
    });
});

// Build configuration
var configuration = builder.Configuration;

// Add controllers and other services
builder.Services.AddControllers();

// Add the connection string as a singleton
builder.Services.AddSingleton(configuration.GetConnectionString("DefaultConnection"));

// Add the email service
builder.Services.AddScoped<IEmailService, EmailService>();

//Add the Portfolios Services (retrieves the names of the portfolios)
builder.Services.AddSingleton<IPortfolioService, PortfolioService>();

//Add PortfolioService
//-->Scoped because a new instance of the service is created for
//each incoming request and disposed of when the request ends
builder.Services.AddScoped<IPortfoliosProcessingService>(provider =>
    new PortfoliosProcessingService(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Use Swagger middleware if in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

// Use the CORS policy BEFORE app.MapControllers();
app.UseCors("MyCorsPolicy");

// Other middleware, endpoints, etc.
app.MapControllers();

app.Run();






