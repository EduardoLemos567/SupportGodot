using System;
using System.IO;

namespace Support
{
    public class ImprovedMemoryStream : Stream
    {
        private byte[] buffer;
        private long position;  // cursor position
        private long used;     // used length in the buffer array
        private readonly long? maxCapacity;
        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => true;
        public override long Length => used;
        public long Capacity
        {
            get => buffer.LongLength;
            set => Resize(value);
        }
        /// <summary>
        /// Number of bytes left to read (position cursor distance to the user/length).
        /// </summary>
        public long UnreadData => used - position;   // Amount of data left to read
        public ImprovedMemoryStream(byte[] initialBuffer, long? maxCapacity = null)
        {
            buffer = initialBuffer;
            position = used = initialBuffer.Length;
            this.maxCapacity = maxCapacity;
        }
        public ImprovedMemoryStream(ReadOnlySpan<byte> initialBuffer, long? maxCapacity = null)
        {
            buffer = initialBuffer.ToArray();
            position = used = initialBuffer.Length;
            this.maxCapacity = maxCapacity;
        }
        public ImprovedMemoryStream(long initialCapacity = 4096, long? maxCapacity = null)
        {
            buffer = new byte[initialCapacity];
            this.maxCapacity = maxCapacity;
        }
        public override long Position
        {
            get => position;
            set => position = Math.Clamp(value, 0, used);
        }
        public override void Flush() { }
        public override int Read(byte[] destinationBuffer, int destinationOffset, int count)
        {
            if (destinationOffset + count > destinationBuffer.Length)
            {
                throw new Exception("'destinationBuffer' has not enough space");
            }
            if (count > 0)
            {
                var unreadData = (int)UnreadData;
                if (count > unreadData) { count = unreadData; }
                Array.Copy(buffer, position, destinationBuffer, destinationOffset, count);
                position += count;
            }
            return count;
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    break;
                case SeekOrigin.Current:
                    offset += position;
                    break;
                case SeekOrigin.End:
                    offset = used - offset;
                    break;
            }
            position = Math.Clamp(offset, 0, used);
            return position;
        }
        /// <summary>
        /// Set occupancy in the buffer array.
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            if (value > Capacity)
            {
                Resize(value);
            }
            used = value;
            if (position > used)
            {
                position = used;
            }
        }
        public override void Write(byte[] originBuffer, int originOffset, int count)
        {
            if (originOffset + count > originBuffer.Length)
            {
                throw new Exception("Buffer has not enough space");
            }
            if (position + count > Capacity)
            {
                Resize(IncreaseCapacityStrategy(position + count));
            }
            Array.Copy(originBuffer, originOffset, buffer, position, count);
            position += count;
            if (position > used) { used = position; }
        }
        /// <summary>
        /// Moves anything after initialPosition to the begining of the buffer, clear everything after.
        /// </summary>
        /// <param name="initialPosition"></param>
        public void ClearCompactUnread(long initialPosition)
        {
            position = initialPosition;
            // Nothing to copy
            if (position == 0) { return; }
            if (UnreadData == 0) { position = used = 0; }
            else
            {
                Array.Copy(buffer, position, buffer, 0, UnreadData);
                position = used = UnreadData;
            }
        }
        private void Resize(long value)
        {
            if (maxCapacity.HasValue && value > maxCapacity.Value)
            {
                throw new Exception("Resize requested exceeds maximum capacity previously set.");
            }
            if (value < used)
            {
                throw new Exception("Cant resize bellow Length, may lose data in use, use SetLength instead.");
            }
            var newBuffer = new byte[value];
            Array.Copy(buffer, newBuffer, used);
            buffer = newBuffer;
        }
        public void CopyTo(ImprovedMemoryStream destination)
        {
            var amount = UnreadData;
            if (amount == 0) { return; }
            if ((destination.position + amount) > destination.Capacity)
            { destination.Resize(IncreaseCapacityStrategy(destination.position + amount)); }
            Array.Copy(buffer, position, destination.buffer, destination.position, amount);
            position += amount;
            destination.position += amount;
            if (destination.used > destination.position) { destination.used = destination.position; }
        }
        /// <summary>
        /// Remove excess capacity.
        /// </summary>
        public void TrimExcess() => Resize(used);
        public void Clear() => used = 0;
        /// <summary>
        /// Get a readonly span slice from the internal buffer.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ReadOnlySpan<byte> AsReadOnlySpan(int offset = 0, int? count = null) => new(buffer, offset, (int)(count.HasValue ? Math.Min(count.Value, used) : used));
        private static long IncreaseCapacityStrategy(long requested) => requested / 2 * 3;
    }
}