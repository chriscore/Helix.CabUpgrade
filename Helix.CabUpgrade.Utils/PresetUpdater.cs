﻿using Helix.CabUpgrade.Utils.Enums;
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
        private readonly IMicMapper _micMapper;

        private const string LegacyCabIdentifier = "HD2_Cab";
        private const string NewCabIdentifier = "HD2_CabMicIr";
        private const int SingleCabBlockType = 2;
        private const int AmpAndCabBlockType = 3;
        private const int DualCabBlockType = 4;

        public PresetUpdater(ILogger<PresetUpdater> logger, ICabMapper cabMapper, IPropertyMapper propertyMapper, IMicMapper micMapper)
        {
            _logger = logger;
            _cabMapper = cabMapper;
            _propertyMapper = propertyMapper;
            _micMapper = micMapper;
        }

        /// <summary>
        /// Takes a JSON HLX file string and optional override cab model for primary and secondary
        /// </summary>
        /// <param name="presetContent"></param>
        /// <param name="overrideCabModel"></param>
        /// <returns></returns>
        public UpdatePresetJsonResponse UpdatePresetJson(string presetContent, PresetUpdaterDefaults defaults)
        {
            _logger.LogInformation($"Starting UpdatePresetJson with defaults: {JsonConvert.SerializeObject(defaults)} and patch data: {presetContent}");
            var json = JObject.Parse(presetContent);

            var version = json["version"].ToObject<int>();
            if (version != 6)
            {
                throw new Exception($"Invalid preset version: {version}. Helix Cab Upgrade currently only supports helix presets using version 6.");
            }

            var schema = json["schema"].ToObject<string>();
            if (!schema.Equals("L6Preset"))
            {
                throw new Exception($"Invalid schema: {schema}. Helix Cab Upgrade currently only supports preset files with schema 'L6Preset'.");
            }
            var patchName = json["data"]["meta"]["name"].ToObject<string>();

            UpdateCabsForDspNode(json, "dsp0", defaults);
            UpdateCabsForDspNode(json, "dsp1", defaults);

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
            _logger.LogInformation("Finished preset conversion");
            return result;
        }

        private void UpdateCabsForDspNode(JObject json, string dsp, PresetUpdaterDefaults defaults)
        {
            var dspxBlocks = json["data"]["tone"][dsp].OfType<JProperty>();
            foreach (var block in dspxBlocks)
            {
                var blockName = block.Name;
                var props = block.Children().Children().OfType<JProperty>().ToList();

                // find 'main' blocks containing legacy cabs only.
                // Blocks with a @model property starting with Legacy Cab Identifier patern
                // Exclude 'secondary' referenced cabs, we will deal with them later.
                if (props.Any(b => b.Name.Equals("@model")
                                && b.Value.ToString().StartsWith(LegacyCabIdentifier)
                                && !b.Value.ToString().StartsWith(NewCabIdentifier)
                                // TODO: exclude blocks which have an @cab property?
                    ) && !block.Name.StartsWith("cab")) 
                {
                    var blockType = block.Value["@type"];
                    /*
                     * if (blockType == null) // secondary cabs and amp&cab block cabs will come in here
                    {
                        _logger.LogInformation($"upgrading attached secondary cab block: {block.Name}");
                        UpgradeLegacyCab(props, defaults.CabModelSecondaryOrAmpCabOverride, defaults.ForceOverrideSecondaryCab, defaults);

                        json["data"]["tone"][dsp][blockName] = new JObject(props);
                    }
                    */
                    if (blockType.Equals(new JValue(SingleCabBlockType)))
                    {
                        _logger.LogInformation($"Upgrading legacy single cab block: {block.Name}");

                        UpgradeLegacyCab(props, defaults.CabModelPrimaryOverride, defaults.ForceOverridePrimaryCab, defaults, false);

                        json["data"]["tone"][dsp][blockName] = new JObject(props);
                    }
                    else if (blockType.Equals(new JValue(DualCabBlockType)))
                    {
                        _logger.LogInformation($"Upgrading legacy dual cab block: {block.Name}");

                        UpgradeLegacyCab(props, defaults.CabModelPrimaryOverride, defaults.ForceOverridePrimaryCab, defaults, true);
                        props.Add(new JProperty("Pan", defaults.Pan));
                        props.Add(new JProperty("Delay", defaults.Delay));

                        // write the main dual cab block properties to the document
                        json["data"]["tone"][dsp][blockName] = new JObject(props);


                        // Now update the linked cab block
                        var linkedCabBlockName = block.Value["@cab"].ToString();
                        if (linkedCabBlockName == null)
                        {
                            _logger.LogWarning($"No @cab property found on Dual block type with name {blockName}: {json}");
                            continue;
                        }

                        var cabProperties = json["data"]["tone"][dsp][linkedCabBlockName].Children().OfType<JProperty>().ToList();                        
                        UpgradeLinkedCab(cabProperties, defaults);
                        // write the secondary dual cab block properties to the document
                        json["data"]["tone"][dsp][linkedCabBlockName] = new JObject(props);
                    }
                    else if (blockType.Equals(new JValue(AmpAndCabBlockType)))
                    {
                        // TODO: implement for amp and cab
                        throw new NotImplementedException("Migration of Amp and Cab blocks are not yet supported - working on it, check back tomorrow!");
                    }
                    else
                    {
                        throw new Exception($"Cab block type ({blockType}) not recognised");
                    }
                }
            }
        }

        private void UpgradeLinkedCab(List<JProperty> cabProperties, PresetUpdaterDefaults defaults)
        {
            UpgradeLegacyCab(cabProperties, defaults.CabModelSecondaryOrAmpCabOverride, defaults.ForceOverrideSecondaryCab, defaults, true);

            // Delay
            cabProperties.Add(new JProperty("Delay", defaults.Delay));

            // Map mic, and change @mic to Mic
            UpdateMicProperty(cabProperties);

            // Pan
            cabProperties.Add(new JProperty("Pan", defaults.Pan));

            // TODO: implement for Amp and Cab block?
        }

        internal void UpgradeLegacyCab(List<JProperty> cabProperties, string? overrideCabModel, bool forceOverride, PresetUpdaterDefaults defaults, bool withPan)
        {
            /* 
            Properties which do not change:
            @enabled, @no_snapshot_bypass, @path, @position, Distance, HighCut, Level, LowCut

            Special case for dual cab: @cab, Delay (does not appear on the secondary), Pan (appears on primary & secondary)
            TODO: implement migration path for Delay and Pan (dual cab blocks)

            Properties with a different key:
            @mic -> Mic

            New properties:	Angle (45.0 or 0?), Position (3dp float)

            Removed properties: EarlyReflections
            */

            var oldCabModel = cabProperties.SingleOrDefault(a => a.Name.Equals("@model")).Value.ToString();
            var newModel = _cabMapper.MapNewCabModel(oldCabModel, overrideCabModel, forceOverride);

            if (withPan)
            {
                newModel += "WithPan";
            }

            _propertyMapper.UpdateBlockPropertyValue(cabProperties, "@model", "@model", newModel);

            UpdateMicProperty(cabProperties);

            // Add new required properties with default values
            cabProperties.Add(new JProperty("Angle", defaults.Angle));
            cabProperties.Add(new JProperty("Position", defaults.Position));

            // Try to remove early reflections
            cabProperties.Remove(cabProperties.SingleOrDefault(a => a.Name.Equals("EarlyReflections")));
        }

        internal void UpdateMicProperty(List<JProperty> cabProperties, NewMic defaultIfUnmappable = NewMic._57_Dynamic)
        {
            var newMic = _micMapper.UpdateMicProperty(cabProperties, defaultIfUnmappable);
            if (newMic != -1)
            {
                _propertyMapper.UpdateBlockPropertyValue(cabProperties, "@mic", "Mic", newMic);
            }
        }
    }
}
