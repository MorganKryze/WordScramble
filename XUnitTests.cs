using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Word_Scramble
{
    /// <summary>
    /// XUnit tests for the Player class
    /// </summary>
    public class XUnitTests
    {
        /// <summary>Fact is a type of test that you run when you are sure of the output.</summary>
        [Fact]
        public void Test1()
        {
            Player p = new Player();
            p.AddWord("test");
            Assert.Equal(4, p.Score);
        }
        /// <summary>Theory is a type of test that you run when you are unsure of the output and want to test multiple inputs.</summary>
        [Theory]
        [InlineData("test")]
        [InlineData("test2")]
        public void Test2(string word)
        {
            Player p = new Player();
            p.AddWord(word);
            Assert.Equal(word.Length, p.Score);
        }
    }
}