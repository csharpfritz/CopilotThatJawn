using Microsoft.AspNetCore.Mvc;

namespace Web.ViewComponents
{
    public class NewsletterSignupViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
