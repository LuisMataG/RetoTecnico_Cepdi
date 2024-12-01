using System.Configuration;
using System.Web.Mvc;

namespace WebSite.Controllers
{
    public class ObtenerPDFController : Controller
    {
        // GET: ObtenerPDF
        public ActionResult Index()
        {
            string WSUrl = ConfigurationManager.AppSettings["WSUrl"];
            ViewBag.WSUrl = WSUrl;
            return View();
        }
    }
}