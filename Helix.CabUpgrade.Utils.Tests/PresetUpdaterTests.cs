using Helix.CabUpgrade.Utils.Tests.Properties;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;

namespace Helix.CabUpgrade.Utils.Tests
{
    public class PresetUpdaterTests
    {

        [Fact]
        public void MapNewCabModel_ThrowsWhenNullMap()
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
            var result = updater.UpdatePresetJson(testCase);


            Assert.Equal(JObject.Parse(testCase), JObject.Parse(result));
        }
    }
}