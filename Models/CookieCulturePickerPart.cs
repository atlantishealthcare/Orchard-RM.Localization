using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace RM.Localization.Models
{
    [OrchardFeature("RM.Localization.CookieCultureSelector")]
    public class CookieCulturePickerPart : ContentPart<CookieCulturePickerPartRecord>
    {
        public virtual string Style { get { return Record.Style; } set { Record.Style = value; } }
    }
}
