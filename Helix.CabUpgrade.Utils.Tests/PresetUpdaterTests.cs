using FluentAssertions;
using Helix.CabUpgrade.Utils.Tests.Properties;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;

namespace Helix.CabUpgrade.Utils.Tests
{
    public class PresetUpdaterTests
    {

        [Fact]
        public void UpdatePresetJson_SimpleLegacySingleCab_dsp0()
        {
            var mockPresetUpdaterLog = new Mock<ILogger<PresetUpdater>>();
            var mockPropertyMapper = new Mock<ILogger<PropertyMapper>>();

            var mockCabMapper = new Mock<CabMapper>();
            mockCabMapper
                .Setup(a => a.MapNewCabModel(It.IsAny<string>(), It.IsAny<string?>()))
                .Returns(() => "Mocked Mapped Cab");

            var propertyMapper = new PropertyMapper(mockPropertyMapper.Object);
            var updater = new PresetUpdater(mockPresetUpdaterLog.Object, mockCabMapper.Object, propertyMapper);

            string testCase = Resources.Legacy_Single_Cab;
            var transformedCab = JToken.Parse(updater.UpdatePresetJson(testCase)).SelectToken("$.data.tone.dsp0.block0");

            transformedCab.Should().BeEquivalentTo(JToken.FromObject(new Dictionary<string, object>
            {
                { "@enabled", true },
                { "@no_snapshot_bypass", false },
                { "@path", 0 },
                { "@position", 2 },
                { "@type", 2 },
                { "Distance", 2.0 },
                { "HighCut", 8000.0 },
                { "Level", 0.0 },
                { "LowCut", 80.0 },
                { "@model", "Mocked Mapped Cab" },
                { "Mic", 1 },
                { "Angle", 45.0 },
                { "Position", 0.39 },
            }));
        }
    }
}