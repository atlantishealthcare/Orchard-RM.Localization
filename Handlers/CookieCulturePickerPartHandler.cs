﻿using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using RM.Localization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RM.Localization.Handlers
{
    [OrchardFeature("RM.Localization.CookieCultureSelector")]
    public class CookieCulturePickerPartHandler : ContentHandler
    {
        public Localizer T { get; set; }

        public CookieCulturePickerPartHandler(IRepository<CookieCulturePickerPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
            T = NullLocalizer.Instance;
        }
    }
}
