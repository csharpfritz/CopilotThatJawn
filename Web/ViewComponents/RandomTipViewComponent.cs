using Microsoft.AspNetCore.Mvc;
using Shared;
using Web.Models;
using Web.Services;

namespace Web.ViewComponents
{
    public class RandomTipViewComponent : ViewComponent
    {
        private readonly IContentService _contentService;
        private readonly ILogger<RandomTipViewComponent> _logger;
        
        public RandomTipViewComponent(IContentService contentService, ILogger<RandomTipViewComponent> logger)
        {
            _contentService = contentService;
            _logger = logger;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var tips = await _contentService.GetAllTipsAsync();
                
                if (tips.Any())
                {
                    var random = new Random();
                    var tipModel = tips[random.Next(tips.Count)];
                    var randomTipModel = new RandomTipModel(tipModel);
                    return View(randomTipModel);
                }
                
                return View(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting random tip");
                return View(null);
            }
        }
    }
}
