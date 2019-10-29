using HalfWerk.CommonModels.Auditlog;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HalfWerk.BffWebshop.Test
{
    [TestClass]
    public class ReplayEventsCommandTest
    {
        [TestMethod]
        public void ShouldCorrectlySetProperties()
        {
            // Arrange        
            // Act
            var replayEventsCommand = new ReplayEventsCommand()
            {
                ExchangeName = "ExchangeName",
                FromTimestamp = null,
                ToTimestamp = null,
                EventType = "EventType",
                Topic = "Topic"
            };

            // Assert
            Assert.AreEqual("ExchangeName", replayEventsCommand.ExchangeName);
            Assert.AreEqual(null, replayEventsCommand.FromTimestamp);
            Assert.AreEqual(null, replayEventsCommand.ToTimestamp);
            Assert.AreEqual("EventType", replayEventsCommand.EventType);
            Assert.AreEqual("Topic", replayEventsCommand.Topic);
        }
    }
}
