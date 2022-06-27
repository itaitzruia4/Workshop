using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataHandler = Workshop.DataLayer.DataHandler;
namespace Tests.UnitTests.DataLayer
{
    [TestClass]
    public class TestReviewsDAL
    {
        DataHandler dataHandler;

        [TestInitialize]
        public void InitSystem()
        {
            dataHandler = DataHandler.getDBHandler();
        }

        [TestMethod]
}
