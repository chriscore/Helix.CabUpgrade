using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Helix.CabUpgrade.Utils.Tests
{
    public class CabMapperTests
    {
        [Fact]
        public void MapNewCabModel_ThrowsWhenEmptyMap()
        {
            var settings = new Settings();
            var mockOptions = new Mock<IOptionsSnapshot<Settings>>();
            mockOptions.Setup(a => a.Value).Returns(settings);
            var mockLog = new Mock<ILogger<CabMapper>>();
            var mapper = new CabMapper(mockLog.Object, mockOptions.Object);

            Assert.Throws<Exception>(() => mapper.MapNewCabModel("missing", null));
        }

        [Fact]
        public void MapNewCabModel_ThrowsWhenMissingDefaultNoOverride()
        {
            var settings = new Settings();
            settings.CabMapping.Add("HD2_Cab4x12XXLV30", new CabInfo { Id = "HD2_CabMicIr_4x12MOONT75", Name = "TestNewCab" });
            var mockOptions = new Mock<IOptionsSnapshot<Settings>>();
            mockOptions.Setup(a => a.Value).Returns(settings);

            var mockLog = new Mock<ILogger<CabMapper>>();
            var mapper = new CabMapper(mockLog.Object, mockOptions.Object);

            Assert.Throws<Exception>(() => mapper.MapNewCabModel("missing", null));
        }

        [Fact]
        public void MapNewCabModel_MapsWhenLegacyCabMatchedNoOverride()
        {
            var settings = new Settings();
            settings.CabMapping.Add("HD2_Cab4x12XXLV30", new CabInfo { Id = "HD2_CabMicIr_4x12MOONT75", Name = "TestNewCab" });
            var mockOptions = new Mock<IOptionsSnapshot<Settings>>();
            mockOptions.Setup(a => a.Value).Returns(settings);

            var mockLog = new Mock<ILogger<CabMapper>>();
            var mapper = new CabMapper(mockLog.Object, mockOptions.Object);

            var newCab = mapper.MapNewCabModel("HD2_Cab4x12XXLV30", null);
            Assert.Equal("HD2_CabMicIr_4x12MOONT75", newCab);
        }

        [Fact]
        public void MapNewCabModel_MapsWhenLegacyCabMatched_WithoutForceOverride()
        {
            var settings = new Settings();
            settings.CabMapping.Add("HD2_Cab4x12XXLV30", new CabInfo { Id = "HD2_CabMicIr_4x12MOONT75", Name = "TestNewCab" });
            var mockOptions = new Mock<IOptionsSnapshot<Settings>>();
            mockOptions.Setup(a => a.Value).Returns(settings);

            var mockLog = new Mock<ILogger<CabMapper>>();
            var mapper = new CabMapper(mockLog.Object, mockOptions.Object);

            var newCab = mapper.MapNewCabModel("HD2_Cab4x12XXLV30", "Kittens", false);
            Assert.Equal("HD2_CabMicIr_4x12MOONT75", newCab);
        }

        [Fact]
        public void MapNewCabModel_MapsWhenLegacyCabMatched_WithForceOverride()
        {
            var settings = new Settings();
            settings.CabMapping.Add("HD2_Cab4x12XXLV30", new CabInfo { Id = "HD2_CabMicIr_4x12MOONT75", Name = "TestNewCab" });
            var mockOptions = new Mock<IOptionsSnapshot<Settings>>();
            mockOptions.Setup(a => a.Value).Returns(settings);

            var mockLog = new Mock<ILogger<CabMapper>>();
            var mapper = new CabMapper(mockLog.Object, mockOptions.Object);

            var newCab = mapper.MapNewCabModel("HD2_Cab4x12XXLV30", "Kittens", true);
            Assert.Equal("Kittens", newCab);
        }

        [Fact]
        public void MapNewCabModel_MapsWhenLegacyCabNotMatched_WithoutForceOverride()
        {
            var settings = new Settings();
            settings.CabMapping.Add("HD2_Cab4x12XXLV30", new CabInfo { Id = "HD2_CabMicIr_4x12MOONT75", Name = "TestNewCab" });
            var mockOptions = new Mock<IOptionsSnapshot<Settings>>();
            mockOptions.Setup(a => a.Value).Returns(settings);

            var mockLog = new Mock<ILogger<CabMapper>>();
            var mapper = new CabMapper(mockLog.Object, mockOptions.Object);

            var newCab = mapper.MapNewCabModel("Dogs", "Kittens");
            Assert.Equal("Kittens", newCab);
        }

        [Fact]
        public void MapNewCabModel_MapsWhenLegacyCabNotMatched_WithForceOverride()
        {
            var settings = new Settings();
            settings.CabMapping.Add("HD2_Cab4x12XXLV30", new CabInfo { Id = "HD2_CabMicIr_4x12MOONT75", Name = "TestNewCab" });
            var mockOptions = new Mock<IOptionsSnapshot<Settings>>();
            mockOptions.Setup(a => a.Value).Returns(settings);

            var mockLog = new Mock<ILogger<CabMapper>>();
            var mapper = new CabMapper(mockLog.Object, mockOptions.Object);

            var newCab = mapper.MapNewCabModel("Dogs", "Kittens", true);
            Assert.Equal("Kittens", newCab);
        }
    }
}