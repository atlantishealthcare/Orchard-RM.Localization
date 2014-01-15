using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Localization.Models;
using RM.Localization.Models;

namespace RM.Localization.ViewModels
{
    public class CookieCultureItemViewModel
    {
        public CultureItemModel CultureItem { get; set; }
        public string Rel { get; set; }
        public bool Current { get; set; }
        public string ReturnUrl { get; set; }

    }
}
