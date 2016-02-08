using System.IO;
using System.Linq;
using NUnit.Framework;

namespace JsCollate
{
    [TestFixture]
    public class JsCollateTests
    {
        static string testDirectory = TestContext.CurrentContext.TestDirectory + "/";

        [SetUp] public void SetUp()
        {
            var path = testDirectory + "results";
            if (Directory.Exists(path)) Directory.Delete(path, true);
            Directory.CreateDirectory(path);
        }

        [Test] public void TestProgram_DataAttribute()
        {
            JsCollator.Collate(testDirectory + "webApp/app.html", testDirectory + "results", "Unit Test", true, true);
            Assert.IsTrue(File.Exists(testDirectory + "results/app.html"));
            Assert.IsTrue(File.Exists(testDirectory + "results/app.js"));
            Assert.IsFalse(File.Exists(testDirectory + "results/calc.js"));

            CompareFileContents("resultsFiles/app.js", "results/app.js");
            CompareFileContents("resultsFiles/test.js", "results/test.js");
            CompareFileContents("resultsFiles/app.html", "results/app.html");
        }

        [Test]
        public void TestProgram_ReplaceWithGrouping()
        {
            JsCollator.Collate(testDirectory + "webApp/replaceApp.html", testDirectory + "results", "Unit Test", true, true);
            Assert.IsTrue(File.Exists(testDirectory + "results/replaceApp.html"));
            Assert.IsTrue(File.Exists(testDirectory + "results/app.js"));
            Assert.IsFalse(File.Exists(testDirectory + "results/calc.js"));

            CompareFileContents("resultsFiles/app.js", "results/app.js");
            CompareFileContents("resultsFiles/test.js", "results/test.js");
            CompareFileContents("resultsFiles/app.html", "results/replaceApp.html");
        }

        private static void CompareFileContents(string expected, string actual)
        {
            var js = File.ReadAllText(testDirectory + "results/app.js");
            var compareJs = File.ReadAllText(testDirectory + "resultsFiles/app.js");
            Assert.AreEqual(compareJs, js);
        }

        [Test] public void TestCollate_DataAttribute()
        {
            var destFiles = ScriptCollator.Collate(testDirectory + "webApp/app.html", "results", false);

            Assert.AreEqual(2, destFiles.Count());
            var appjs = destFiles.First(x => x.FileName == "app.js");
            Assert.IsTrue(appjs != null);
            Assert.IsTrue(appjs.FileContents.Contains("function Calc()"));
            Assert.IsTrue(appjs.FileContents.Contains("document.addEventListener("));

            var testjs = destFiles.First(x => x.FileName == "test.js");
            Assert.IsTrue(testjs != null);
            Assert.IsFalse(testjs.FileContents.Contains("function Calc()"));
        }
        
        [Test] public void TestCollate_ReplaceWithGrouping()
        {
            var destFiles = ScriptCollator.Collate(testDirectory + "webApp/replaceApp.html", "results", false);

            Assert.AreEqual(2, destFiles.Count());
            var appjs = destFiles.First(x => x.FileName == "app.js");
            Assert.IsTrue(appjs != null);
            Assert.IsTrue(appjs.FileContents.Contains("function Calc()"));
            Assert.IsTrue(appjs.FileContents.Contains("document.addEventListener("));

            var testjs = destFiles.First(x => x.FileName == "test.js");
            Assert.IsTrue(testjs != null);
            Assert.IsFalse(testjs.FileContents.Contains("function Calc()"));
        }
    }
}
