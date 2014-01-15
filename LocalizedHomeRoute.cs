﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard;
using Orchard.Alias.Implementation.Holder;
using Orchard.ContentManagement.Aspects;
using Orchard.Localization.Services;
using Orchard.Mvc.Routes;
using Orchard.Alias.Implementation.Map;
using Orchard.ContentManagement;
using Orchard.Localization.Models;
using RM.Localization.Services;
using System.Globalization;
using Orchard.Environment.Extensions;

namespace RM.Localization
{
    [OrchardFeature("RM.Localization")]
    public class LocalizedHomeRoutesProvider : IRouteProvider
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public LocalizedHomeRoutesProvider(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new RouteDescriptor {
                    Priority = 11,
                    Route = new Route(
                        string.Empty,
                        new RouteValueDictionary {
                            {"area", "RM.Localization"},
                            {"controller", "LocalizedHome"},
                            {"action", "RedirectToCulture"}
                        },
                        new RouteValueDictionary (),
                        new RouteValueDictionary {
                            {"area", "RM.Localization"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 10,
                    Route = new Route(
                        "{culture}/NotTranslated/{id}",
                        new RouteValueDictionary {
                            {"area", "RM.Localization"},
                            {"controller", "LocalizedHome"},
                            {"action", "NotTranslated"},
                            {"culture", "-"},
                            {"id", string.Empty }
                        },
                        new RouteValueDictionary (),
                        new RouteValueDictionary {
                            {"area", "RM.Localization"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 0,
                    Route = new Route(
                        "{culture}/Culture",
                        new RouteValueDictionary {
                            {"area", "RM.Localization"},
                            {"controller", "CookieCulture"},
                            {"action", "SetCulture"}
                        },
                        new RouteValueDictionary (),
                        new RouteValueDictionary {
                            {"area", "RM.Localization"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 0,
                    Route = new Route(
                        "Browser/Culture",
                        new RouteValueDictionary {
                            {"area", "RM.Localization"},
                            {"controller", "CookieCulture"},
                            {"action", "ResetCulture"}
                        },
                        new RouteValueDictionary (),
                        new RouteValueDictionary {
                            {"area", "RM.Localization"}
                        },
                        new MvcRouteHandler())
                }
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes()) routes.Add(routeDescriptor);
        }
    }
}
