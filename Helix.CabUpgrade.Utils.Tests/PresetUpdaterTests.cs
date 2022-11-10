using FluentAssertions;
using Helix.CabUpgrade.Utils.Enums;
using Helix.CabUpgrade.Utils.Interfaces;
using Helix.CabUpgrade.Utils.Tests.Properties;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;

namespace Helix.CabUpgrade.Utils.Tests
{
    public class PresetUpdaterTests
    {
        private static CabMapper CreateCabMapper()
        {
            var mockCabLogger = new Mock<ILogger<CabMapper>>();
            var settings = Settings.FromString(Resources.Settings);
            var mockOptions = new Mock<IOptionsSnapshot<Settings>>();
            mockOptions.Setup(a => a.Value).Returns(settings);

            var cabMapper = new CabMapper(mockCabLogger.Object, mockOptions.Object);
            return cabMapper;
        }

        [Fact]
        public void UpdatePresetJson_SimpleLegacySingleCab_dsp0()
        {
            var mockPresetUpdaterLog = new Mock<ILogger<PresetUpdater>>();
            var mockPropertyMapper = new Mock<ILogger<PropertyMapper>>();
            var mockMicMapper = new Mock<IMicMapper>();
            mockMicMapper
                .Setup(a => a.MapMic(It.IsAny<int>(), It.IsAny<NewMic>()))
                .Returns(() => 0);

            var defaults = new PresetUpdaterDefaults();

            var propertyMapper = new PropertyMapper(mockPropertyMapper.Object);
            var updater = new PresetUpdater(mockPresetUpdaterLog.Object, CreateCabMapper(), propertyMapper, mockMicMapper.Object);

            string testCase = Resources.Legacy_Single_Cab;
            var transformedCab = JToken.Parse(updater.UpdatePresetJson(testCase, defaults).PresetJson).SelectToken("$.data.tone.dsp0.block0");

            Assert.Equal(true, transformedCab["@enabled"].ToObject<bool>());
            Assert.Equal(2.0, transformedCab["Distance"].ToObject<float>());
            Assert.Equal(8000.0, transformedCab["HighCut"].ToObject<float>());
            Assert.Equal(0.0, transformedCab["Level"].ToObject<float>());
            Assert.Equal(80.0, transformedCab["LowCut"].ToObject<float>());
            Assert.Equal("HD2_CabMicIr_4x12XXLV30", transformedCab["@model"].ToObject<string>());
            Assert.Equal(0, transformedCab["Mic"].ToObject<int>());
            Assert.Equal(45.0, transformedCab["Angle"].ToObject<float>());
            Assert.Equal(0.39, transformedCab["Position"].ToObject<float>(), 2);
        }

        [Fact]
        public void UpdatePresetJson_SimpleLegacySingleCab_dsp1()
        {
            var mockPresetUpdaterLog = new Mock<ILogger<PresetUpdater>>();
            var mockPropertyMapper = new Mock<ILogger<PropertyMapper>>();
            var mockMicMapper = new Mock<IMicMapper>();
            mockMicMapper
                .Setup(a => a.MapMic(It.IsAny<int>(), It.IsAny<NewMic>()))
                .Returns(() => 0);

            var defaults = new PresetUpdaterDefaults();

            var propertyMapper = new PropertyMapper(mockPropertyMapper.Object);
            var updater = new PresetUpdater(mockPresetUpdaterLog.Object, CreateCabMapper(), propertyMapper, mockMicMapper.Object);

            string testCase = Resources.Legacy_Single_Cab_Path_B;
            var transformedCab = JToken.Parse(updater.UpdatePresetJson(testCase, defaults).PresetJson).SelectToken("$.data.tone.dsp1.block0");

            Assert.Equal(true, transformedCab["@enabled"].ToObject<bool>());
            Assert.Equal(2.0, transformedCab["Distance"].ToObject<float>());
            Assert.Equal(8000.0, transformedCab["HighCut"].ToObject<float>());
            Assert.Equal(0.0, transformedCab["Level"].ToObject<float>());
            Assert.Equal(80.0, transformedCab["LowCut"].ToObject<float>());
            Assert.Equal("HD2_CabMicIr_4x12XXLV30", transformedCab["@model"].ToObject<string>());
            Assert.Equal(0, transformedCab["Mic"].ToObject<int>());
            Assert.Equal(45.0, transformedCab["Angle"].ToObject<float>());
            Assert.Equal(0.39, transformedCab["Position"].ToObject<float>(), 2);
        }

        [Fact]
        public void UpdatePresetJson_SimpleLegacyAmpAndCab_dsp0()
        {
            var mockPresetUpdaterLog = new Mock<ILogger<PresetUpdater>>();
            var mockPropertyMapper = new Mock<ILogger<PropertyMapper>>();
            var mockMicMapper = new Mock<IMicMapper>();
            mockMicMapper
                .Setup(a => a.MapMic(It.IsAny<int>(), It.IsAny<NewMic>()))
                .Returns(() => 0);

            var defaults = new PresetUpdaterDefaults();

            var propertyMapper = new PropertyMapper(mockPropertyMapper.Object);
            var updater = new PresetUpdater(mockPresetUpdaterLog.Object, CreateCabMapper(), propertyMapper, mockMicMapper.Object);

            string testCase = Resources.Legacy_Amp_and_Cab;
            var result = updater.UpdatePresetJson(testCase, defaults);
            var transformedCab = JToken.Parse(result.PresetJson).SelectToken("$.data.tone.dsp0.cab0");

            Assert.Equal(true, transformedCab["@enabled"].ToObject<bool>());
            Assert.Equal(1.0, transformedCab["Distance"].ToObject<float>());
            Assert.Equal(8000.0, transformedCab["HighCut"].ToObject<float>());
            Assert.Equal(0.0, transformedCab["Level"].ToObject<float>());
            Assert.Equal(80.0, transformedCab["LowCut"].ToObject<float>()); 
            Assert.Equal("HD2_CabMicIr_2x12JazzRivet", transformedCab["@model"].ToObject<string>());
            Assert.Equal(0, transformedCab["Mic"].ToObject<int>());
            Assert.Equal(45.0, transformedCab["Angle"].ToObject<float>());
            Assert.Equal(0.39, transformedCab["Position"].ToObject<float>(), 2);
        }

        [Fact]
        public void UpdatePresetJson_SimpleLegacyDualCab_dsp0()
        {
            var mockPresetUpdaterLog = new Mock<ILogger<PresetUpdater>>();
            var mockPropertyMapper = new Mock<ILogger<PropertyMapper>>();
            var mockMicMapper = new Mock<IMicMapper>();
            mockMicMapper
                .Setup(a => a.MapMic(It.IsAny<int>(), It.IsAny<NewMic>()))
                .Returns(() => 0);

            var defaults = new PresetUpdaterDefaults();

            var propertyMapper = new PropertyMapper(mockPropertyMapper.Object);
            var updater = new PresetUpdater(mockPresetUpdaterLog.Object, CreateCabMapper(), propertyMapper, mockMicMapper.Object);

            string testCase = Resources.Legacy_Dual_Cab;
            var result = updater.UpdatePresetJson(testCase, defaults);

            var transformedCabPrimary = JToken.Parse(result.PresetJson).SelectToken("$.data.tone.dsp0.block0");

            Assert.Equal("cab0", transformedCabPrimary["@cab"].ToObject<string>());
            Assert.Equal(true, transformedCabPrimary["@enabled"].ToObject<bool>());
            Assert.Equal(0, transformedCabPrimary["Mic"].ToObject<int>());
            Assert.Equal("HD2_CabMicIr_2x12MailC12QWithPan", transformedCabPrimary["@model"].ToObject<string>());
            Assert.Equal(false, transformedCabPrimary["@no_snapshot_bypass"].ToObject<bool>());
            Assert.Equal(0, transformedCabPrimary["@path"].ToObject<int>());
            Assert.Equal(4, transformedCabPrimary["@type"].ToObject<int>());
            Assert.Equal(2.50, transformedCabPrimary["Distance"].ToObject<float>());
            Assert.Equal(8000.0, transformedCabPrimary["HighCut"].ToObject<float>());
            Assert.Equal(0.0, transformedCabPrimary["Level"].ToObject<float>());
            Assert.Equal(80.0, transformedCabPrimary["LowCut"].ToObject<float>());
            Assert.Equal(45.0, transformedCabPrimary["Angle"].ToObject<float>());
            Assert.Equal(0.0, transformedCabPrimary["Delay"].ToObject<float>());
            Assert.Equal(2, transformedCabPrimary["@position"].ToObject<int>());
            Assert.Equal(0.50, transformedCabPrimary["Pan"].ToObject<float>());
            Assert.Equal(45.0, transformedCabPrimary["Angle"].ToObject<float>());
            Assert.Equal(0.39, transformedCabPrimary["Position"].ToObject<float>(), 2);

            var transformedCabSecondary = JToken.Parse(result.PresetJson).SelectToken("$.data.tone.dsp0.cab0");

            Assert.Equal(true, transformedCabSecondary["@enabled"].ToObject<bool>());
            Assert.Equal(2.50, transformedCabSecondary["Distance"].ToObject<float>());
            Assert.Equal(8000.0, transformedCabSecondary["HighCut"].ToObject<float>());
            Assert.Equal(0.0, transformedCabSecondary["Level"].ToObject<float>());
            Assert.Equal(80.0, transformedCabSecondary["LowCut"].ToObject<float>());
            Assert.Equal("HD2_CabMicIr_2x12MailC12QWithPan", transformedCabSecondary["@model"].ToObject<string>());
            Assert.Equal(0, transformedCabSecondary["Mic"].ToObject<int>());
            Assert.Equal(45.0, transformedCabSecondary["Angle"].ToObject<float>());
            Assert.Equal(0.39, transformedCabSecondary["Position"].ToObject<float>(), 2);
            Assert.Equal(0.0, transformedCabSecondary["Delay"].ToObject<float>());
            Assert.Equal(0.50, transformedCabSecondary["Pan"].ToObject<float>());
        }
    }
}