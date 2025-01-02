using System.IO;
using System.IO.Compression;
using BrotliCompressionLevel = System.IO.Compression.CompressionLevel;

namespace Support
{
    public static class Compression
    {
        public static void Compress(Stream input, Stream output, bool maximum = false)
        {
            var press = new BrotliStream(output, maximum ? BrotliCompressionLevel.Optimal : BrotliCompressionLevel.Fastest);
            input.CopyTo(press);
            press.Flush();
        }
        public static void Decompress(Stream input, Stream output)
        {
            var press = new BrotliStream(input, CompressionMode.Decompress);
            press.CopyTo(output);
            press.Flush();
        }
    }
}