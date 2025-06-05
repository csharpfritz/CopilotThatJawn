using Microsoft.AspNetCore.Mvc;

namespace Web.ViewComponents
{
    public class PopularToolsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
