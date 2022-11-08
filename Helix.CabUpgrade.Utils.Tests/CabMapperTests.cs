using Microsoft.Extensions.Logging;
using Moq;

namespace Helix.CabUpgrade.Utils.Tests
{
    public class CabMapperTests
    {
        [Fact]
        public void MapNewCabModel_ThrowsWhenNullMap()
        {
            var mockLog = new Mock<ILogger<CabMapper>>();
            
            Assert.Throws<ArgumentException>(() => new CabMapper(mockLog.Object, null));
        }

        [Fact]
        public void MapNewCabModel_ThrowsWhenEmptyMap()
        {
            var map = new CabMapConfiguration();
            var mockLog = new Mock<ILogger<CabMapper>>();
            var mapper = new CabMapper(mockLog.Object, map);

            Assert.Throws<Exception>(() => mapper.MapNewCabModel("missing", null));
        }

        [Fact]
        public void MapNewCabModel_ThrowsWhenMissingDefaultNoOverride()
        {
            var map = new CabMapConfiguration();
            map._cabMap.Add("HD2_Cab4x12XXLV30", "HD2_CabMicIr_4x12MOONT75");
            var mockLog = new Mock<ILogger<CabMapper>>();
            var mapper = new CabMapper(mockLog.Object, map);

            Assert.Throws<Exception>(() => mapper.MapNewCabModel("missing", null));
        }

        [Fact]
        public void MapNewCabModel_MapsWhenLegacyCabMatchedNoOverride()
        {
            var map = new CabMapConfiguration();
            map._cabMap.Add("HD2_Cab4x12XXLV30", "HD2_CabMicIr_4x12MOONT75");
            var mockLog = new Mock<ILogger<CabMapper>>();
            var mapper = new CabMapper(mockLog.Object, map);

            var newCab = mapper.MapNewCabModel("HD2_Cab4x12XXLV30", null);
            Assert.Equal("HD2_CabMicIr_4x12MOONT75", newCab);
        }

        [Fact]
        public void MapNewCabModel_MapsWhenLegacyCabMatched_WithOverride()
        {
            var map = new CabMapConfiguration();
            map._cabMap.Add("HD2_Cab4x12XXLV30", "HD2_CabMicIr_4x12MOONT75");
            var mockLog = new Mock<ILogger<CabMapper>>();
            var mapper = new CabMapper(mockLog.Object, map);

            var newCab = mapper.MapNewCabModel("HD2_Cab4x12XXLV30", "Kittens");
            Assert.Equal("Kittens", newCab);
        }

        [Fact]
        public void MapNewCabModel_MapsWhenLegacyCabNotMatched_WithOverride()
        {
            var map = new CabMapConfiguration();
            map._cabMap.Add("HD2_Cab4x12XXLV30", "HD2_CabMicIr_4x12MOONT75");
            var mockLog = new Mock<ILogger<CabMapper>>();
            var mapper = new CabMapper(mockLog.Object, map);

            var newCab = mapper.MapNewCabModel("Dogs", "Kittens");
            Assert.Equal("Kittens", newCab);
        }
    }
}