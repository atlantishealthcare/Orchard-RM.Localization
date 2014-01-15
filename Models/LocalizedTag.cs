using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Environment.Extensions;

namespace RM.Localization.Models
{
    [OrchardFeature("RM.Localization.LocalizedTags")]
    public class LocalizedTag
    {
        public string TagName { get; set; }
        public int Count { get; set; }
        public int Bucket { get; set; }
    }
}
