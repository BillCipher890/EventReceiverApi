using Microsoft.EntityFrameworkCore;
using EventReceiverApi.Web.Middlewares;
using EventReceiverApi.DataStorage;
using EventReceiverApi.DataStorage.Repositories;
using EventReceiverApi.BusinessLogic.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddDbContext<AppDBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.MapControllers();

app.Run();
