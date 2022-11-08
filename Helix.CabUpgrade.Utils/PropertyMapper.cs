using Helix.CabUpgrade.Utils.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix.CabUpgrade.Utils
{
    public class PropertyMapper : IPropertyMapper
    {
        private readonly ILogger<PropertyMapper> _logger;

        public PropertyMapper(ILogger<PropertyMapper> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Updates a property value for a block
        /// </summary>
        /// <param name="properties">The block properties</param>
        /// <param name="oldKey">The legacy property key name</param>
        /// <param name="newKey">The new property key name</param>
        /// <param name="newValue">The new value to be set. Leave null to keep the legacy value</param>
        public void UpdateBlockPropertyValue(List<JProperty> properties, string oldKey, string newKey, object? newValue = null)
        {
            var oldKV = properties.SingleOrDefault(a => a.Name.Equals(oldKey));
            if (oldKV == null)
            {
                _logger.LogWarning($"Property {oldKey} not found in block: {string.Join(", ", properties.Select(p => p.Name))} - skipping property mapping for this property.");
                return;
            }

            JProperty newKV;
            if (newValue != null)
            {
                newKV = new JProperty(newKey, newValue);
            }
            else
            {
                newKV = new JProperty(newKey, oldKV.Value);
            }

            properties.Remove(oldKV);
            properties.Add(newKV);
        }
    }
}
