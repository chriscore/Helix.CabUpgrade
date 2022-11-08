using Helix.CabUpgrade.Utils.Enums;
using Newtonsoft.Json.Linq;

namespace Helix.CabUpgrade.Utils.Interfaces
{
    public interface IMicMapper
    {
        int MapMic(int legacyMic, NewMic defaultMicIfNotMappable);
        int UpdateMicProperty(List<JProperty> cabProperties, NewMic defaultMicIfNotMappable);
    }
}