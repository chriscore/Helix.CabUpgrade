using Helix.CabUpgrade.Utils.Enums;
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
    public class MicMapper : IMicMapper
    {
        private readonly ILogger<MicMapper> _logger;

        public MicMapper(ILogger<MicMapper> logger)
        {
            _logger = logger;

            MicMappingTable = new Dictionary<LegacyMic, NewMic>()
            {
                { LegacyMic._57_Dynamic, NewMic._57_Dynamic }, // 0, 0
                //{ LegacyMic._409_Dynamic, NewMic.NONE }, // 1, x
                { LegacyMic._421_Dynamic, NewMic._421_Dynamic }, // 2, 1
                { LegacyMic._30_Dynamic, NewMic._30_Dynamic }, // 3, 4
                //{ LegacyMic._20_Dynamic, NewMic.NONE}, // 4, x
                { LegacyMic._121_Ribbon, NewMic._121_Ribbon}, // 5, 5
                { LegacyMic._160_Ribbon, NewMic._160_Ribbon }, // 6, 6
                { LegacyMic._4038_Ribbon, NewMic._4038_Ribbon }, // 7, 7
                { LegacyMic._414_Cond, NewMic._414_Cond }, // 8, 9
                //{ LegacyMic._84_Cond, NewMic.NONE}, // 9, x
                { LegacyMic._67_Cond, NewMic._67_Cond}, // 10, 11
                //{ LegacyMic._87_Cond, NewMic.NONE}, // 11, x
                { LegacyMic._47_Cond, NewMic._47_Cond_FET}, // 12, 10
                //{ LegacyMic._112_Dynamic, NewMic.NONE}, // 13, x
                //{ LegacyMic._12_Dynamic, NewMic.NONE}, // 14, x
                { LegacyMic._7_Dynamic, NewMic._7_Dynamic} // 15, 2

                // new mic _906_Dynamic is not used
            };
        }

        private Dictionary<LegacyMic, NewMic> MicMappingTable { get; set; }

        public int MapMic(int legacyMic, NewMic defaultMicIfNotMappable)
        {
            NewMic newMic;

            var legacy = (LegacyMic)legacyMic;
            if (MicMappingTable.TryGetValue(legacy, out newMic))
            {
                _logger.LogInformation($"Mapping legacy mic: {legacy} to new mic: {newMic}");
            }
            else
            {
                newMic = defaultMicIfNotMappable;
                _logger.LogInformation($"Could not map legacy mic: {legacy}. Using default new mic: {defaultMicIfNotMappable}");
            }

            return (int)newMic;
        }

        public int UpdateMicProperty(List<JProperty> cabProperties, NewMic defaultMicIfNotMappable)
        {
            var oldKV = cabProperties.SingleOrDefault(a => a.Name.Equals("@mic"));
            if (oldKV == null)
            {
                return -1; // no mic property found
            }

            var oldMic = oldKV.Value.ToObject<int>();
            return MapMic(oldMic, defaultMicIfNotMappable);
        }
    }
}
