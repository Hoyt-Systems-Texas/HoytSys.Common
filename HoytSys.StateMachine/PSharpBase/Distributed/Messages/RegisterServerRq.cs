/* Generated SBE (Simple Binary Encoding) message codec */

#pragma warning disable 1591 // disable warning on missing comments
using System;
using Org.SbeTool.Sbe.Dll;

namespace A19.StateMachine.PSharpBase.Distributed.Messages
{
    public sealed partial class RegisterServerRq
    {
        public const ushort BlockLength = (ushort)2;
        public const ushort TemplateId = (ushort)1;
        public const ushort SchemaId = (ushort)1;
        public const ushort SchemaVersion = (ushort)0;
        public const string SemanticType = "";

        private readonly RegisterServerRq _parentMessage;
        private DirectBuffer _buffer;
        private int _offset;
        private int _limit;
        private int _actingBlockLength;
        private int _actingVersion;

        public int Offset { get { return _offset; } }

        public RegisterServerRq()
        {
            _parentMessage = this;
        }

        public void WrapForEncode(DirectBuffer buffer, int offset)
        {
            _buffer = buffer;
            _offset = offset;
            _actingBlockLength = BlockLength;
            _actingVersion = SchemaVersion;
            Limit = offset + _actingBlockLength;
        }

        public void WrapForDecode(DirectBuffer buffer, int offset, int actingBlockLength, int actingVersion)
        {
            _buffer = buffer;
            _offset = offset;
            _actingBlockLength = actingBlockLength;
            _actingVersion = actingVersion;
            Limit = offset + _actingBlockLength;
        }

        public int Size
        {
            get
            {
                return _limit - _offset;
            }
        }

        public int Limit
        {
            get
            {
                return _limit;
            }
            set
            {
                _buffer.CheckLimit(value);
                _limit = value;
            }
        }


        public const int ServerId = 1;
        public const int ServerSinceVersion = 0;
        public const int ServerDeprecated = 0;
        public bool ServerInActingVersion()
        {
            return _actingVersion >= ServerSinceVersion;
        }

        public static string ServerMetaAttribute(MetaAttribute metaAttribute)
        {
            switch (metaAttribute)
            {
                case MetaAttribute.Epoch: return "";
                case MetaAttribute.TimeUnit: return "";
                case MetaAttribute.SemanticType: return "";
                case MetaAttribute.Presence: return "required";
            }

            return "";
        }

        public const ushort ServerNullValue = (ushort)65535;
        public const ushort ServerMinValue = (ushort)0;
        public const ushort ServerMaxValue = (ushort)65534;

        public ushort Server
        {
            get
            {
                return _buffer.Uint16GetLittleEndian(_offset + 0);
            }
            set
            {
                _buffer.Uint16PutLittleEndian(_offset + 0, value);
            }
        }


        public const int UriId = 2;
        public const int UriSinceVersion = 0;
        public const int UriDeprecated = 0;
        public bool UriInActingVersion()
        {
            return _actingVersion >= UriSinceVersion;
        }

        public const string UriCharacterEncoding = "UTF-8";


        public static string UriMetaAttribute(MetaAttribute metaAttribute)
        {
            switch (metaAttribute)
            {
                case MetaAttribute.Epoch: return "unix";
                case MetaAttribute.TimeUnit: return "nanosecond";
                case MetaAttribute.SemanticType: return "";
                case MetaAttribute.Presence: return "required";
            }

            return "";
        }

        public const int UriHeaderSize = 4;
        
        public int UriLength()
        {
            _buffer.CheckLimit(_parentMessage.Limit + 4);
            return (int)_buffer.Uint32GetLittleEndian(_parentMessage.Limit);
        }

        public int GetUri(byte[] dst, int dstOffset, int length) =>
            GetUri(new Span<byte>(dst, dstOffset, length));

        public int GetUri(Span<byte> dst)
        {
            const int sizeOfLengthField = 4;
            int limit = _parentMessage.Limit;
            _buffer.CheckLimit(limit + sizeOfLengthField);
            int dataLength = (int)_buffer.Uint32GetLittleEndian(limit);
            int bytesCopied = Math.Min(dst.Length, dataLength);
            _parentMessage.Limit = limit + sizeOfLengthField + dataLength;
            _buffer.GetBytes(limit + sizeOfLengthField, dst.Slice(0, bytesCopied));

            return bytesCopied;
        }
        
        // Allocates and returns a new byte array
        public byte[] GetUriBytes()
        {
            const int sizeOfLengthField = 4;
            int limit = _parentMessage.Limit;
            _buffer.CheckLimit(limit + sizeOfLengthField);
            int dataLength = (int)_buffer.Uint32GetLittleEndian(limit);
            byte[] data = new byte[dataLength];
            _parentMessage.Limit = limit + sizeOfLengthField + dataLength;
            _buffer.GetBytes(limit + sizeOfLengthField, data);

            return data;
        }

        public int SetUri(byte[] src, int srcOffset, int length) =>
            SetUri(new ReadOnlySpan<byte>(src, srcOffset, length));

        public int SetUri(ReadOnlySpan<byte> src)
        {
            const int sizeOfLengthField = 4;
            int limit = _parentMessage.Limit;
            _parentMessage.Limit = limit + sizeOfLengthField + src.Length;
            _buffer.Uint32PutLittleEndian(limit, (uint)src.Length);
            _buffer.SetBytes(limit + sizeOfLengthField, src);

            return src.Length;
        }
    }
}
