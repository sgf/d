using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d2;

// sk_isize_t
[StructLayout (LayoutKind.Sequential)]
public unsafe partial struct SzInt : IEquatable<SzInt>
{
    public static readonly SzInt Empty;

    public SzInt (int width, int height)
    {
        w = width;
        h = height;
    }

    public SzInt (PtInt pt)
    {
        w = pt.X;
        h = pt.Y;
    }

    public readonly bool IsEmpty => this == Empty;

    public readonly PtInt ToPointI () => new PtInt (w, h);

    public readonly override string ToString () =>
        $"{{Width={w}, Height={h}}}";

    public static SzInt Add (SzInt sz1, SzInt sz2) => sz1 + sz2;

    public static SzInt Subtract (SzInt sz1, SzInt sz2) => sz1 - sz2;

    public static SzInt operator + (SzInt sz1, SzInt sz2) =>
        new SzInt (sz1.Width + sz2.Width, sz1.Height + sz2.Height);

    public static SzInt operator - (SzInt sz1, SzInt sz2) =>
        new SzInt (sz1.Width - sz2.Width, sz1.Height - sz2.Height);

    public static explicit operator PtInt (SzInt size) =>
        new PtInt (size.Width, size.Height);

        // public int32_t w
        private Int32 w;
        public Int32 Width {
            readonly get => w;
            set => w = value;
        }

        // public int32_t h
        private Int32 h;
        public Int32 Height {
            readonly get => h;
            set => h = value;
        }

        public readonly bool Equals (SzInt obj) =>
            w == obj.w && h == obj.h;

        public readonly override bool Equals (object obj) =>
            obj is SzInt f && Equals (f);

        public static bool operator == (SzInt left, SzInt right) =>
            left.Equals (right);

        public static bool operator != (SzInt left, SzInt right) =>
            !left.Equals (right);

        public readonly override int GetHashCode ()
        {
            var hash = new HashCode ();
            hash.Add (w);
            hash.Add (h);
            return hash.ToHashCode ();
        }

    }
