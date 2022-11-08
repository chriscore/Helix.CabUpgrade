namespace Helix.CabUpgrade.Utils.Interfaces
{
    public interface IPresetUpdater
    {
        UpdatePresetJsonResponse UpdatePresetJson(string presetContent);
    }
}