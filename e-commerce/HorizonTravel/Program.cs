
using HorizonTravel.Libraries.Login;
using HorizonTravel.Libraries.Middleware;
using HorizonTravel.Libraries.Sessao;
using HorizonTravel.Repository;
using HorizonTravel.Repository.Contract;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// add a interface como um servi�o
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IFuncionarioRepositoty, FuncionarioRepository>();
builder.Services.AddScoped<IParceiroRepository, ParceiroRepository>();
builder.Services.AddScoped<IPacoteRepository, PacoteRepository>();
builder.Services.AddScoped<IPromocaoRepository, PromocaoRepository>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<IAvaliacaoRepository, AvaliacaoRepository>();

builder.Services.AddDistributedMemoryCache();

//corrigindo peoblemas com o TEMPDATA
builder.Services.AddSession(options =>
{
    //definindo tempo pra dura��o
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    //mostrarndo pro navegador que o cokkie � essencial
    options.Cookie.IsEssential = true;
});
builder.Services.AddMvc().AddSessionStateTempDataProvider();

// ex.: em ConfigureServices
builder.Services.AddScoped<HorizonTravel.Libraries.Login.LoginFuncionario>();
builder.Services.AddScoped<HorizonTravel.Libraries.Sessao.Sessao>();
builder.Services.AddScoped<HorizonTravel.Libraries.Login.LoginUsuario>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();

//add para manipular a sess�o
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
app.UseSession();
app.UseAuthorization();
app.UseCookiePolicy();

app.UseMiddleware<ValidateAntiForgeryTokenMiddleware>();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name:"default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
