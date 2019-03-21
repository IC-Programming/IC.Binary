using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IC.Binary
{
    internal class Internal
    {
        internal static byte ReadBit8(Stream stream) => (Read(stream, 1)[0]);
        internal static short ReadBit16(Stream stream) => ToBit16(Read(stream, 2));
        internal static int ReadBit32(Stream stream) => ToBit32(Read(stream, 4));
        internal static long ReadBit64(Stream stream) => ToBit64(Read(stream, 8));
        internal static void WriteBit8(Stream stream, byte b) => Write(stream, FromBit8(b));
        internal static void WriteBit16(Stream stream, short b) => Write(stream, FromBit16(b));
        internal static void WriteBit32(Stream stream, int b) => Write(stream, FromBit32(b));
        internal static void WriteBit64(Stream stream, long b) => Write(stream, FromBit64(b));
        internal static byte ToBit8(byte[] b, int s = 0) => b[s];
        internal static short ToBit16(byte[] b, int s = 0) => (short)((b[s + 1] << 8) | b[s]);
        internal static int ToBit32(byte[] b, int s = 0) => ((b[s + 3] << 0x18) | (b[s + 2] << 0x10) | (b[s + 1] << 8) | b[s]);
        internal static long ToBit64(byte[] b, int s = 0) => ((((long)((uint)((b[s + 7] << 0x18) | (b[s + 6] << 0x10) | (b[s + 5] << 8) | b[s + 4]))) << 0x20) | (uint)((b[s + 3] << 0x18) | (b[s + 2] << 0x10) | (b[s + 1] << 8) | b[s]));
        internal static byte[] FromBit8(byte v) => new byte[] { v };
        internal static byte[] FromBit16(short v)
        {
            var ret = new byte[2];
            ret[0] = (byte)v;
            ret[1] = (byte)(v >> 8);
            return ret;
        }
        internal static byte[] FromBit32(int v)
        {
            var ret = new byte[4];
            ret[0] = (byte)v;
            ret[1] = (byte)(v >> 8);
            ret[2] = (byte)(v >> 0x10);
            ret[3] = (byte)(v >> 0x18);
            return ret;
        }
        internal static byte[] FromBit64(long v)
        {
            var ret = new byte[8];
            ret[0] = (byte)v;
            ret[1] = (byte)(v >> 8);
            ret[2] = (byte)(v >> 0x10);
            ret[3] = (byte)(v >> 0x18);
            ret[4] = (byte)(v >> 0x20);
            ret[5] = (byte)(v >> 0x28);
            ret[6] = (byte)(v >> 0x30);
            ret[7] = (byte)(v >> 0x38);
            return ret;
        }
        internal static byte[] Read(Stream stream, int size)
        {
            var buf = new byte[size];
            if (stream.Read(buf, 0, size) == size)
                return buf;
            throw new ArgumentOutOfRangeException();
        }
        internal static void Write(Stream stream, byte[] b) => stream.Write(b, 0, b.Length);
        internal static char[] ToChars(byte[] V)
        {
            var ret = new char[V.Length];
            for (int i = 0; i < V.Length; i++)
                ret[i] = (char)V[i];
            return ret;
        }
    }
    internal class InternalMaths
    {
        internal static int MathBox(int num, int by)
        {
            int ret = 1;
            for (int i = 0; i < by - 1; i++)
                ret = ret * num;
            return by == 0 ? 0 : ret;
        }
    }
#pragma warning disable CS0660
#pragma warning disable CS0661
    internal struct Bit
    {
        private bool _Value;
        public Bit(bool V)
        {
            _Value = V;
        }
        public Bit(long V) : this((byte)V) { }
        public Bit(int V) : this((byte)V) { }
        public Bit(short V) : this((byte)V) { }
        public Bit(byte V)
        {
            _Value = (V | 0xFE) == 0xFF;
        }
        public bool Value { get => _Value; set => _Value = value; }
        public int ValueInt { get => _Value == false ? 0 : 1; set => _Value = value != 0; }
        public static implicit operator Bit(int v) => new Bit(v);
        public static bool operator ==(Bit a, Bit b) => a._Value == b._Value;
        public static bool operator !=(Bit a, Bit b) => a._Value != b._Value;
        public override string ToString()
        {
            return (_Value == false ? 0 : 1).ToString();
        }
        public static Bit[] FromInt4(Int4 b4)
        {
            var _Value = b4.Value;
            var ret = new Bit[4];
            for (int i = 3; i > -1; i--)
                ret[3 - i] = new Bit((_Value & ((byte)InternalMaths.MathBox(2, i + 1))) != 0);
            return ret;
        }
        public static Bit[] FromInt8(byte b8)
        {
            var _Value = b8;
            var ret = new Bit[8];
            for (int i = 7; i > -1; i--)
                ret[7 - i] = new Bit((_Value & ((byte)InternalMaths.MathBox(2, i + 1))) != 0);
            return ret;
        }
    }
    internal struct Int4
    {
        public static bool StringBinary = true;
        private byte _Value;
        public Int4(int val)
        {
            _Value = (byte)(val > 15 ? 15 : (val < 0 ? 0 : val));
        }
        public static implicit operator Int4(int v) => new Int4(v);
        public static bool operator ==(Int4 a, Int4 b) => a._Value == b._Value;
        public static bool operator !=(Int4 a, Int4 b) => a._Value != b._Value;
        public int Value
        {
            get => _Value;
            set => _Value = (byte)(value > 15 ? 15 : (value < 0 ? 0 : value));
        }
        public override string ToString()
        {
            if (!StringBinary)
                return _Value.ToString();
            string ret = "";
            for (sbyte i = 3; i > -1; i--)
                ret += ((_Value & ((byte)InternalMaths.MathBox(2, i + 1))) != 0 ? 1 : 0).ToString();
            return ret;
        }
        public static Int4[] FromInt8(byte V) => new Int4[] { ((V >> 4) & 0x0F), (V & 0x0F) };
        public static Int4[] GetInt4s(byte[] Vs)
        {
            var ret = new Int4[Vs.Length * 2];
            int pos = 0;
            Read:
            var curV = FromInt8(Vs[pos]);
            ret[(pos * 2)] = curV[0];
            ret[(pos * 2) + 1] = curV[1];
            pos++;
            if (Vs.Length > pos)
                goto Read;
            return ret;
        }
    }
#pragma warning restore CS0661
#pragma warning restore CS0660
}