using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityMath.Test
{
    [TestClass]
    public class RoundTest
    {
        [TestMethod]
        public void Assert_Round_120344_Equals_100000()
        {

           Assert.AreEqual( UtilityMath.Conversion.RoundingHelper.RoundNearestPowerOfTen(120344),100000);

        }
        [TestMethod]
        public void Assert_Round_99444344_Equals_10000000()
        {

            Assert.AreEqual(UtilityMath.Conversion.RoundingHelper.RoundNearestPowerOfTen(99444344), 10000000);

        }
    }
}
