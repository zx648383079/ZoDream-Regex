using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZoDream.Shared;

namespace ZoDream.Tests;
[TestClass]
public class RegexTest
{
    [TestMethod]
    public void TestRender()
    {
        var regex = new RegexAnalyze("aa1222", @"\d");
        regex.Match();
        var content = regex.Compiler("{for} {" +
            "   '{}'," +
            "} {end}");
        Assert.IsTrue(content.IndexOf("1") > 0);
    }
}