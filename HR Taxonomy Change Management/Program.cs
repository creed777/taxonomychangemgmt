using HR_Taxonomy_Change_Management.Domain;
using HR_Taxonomy_Change_Management.Misc;
using HR_Taxonomy_Change_Management.Repository;
using HR_Taxonomy_Change_Management.Service;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

//builder.Configuration.AddAzureAppConfiguration(options =>
//    options.Connect(
//        builder.Configuration["ConnectionStrings:AppConfig"])
//        .UseFeatureFlags());
//builder.Services.AddFeatureManagement();

builder.WebHost.UseStaticWebAssets();

var connectionString = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
builder.Services.AddDbContext<TaxonomyContext>(options =>
    options.UseSqlServer(connectionString),
    ServiceLifetime.Transient);

var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ') ?? builder.Configuration["MicrosoftGraph:Scopes"]?.Split(' ');

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
        .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
        .AddInMemoryTokenCaches();

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Logging.ClearProviders();
builder.Logging.AddDebug();
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorPages(options =>
    {
        options.Conventions.AuthorizeFolder("/Pages", "BasicAccess" );
    });

builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();
builder.Services.AddTelerikBlazor();

//Repository Injection
//builder.Services.AddScoped<TaxonomyContext>();
builder.Services.AddScoped<IChangeRepository, ChangeRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<ITaxonomyRepository, TaxonomyRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();

//Domain Injection
builder.Services.AddScoped<IRequestDomain, RequestDomain>();
builder.Services.AddScoped<IChangeDomain, ChangeDomain>();
builder.Services.AddScoped<ITaxonomyDomain, TaxonomyDomain>();
builder.Services.AddScoped<IAdminDomain, AdminDomain>();

//Service Injection
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IHelperService, HelperService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization(); 
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
