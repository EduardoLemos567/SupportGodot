using System;
using System.Collections.Generic;
using Godot;
using Support.Numerics;

namespace Support.FloodFill
{
    public class TextureFloodFiller : FloodFiller
    {
        private readonly Image image;
        private readonly Color color;
        public Image Result => image;
        public TextureFloodFiller(in Vec2<int> firstPixel, in Color color, in Image image)
            : base(PositionToId(firstPixel, image))
        {
            this.image = Image.CreateEmpty(image.GetWidth(), image.GetHeight(), image.HasMipmaps(), image.GetFormat());
            this.color = color;
        }
        public TextureFloodFiller(in Vec2<int> firstPixel, in Color color, in TextureFloodFiller other)
            : base(PositionToId(firstPixel, other.image))
        {
            image = other.image;
            this.color = color;
        }
        private static int PositionToId(in Vec2<int> pos, in Image texture) => pos.x + pos.y * texture.GetWidth();
        private static Vec2<int> IdToPosition(in int id, in Image texture)
        {
            var d = Math.DivRem(id, texture.GetWidth(), out var r);
            return new(r, d);
        }
        protected override bool IsAvailable(int id)
        {
            var pos = IdToPosition(id, image);
            return image.GetPixel(pos.x, pos.y) == default;
        }
        protected override void Consume(int id)
        {
            var pos = IdToPosition(id, image);
            image.SetPixel(pos.x, pos.y, color.ToGodotColor());
        }
        protected override IEnumerable<int> NeighborIds(int id)
        {
            var pos = IdToPosition(id, image);
            if (pos.y < image.GetHeight())
            {
                yield return PositionToId(pos + Vec2<int>.Up, image);
            }
            if (pos.x < image.GetWidth())
            {
                yield return PositionToId(pos + Vec2<int>.Right, image);
            }
            if (pos.y > 0)
            {
                yield return PositionToId(pos + Vec2<int>.Down, image);
            }
            if (pos.x > 0)
            {
                yield return PositionToId(pos + Vec2<int>.Left, image);
            }
        }
    }
}
