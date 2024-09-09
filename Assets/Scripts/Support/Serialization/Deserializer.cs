using System;
using System.IO;
using System.Text;
using Godot;

namespace Support.Serialization
{
    public class Deserializer<T> where T : GodotObject
    {
        private ImprovedMemoryStream data;
        public ReadOnlySpan<byte> Data => data.AsReadOnlySpan();
        public long DataSize => data.Length;
        public Deserializer(string path)
        {
            using var file = File.Open(path, FileMode.Open, System.IO.FileAccess.Read);
            data = new(file.Length);
            file.CopyTo(data);
            data.Flush();
            file.Close();
            data.Position = 0;
        }
        public Deserializer(Serializer<T> serializer)
        {
            data = new(serializer.Data)
            {
                Position = 0
            };
        }
        public Deserializer<T> Decompress()
        {
            var result = new ImprovedMemoryStream();
            Compression.Decompress(data, result);
            data = result;
            data.Position = 0;
            return this;
        }
        public Deserializer<T> Decrypt(string password)
        {
            var result = new ImprovedMemoryStream();
            Encryption.Decrypt(data, result, password);
            data = result;
            data.Position = 0;
            return this;
        }
        public T Unpack()
        {
            try
            {
                return (T)Json.ParseString(Encoding.UTF8.GetString(data.AsReadOnlySpan()));
            }
            finally
            {
                data.Position = 0;
            }
        }
    }
}