using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace RM.Localization
{
    [OrchardFeature("RM.Localization.LocalizedTags")]
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                             new RouteDescriptor {   Priority = 6,
                                                     Route = new Route(
                                                         "Tags/{tagName}",
                                                         new RouteValueDictionary {
                                                                                      {"area", "RM.Localization"},
                                                                                      {"controller", "LocalizedTags"},
                                                                                      {"action", "Search"}
                                                         },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "RM.Localization"}
                                                         },
                                                         new MvcRouteHandler())
                             },
                             new RouteDescriptor {   Priority = 6,
                                                     Route = new Route(
                                                         "Tags",
                                                         new RouteValueDictionary {
                                                                                      {"area", "RM.Localization"},
                                                                                      {"controller", "LocalizedTags"},
                                                                                      {"action", "Index"}
                                                         },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "RM.Localization"}
                                                         },
                                                         new MvcRouteHandler())
                             }
                         };
        }
    }
}