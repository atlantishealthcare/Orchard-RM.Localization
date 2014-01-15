using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Extensions;
using Orchard.Themes;
using RM.Localization.Services;
using System.Text.RegularExpressions;

namespace RM.Localization.Controllers
{
    [HandleError, Themed]
    [OrchardFeature("RM.Localization.CookieCultureSelector")]
    public class CookieCultureController : Controller
    {
        private static Regex _regex = new Regex(@"rnd=\d*", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private readonly ICookieCultureService _cookieCultureService;
        private readonly ICultureService _cultureService;

        public CookieCultureController(ICookieCultureService cookieCultureService, ICultureService cultureService)
        {
            _cookieCultureService = cookieCultureService;
            _cultureService = cultureService;
        }

        [HttpGet]
        public ActionResult SetCulture(string culture, string returnUrl) {
            _cookieCultureService.SetCulture(culture);
            return this.RedirectLocal(MakeUniqueUrl(returnUrl));
        }

        [HttpGet]
        public ActionResult ResetCulture(string returnUrl)
        {
            _cookieCultureService.ResetCulture();
            return this.RedirectLocal(MakeUniqueUrl(returnUrl));
        }

        private string MakeUniqueUrl(string url)
        {
            var uri = new UriBuilder(new Uri(Request.Url, url));
            var rnd = string.Format("rnd={0}", DateTime.Now.Ticks.ToString());
            if (_regex.IsMatch(uri.Query))
            {
                uri.Query = _regex.Replace(uri.Query.Substring(1), rnd);
            }
            else
            {
                uri.Query = string.IsNullOrEmpty(uri.Query) ? rnd : string.Format("{0}&{1}", uri.Query.Substring(1), rnd);
            }
            return uri.ToString();
        }
    }
}
