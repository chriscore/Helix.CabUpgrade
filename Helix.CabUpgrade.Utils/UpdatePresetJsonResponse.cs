using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix.CabUpgrade.Utils
{
    public class UpdatePresetJsonResponse
    {
        public UpdatePresetJsonResponse(string presetJson, string patchName)
        {
            PresetJson = presetJson;
            PatchName = patchName;
        }

        public string PresetJson { get; set; }
        public string PatchName { get; set; }
    }
}
