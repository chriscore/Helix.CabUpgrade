using Microsoft.Extensions.Logging;

namespace Helix.CabUpgrade.Utils
{
    public class CabMapper
    {
        private readonly ILogger<CabMapper> _logger;

        public Dictionary<string, string> CabModelMap { get; set; }

        public CabMapper()
        { }

        public CabMapper(ILogger<CabMapper> logger, Dictionary<string, string> cabModelMap)
        {
            if (cabModelMap == null)
            {
                throw new ArgumentException("Failed to create CabMapper. Cab model map cannot be null", "cabModelMap");
            }

            _logger = logger;
            CabModelMap = cabModelMap;
        }

        public virtual string MapNewCabModel(string oldCabModel, string? overrideCabModel)
        {
            _logger.LogInformation($"Mapping legacy cab model: {oldCabModel}");
            string? newCabModel = null;

            if (overrideCabModel != null)
            {
                newCabModel = overrideCabModel;
                _logger.LogInformation($"Mapped {oldCabModel} -> {newCabModel} using override");
            }
            else if (CabModelMap.ContainsKey(oldCabModel))
            {
                newCabModel = CabModelMap[oldCabModel];
                _logger.LogInformation($"Mapped {oldCabModel} -> {newCabModel}");
            }

            if (newCabModel == null)
            {
                throw new Exception($"No default mapping defined for cab {oldCabModel} and no override cab was chosen");
            }

            return newCabModel;
        }
    }
}