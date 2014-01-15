using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Environment.Extensions;
using RM.Localization.Models;

namespace RM.Localization.ViewModels
{
    [OrchardFeature("RM.Localization.LocalizedTags")]
    public class LocalizedTagsIndexViewModel 
    {
        public IList<LocalizedTag> Tags { get; set; }
    }
}
