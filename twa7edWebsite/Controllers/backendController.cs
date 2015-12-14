using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace twa7edWebsite.Controllers
{
    [Authorize]
    public class backendController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
