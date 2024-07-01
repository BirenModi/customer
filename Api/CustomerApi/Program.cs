using Customer.Api;
using Customer.Business.IServices;
using Customer.Business.Services;
using Customer.Data.Context;
using Customer.Data.CoreService;
using Customer.Data.ICoreService;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<CustomerApiDbContext>();
builder.Services.AddTransient<ICustomerServices, CustomerServices>();
builder.Services.AddTransient<ICoreCustomerServices, CoreCustomerServices>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddLogging(builder =>
{
    builder.ClearProviders();
    builder.SetMinimumLevel(LogLevel.Trace);
    builder.AddNLog();
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("CustomerPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
                                         .AllowAnyHeader()
                                         .AllowAnyMethod();
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<Customer.Api.Middleware.ExceptionHandlingMiddleware>();
app.UseCors("CustomerPolicy");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
using (var context = new CustomerApiDbContext())
{
    context.Database.EnsureDeleted();
    if (context.Database.EnsureCreated())
    {
        await context.Customers.AddAsync(new Customer.Data.Model.Customers
        {
            CustomerId = Guid.NewGuid(),
            FullName = "Biren Modi",
            DateOfBirth = new DateOnly(1991, 9, 28)
        });

        await context.Customers.AddAsync(new Customer.Data.Model.Customers
        {
            CustomerId = Guid.NewGuid(),
            FullName = "Developer Testing",
            DateOfBirth = new DateOnly(1991, 6, 28)
        });

        await context.SaveChangesAsync();
    }
}
app.Run();

