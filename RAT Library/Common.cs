using System.Text;

namespace RAT_Library
{
    public static class Common
    {
        public static readonly Encoding MessageEncoding = Encoding.GetEncoding(1251);
        public const int BufferSize = 0xFFFFFF;
        public const int Port = 80;
    }
}
