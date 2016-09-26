namespace LightInject.Tests
{
    using System;
    using Xunit;

    public class LoggingTests
    {
        [Fact]
        public void ShouldLogInfoMessage()
        {
            Action<LogEntry> logAction = entry =>
            {
                Assert.Equal(LogLevel.Info, entry.Level);
                Assert.Equal("SomeMessage", entry.Message);
            };
          
            logAction.Info("SomeMessage");  
        }

        [Fact]
        public void ShouldLogWarningMessage()
        {
            Action<LogEntry> logAction = entry =>
            {
                Assert.Equal(LogLevel.Warning, entry.Level);
                Assert.Equal("SomeMessage", entry.Message);
            };

            logAction.Warning("SomeMessage");
        }
    }
}