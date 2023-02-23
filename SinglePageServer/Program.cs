using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting();
builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression();

var app = builder.Build();

var provider = new FileExtensionContentTypeProvider();
provider.Mappings.Remove(".m3u8");
provider.Mappings[".m3u8"] = "audio/x-mpegurl";

app.UseStatusCodePages();
app.UseResponseCaching();
app.UseResponseCompression();

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider,
    ServeUnknownFileTypes = true,
    HttpsCompression = HttpsCompressionMode.Compress,

});


app.MapFallbackToFile("index.html");

app.Run();