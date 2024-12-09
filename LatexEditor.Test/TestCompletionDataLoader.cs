using NUnit.Framework;

namespace LatexEditor.Test
{
    [TestFixture]
    public class TestCompletionDataLoader
    {
        [Test]
        public void TestGetFromFile()
        {
            var list = LatexCompletionDataLoader.GetFromFile("test.cwl");
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list[0], Is.EqualTo("hello"));
            Assert.That(list[1], Is.EqualTo("world"));
        }
    }
}
