namespace Helix.CabUpgrade.Utils
{
    public class Settings
    {
        /// <summary>
        /// A dictionary of legacy cab ID to new cabinfo
        /// </summary>
        public Dictionary<string, CabInfo> CabMapping { get; set; } = new Dictionary<string, CabInfo>();
    }
}
