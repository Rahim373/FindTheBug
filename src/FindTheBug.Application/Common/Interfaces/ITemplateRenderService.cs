namespace FindTheBug.Application.Common.Interfaces;

public interface ITemplateRenderService
{
    Task<string> RenderAsync<TModel>(string templateName, TModel model);
}
