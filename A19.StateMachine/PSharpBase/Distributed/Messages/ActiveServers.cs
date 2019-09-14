/* Generated SBE (Simple Binary Encoding) message codec */

#pragma warning disable 1591 // disable warning on missing comments
using System;
using Org.SbeTool.Sbe.Dll;

namespace A19.StateMachine.PSharpBase.Distributed.Messages
{
    public sealed partial class ActiveServers
    {
        public const ushort BlockLength = (ushort)0;
        public const ushort TemplateId = (ushort)2;
        public const ushort SchemaId = (ushort)1;
        public const ushort SchemaVersion = (ushort)0;
        public const string SemanticType = "";

        private readonly ActiveServers _parentMessage;
        private DirectBuffer _buffer;
        private int _offset;
        private int _limit;
        private int _actingBlockLength;
        private int _actingVersion;

        public int Offset { get { return _offset; } }

        public ActiveServers()
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


        private readonly ServersGroup _servers = new ServersGroup();

        public const long ServersId = 1;
        public const int ServersSinceVersion = 0;
        public const int ServersDeprecated = 0;
        public bool ServersInActingVersion()
        {
            return _actingVersion >= ServersSinceVersion;
        }

        public ServersGroup Servers
        {
            get
            {
                _servers.WrapForDecode(_parentMessage, _buffer, _actingVersion);
                return _servers;
            }
        }

        public ServersGroup ServersCount(int count)
        {
            _servers.WrapForEncode(_parentMessage, _buffer, count);
            return _servers;
        }

        public sealed partial class ServersGroup
        {
            private readonly GroupSizeEncoding _dimensions = new GroupSizeEncoding();
            private ActiveServers _parentMessage;
            private DirectBuffer _buffer;
            private int _blockLength;
            private int _actingVersion;
            private int _count;
            private int _index;
            private int _offset;

            public void WrapForDecode(ActiveServers parentMessage, DirectBuffer buffer, int actingVersion)
            {
                _parentMessage = parentMessage;
                _buffer = buffer;
                _dimensions.Wrap(buffer, parentMessage.Limit, actingVersion);
                _blockLength = _dimensions.BlockLength;
                _count = (int) _dimensions.NumInGroup;
                _actingVersion = actingVersion;
                _index = -1;
                _parentMessage.Limit = parentMessage.Limit + SbeHeaderSize;
            }

            public void WrapForEncode(ActiveServers parentMessage, DirectBuffer buffer, int count)
            {
                if ((uint) count > 65534)
                {
                    ThrowHelper.ThrowCountOutOfRangeException(count);
                }

                _parentMessage = parentMessage;
                _buffer = buffer;
                _dimensions.Wrap(buffer, parentMessage.Limit, _actingVersion);
                _dimensions.BlockLength = (ushort)6;
                _dimensions.NumInGroup = (ushort)count;
                _index = -1;
                _count = count;
                _blockLength = 6;
                _actingVersion = SchemaVersion;
                parentMessage.Limit = parentMessage.Limit + SbeHeaderSize;
            }

            public const int SbeBlockLength = 6;
            public const int SbeHeaderSize = 4;
            public int ActingBlockLength { get { return _blockLength; } }

            public int Count { get { return _count; } }

            public bool HasNext { get { return (_index + 1) < _count; } }

            public ServersGroup Next()
            {
                if (_index + 1 >= _count)
                {
                    ThrowHelper.ThrowInvalidOperationException();
                }

                _offset = _parentMessage.Limit;
                _parentMessage.Limit = _offset + _blockLength;
                ++_index;

                return this;
            }

            public System.Collections.IEnumerator GetEnumerator()
            {
                while (this.HasNext)
                {
                    yield return this.Next();
                }
            }

            public const int HostId = 2;
            public const int HostSinceVersion = 0;
            public const int HostDeprecated = 0;
            public bool HostInActingVersion()
            {
                return _actingVersion >= HostSinceVersion;
            }

            public static string HostMetaAttribute(MetaAttribute metaAttribute)
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

            private readonly Host _host = new Host();

            public Host Host
            {
                get
                {
                    _host.Wrap(_buffer, _offset + 0, _actingVersion);
                    return _host;
                }
            }
        }
    }
}
