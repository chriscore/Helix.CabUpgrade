using Newtonsoft.Json.Linq;

namespace Helix.CabUpgrade.Utils.Interfaces
{
    public interface IPropertyMapper
    {
        void UpdateBlockPropertyValue(List<JProperty> properties, string oldKey, string newKey, object? newValue = null);
    }
}