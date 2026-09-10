<<<<<<< HEAD
using HorizonTravel.Repository;
using HorizonTravel.Repository.Contract;

var builder = WebApplication.CreateBuilder(args);
=======
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
>>>>>>> a5f08bc4d6687722b9189c5ad78a9a3155867564

// Add services to the container.
builder.Services.AddControllersWithViews();
// add a interface como um serviço
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
