using Capnp;
using Capnp.Rpc;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CapnpGen
{
    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xb529b57e7db04faaUL)]
    public enum WeaponType : ushort
    {
        none,
        pistol,
        rifle,
        smg,
        shotgun,
        sniper
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x8ed442bf76ca3473UL)]
    public enum WeaponSlot : ushort
    {
        primary,
        secondary
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xca11b0569a00f028UL)]
    public class WeaponSlotState : ICapnpSerializable
    {
        public const UInt64 typeId = 0xca11b0569a00f028UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Weapon = reader.Weapon;
            AmmoInMag = reader.AmmoInMag;
            ReserveAmmo = reader.ReserveAmmo;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Weapon = Weapon;
            writer.AmmoInMag = AmmoInMag;
            writer.ReserveAmmo = ReserveAmmo;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public CapnpGen.WeaponType Weapon
        {
            get;
            set;
        }

        public ushort AmmoInMag
        {
            get;
            set;
        }

        public ushort ReserveAmmo
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
            public CapnpGen.WeaponType Weapon => (CapnpGen.WeaponType)ctx.ReadDataUShort(0UL, (ushort)0);
            public ushort AmmoInMag => ctx.ReadDataUShort(16UL, (ushort)0);
            public ushort ReserveAmmo => ctx.ReadDataUShort(32UL, (ushort)0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public CapnpGen.WeaponType Weapon
            {
                get => (CapnpGen.WeaponType)this.ReadDataUShort(0UL, (ushort)0);
                set => this.WriteData(0UL, (ushort)value, (ushort)0);
            }

            public ushort AmmoInMag
            {
                get => this.ReadDataUShort(16UL, (ushort)0);
                set => this.WriteData(16UL, value, (ushort)0);
            }

            public ushort ReserveAmmo
            {
                get => this.ReadDataUShort(32UL, (ushort)0);
                set => this.WriteData(32UL, value, (ushort)0);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xd89708c2aa7cdf4bUL)]
    public class SelectLoadout : ICapnpSerializable
    {
        public const UInt64 typeId = 0xd89708c2aa7cdf4bUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            PlayerId = reader.PlayerId;
            SkinId = reader.SkinId;
            SecondaryWeapon = reader.SecondaryWeapon;
            PlayerName = reader.PlayerName;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.PlayerId = PlayerId;
            writer.SkinId = SkinId;
            writer.SecondaryWeapon = SecondaryWeapon;
            writer.PlayerName = PlayerName;
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

        public ushort SkinId
        {
            get;
            set;
        }

        public CapnpGen.WeaponType SecondaryWeapon
        {
            get;
            set;
        }

        public string PlayerName
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
            public ushort SkinId => ctx.ReadDataUShort(64UL, (ushort)0);
            public CapnpGen.WeaponType SecondaryWeapon => (CapnpGen.WeaponType)ctx.ReadDataUShort(80UL, (ushort)0);
            public string PlayerName => ctx.ReadText(0, null);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(2, 1);
            }

            public ulong PlayerId
            {
                get => this.ReadDataULong(0UL, 0UL);
                set => this.WriteData(0UL, value, 0UL);
            }

            public ushort SkinId
            {
                get => this.ReadDataUShort(64UL, (ushort)0);
                set => this.WriteData(64UL, value, (ushort)0);
            }

            public CapnpGen.WeaponType SecondaryWeapon
            {
                get => (CapnpGen.WeaponType)this.ReadDataUShort(80UL, (ushort)0);
                set => this.WriteData(80UL, (ushort)value, (ushort)0);
            }

            public string PlayerName
            {
                get => this.ReadText(0, null);
                set => this.WriteText(0, value, null);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xda41ead69a89606aUL)]
    public class ClientInput : ICapnpSerializable
    {
        public const UInt64 typeId = 0xda41ead69a89606aUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Sequence = reader.Sequence;
            DtMs = reader.DtMs;
            W = reader.W;
            A = reader.A;
            S = reader.S;
            D = reader.D;
            Run = reader.Run;
            AimYaw = reader.AimYaw;
            JumpPressed = reader.JumpPressed;
            ShootPressed = reader.ShootPressed;
            ReloadPressed = reader.ReloadPressed;
            AimPitch = reader.AimPitch;
            FaceYaw = reader.FaceYaw;
            InteractPressed = reader.InteractPressed;
            SwitchWeaponPressed = reader.SwitchWeaponPressed;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Sequence = Sequence;
            writer.DtMs = DtMs;
            writer.W = W;
            writer.A = A;
            writer.S = S;
            writer.D = D;
            writer.Run = Run;
            writer.AimYaw = AimYaw;
            writer.JumpPressed = JumpPressed;
            writer.ShootPressed = ShootPressed;
            writer.ReloadPressed = ReloadPressed;
            writer.AimPitch = AimPitch;
            writer.FaceYaw = FaceYaw;
            writer.InteractPressed = InteractPressed;
            writer.SwitchWeaponPressed = SwitchWeaponPressed;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
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

        public float AimYaw
        {
            get;
            set;
        }

        public bool JumpPressed
        {
            get;
            set;
        }

        public bool ShootPressed
        {
            get;
            set;
        }

        public bool ReloadPressed
        {
            get;
            set;
        }

        public float AimPitch
        {
            get;
            set;
        }

        public float FaceYaw
        {
            get;
            set;
        }

        public bool InteractPressed
        {
            get;
            set;
        }

        public bool SwitchWeaponPressed
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
            public uint Sequence => ctx.ReadDataUInt(0UL, 0U);
            public ushort DtMs => ctx.ReadDataUShort(32UL, (ushort)0);
            public bool W => ctx.ReadDataBool(48UL, false);
            public bool A => ctx.ReadDataBool(49UL, false);
            public bool S => ctx.ReadDataBool(50UL, false);
            public bool D => ctx.ReadDataBool(51UL, false);
            public bool Run => ctx.ReadDataBool(52UL, false);
            public float AimYaw => ctx.ReadDataFloat(64UL, 0F);
            public bool JumpPressed => ctx.ReadDataBool(53UL, false);
            public bool ShootPressed => ctx.ReadDataBool(54UL, false);
            public bool ReloadPressed => ctx.ReadDataBool(55UL, false);
            public float AimPitch => ctx.ReadDataFloat(96UL, 0F);
            public float FaceYaw => ctx.ReadDataFloat(128UL, 0F);
            public bool InteractPressed => ctx.ReadDataBool(56UL, false);
            public bool SwitchWeaponPressed => ctx.ReadDataBool(57UL, false);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(3, 0);
            }

            public uint Sequence
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }

            public ushort DtMs
            {
                get => this.ReadDataUShort(32UL, (ushort)0);
                set => this.WriteData(32UL, value, (ushort)0);
            }

            public bool W
            {
                get => this.ReadDataBool(48UL, false);
                set => this.WriteData(48UL, value, false);
            }

            public bool A
            {
                get => this.ReadDataBool(49UL, false);
                set => this.WriteData(49UL, value, false);
            }

            public bool S
            {
                get => this.ReadDataBool(50UL, false);
                set => this.WriteData(50UL, value, false);
            }

            public bool D
            {
                get => this.ReadDataBool(51UL, false);
                set => this.WriteData(51UL, value, false);
            }

            public bool Run
            {
                get => this.ReadDataBool(52UL, false);
                set => this.WriteData(52UL, value, false);
            }

            public float AimYaw
            {
                get => this.ReadDataFloat(64UL, 0F);
                set => this.WriteData(64UL, value, 0F);
            }

            public bool JumpPressed
            {
                get => this.ReadDataBool(53UL, false);
                set => this.WriteData(53UL, value, false);
            }

            public bool ShootPressed
            {
                get => this.ReadDataBool(54UL, false);
                set => this.WriteData(54UL, value, false);
            }

            public bool ReloadPressed
            {
                get => this.ReadDataBool(55UL, false);
                set => this.WriteData(55UL, value, false);
            }

            public float AimPitch
            {
                get => this.ReadDataFloat(96UL, 0F);
                set => this.WriteData(96UL, value, 0F);
            }

            public float FaceYaw
            {
                get => this.ReadDataFloat(128UL, 0F);
                set => this.WriteData(128UL, value, 0F);
            }

            public bool InteractPressed
            {
                get => this.ReadDataBool(56UL, false);
                set => this.WriteData(56UL, value, false);
            }

            public bool SwitchWeaponPressed
            {
                get => this.ReadDataBool(57UL, false);
                set => this.WriteData(57UL, value, false);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xe268618f087c6fcaUL)]
    public class ClientMsg : ICapnpSerializable
    {
        public const UInt64 typeId = 0xe268618f087c6fcaUL;
        public enum WHICH : ushort
        {
            SelectLoadout = 0,
            Input = 1,
            undefined = 65535
        }

        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            switch (reader.which)
            {
                case WHICH.SelectLoadout:
                    SelectLoadout = CapnpSerializable.Create<CapnpGen.SelectLoadout>(reader.SelectLoadout);
                    break;
                case WHICH.Input:
                    Input = CapnpSerializable.Create<CapnpGen.ClientInput>(reader.Input);
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
                    case WHICH.SelectLoadout:
                        _content = null;
                        break;
                    case WHICH.Input:
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
                case WHICH.SelectLoadout:
                    SelectLoadout?.serialize(writer.SelectLoadout);
                    break;
                case WHICH.Input:
                    Input?.serialize(writer.Input);
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

        public CapnpGen.SelectLoadout SelectLoadout
        {
            get => _which == WHICH.SelectLoadout ? (CapnpGen.SelectLoadout)_content : null;
            set
            {
                _which = WHICH.SelectLoadout;
                _content = value;
            }
        }

        public CapnpGen.ClientInput Input
        {
            get => _which == WHICH.Input ? (CapnpGen.ClientInput)_content : null;
            set
            {
                _which = WHICH.Input;
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
            public CapnpGen.SelectLoadout.READER SelectLoadout => which == WHICH.SelectLoadout ? ctx.ReadStruct(0, CapnpGen.SelectLoadout.READER.create) : default;
            public CapnpGen.ClientInput.READER Input => which == WHICH.Input ? ctx.ReadStruct(0, CapnpGen.ClientInput.READER.create) : default;
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

            public CapnpGen.SelectLoadout.WRITER SelectLoadout
            {
                get => which == WHICH.SelectLoadout ? BuildPointer<CapnpGen.SelectLoadout.WRITER>(0) : default;
                set => Link(0, value);
            }

            public CapnpGen.ClientInput.WRITER Input
            {
                get => which == WHICH.Input ? BuildPointer<CapnpGen.ClientInput.WRITER>(0) : default;
                set => Link(0, value);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x9f99eaff7ed96bf2UL)]
    public class ScoreUpdate : ICapnpSerializable
    {
        public const UInt64 typeId = 0x9f99eaff7ed96bf2UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Score = reader.Score;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Score = Score;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public uint Score
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
            public uint Score => ctx.ReadDataUInt(0UL, 0U);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public uint Score
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xb0abbed64b0b0e9bUL)]
    public class PlayerScore : ICapnpSerializable
    {
        public const UInt64 typeId = 0xb0abbed64b0b0e9bUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            PlayerId = reader.PlayerId;
            Score = reader.Score;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.PlayerId = PlayerId;
            writer.Score = Score;
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

        public uint Score
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
            public uint Score => ctx.ReadDataUInt(64UL, 0U);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(2, 0);
            }

            public ulong PlayerId
            {
                get => this.ReadDataULong(0UL, 0UL);
                set => this.WriteData(0UL, value, 0UL);
            }

            public uint Score
            {
                get => this.ReadDataUInt(64UL, 0U);
                set => this.WriteData(64UL, value, 0U);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xf95e275bc929f542UL)]
    public class MatchEnded : ICapnpSerializable
    {
        public const UInt64 typeId = 0xf95e275bc929f542UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Scores = reader.Scores?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.PlayerScore>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Scores.Init(Scores, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public IReadOnlyList<CapnpGen.PlayerScore> Scores
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
            public IReadOnlyList<CapnpGen.PlayerScore.READER> Scores => ctx.ReadList(0).Cast(CapnpGen.PlayerScore.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 1);
            }

            public ListOfStructsSerializer<CapnpGen.PlayerScore.WRITER> Scores
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpGen.PlayerScore.WRITER>>(0);
                set => Link(0, value);
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
            Primary = CapnpSerializable.Create<CapnpGen.WeaponSlotState>(reader.Primary);
            Secondary = CapnpSerializable.Create<CapnpGen.WeaponSlotState>(reader.Secondary);
            EquippedSlot = reader.EquippedSlot;
            IsReloading = reader.IsReloading;
            SkinId = reader.SkinId;
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
            Primary?.serialize(writer.Primary);
            Secondary?.serialize(writer.Secondary);
            writer.EquippedSlot = EquippedSlot;
            writer.IsReloading = IsReloading;
            writer.SkinId = SkinId;
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

        public CapnpGen.WeaponSlotState Primary
        {
            get;
            set;
        }

        public CapnpGen.WeaponSlotState Secondary
        {
            get;
            set;
        }

        public CapnpGen.WeaponSlot EquippedSlot
        {
            get;
            set;
        }

        public bool IsReloading
        {
            get;
            set;
        }

        public ushort SkinId
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
            public CapnpGen.WeaponSlotState.READER Primary => ctx.ReadStruct(0, CapnpGen.WeaponSlotState.READER.create);
            public CapnpGen.WeaponSlotState.READER Secondary => ctx.ReadStruct(1, CapnpGen.WeaponSlotState.READER.create);
            public CapnpGen.WeaponSlot EquippedSlot => (CapnpGen.WeaponSlot)ctx.ReadDataUShort(336UL, (ushort)0);
            public bool IsReloading => ctx.ReadDataBool(352UL, false);
            public ushort SkinId => ctx.ReadDataUShort(368UL, (ushort)0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(6, 2);
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

            public CapnpGen.WeaponSlotState.WRITER Primary
            {
                get => BuildPointer<CapnpGen.WeaponSlotState.WRITER>(0);
                set => Link(0, value);
            }

            public CapnpGen.WeaponSlotState.WRITER Secondary
            {
                get => BuildPointer<CapnpGen.WeaponSlotState.WRITER>(1);
                set => Link(1, value);
            }

            public CapnpGen.WeaponSlot EquippedSlot
            {
                get => (CapnpGen.WeaponSlot)this.ReadDataUShort(336UL, (ushort)0);
                set => this.WriteData(336UL, (ushort)value, (ushort)0);
            }

            public bool IsReloading
            {
                get => this.ReadDataBool(352UL, false);
                set => this.WriteData(352UL, value, false);
            }

            public ushort SkinId
            {
                get => this.ReadDataUShort(368UL, (ushort)0);
                set => this.WriteData(368UL, value, (ushort)0);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xc12f8f557ce7af1aUL)]
    public class WeaponSpawnState : ICapnpSerializable
    {
        public const UInt64 typeId = 0xc12f8f557ce7af1aUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            SpawnId = reader.SpawnId;
            Weapon = reader.Weapon;
            Available = reader.Available;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.SpawnId = SpawnId;
            writer.Weapon = Weapon;
            writer.Available = Available;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public ushort SpawnId
        {
            get;
            set;
        }

        public CapnpGen.WeaponType Weapon
        {
            get;
            set;
        }

        public bool Available
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
            public ushort SpawnId => ctx.ReadDataUShort(0UL, (ushort)0);
            public CapnpGen.WeaponType Weapon => (CapnpGen.WeaponType)ctx.ReadDataUShort(16UL, (ushort)0);
            public bool Available => ctx.ReadDataBool(32UL, false);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public ushort SpawnId
            {
                get => this.ReadDataUShort(0UL, (ushort)0);
                set => this.WriteData(0UL, value, (ushort)0);
            }

            public CapnpGen.WeaponType Weapon
            {
                get => (CapnpGen.WeaponType)this.ReadDataUShort(16UL, (ushort)0);
                set => this.WriteData(16UL, (ushort)value, (ushort)0);
            }

            public bool Available
            {
                get => this.ReadDataBool(32UL, false);
                set => this.WriteData(32UL, value, false);
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
            Pitch = reader.Pitch;
            Weapon = reader.Weapon;
            Slot = reader.Slot;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.ShooterId = ShooterId;
            writer.X = X;
            writer.Y = Y;
            writer.Z = Z;
            writer.Yaw = Yaw;
            writer.Pitch = Pitch;
            writer.Weapon = Weapon;
            writer.Slot = Slot;
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

        public float Pitch
        {
            get;
            set;
        }

        public CapnpGen.WeaponType Weapon
        {
            get;
            set;
        }

        public CapnpGen.WeaponSlot Slot
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
            public float Pitch => ctx.ReadDataFloat(192UL, 0F);
            public CapnpGen.WeaponType Weapon => (CapnpGen.WeaponType)ctx.ReadDataUShort(224UL, (ushort)0);
            public CapnpGen.WeaponSlot Slot => (CapnpGen.WeaponSlot)ctx.ReadDataUShort(240UL, (ushort)0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(4, 0);
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

            public float Pitch
            {
                get => this.ReadDataFloat(192UL, 0F);
                set => this.WriteData(192UL, value, 0F);
            }

            public CapnpGen.WeaponType Weapon
            {
                get => (CapnpGen.WeaponType)this.ReadDataUShort(224UL, (ushort)0);
                set => this.WriteData(224UL, (ushort)value, (ushort)0);
            }

            public CapnpGen.WeaponSlot Slot
            {
                get => (CapnpGen.WeaponSlot)this.ReadDataUShort(240UL, (ushort)0);
                set => this.WriteData(240UL, (ushort)value, (ushort)0);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xad037e11cb77255cUL)]
    public class WeaponPickedUp : ICapnpSerializable
    {
        public const UInt64 typeId = 0xad037e11cb77255cUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            PlayerId = reader.PlayerId;
            SpawnId = reader.SpawnId;
            Weapon = reader.Weapon;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.PlayerId = PlayerId;
            writer.SpawnId = SpawnId;
            writer.Weapon = Weapon;
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

        public ushort SpawnId
        {
            get;
            set;
        }

        public CapnpGen.WeaponType Weapon
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
            public ushort SpawnId => ctx.ReadDataUShort(64UL, (ushort)0);
            public CapnpGen.WeaponType Weapon => (CapnpGen.WeaponType)ctx.ReadDataUShort(80UL, (ushort)0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(2, 0);
            }

            public ulong PlayerId
            {
                get => this.ReadDataULong(0UL, 0UL);
                set => this.WriteData(0UL, value, 0UL);
            }

            public ushort SpawnId
            {
                get => this.ReadDataUShort(64UL, (ushort)0);
                set => this.WriteData(64UL, value, (ushort)0);
            }

            public CapnpGen.WeaponType Weapon
            {
                get => (CapnpGen.WeaponType)this.ReadDataUShort(80UL, (ushort)0);
                set => this.WriteData(80UL, (ushort)value, (ushort)0);
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
            WeaponPickedUp = 2,
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
                case WHICH.WeaponPickedUp:
                    WeaponPickedUp = CapnpSerializable.Create<CapnpGen.WeaponPickedUp>(reader.WeaponPickedUp);
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
                    case WHICH.WeaponPickedUp:
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
                case WHICH.ShotFired:
                    ShotFired?.serialize(writer.ShotFired);
                    break;
                case WHICH.Noop:
                    break;
                case WHICH.WeaponPickedUp:
                    WeaponPickedUp?.serialize(writer.WeaponPickedUp);
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

        public CapnpGen.WeaponPickedUp WeaponPickedUp
        {
            get => _which == WHICH.WeaponPickedUp ? (CapnpGen.WeaponPickedUp)_content : null;
            set
            {
                _which = WHICH.WeaponPickedUp;
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
            public CapnpGen.WeaponPickedUp.READER WeaponPickedUp => which == WHICH.WeaponPickedUp ? ctx.ReadStruct(0, CapnpGen.WeaponPickedUp.READER.create) : default;
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

            public CapnpGen.WeaponPickedUp.WRITER WeaponPickedUp
            {
                get => which == WHICH.WeaponPickedUp ? BuildPointer<CapnpGen.WeaponPickedUp.WRITER>(0) : default;
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
            Spawns = reader.Spawns?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.WeaponSpawnState>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.ServerTick = ServerTick;
            writer.Players.Init(Players, (_s1, _v1) => _v1?.serialize(_s1));
            writer.Events.Init(Events, (_s1, _v1) => _v1?.serialize(_s1));
            writer.Spawns.Init(Spawns, (_s1, _v1) => _v1?.serialize(_s1));
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

        public IReadOnlyList<CapnpGen.WeaponSpawnState> Spawns
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
            public IReadOnlyList<CapnpGen.WeaponSpawnState.READER> Spawns => ctx.ReadList(2).Cast(CapnpGen.WeaponSpawnState.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 3);
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

            public ListOfStructsSerializer<CapnpGen.WeaponSpawnState.WRITER> Spawns
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpGen.WeaponSpawnState.WRITER>>(2);
                set => Link(2, value);
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

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x954f28b6098186eeUL)]
    public class PlayerMeta : ICapnpSerializable
    {
        public const UInt64 typeId = 0x954f28b6098186eeUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            PlayerId = reader.PlayerId;
            Name = reader.Name;
            SkinId = reader.SkinId;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.PlayerId = PlayerId;
            writer.Name = Name;
            writer.SkinId = SkinId;
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

        public string Name
        {
            get;
            set;
        }

        public ushort SkinId
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
            public string Name => ctx.ReadText(0, null);
            public ushort SkinId => ctx.ReadDataUShort(64UL, (ushort)0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(2, 1);
            }

            public ulong PlayerId
            {
                get => this.ReadDataULong(0UL, 0UL);
                set => this.WriteData(0UL, value, 0UL);
            }

            public string Name
            {
                get => this.ReadText(0, null);
                set => this.WriteText(0, value, null);
            }

            public ushort SkinId
            {
                get => this.ReadDataUShort(64UL, (ushort)0);
                set => this.WriteData(64UL, value, (ushort)0);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x97fab85fa000c10eUL)]
    public class Roster : ICapnpSerializable
    {
        public const UInt64 typeId = 0x97fab85fa000c10eUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Players = reader.Players?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.PlayerMeta>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Players.Init(Players, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public IReadOnlyList<CapnpGen.PlayerMeta> Players
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
            public IReadOnlyList<CapnpGen.PlayerMeta.READER> Players => ctx.ReadList(0).Cast(CapnpGen.PlayerMeta.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 1);
            }

            public ListOfStructsSerializer<CapnpGen.PlayerMeta.WRITER> Players
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpGen.PlayerMeta.WRITER>>(0);
                set => Link(0, value);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x96de42fda64c9f07UL)]
    public class PlayerJoined : ICapnpSerializable
    {
        public const UInt64 typeId = 0x96de42fda64c9f07UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Player = CapnpSerializable.Create<CapnpGen.PlayerMeta>(reader.Player);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            Player?.serialize(writer.Player);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public CapnpGen.PlayerMeta Player
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
            public CapnpGen.PlayerMeta.READER Player => ctx.ReadStruct(0, CapnpGen.PlayerMeta.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 1);
            }

            public CapnpGen.PlayerMeta.WRITER Player
            {
                get => BuildPointer<CapnpGen.PlayerMeta.WRITER>(0);
                set => Link(0, value);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x85083e281cc922c4UL)]
    public class LobbyInfo : ICapnpSerializable
    {
        public const UInt64 typeId = 0x85083e281cc922c4UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            PlayerCount = reader.PlayerCount;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.PlayerCount = PlayerCount;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public ushort PlayerCount
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
            public ushort PlayerCount => ctx.ReadDataUShort(0UL, (ushort)0);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public ushort PlayerCount
            {
                get => this.ReadDataUShort(0UL, (ushort)0);
                set => this.WriteData(0UL, value, (ushort)0);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xd3649a02b37b8c1eUL)]
    public class ServerFull : ICapnpSerializable
    {
        public const UInt64 typeId = 0xd3649a02b37b8c1eUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            MaxPlayers = reader.MaxPlayers;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.MaxPlayers = MaxPlayers;
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public uint MaxPlayers
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
            public uint MaxPlayers => ctx.ReadDataUInt(0UL, 0U);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 0);
            }

            public uint MaxPlayers
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
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
            ScoreUpdate = 2,
            MatchEnded = 3,
            Roster = 4,
            PlayerJoined = 5,
            LobbyInfo = 6,
            ServerFull = 7,
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
                case WHICH.ScoreUpdate:
                    ScoreUpdate = CapnpSerializable.Create<CapnpGen.ScoreUpdate>(reader.ScoreUpdate);
                    break;
                case WHICH.MatchEnded:
                    MatchEnded = CapnpSerializable.Create<CapnpGen.MatchEnded>(reader.MatchEnded);
                    break;
                case WHICH.Roster:
                    Roster = CapnpSerializable.Create<CapnpGen.Roster>(reader.Roster);
                    break;
                case WHICH.PlayerJoined:
                    PlayerJoined = CapnpSerializable.Create<CapnpGen.PlayerJoined>(reader.PlayerJoined);
                    break;
                case WHICH.LobbyInfo:
                    LobbyInfo = CapnpSerializable.Create<CapnpGen.LobbyInfo>(reader.LobbyInfo);
                    break;
                case WHICH.ServerFull:
                    ServerFull = CapnpSerializable.Create<CapnpGen.ServerFull>(reader.ServerFull);
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
                    case WHICH.ScoreUpdate:
                        _content = null;
                        break;
                    case WHICH.MatchEnded:
                        _content = null;
                        break;
                    case WHICH.Roster:
                        _content = null;
                        break;
                    case WHICH.PlayerJoined:
                        _content = null;
                        break;
                    case WHICH.LobbyInfo:
                        _content = null;
                        break;
                    case WHICH.ServerFull:
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
                case WHICH.ScoreUpdate:
                    ScoreUpdate?.serialize(writer.ScoreUpdate);
                    break;
                case WHICH.MatchEnded:
                    MatchEnded?.serialize(writer.MatchEnded);
                    break;
                case WHICH.Roster:
                    Roster?.serialize(writer.Roster);
                    break;
                case WHICH.PlayerJoined:
                    PlayerJoined?.serialize(writer.PlayerJoined);
                    break;
                case WHICH.LobbyInfo:
                    LobbyInfo?.serialize(writer.LobbyInfo);
                    break;
                case WHICH.ServerFull:
                    ServerFull?.serialize(writer.ServerFull);
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

        public CapnpGen.ScoreUpdate ScoreUpdate
        {
            get => _which == WHICH.ScoreUpdate ? (CapnpGen.ScoreUpdate)_content : null;
            set
            {
                _which = WHICH.ScoreUpdate;
                _content = value;
            }
        }

        public CapnpGen.MatchEnded MatchEnded
        {
            get => _which == WHICH.MatchEnded ? (CapnpGen.MatchEnded)_content : null;
            set
            {
                _which = WHICH.MatchEnded;
                _content = value;
            }
        }

        public CapnpGen.Roster Roster
        {
            get => _which == WHICH.Roster ? (CapnpGen.Roster)_content : null;
            set
            {
                _which = WHICH.Roster;
                _content = value;
            }
        }

        public CapnpGen.PlayerJoined PlayerJoined
        {
            get => _which == WHICH.PlayerJoined ? (CapnpGen.PlayerJoined)_content : null;
            set
            {
                _which = WHICH.PlayerJoined;
                _content = value;
            }
        }

        public CapnpGen.LobbyInfo LobbyInfo
        {
            get => _which == WHICH.LobbyInfo ? (CapnpGen.LobbyInfo)_content : null;
            set
            {
                _which = WHICH.LobbyInfo;
                _content = value;
            }
        }

        public CapnpGen.ServerFull ServerFull
        {
            get => _which == WHICH.ServerFull ? (CapnpGen.ServerFull)_content : null;
            set
            {
                _which = WHICH.ServerFull;
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
            public CapnpGen.ScoreUpdate.READER ScoreUpdate => which == WHICH.ScoreUpdate ? ctx.ReadStruct(0, CapnpGen.ScoreUpdate.READER.create) : default;
            public CapnpGen.MatchEnded.READER MatchEnded => which == WHICH.MatchEnded ? ctx.ReadStruct(0, CapnpGen.MatchEnded.READER.create) : default;
            public CapnpGen.Roster.READER Roster => which == WHICH.Roster ? ctx.ReadStruct(0, CapnpGen.Roster.READER.create) : default;
            public CapnpGen.PlayerJoined.READER PlayerJoined => which == WHICH.PlayerJoined ? ctx.ReadStruct(0, CapnpGen.PlayerJoined.READER.create) : default;
            public CapnpGen.LobbyInfo.READER LobbyInfo => which == WHICH.LobbyInfo ? ctx.ReadStruct(0, CapnpGen.LobbyInfo.READER.create) : default;
            public CapnpGen.ServerFull.READER ServerFull => which == WHICH.ServerFull ? ctx.ReadStruct(0, CapnpGen.ServerFull.READER.create) : default;
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

            public CapnpGen.ScoreUpdate.WRITER ScoreUpdate
            {
                get => which == WHICH.ScoreUpdate ? BuildPointer<CapnpGen.ScoreUpdate.WRITER>(0) : default;
                set => Link(0, value);
            }

            public CapnpGen.MatchEnded.WRITER MatchEnded
            {
                get => which == WHICH.MatchEnded ? BuildPointer<CapnpGen.MatchEnded.WRITER>(0) : default;
                set => Link(0, value);
            }

            public CapnpGen.Roster.WRITER Roster
            {
                get => which == WHICH.Roster ? BuildPointer<CapnpGen.Roster.WRITER>(0) : default;
                set => Link(0, value);
            }

            public CapnpGen.PlayerJoined.WRITER PlayerJoined
            {
                get => which == WHICH.PlayerJoined ? BuildPointer<CapnpGen.PlayerJoined.WRITER>(0) : default;
                set => Link(0, value);
            }

            public CapnpGen.LobbyInfo.WRITER LobbyInfo
            {
                get => which == WHICH.LobbyInfo ? BuildPointer<CapnpGen.LobbyInfo.WRITER>(0) : default;
                set => Link(0, value);
            }

            public CapnpGen.ServerFull.WRITER ServerFull
            {
                get => which == WHICH.ServerFull ? BuildPointer<CapnpGen.ServerFull.WRITER>(0) : default;
                set => Link(0, value);
            }
        }
    }
}