using System;
using System.Collections.Generic;
using System.Web;
using Orchard.Caching;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;
using RM.Localization.Services;

namespace RM.Localization.Providers
{
    [OrchardFeature("RM.Localization.CookieCultureSelector")]
    public class CookieCultureSelector : ICultureSelector
    {
        private readonly ICookieCultureService _cookieCultureService;
        private readonly Lazy<ICultureManager> _lazyCultureManager;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;

        public CookieCultureSelector(ICookieCultureService cookieCultureService,
            Lazy<ICultureManager> lazyCultureManager,
            ICacheManager cacheManager,
            ISignals signals)
        {
            _cookieCultureService = cookieCultureService;
            _lazyCultureManager = lazyCultureManager;
            _cacheManager = cacheManager;
            _signals = signals;
        }

        public CultureSelectorResult GetCulture(HttpContextBase context) {
            
            var cultureCookie = _cookieCultureService.GetCulture();
            if (cultureCookie == null || cultureCookie == "Browser") return null;

            var cultureName = CultureHelper.GetSpecificOrNeutralCulture(ListCultures(), cultureCookie);

            return cultureName == null ? null : new CultureSelectorResult { Priority = 0, CultureName = cultureName };
        }

        public IEnumerable<string> ListCultures() {
            return _cacheManager.Get("ListCultures", ctx => {
                ctx.Monitor(_signals.When("culturesChanged"));

                return _lazyCultureManager.Value.ListCultures();
            });
        }
    }
}
