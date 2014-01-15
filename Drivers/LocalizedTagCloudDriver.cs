using Orchard.ContentManagement.Drivers;
using RM.Localization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Environment.Extensions;

namespace RM.Localization.Drivers
{
    [OrchardFeature("RM.Localization.LocalizedTags")]
    public class LocalizedTagCloudDriver : ContentPartDriver<LocalizedTagCloudPart>
    {
        protected override DriverResult Display(LocalizedTagCloudPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_LocalizedTagCloud",
                () => shapeHelper.Parts_LocalizedTagCloud(Tags: part.Tags));
        }
    }
}
