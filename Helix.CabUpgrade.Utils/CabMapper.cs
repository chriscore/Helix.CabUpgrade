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

        public virtual string MapNewCabModel(string oldCabModel, string? overrideCabModel)
        {
            _logger.LogInformation($"Mapping legacy cab model: {oldCabModel}");
            string? newCabModel = null;

            if (overrideCabModel != null)
            {
                newCabModel = overrideCabModel;
                _logger.LogInformation($"Mapped {oldCabModel} -> {newCabModel} using override");
            }
            else if (_settings.CabMapping.ContainsKey(oldCabModel))
            {
                newCabModel = _settings.CabMapping[oldCabModel];
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