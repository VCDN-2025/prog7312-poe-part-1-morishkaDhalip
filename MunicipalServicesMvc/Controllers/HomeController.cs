using Microsoft.AspNetCore.Mvc;
using MunicipalServicesMvc.Services;

namespace MunicipalServicesMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IssueStore _store;
        public HomeController(IssueStore store) { _store = store; }

        public IActionResult Index() => View(_store.Recent(10));
    }
}
