using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Workshop.DomainLayer.Loggers;

namespace Tests.UnitTests.DomainLayer.Loggers
{
    [TestClass]
    public class TestLogger
    {
        private Logger log;

        [TestInitialize]
        public void Setup()
        {
            log = new Logger();
        }

        [TestMethod]
        public void TestLogEvent()
        {
            log.LogEvent("Testing event log");
        }

        [TestMethod]
        public void TestLogError()
        {
            log.LogEvent("Testing error log");
        }

        [TestMethod]
        public void TestLogDebug()
        {
            log.LogEvent("Testing debug log");
        }
    }
}