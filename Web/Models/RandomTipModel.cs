using Shared;

namespace Web.Models;

/// <summary>
/// A wrapper class for Shared.TipModel to be used specifically with the RandomTip view component
/// </summary>
public class RandomTipModel
{
    private readonly TipModel _tipModel;

    public RandomTipModel(TipModel tipModel)
    {
        _tipModel = tipModel;
    }

    public string Title => _tipModel.Title;
    public string Category => _tipModel.Category;
    public List<string> Tags => _tipModel.Tags;
    public string Description => _tipModel.Description;
    public DateTime PublishedDate => _tipModel.PublishedDate;
    public string Author => _tipModel.Author;
    public string Difficulty => _tipModel.Difficulty;
    public int ReadingTimeMinutes => _tipModel.ReadingTimeMinutes;
    public string UrlSlug => _tipModel.UrlSlug;    public static implicit operator RandomTipModel?(TipModel? tipModel) => 
        tipModel != null ? new RandomTipModel(tipModel) : null;
}
