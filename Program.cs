using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Scalax_server;
using ScalaxServer.Hubs;


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream", }
    );

});

builder.Services.AddSignalR();


var app = builder.Build();


//app.UseDefaultFiles();
//app.UseStaticFiles();
app.UseRouting();

app.Use(async (ctx, next) => {
    Console.Write($"\n\n\n\n root:: {Environment.ProcessPath} \n\n\n\n");
    await next.Invoke();
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider("/app/wwwroot/"),
});


app.UseEndpoints(eps =>
{
    eps.MapHub<CommunicateHub>("/Hubs/Communicate");
    
    eps.MapGet("/", () => Results.Ok(new { ok = true, }));


    eps.MapPost("/fu", async (HttpRequest req) =>
    {
        Console.Write($"\n\n {req?.Headers?.Referer} \n\n");
        Console.Write($"\n\n {req.Form.Files.ToArray().Length} \n\n");

        if (((bool)(!((req?.Headers?.Referer+" ")?.Contains("-")))))
            return Results.BadRequest("NO Ref -H");
        else if (!req.Form.Files.Any())
            return Results.BadRequest("0 F's");
        else if (!(req.Headers.Referer.ToString().Split('-').ElementAt(1).Equals(File.ReadAllText(CONSTANTS.ACTIVE_TOKENS_TXTFILE_PATH).Replace(" ", ""))))
            return Results.Unauthorized();
        else

            foreach (IFormFile file in req.Form.Files)
            {
                using FileStream? f_stream = new FileStream(Path.Combine(CONSTANTS.WWWROOT_DIR_PATH, file.FileName), FileMode.Create);
                await file.CopyToAsync(f_stream);
            }

        return Results.Ok(new
        {
            req.Form.Files.First().FileName,
            size = req.Form.Files.First().Length / 1024,
        });

    });


});


app.Run();
