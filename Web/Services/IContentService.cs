using Web.Models;

namespace Web.Services;

/// <summary>
/// Interface for content management operations
/// </summary>
public interface IContentService
{
    Task<List<TipModel>> GetAllTipsAsync();
    Task<TipModel?> GetTipBySlugAsync(string slug);
    Task<List<TipModel>> SearchTipsAsync(TipSearchRequest request);
    Task<List<string>> GetCategoriesAsync();
    Task<List<string>> GetTagsAsync();
    Task<List<TipModel>> GetRelatedTipsAsync(TipModel tip, int count = 3);
    Task RefreshContentAsync();
}
