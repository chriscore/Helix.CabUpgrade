using Helix.CabUpgrade.Utils.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Helix.CabUpgrade.Utils.Tests")]

namespace Helix.CabUpgrade.Utils
{
    public class CabMapper : ICabMapper
    {
        private readonly ILogger<CabMapper> _logger;

        private Settings _settings;

        public CabMapper(ILogger<CabMapper> logger, IOptionsSnapshot<Settings> options)
        {
            _logger = logger;
            _settings = options.Value;
        }

        public virtual string MapNewCabModel(string oldCabModel, string? overrideCabModel, bool forceOverride = false)
        {
            _logger.LogInformation($"Mapping legacy cab model: {oldCabModel}");
            string? newCabModel = null;

            if (overrideCabModel != null & forceOverride)
            {
                newCabModel = overrideCabModel;
                _logger.LogInformation($"Mapped {oldCabModel} -> {newCabModel} using forced override");
            }
            else if (_settings.CabMapping.TryGetValue(oldCabModel, out var value))
            {
                newCabModel = value.Id;
                _logger.LogInformation($"Mapped {oldCabModel} -> {value.Name} from legacy cab key");
            }
            else if (overrideCabModel != null)
            {
                newCabModel = overrideCabModel;
                _logger.LogInformation($"Mapped {oldCabModel} -> {newCabModel} using fallback override - no legacy cab match");
            }

            if (newCabModel == null)
            {
                throw new Exception($"No default mapping defined for cab {oldCabModel} and no override cab was chosen. This means that there is not a new cab that directly corresponds to the legacy cab model '{oldCabModel}' in the new firmware. To continue, please choose an override cab whcih will be used instead from the drop down, and try again.");
            }

            return newCabModel;
        }
    }
}