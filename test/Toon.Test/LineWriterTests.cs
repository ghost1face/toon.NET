using System.Linq;
using Xunit;

namespace Toon.Test
{
    public class LineWriterTests
    {
        [Fact]
        public void PushAndToString_WritesIndentedLines()
        {
            var writer = new LineWriter(2);
            writer.Push("line1", 0);
            writer.Push('x', 1);

            var result = writer.ToString().Split('\n');

            Assert.Equal(2, result.Length);
            Assert.Contains("line1", result[0]);
            Assert.Contains("x", result[1]);
        }

        [Fact]
        public void Handles_Indentation_Size()
        {
            var indentationSize = 4;
            var writer = new LineWriter(indentationSize);

            writer.Push("first line", 1);
            writer.Push("second line", 5);

            var output = writer.ToString().Split('\n');

            Assert.Equal(2, output.Length);
            Assert.Contains(string.Join("", Enumerable.Repeat(" ", indentationSize * 1)), output[0]);
            Assert.Contains(string.Join("", Enumerable.Repeat(" ", indentationSize * 5)), output[1]);
        }
    }
}
