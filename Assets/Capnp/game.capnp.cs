using Capnp;
using Capnp.Rpc;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CapnpGen
{
    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xda41ead69a89606aUL)]
    public class ClientInput : ICapnpSerializable
    {
        public const UInt64 typeId = 0xda41ead69a89606aUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            PlayerId = reader.PlayerId;
            Sequence = reader.Sequence;
            DtMs = reader.DtMs;
            W = reader.W;
            A = reader.A;
            S = reader.S;
            D = reader.D;
            Run = reader.Run;
            JumpPressed = reader.JumpPressed;
            AimYaw = reader.AimYaw;
            ShootPressed = reader.ShootPressed;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.PlayerId = PlayerId;
            writer.Sequence = Sequence;
            writer.DtMs = DtMs;
            writer.W = W;
            writer.A = A;
            writer.S = S;
            writer.D = D;
            writer.Run = Run;
            writer.JumpPressed = JumpPressed;
            writer.AimYaw = AimYaw;
            writer.ShootPressed = ShootPressed;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public ulong PlayerId
        {
            get;
            set;
        }

        public uint Sequence
        {
            get;
            set;
        }

        public ushort DtMs
        {
            get;
            set;
        }

        public bool W
        {
            get;
            set;
        }

        public bool A
        {
            get;
            set;
        }

        public bool S
        {
            get;
            set;
        }

        public bool D
        {
            get;
            set;
        }

        public bool Run
        {
            get;
            set;
        }

        public bool JumpPressed
        {
            get;
            set;
        }

        public float AimYaw
        {
            get;
            set;
        }

        public bool ShootPressed
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public ulong PlayerId => ctx.ReadDataULong(0UL, 0UL);
            public uint Sequence => ctx.ReadDataUInt(64UL, 0U);
            public ushort DtMs => ctx.ReadDataUShort(96UL, (ushort)0);
            public bool W => ctx.ReadDataBool(112UL, false);
            public bool A => ctx.ReadDataBool(113UL, false);
            public bool S => ctx.ReadDataBool(114UL, false);
            public bool D => ctx.ReadDataBool(115UL, false);
            public bool Run => ctx.ReadDataBool(116UL, false);
            public bool JumpPressed => ctx.ReadDataBool(117UL, false);
            public float AimYaw => ctx.ReadDataFloat(128UL, 0F);
            public bool ShootPressed => ctx.ReadDataBool(118UL, false);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(3, 0);
            }

            public ulong PlayerId
            {
                get => this.ReadDataULong(0UL, 0UL);
                set => this.WriteData(0UL, value, 0UL);
            }

            public uint Sequence
            {
                get => this.ReadDataUInt(64UL, 0U);
                set => this.WriteData(64UL, value, 0U);
            }

            public ushort DtMs
            {
                get => this.ReadDataUShort(96UL, (ushort)0);
                set => this.WriteData(96UL, value, (ushort)0);
            }

            public bool W
            {
                get => this.ReadDataBool(112UL, false);
                set => this.WriteData(112UL, value, false);
            }

            public bool A
            {
                get => this.ReadDataBool(113UL, false);
                set => this.WriteData(113UL, value, false);
            }

            public bool S
            {
                get => this.ReadDataBool(114UL, false);
                set => this.WriteData(114UL, value, false);
            }

            public bool D
            {
                get => this.ReadDataBool(115UL, false);
                set => this.WriteData(115UL, value, false);
            }

            public bool Run
            {
                get => this.ReadDataBool(116UL, false);
                set => this.WriteData(116UL, value, false);
            }

            public bool JumpPressed
            {
                get => this.ReadDataBool(117UL, false);
                set => this.WriteData(117UL, value, false);
            }

            public float AimYaw
            {
                get => this.ReadDataFloat(128UL, 0F);
                set => this.WriteData(128UL, value, 0F);
            }

            public bool ShootPressed
            {
                get => this.ReadDataBool(118UL, false);
                set => this.WriteData(118UL, value, false);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xd7707db593543632UL)]
    public class PlayerState : ICapnpSerializable
    {
        public const UInt64 typeId = 0xd7707db593543632UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            PlayerId = reader.PlayerId;
            X = reader.X;
            Y = reader.Y;
            Z = reader.Z;
            Vx = reader.Vx;
            Vy = reader.Vy;
            Vz = reader.Vz;
            LastProcessedSequence = reader.LastProcessedSequence;
            Yaw = reader.Yaw;
            Health = reader.Health;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.PlayerId = PlayerId;
            writer.X = X;
            writer.Y = Y;
            writer.Z = Z;
            writer.Vx = Vx;
            writer.Vy = Vy;
            writer.Vz = Vz;
            writer.LastProcessedSequence = LastProcessedSequence;
            writer.Yaw = Yaw;
            writer.Health = Health;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public ulong PlayerId
        {
            get;
            set;
        }

        public float X
        {
            get;
            set;
        }

        public float Y
        {
            get;
            set;
        }

        public float Z
        {
            get;
            set;
        }

        public float Vx
        {
            get;
            set;
        }

        public float Vy
        {
            get;
            set;
        }

        public float Vz
        {
            get;
            set;
        }

        public uint LastProcessedSequence
        {
            get;
            set;
        }

        public float Yaw
        {
            get;
            set;
        }

        public ushort Health
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public ulong PlayerId => ctx.ReadDataULong(0UL, 0UL);
            public float X => ctx.ReadDataFloat(64UL, 0F);
            public float Y => ctx.ReadDataFloat(96UL, 0F);
            public float Z => ctx.ReadDataFloat(128UL, 0F);
            public float Vx => ctx.ReadDataFloat(160UL, 0F);
            public float Vy => ctx.ReadDataFloat(192UL, 0F);
            public float Vz => ctx.ReadDataFloat(224UL, 0F);
            public uint LastProcessedSequence => ctx.ReadDataUInt(256UL, 0U);
            public float Yaw => ctx.ReadDataFloat(288UL, 0F);
            public ushort Health => ctx.ReadDataUShort(320UL, (ushort)0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(6, 0);
            }

            public ulong PlayerId
            {
                get => this.ReadDataULong(0UL, 0UL);
                set => this.WriteData(0UL, value, 0UL);
            }

            public float X
            {
                get => this.ReadDataFloat(64UL, 0F);
                set => this.WriteData(64UL, value, 0F);
            }

            public float Y
            {
                get => this.ReadDataFloat(96UL, 0F);
                set => this.WriteData(96UL, value, 0F);
            }

            public float Z
            {
                get => this.ReadDataFloat(128UL, 0F);
                set => this.WriteData(128UL, value, 0F);
            }

            public float Vx
            {
                get => this.ReadDataFloat(160UL, 0F);
                set => this.WriteData(160UL, value, 0F);
            }

            public float Vy
            {
                get => this.ReadDataFloat(192UL, 0F);
                set => this.WriteData(192UL, value, 0F);
            }

            public float Vz
            {
                get => this.ReadDataFloat(224UL, 0F);
                set => this.WriteData(224UL, value, 0F);
            }

            public uint LastProcessedSequence
            {
                get => this.ReadDataUInt(256UL, 0U);
                set => this.WriteData(256UL, value, 0U);
            }

            public float Yaw
            {
                get => this.ReadDataFloat(288UL, 0F);
                set => this.WriteData(288UL, value, 0F);
            }

            public ushort Health
            {
                get => this.ReadDataUShort(320UL, (ushort)0);
                set => this.WriteData(320UL, value, (ushort)0);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xd03c08172bce8563UL)]
    public class ShotFired : ICapnpSerializable
    {
        public const UInt64 typeId = 0xd03c08172bce8563UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            ShooterId = reader.ShooterId;
            X = reader.X;
            Y = reader.Y;
            Z = reader.Z;
            Yaw = reader.Yaw;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.ShooterId = ShooterId;
            writer.X = X;
            writer.Y = Y;
            writer.Z = Z;
            writer.Yaw = Yaw;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public ulong ShooterId
        {
            get;
            set;
        }

        public float X
        {
            get;
            set;
        }

        public float Y
        {
            get;
            set;
        }

        public float Z
        {
            get;
            set;
        }

        public float Yaw
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public ulong ShooterId => ctx.ReadDataULong(0UL, 0UL);
            public float X => ctx.ReadDataFloat(64UL, 0F);
            public float Y => ctx.ReadDataFloat(96UL, 0F);
            public float Z => ctx.ReadDataFloat(128UL, 0F);
            public float Yaw => ctx.ReadDataFloat(160UL, 0F);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(3, 0);
            }

            public ulong ShooterId
            {
                get => this.ReadDataULong(0UL, 0UL);
                set => this.WriteData(0UL, value, 0UL);
            }

            public float X
            {
                get => this.ReadDataFloat(64UL, 0F);
                set => this.WriteData(64UL, value, 0F);
            }

            public float Y
            {
                get => this.ReadDataFloat(96UL, 0F);
                set => this.WriteData(96UL, value, 0F);
            }

            public float Z
            {
                get => this.ReadDataFloat(128UL, 0F);
                set => this.WriteData(128UL, value, 0F);
            }

            public float Yaw
            {
                get => this.ReadDataFloat(160UL, 0F);
                set => this.WriteData(160UL, value, 0F);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xb2f2e8ad2c2811ecUL)]
    public class ServerEvent : ICapnpSerializable
    {
        public const UInt64 typeId = 0xb2f2e8ad2c2811ecUL;
        public enum WHICH : ushort
        {
            ShotFired = 0,
            Noop = 1,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.ShotFired:
                    ShotFired = CapnpSerializable.Create<CapnpGen.ShotFired>(reader.ShotFired);
                    break;
                case WHICH.Noop:
                    which = reader.which;
                    break;
            }

            applyDefaults();
        }

        private WHICH _which = WHICH.undefined;
        private object _content;
        public WHICH which
        {
            get => _which;
            set
            {
                if (value == _which)
                    return;
                _which = value;
                switch (value)
                {
                    case WHICH.ShotFired:
                        _content = null;
                        break;
                    case WHICH.Noop:
                        break;
                }
            }
        }

        public void serialize(WRITER writer)
        {
            writer.which = which;
            switch (which)
            {
                case WHICH.ShotFired:
                    ShotFired?.serialize(writer.ShotFired);
                    break;
                case WHICH.Noop:
                    break;
            }
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public CapnpGen.ShotFired ShotFired
        {
            get => _which == WHICH.ShotFired ? (CapnpGen.ShotFired)_content : null;
            set
            {
                _which = WHICH.ShotFired;
                _content = value;
            }
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public WHICH which => (WHICH)ctx.ReadDataUShort(0U, (ushort)0);
            public CapnpGen.ShotFired.READER ShotFired => which == WHICH.ShotFired ? ctx.ReadStruct(0, CapnpGen.ShotFired.READER.create) : default;
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 1);
            }

            public WHICH which
            {
                get => (WHICH)this.ReadDataUShort(0U, (ushort)0);
                set => this.WriteData(0U, (ushort)value, (ushort)0);
            }

            public CapnpGen.ShotFired.WRITER ShotFired
            {
                get => which == WHICH.ShotFired ? BuildPointer<CapnpGen.ShotFired.WRITER>(0) : default;
                set => Link(0, value);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xf23306579fb46453UL)]
    public class Snapshot : ICapnpSerializable
    {
        public const UInt64 typeId = 0xf23306579fb46453UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            ServerTick = reader.ServerTick;
            Players = reader.Players?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.PlayerState>(_));
            Events = reader.Events?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.ServerEvent>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.ServerTick = ServerTick;
            writer.Players.Init(Players, (_s1, _v1) => _v1?.serialize(_s1));
            writer.Events.Init(Events, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public ulong ServerTick
        {
            get;
            set;
        }

        public IReadOnlyList<CapnpGen.PlayerState> Players
        {
            get;
            set;
        }

        public IReadOnlyList<CapnpGen.ServerEvent> Events
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public ulong ServerTick => ctx.ReadDataULong(0UL, 0UL);
            public IReadOnlyList<CapnpGen.PlayerState.READER> Players => ctx.ReadList(0).Cast(CapnpGen.PlayerState.READER.create);
            public IReadOnlyList<CapnpGen.ServerEvent.READER> Events => ctx.ReadList(1).Cast(CapnpGen.ServerEvent.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 2);
            }

            public ulong ServerTick
            {
                get => this.ReadDataULong(0UL, 0UL);
                set => this.WriteData(0UL, value, 0UL);
            }

            public ListOfStructsSerializer<CapnpGen.PlayerState.WRITER> Players
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpGen.PlayerState.WRITER>>(0);
                set => Link(0, value);
            }

            public ListOfStructsSerializer<CapnpGen.ServerEvent.WRITER> Events
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpGen.ServerEvent.WRITER>>(1);
                set => Link(1, value);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xe268618f087c6fcaUL)]
    public class ClientMsg : ICapnpSerializable
    {
        public const UInt64 typeId = 0xe268618f087c6fcaUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Input = CapnpSerializable.Create<CapnpGen.ClientInput>(reader.Input);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            Input?.serialize(writer.Input);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public CapnpGen.ClientInput Input
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public CapnpGen.ClientInput.READER Input => ctx.ReadStruct(0, CapnpGen.ClientInput.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 1);
            }

            public CapnpGen.ClientInput.WRITER Input
            {
                get => BuildPointer<CapnpGen.ClientInput.WRITER>(0);
                set => Link(0, value);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xe065b22feca9821dUL)]
    public class AssignId : ICapnpSerializable
    {
        public const UInt64 typeId = 0xe065b22feca9821dUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            PlayerId = reader.PlayerId;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.PlayerId = PlayerId;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public ulong PlayerId
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public ulong PlayerId => ctx.ReadDataULong(0UL, 0UL);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public ulong PlayerId
            {
                get => this.ReadDataULong(0UL, 0UL);
                set => this.WriteData(0UL, value, 0UL);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xc86a7cbc9d1ca950UL)]
    public class ServerMsg : ICapnpSerializable
    {
        public const UInt64 typeId = 0xc86a7cbc9d1ca950UL;
        public enum WHICH : ushort
        {
            AssignId = 0,
            Snapshot = 1,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.AssignId:
                    AssignId = CapnpSerializable.Create<CapnpGen.AssignId>(reader.AssignId);
                    break;
                case WHICH.Snapshot:
                    Snapshot = CapnpSerializable.Create<CapnpGen.Snapshot>(reader.Snapshot);
                    break;
            }

            applyDefaults();
        }

        private WHICH _which = WHICH.undefined;
        private object _content;
        public WHICH which
        {
            get => _which;
            set
            {
                if (value == _which)
                    return;
                _which = value;
                switch (value)
                {
                    case WHICH.AssignId:
                        _content = null;
                        break;
                    case WHICH.Snapshot:
                        _content = null;
                        break;
                }
            }
        }

        public void serialize(WRITER writer)
        {
            writer.which = which;
            switch (which)
            {
                case WHICH.AssignId:
                    AssignId?.serialize(writer.AssignId);
                    break;
                case WHICH.Snapshot:
                    Snapshot?.serialize(writer.Snapshot);
                    break;
            }
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public CapnpGen.AssignId AssignId
        {
            get => _which == WHICH.AssignId ? (CapnpGen.AssignId)_content : null;
            set
            {
                _which = WHICH.AssignId;
                _content = value;
            }
        }

        public CapnpGen.Snapshot Snapshot
        {
            get => _which == WHICH.Snapshot ? (CapnpGen.Snapshot)_content : null;
            set
            {
                _which = WHICH.Snapshot;
                _content = value;
            }
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public WHICH which => (WHICH)ctx.ReadDataUShort(0U, (ushort)0);
            public CapnpGen.AssignId.READER AssignId => which == WHICH.AssignId ? ctx.ReadStruct(0, CapnpGen.AssignId.READER.create) : default;
            public CapnpGen.Snapshot.READER Snapshot => which == WHICH.Snapshot ? ctx.ReadStruct(0, CapnpGen.Snapshot.READER.create) : default;
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 1);
            }

            public WHICH which
            {
                get => (WHICH)this.ReadDataUShort(0U, (ushort)0);
                set => this.WriteData(0U, (ushort)value, (ushort)0);
            }

            public CapnpGen.AssignId.WRITER AssignId
            {
                get => which == WHICH.AssignId ? BuildPointer<CapnpGen.AssignId.WRITER>(0) : default;
                set => Link(0, value);
            }

            public CapnpGen.Snapshot.WRITER Snapshot
            {
                get => which == WHICH.Snapshot ? BuildPointer<CapnpGen.Snapshot.WRITER>(0) : default;
                set => Link(0, value);
            }
        }
    }
}