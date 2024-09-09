using System;
using System.IO;
using System.Text;
using Godot;

namespace Support.Serialization
{
    public class Serializer<T> where T : GodotObject
    {
        private ImprovedMemoryStream data;
        public ReadOnlySpan<byte> Data => data.AsReadOnlySpan();
        public long DataSize => data.Length;
        public Serializer(T obj)
        {
            data = new(Encoding.UTF8.GetBytes(Json.Stringify(obj)))
            {
                Position = 0
            };
        }
        public Serializer(Deserializer<T> deserializer)
        {
            data = new(deserializer.Data)
            {
                Position = 0
            };
        }
        public Serializer<T> Compress(bool maximum = false)
        {
            var result = new ImprovedMemoryStream();
            Compression.Compress(data, result, maximum);
            data = result;
            data.Position = 0;
            return this;
        }
        public Serializer<T> Encrypt(string password)
        {
            var result = new ImprovedMemoryStream();
            Encryption.Encrypt(data, result, password);
            data = result;
            data.Position = 0;
            return this;
        }
        public void WriteFile(string path)
        {
            using var file = File.Open(path, FileMode.Create, System.IO.FileAccess.Write);
            data.CopyTo(file);
            file.Flush();
            file.Close();
            data.Position = 0;
        }
    }
}