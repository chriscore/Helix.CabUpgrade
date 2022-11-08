using Helix.CabUpgrade.Utils.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Helix.CabUpgrade.Utils.Tests
{
    public class MicMapperTests
    {
        [Fact]
        public void MapMic_NoTargetExists_UsesDefault()
        {
            var mockLog = new Mock<ILogger<MicMapper>>();
            var mapper = new MicMapper(mockLog.Object);

            var result = mapper.MapMic((int)LegacyMic._20_Dynamic, NewMic._57_Dynamic);

            Assert.Equal(NewMic._57_Dynamic, (NewMic)result);
        }

        [Fact]
        public void MapMic_InvalidInput_UsesDefault()
        {
            var mockLog = new Mock<ILogger<MicMapper>>();
            var mapper = new MicMapper(mockLog.Object);

            var result = mapper.MapMic(999, NewMic._57_Dynamic);

            Assert.Equal(NewMic._57_Dynamic, (NewMic)result);
        }

        [Fact]
        public void MapMic_BothExist()
        {
            var mockLog = new Mock<ILogger<MicMapper>>();
            var mapper = new MicMapper(mockLog.Object);

            var result = mapper.MapMic((int)LegacyMic._421_Dynamic, NewMic._57_Dynamic);

            Assert.Equal(NewMic._421_Dynamic, (NewMic)result);
        }
    }
}
