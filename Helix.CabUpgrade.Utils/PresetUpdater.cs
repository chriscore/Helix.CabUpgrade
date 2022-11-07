using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Helix.CabUpgrade.Utils
{
    public class PresetUpdater
    {
        private readonly ILogger<PresetUpdater> _logger;
        private readonly CabMapper _cabMapper;
        private readonly PropertyMapper _propertyMapper;

        private const string LegacyCabIdentifier = "HD2_Cab"; // must check this doesnt also match new cab identifier
        private const string NewCabIdentifier = "HD2_CabMicIr";
        private const int SingleCabBlockType = 2;
        private const int DualCabBlockType = 4;
        private const string overrideCabModel = null;

        public PresetUpdater(ILogger<PresetUpdater> logger, CabMapper cabMapper, PropertyMapper propertyMapper)
        {
            _logger = logger;
            _cabMapper = cabMapper;
            _propertyMapper = propertyMapper;
        }

        public string UpdatePresetJson(string presetContent)
        {
            var json = JObject.Parse(presetContent);

            var blocks = json["data"]["tone"]["dsp0"].OfType<JProperty>();

            foreach (var block in blocks)
            {
                var blockName = block.Name;
                var props = block.Children().Children().OfType<JProperty>().ToList();

                if (props.Any(b => b.Name.Equals("@model")
                                && b.Value.ToString().StartsWith(LegacyCabIdentifier)
                                && !b.Value.ToString().StartsWith(NewCabIdentifier)
                    )) // find legacy cabs only
                {
                    // @type property:	
                    // legacy single cab: 2, new single cab: 2, 
                    // amp & cab = 3
                    // legacy dual cab: 4, new dual cab: 4

                    var blockType = block.Value["@type"];
                    if (blockType.Equals(new JValue(SingleCabBlockType)))
                    {
                        _logger.LogInformation($"upgrading legacy single cab block: {block.Name}");

                        UpgradeLegacyCab(props, overrideCabModel, block.Name.StartsWith("cab"));

                        json["data"]["tone"]["dsp0"][blockName] = new JObject(props);
                    }
                    else if (blockType.Equals(new JValue(DualCabBlockType)))
                    {
                        _logger.LogInformation($"upgrading legacy dual cab block: {block.Name}");

                        UpgradeLegacyCab(props, overrideCabModel, block.Name.StartsWith("cab"));

                        json["data"]["tone"]["dsp0"][blockName] = new JObject(props);
                    }
                    else // secondary cabs and amp&cab block cabs will come in here
                    {
                        _logger.LogInformation($"upgrading attached cab block: {block.Name}");
                        UpgradeLegacyCab(props, overrideCabModel, block.Name.StartsWith("cab"));

                        json["data"]["tone"]["dsp0"][blockName] = new JObject(props);
                    }
                }
            }

            string result;
            
            // reproduce the way helix hlx files are serialised.. maybe this is important, maybe not
            var sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            using (var jtw = new JsonTextWriter(sw)
            {
                Formatting = Formatting.Indented,
                Indentation = 1,
                IndentChar = ' '
            })
            {
                (new JsonSerializer()).Serialize(jtw, json);
            }

            result = sb.ToString();
            return result;
        }
        
        internal void UpgradeLegacyCab(List<JProperty> cabProperties, string overrideCabModel, bool isSecondaryDualCab)
        {
            /* 
            Properties which do not change:
            @enabled, @no_snapshot_bypass, @path, @position, Distance, HighCut, Level, LowCut

            Special case for dual cab: @cab, Delay (does not appear on the secondary), Pan (appears on primary & secondary)

            Properties with a different key:
            @mic -> Mic

            New properties:	Angle (45.0 or 0?), Position (3dp float

            Removed properties: EarlyReflections
            */
            var oldCabModel = cabProperties.SingleOrDefault(a => a.Name.Equals("@model")).Value.ToString();
            var newModel = _cabMapper.MapNewCabModel(oldCabModel, overrideCabModel);

            _propertyMapper.UpdateBlockPropertyValue(cabProperties, "@mic", "Mic");

            // Add new required properties with default values
            cabProperties.Add(new JProperty("Angle", 45.0));
            cabProperties.Add(new JProperty("Position", 0.390));

            // try to remove early reflections
            bool success = cabProperties.Remove(cabProperties.SingleOrDefault(a => a.Name.Equals("EarlyReflections")));
        }
    }
}
