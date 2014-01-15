using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.ContentManagement.Records;

namespace RM.Localization.Models
{
    [OrchardFeature("RM.Localization.CookieCultureSelector")]
    public class CookieCulturePickerPartRecord : ContentPartRecord
    {
        public virtual string Style { get; set; }
    }
}
