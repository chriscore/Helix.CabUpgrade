﻿using FluentAssertions;
using Helix.CabUpgrade.Utils.Enums;
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
            var mockMicMapper = new Mock<IMicMapper>();
            mockMicMapper
                .Setup(a => a.MapMic(It.IsAny<int>(), It.IsAny<NewMic>()))
                .Returns(() => 0);

            var mockCabMapper = new Mock<ICabMapper>();
            mockCabMapper
                .Setup(a => a.MapNewCabModel(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<bool>()))
                .Returns(() => "Mocked Mapped Cab");


            var defaults = new PresetUpdaterDefaults();

            var propertyMapper = new PropertyMapper(mockPropertyMapper.Object);
            var updater = new PresetUpdater(mockPresetUpdaterLog.Object, mockCabMapper.Object, propertyMapper, mockMicMapper.Object);

            string testCase = Resources.Legacy_Single_Cab;
            var transformedCab = JToken.Parse(updater.UpdatePresetJson(testCase, defaults).PresetJson).SelectToken("$.data.tone.dsp0.block0");

            Assert.Equal(true, transformedCab["@enabled"].ToObject<bool>());
            Assert.Equal(2.0, transformedCab["Distance"].ToObject<float>());
            Assert.Equal(8000.0, transformedCab["HighCut"].ToObject<float>());
            Assert.Equal(0.0, transformedCab["Level"].ToObject<float>());
            Assert.Equal(80.0, transformedCab["LowCut"].ToObject<float>());
            Assert.Equal("Mocked Mapped Cab", transformedCab["@model"].ToObject<string>());
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

            var mockCabMapper = new Mock<ICabMapper>();
            mockCabMapper
                .Setup(a => a.MapNewCabModel(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<bool>()))
                .Returns(() => "Mocked Mapped Cab");

            var defaults = new PresetUpdaterDefaults();

            var propertyMapper = new PropertyMapper(mockPropertyMapper.Object);
            var updater = new PresetUpdater(mockPresetUpdaterLog.Object, mockCabMapper.Object, propertyMapper, mockMicMapper.Object);

            string testCase = Resources.Legacy_Single_Cab_Path_B;
            var transformedCab = JToken.Parse(updater.UpdatePresetJson(testCase, defaults).PresetJson).SelectToken("$.data.tone.dsp1.block0");

            Assert.Equal(true, transformedCab["@enabled"].ToObject<bool>());
            Assert.Equal(2.0, transformedCab["Distance"].ToObject<float>());
            Assert.Equal(8000.0, transformedCab["HighCut"].ToObject<float>());
            Assert.Equal(0.0, transformedCab["Level"].ToObject<float>());
            Assert.Equal(80.0, transformedCab["LowCut"].ToObject<float>());
            Assert.Equal("Mocked Mapped Cab", transformedCab["@model"].ToObject<string>());
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

            var mockCabMapper = new Mock<ICabMapper>();
            mockCabMapper
                .Setup(a => a.MapNewCabModel(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<bool>()))
                .Returns(() => "Mocked Mapped Cab");

            var defaults = new PresetUpdaterDefaults();

            var propertyMapper = new PropertyMapper(mockPropertyMapper.Object);
            var updater = new PresetUpdater(mockPresetUpdaterLog.Object, mockCabMapper.Object, propertyMapper, mockMicMapper.Object);

            string testCase = Resources.Legacy_Amp_and_Cab;
            var result = updater.UpdatePresetJson(testCase, defaults);
            var transformedCab = JToken.Parse(result.PresetJson).SelectToken("$.data.tone.dsp0.cab0");

            Assert.Equal(true, transformedCab["@enabled"].ToObject<bool>());
            Assert.Equal(3.0, transformedCab["Distance"].ToObject<float>());
            Assert.Equal(8000.0, transformedCab["HighCut"].ToObject<float>());
            Assert.Equal(0.0, transformedCab["Level"].ToObject<float>());
            Assert.Equal(80.0, transformedCab["LowCut"].ToObject<float>()); 
            Assert.Equal("Mocked Mapped Cab", transformedCab["@model"].ToObject<string>());
            Assert.Equal(0, transformedCab["Mic"].ToObject<int>());
            Assert.Equal(45.0, transformedCab["Angle"].ToObject<float>());
            Assert.Equal(0.39, transformedCab["Position"].ToObject<float>(), 2);
        }
    }
}