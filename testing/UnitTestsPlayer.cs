namespace testing;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    [DataRow("test", 4)]
    public void TestMethod1(string word, int expectedScore)
    {
        var p = new Player();
        p.AddWord(word);
        Assert.AreEqual(expectedScore, p.Score);
    }
    [TestMethod]
    [DataRow("test")]
    [DataRow("test2")]
    public void TestMethod2(string word)
    {
        var p = new Player();
        p.AddWord(word);
        Assert.AreEqual(word.Length, p.Score);
    }
    

}