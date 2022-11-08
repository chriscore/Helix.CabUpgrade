using Helix.CabUpgrade.Utils.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Helix.CabUpgrade.Utils
{
    public class PresetUpdater : IPresetUpdater
    {
        private readonly ILogger<PresetUpdater> _logger;
        private readonly ICabMapper _cabMapper;
        private readonly IPropertyMapper _propertyMapper;
        private PresetUpdaterDefaults _defaults;

        private const string LegacyCabIdentifier = "HD2_Cab";
        private const string NewCabIdentifier = "HD2_CabMicIr";
        private const int SingleCabBlockType = 2;
        private const int DualCabBlockType = 4;

        public PresetUpdater(ILogger<PresetUpdater> logger, ICabMapper cabMapper, IPropertyMapper propertyMapper, PresetUpdaterDefaults defaults)
        {
            _logger = logger;
            _cabMapper = cabMapper;
            _propertyMapper = propertyMapper;
            _defaults = defaults;
        }

        /// <summary>
        /// Takes a JSON HLX file string and optional override cab model for primary and secondary
        /// </summary>
        /// <param name="presetContent"></param>
        /// <param name="overrideCabModel"></param>
        /// <returns></returns>
        public UpdatePresetJsonResponse UpdatePresetJson(string presetContent)
        {
            var json = JObject.Parse(presetContent);

            var version = json["version"].ToObject<int>();
            if (version != 6)
            {
                throw new Exception($"Invalid preset version: {version}");
            }

            var schema = json["schema"].ToObject<string>();
            if (!schema.Equals("L6Preset"))
            {
                throw new Exception($"Invalid schema: {schema}");
            }
            var patchName = json["data"]["meta"]["name"].ToObject<string>();

            UpdateCabsForDspNode(json, "dsp0");
            UpdateCabsForDspNode(json, "dsp1");

            // todo: reproduce the way helix hlx files are serialised..
            // maybe this is important, maybe not
            // this isnt right yet.
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

            var result = new UpdatePresetJsonResponse(sb.ToString(), patchName);
            return result;
        }

        private void UpdateCabsForDspNode(JObject json, string dsp)
        {
            var dspxBlocks = json["data"]["tone"][dsp].OfType<JProperty>();
            foreach (var block in dspxBlocks)
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
                    if (blockType == null) // secondary cabs and amp&cab block cabs will come in here
                    {
                        _logger.LogInformation($"upgrading attached cab block: {block.Name}");
                        UpgradeLegacyCab(props, _defaults.CabModelSecondaryOrAmpCabOverride, block.Name.StartsWith("cab")); // last arg always true?

                        json["data"]["tone"][dsp][blockName] = new JObject(props);
                    }
                    else if (blockType.Equals(new JValue(SingleCabBlockType)))
                    {
                        _logger.LogInformation($"upgrading legacy single cab block: {block.Name}");

                        UpgradeLegacyCab(props, _defaults.CabModelPrimaryOverride, block.Name.StartsWith("cab")); // last arg always false?

                        json["data"]["tone"][dsp][blockName] = new JObject(props);
                    }
                    else if (blockType.Equals(new JValue(DualCabBlockType)))
                    {
                        _logger.LogInformation($"upgrading legacy dual cab block: {block.Name}");

                        UpgradeLegacyCab(props, _defaults.CabModelPrimaryOverride, block.Name.StartsWith("cab")); // last arg always false?

                        json["data"]["tone"][dsp][blockName] = new JObject(props); // TODO: need to test dual cabs
                    }
                    else
                    {
                        throw new Exception($"Cab block type ({blockType}) not recognised");
                    }
                }
            }
        }

        internal void UpgradeLegacyCab(List<JProperty> cabProperties, string? overrideCabModel, bool isSecondaryDualCab)
        {
            /* 
            Properties which do not change:
            @enabled, @no_snapshot_bypass, @path, @position, Distance, HighCut, Level, LowCut

            Special case for dual cab: @cab, Delay (does not appear on the secondary), Pan (appears on primary & secondary)
            TODO: implement migration path for Delay and Pan

            Properties with a different key:
            @mic -> Mic

            New properties:	Angle (45.0 or 0?), Position (3dp float)

            Removed properties: EarlyReflections
            */

            var oldCabModel = cabProperties.SingleOrDefault(a => a.Name.Equals("@model")).Value.ToString();
            var newModel = _cabMapper.MapNewCabModel(oldCabModel, overrideCabModel);
            _propertyMapper.UpdateBlockPropertyValue(cabProperties, "@model", "@model", newModel);
            _propertyMapper.UpdateBlockPropertyValue(cabProperties, "@mic", "Mic");

            // Add new required properties with default values
            cabProperties.Add(new JProperty("Angle", _defaults.Angle));
            cabProperties.Add(new JProperty("Position", _defaults.Position));

            // try to remove early reflections
            bool success = cabProperties.Remove(cabProperties.SingleOrDefault(a => a.Name.Equals("EarlyReflections")));
        }
    }
}
