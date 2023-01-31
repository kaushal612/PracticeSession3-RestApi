using Microsoft.EntityFrameworkCore;
using PracticeSession3;
using PracticeSession3.Context;
using PracticeSession3.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CustomerContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Customerconnex")));
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddControllers().AddNewtonsoftJson();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDeveloperExceptionPage();

//app.UseHttpLogging();

app.UseMyMiddleware();

app.MapControllers();

app.Run();


