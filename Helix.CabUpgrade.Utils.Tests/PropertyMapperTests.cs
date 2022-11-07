using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using System.IO.MemoryMappedFiles;

namespace Helix.CabUpgrade.Utils.Tests
{
    public class PropertyMapperTests
    {
        public PropertyMapper CreateMapper()
        {
            var logMock = new Mock<ILogger<PropertyMapper>>();
            logMock.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var logLevel = (LogLevel)invocation.Arguments[0]; // The first two will always be whatever is specified in the setup above
                    var eventId = (EventId)invocation.Arguments[1];  // so I'm not sure you would ever want to actually use them
                    var state = invocation.Arguments[2];
                    var exception = (Exception)invocation.Arguments[3];
                    var formatter = invocation.Arguments[4];

                    var invokeMethod = formatter.GetType().GetMethod("Invoke");
                    var logMessage = (string)invokeMethod?.Invoke(formatter, new[] { state, exception });

                    Console.WriteLine(logMessage);

                    if (logLevel == LogLevel.Warning)
                    {
                        WarnLogs.Add(logMessage);
                    }
                }));

            return new PropertyMapper(logMock.Object);
        }

        List<string> WarnLogs { get; set; } = new List<string>();

        [Fact]
        public void EmptyPropertiesDoesNotThrow()
        {
            var properties = new List<JProperty>();

            var mapper = CreateMapper();
            mapper.UpdateBlockPropertyValue(properties, "@mic", "Mic");
        }

        [Fact]
        public void MissingProperty_LogsWarn_NoChange()
        {
            var properties = new List<JProperty>()
            {
                new JProperty("Model", "Mesa"),
                new JProperty("@path", 0)
            };
            var legacyProperty = "@mic";
            var newProperty = "Mic";

            var mapper = CreateMapper();
            mapper.UpdateBlockPropertyValue(properties, legacyProperty, newProperty);

            Assert.Single(WarnLogs);
            Assert.Equal($"Property {legacyProperty} not found in block: Model, @path - skipping property mapping for this property.", WarnLogs.SingleOrDefault());
            Assert.Equal(new List<JProperty>()
            {
                new JProperty("Model", "Mesa"),
                new JProperty("@path", 0)
            }, properties);
        }

        [Fact]
        public void MapsLegacyPropertyToNewName_MaintainsValue()
        {
            var properties = new List<JProperty>()
            {
                new JProperty("Model", "Mesa"),
                new JProperty("@mic", 5),
                new JProperty("@path", 0)
            };

            var mapper = CreateMapper();
            mapper.UpdateBlockPropertyValue(properties, "@mic", "Mic");

            Assert.Equal(new List<JProperty>()
            {
                new JProperty("Model", "Mesa"),
                new JProperty("@path", 0),
                new JProperty("Mic", 5)
            }, properties);
        }

        [Fact]
        public void MapsLegacyPropertyToNewName_OverridesValue()
        {
            var properties = new List<JProperty>()
            {
                new JProperty("Model", "Mesa"),
                new JProperty("@mic", 5),
                new JProperty("@path", 0)
            };

            var mapper = CreateMapper();
            mapper.UpdateBlockPropertyValue(properties, "@mic", "Mic", 10);

            Assert.Equal(new List<JProperty>()
            {
                new JProperty("Model", "Mesa"),
                new JProperty("@path", 0),
                new JProperty("Mic", 10)
            }, properties);
        }
    }
}