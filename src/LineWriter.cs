using System.Text;

namespace Toon
{
    internal class LineWriter : ILineWriter
    {
        private readonly StringBuilder builder;
        private readonly string indentationString;

        public LineWriter(int indentationSize)
        {
            builder = new StringBuilder();

            indentationString = new string(Constants.Space, indentationSize);
        }

        public void Push(string content, int depth)
        {
            var indent = Repeat(indentationString, depth);

            if (builder.Length > 0)
            {
                builder.Append(Constants.NewLine);
            }

            builder.Append(indent)
                .Append(content);
        }

        public void Push(char content, int depth)
        {
            var indent = Repeat(indentationString, depth);

            if (builder.Length > 0)
            {
                builder.Append(Constants.NewLine);
            }

            builder.Append(indent)
                .Append(content);
        }

        public override string ToString()
        {
            return builder.ToString();
        }

        public void Flush()
        {
            // no-op for this implementation
        }

        private static string Repeat(string text, int count)
        {
            var output = "";
            for (int i = 0; i < count; i++)
            {
                output += text;
            }

            return output;
        }
    }
}
