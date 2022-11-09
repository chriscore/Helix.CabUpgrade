using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix.CabUpgrade.Utils
{
    public class PresetUpdaterDefaults
    {
        public double Angle { get; set; } = 45.0;
        public double Position { get; set; } = 0.390;
        public string? CabModelPrimaryOverride { get; set; } = null;
        public string? CabModelSecondaryOrAmpCabOverride { get; set; } = null;
        public bool ForceOverridePrimaryCab { get; set; }
        public bool ForceOverrideSecondaryCab { get; set; }
    }
}
