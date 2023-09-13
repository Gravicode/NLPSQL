using System.Text;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using MudBlazor.Services;
using SemanticKernel.Data.NlpSql;
using NlpSql.DemoWeb.Data;

var builder = WebApplication.CreateBuilder(args);
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

var configBuilder = new ConfigurationBuilder()
  .SetBasePath(Directory.GetCurrentDirectory())
  .AddJsonFile("appsettings.json", optional: false);
IConfiguration Configuration = configBuilder.Build();

builder.Services.AddSingleton(KernelFactory.Create(Configuration));
builder.Services.AddSingleton(SqlConnectionProvider.Create(Configuration));
//builder.Services.AddSingleton<NlpSqlWeb>();

AppConstants.OpenAIApiKey = Configuration["OPENAI_API_KEY"];
AppConstants.OrgID = Configuration["ORG_ID"];
AppConstants.Model = Configuration["MODEL"];
AppConstants.SchemaNames = Configuration["SchemaNames"];

builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.MaximumReceiveMessageSize = 128 * 1024; // 1MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    //app.UseHttpsRedirection();
    StaticWebAssetsLoader.UseStaticWebAssets(
              app.Environment,
              app.Configuration);

}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
