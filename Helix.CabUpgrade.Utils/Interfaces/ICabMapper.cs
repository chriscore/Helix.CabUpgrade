namespace Helix.CabUpgrade.Utils.Interfaces
{
    public interface ICabMapper
    {
        string MapNewCabModel(string oldCabModel, string? overrideCabModel);
    }
}