namespace DeHive.Abstractions;

public class SlicedStream : Stream
{
    private readonly Stream _baseStream;
    private readonly long _startOffset;
    private readonly long _length;
    private long _position;

    public SlicedStream(Stream baseStream, long startOffset, long length)
    {
        if (!baseStream.CanSeek)
            throw new ArgumentException("Base stream must support seeking", nameof(baseStream));
        if (startOffset < 0 || length < 0 || startOffset + length > baseStream.Length)
            throw new ArgumentException("Invalid offset or length");

        _baseStream = baseStream;
        _startOffset = startOffset;
        _length = length;
        _position = 0;

        _baseStream.Seek(_startOffset, SeekOrigin.Begin);
    }

    public override bool CanRead => _baseStream.CanRead;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => _length;

    public override long Position
    {
        get => _position;
        set
        {
            if (value < 0 || value > _length)
                throw new ArgumentOutOfRangeException(nameof(value));
            _position = value;
            _baseStream.Seek(_startOffset + _position, SeekOrigin.Begin);
        }
    }

    public override void Flush() => _baseStream.Flush();

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (_position >= _length)
            return 0;

        var maxCount = (int)Math.Min(count, _length - _position);
        var bytesRead = _baseStream.Read(buffer, offset, maxCount);
        _position += bytesRead;
        return bytesRead;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        var targetPosition = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => _position + offset,
            SeekOrigin.End => _length + offset,
            _ => throw new ArgumentException("Invalid SeekOrigin", nameof(origin))
        };

        if (targetPosition < 0 || targetPosition > _length)
            throw new ArgumentOutOfRangeException(nameof(offset));

        _position = targetPosition;
        return _baseStream.Seek(_startOffset + _position, SeekOrigin.Begin) - _startOffset;
    }

    public override void SetLength(long value) =>
        throw new NotSupportedException("SliceStream does not support SetLength.");

    public override void Write(byte[] buffer, int offset, int count) =>
        throw new NotSupportedException("SliceStream is read-only.");
}