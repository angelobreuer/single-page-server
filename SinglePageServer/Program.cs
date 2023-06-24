using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting();
builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression();

var app = builder.Build();

var configuration = app.Configuration.GetSection("Server");
var provider = new FileExtensionContentTypeProvider();

var fallbackFile = configuration.GetValue<string?>("FallbackFile");
var mimeTypeMappings = configuration.GetSection("MimeTypes");

foreach (var (fileExtension, mimeType) in mimeTypeMappings.AsEnumerable())
{
    provider.Mappings.Remove(fileExtension);

    if (mimeType is not null)
    {
        provider.Mappings[fileExtension] = mimeType;
    }
}

app.UseStatusCodePages();
app.UseResponseCaching();
app.UseResponseCompression();
app.UseDefaultFiles();

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider,
    ServeUnknownFileTypes = true,
    HttpsCompression = HttpsCompressionMode.Compress,
});

if (fallbackFile is not null)
{
    app.MapFallbackToFile(fallbackFile);
}

app.Run();