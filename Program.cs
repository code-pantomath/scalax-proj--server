using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

using ScalaxServer.Hubs;


File.CreateText(Environment.CurrentDirectory + "active_cons.txt");


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" }    
    );

});

builder.Services.AddSignalR();


var app = builder.Build();


//app.UseDefaultFiles();
//app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints(eps =>
{
    eps.MapHub<CommunicateHub>("/Hubs/Communicate");
});

//app.MapBlazorHub();
//app.MapHub<CommunicateHub>("/Hubs/Communicate");
//app.MapFallbackToPage("/_Host");


app.Run();