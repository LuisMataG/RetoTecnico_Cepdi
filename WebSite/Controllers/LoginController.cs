using System.Configuration;
using System.Web.Mvc;
namespace WebSite.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            string baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            ViewBag.BaseUrl = baseUrl;
            return View();
        }
    }
}