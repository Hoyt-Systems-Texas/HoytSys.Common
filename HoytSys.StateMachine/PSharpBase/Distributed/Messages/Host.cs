/* Generated SBE (Simple Binary Encoding) message codec */

#pragma warning disable 1591 // disable warning on missing comments
using System;
using Org.SbeTool.Sbe.Dll;

namespace A19.StateMachine.PSharpBase.Distributed.Messages
{
    public sealed partial class Host
    {
        private DirectBuffer _buffer;
        private int _offset;
        private int _actingVersion;

        public void Wrap(DirectBuffer buffer, int offset, int actingVersion)
        {
            _offset = offset;
            _actingVersion = actingVersion;
            _buffer = buffer;
        }

        public const int Size = 6;

        public const ushort ServerIdNullValue = (ushort)65535;
        public const ushort ServerIdMinValue = (ushort)0;
        public const ushort ServerIdMaxValue = (ushort)65534;

        public ushort ServerId
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


        private readonly VarStringEncoding _hostName = new VarStringEncoding();

        public VarStringEncoding HostName
        {
            get
            {
                _hostName.Wrap(_buffer, _offset + 2, _actingVersion);
                return _hostName;
            }
        }
    }
}
