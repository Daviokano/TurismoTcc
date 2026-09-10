
using HorizonTravel.Libraries.Login;
using HorizonTravel.Repository;
using HorizonTravel.Repository.Contract;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// add a interface como um serviço
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddDistributedMemoryCache();

//corrigindo peoblemas com o TEMPDATA
builder.Services.AddSession(options =>
{
    //definindo tempo pra duração
    options.IdleTimeout = TimeSpan.FromSeconds(60);
    options.Cookie.HttpOnly = true;
    //mostrarndo pro navegador que o cokkie é essencial
    options.Cookie.IsEssential = true;
});
builder.Services.AddMvc().AddSessionStateTempDataProvider();

builder.Services.AddScoped<HorizonTravel.Libraries.Sessao.Sessao>();
builder.Services.AddScoped<LoginUsuario>();

//add para manipular a sessão
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseDefaultFiles();
app.UseRouting();
app.UseAuthorization();
app.UseCookiePolicy();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
