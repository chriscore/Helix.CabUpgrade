using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix.CabUpgrade.Utils
{
    public class PresetUpdaterDefaults
    {
        public double Delay { get; set; } = 0.0;
        public double Pan { get; set; } = 0.50;
        public string? SelectedPrimaryCab { get; set; } = null;
        public string? SelectedSecondaryCab { get; set; } = null;
        public bool ForceOverridePrimaryCab { get; set; }
        public bool ForceOverrideSecondaryCab { get; set; }
        public decimal AnglePrimaryCab { get; set; } = decimal.Zero;
        public decimal AngleSecondaryCab { get; set; } = decimal.Zero;
        public decimal PositionPrimaryCab { get; set; } = 0.150M;
        public decimal PositionSecondaryCab { get; set; } = 0.150M;
    }
}
