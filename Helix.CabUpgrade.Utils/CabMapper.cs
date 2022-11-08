using Helix.CabUpgrade.Utils.Interfaces;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Helix.CabUpgrade.Utils.Tests")]

namespace Helix.CabUpgrade.Utils
{
    public class CabMapper : ICabMapper
    {
        private readonly ILogger<CabMapper> _logger;

        private Dictionary<string, string> CabModelMap { get; set; }

        public CabMapper(ILogger<CabMapper> logger, CabMapConfiguration cabModelMap)
        {
            if (cabModelMap == null)
            {
                throw new ArgumentException("Failed to create CabMapper. Cab model map cannot be null", "cabModelMap");
            }

            _logger = logger;
            CabModelMap = cabModelMap._cabMap;
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