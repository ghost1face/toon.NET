namespace Toon
{
    public interface ILineWriter
    {
        void Push(string content, int depth);
        void Push(char content, int depth);
        string ToString();
        void Flush();
    }
}
