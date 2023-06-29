using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting();
builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression();

var app = builder.Build();

var configuration = app.Configuration.GetSection("Server");
var mimeTypeMappings = configuration.GetSection("MimeTypes");

var provider = new FileExtensionContentTypeProvider();

foreach (var (fileExtension, mimeType) in mimeTypeMappings.AsEnumerable())
{
    provider.Mappings.Remove(fileExtension);

    if (mimeType is not null)
    {
        provider.Mappings[fileExtension] = mimeType;
    }
}

app.UseResponseCaching();
app.UseResponseCompression();

app.UseDefaultFiles();

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider,
    ServeUnknownFileTypes = true,
    HttpsCompression = HttpsCompressionMode.Compress,
});

app.UseStatusCodePages();

app.Run();