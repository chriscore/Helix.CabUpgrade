using FluentAssertions;
using Helix.CabUpgrade.Utils.Interfaces;
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

            var mockCabMapper = new Mock<ICabMapper>();
            mockCabMapper
                .Setup(a => a.MapNewCabModel(It.IsAny<string>(), It.IsAny<string?>()))
                .Returns(() => "Mocked Mapped Cab");

            var defaults = new PresetUpdaterDefaults();

            var propertyMapper = new PropertyMapper(mockPropertyMapper.Object);
            var updater = new PresetUpdater(mockPresetUpdaterLog.Object, mockCabMapper.Object, propertyMapper, defaults);

            string testCase = Resources.Legacy_Single_Cab;
            var transformedCab = JToken.Parse(updater.UpdatePresetJson(testCase).PresetJson).SelectToken("$.data.tone.dsp0.block0");

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

        [Fact]
        public void UpdatePresetJson_SimpleLegacySingleCab_dsp1()
        {
            var mockPresetUpdaterLog = new Mock<ILogger<PresetUpdater>>();
            var mockPropertyMapper = new Mock<ILogger<PropertyMapper>>();

            var mockCabMapper = new Mock<ICabMapper>();
            mockCabMapper
                .Setup(a => a.MapNewCabModel(It.IsAny<string>(), It.IsAny<string?>()))
                .Returns(() => "Mocked Mapped Cab");

            var defaults = new PresetUpdaterDefaults();

            var propertyMapper = new PropertyMapper(mockPropertyMapper.Object);
            var updater = new PresetUpdater(mockPresetUpdaterLog.Object, mockCabMapper.Object, propertyMapper, defaults);

            string testCase = Resources.Legacy_Single_Cab_Path_B;
            var transformedCab = JToken.Parse(updater.UpdatePresetJson(testCase).PresetJson).SelectToken("$.data.tone.dsp1.block0");

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

        [Fact]
        public void UpdatePresetJson_SimpleLegacyAmpAndCab_dsp0()
        {
            var mockPresetUpdaterLog = new Mock<ILogger<PresetUpdater>>();
            var mockPropertyMapper = new Mock<ILogger<PropertyMapper>>();

            var mockCabMapper = new Mock<ICabMapper>();
            mockCabMapper
                .Setup(a => a.MapNewCabModel(It.IsAny<string>(), It.IsAny<string?>()))
                .Returns(() => "Mocked Mapped Cab");

            var defaults = new PresetUpdaterDefaults();

            var propertyMapper = new PropertyMapper(mockPropertyMapper.Object);
            var updater = new PresetUpdater(mockPresetUpdaterLog.Object, mockCabMapper.Object, propertyMapper, defaults);

            string testCase = Resources.Legacy_Amp_and_Cab;
            var result = updater.UpdatePresetJson(testCase);
            var transformedCab = JToken.Parse(result.PresetJson).SelectToken("$.data.tone.dsp0.cab0");

            transformedCab.Should().BeEquivalentTo(JToken.FromObject(new Dictionary<string, object>
            {
                { "@enabled", true },
                { "Distance", 2.0 },
                { "HighCut", 8000.0 },
                { "Level", 0.0 },
                { "LowCut", 80.0 },
                { "@model", "Mocked Mapped Cab" },
                { "Mic", 6 },
                { "Angle", 45.0 },
                { "Position", 0.39 },
            }));
        }
    }
}