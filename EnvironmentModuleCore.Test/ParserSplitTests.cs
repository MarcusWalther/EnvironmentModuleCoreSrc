
using EnvironmentModuleCore.Template;

namespace EnvironmentModuleCore.Test
{
    [TestClass]
    public class ParserSplitTests
    {
        /// <summary>
        /// Test if an empty command block is split correctly.
        /// </summary>
        [TestMethod]
        public void TestEmptyString()
        {
            var parser = new Parser();
            var parts = parser.SplitCommand("");
            Assert.HasCount(0, parts);
        }

        /// <summary>
        /// Test if a combined condition can be split.
        /// </summary>
        [TestMethod]
        public void TestCombinedCondition()
        {
            var parser = new Parser();
            var parts = parser.SplitCommand("Value == 5 && MyValue != 2.4");
            Assert.HasCount(7, parts);
            Assert.AreEqual("Value", parts[0]);
            Assert.AreEqual("&&", parts[3]);
            Assert.AreEqual("2.4", parts[6]);
        }

        /// <summary>
        /// Test if a string is detected correctly.
        /// </summary>
        [TestMethod]
        public void TestString()
        {
            var parser = new Parser();
            var parts = parser.SplitCommand("Value == \"My little text > 5 \" && 5 > 3");
            Assert.HasCount(7, parts);
            Assert.AreEqual("Value", parts[0]);
            Assert.AreEqual("==", parts[1]);
            Assert.AreEqual("\"My little text > 5 \"", parts[2]);
            Assert.AreEqual("5", parts[4]);
        }
    }
}
