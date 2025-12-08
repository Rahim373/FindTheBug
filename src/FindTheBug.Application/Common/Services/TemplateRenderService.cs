using FindTheBug.Application.Common.Interfaces;
using Fluid;

namespace FindTheBug.Application.Common.Services;

public class TemplateRenderService : ITemplateRenderService
{
    private readonly FluidParser _parser = new();

    public async Task<string> RenderAsync<TModel>(string templateName, TModel model)
    {
        // Construct the path to the template file
        // Assuming templates are stored in Features/{FeatureName}/Templates/{templateName}
        // But for a generic service, we might need a more flexible way to find templates.
        // For now, let's search recursively or assume a convention.
        // Given the current usage: Features/Invoices/Templates/InvoiceTemplate.html

        // Let's try to find the file in the base directory recursively
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var templateFiles = Directory.GetFiles(baseDir, templateName, SearchOption.AllDirectories);

        if (templateFiles.Length == 0)
        {
            throw new FileNotFoundException($"Template '{templateName}' not found in {baseDir}");
        }

        var templatePath = templateFiles[0];
        var templateSource = await File.ReadAllTextAsync(templatePath);

        if (!_parser.TryParse(templateSource, out var template, out var error))
        {
            throw new InvalidOperationException($"Failed to parse template '{templateName}': {error}");
        }

        var context = new TemplateContext(model);
        return await template.RenderAsync(context);
    }
}
