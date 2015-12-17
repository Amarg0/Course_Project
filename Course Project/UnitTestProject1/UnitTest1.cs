using System;
using Course_Project;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using EasyAssertions;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string text =
                System.IO.File.ReadAllText(
                    @"C:\Users\Amargo\Documents\GitHub\Course_Project\Course Project\TestBase\1.txt");
            Should.Throw<PositiveException>(
                () => Parser.GetAnalysisResult(new ArgsForAnalysisThread(text, new ProgressBar())));

        }
    }
}
