using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace JsCollate
{
    [TestFixture]
    public class JsCollateTests
    {
        [SetUp] public void SetUp()
        {
            if (Directory.Exists("results")) Directory.Delete("results", true);
            Directory.CreateDirectory("results");
        }

        [Test] public void TestProgram()
        {
            JsCollator.Collate("../../webApp/app.html", "results", "Unit Test", true);
            Assert.IsTrue(File.Exists("results/app.html"));
            Assert.IsTrue(File.Exists("results/app.js"));
            Assert.IsFalse(File.Exists("results/calc.js"));

            CompareFileContents("../../resultsFiles/app.js", "results/app.js");
            CompareFileContents("../../resultsFiles/test.js", "results/test.js");
            CompareFileContents("../../resultsFiles/app.html", "results/app.html");
        }

        private static void CompareFileContents(string expected, string actual)
        {
            var js = File.ReadAllText("results/app.js");
            var compareJs = File.ReadAllText("../../resultsFiles/app.js");
            Assert.AreEqual(compareJs, js);
        }

        [Test] public void TestCollate()
        {
            var destFiles = ScriptCollator.Collate("../../webApp/app.html", "results");

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
