using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace d2;

// sk_ipoint_t
[StructLayout (LayoutKind.Sequential)]
public unsafe partial struct PtInt : IEquatable<PtInt>
{
    public static readonly PtInt Empty;

    public PtInt (SzInt sz)
    {
        x = sz.Width;
        y = sz.Height;
    }

    public PtInt (int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public readonly bool IsEmpty => this == Empty;

    public readonly int Length => (int)Math.Sqrt (x * x + y * y);

    public readonly int LengthSquared => x * x + y * y;

    public void Offset (PtInt p)
    {
        x += p.X;
        y += p.Y;
    }

    public void Offset (int dx, int dy)
    {
        x += dx;
        y += dy;
    }

    public readonly override string ToString () => $"{{X={x},Y={y}}}";

    public static PtInt Normalize (PtInt point)
    {
        var ls = point.x * point.x + point.y * point.y;
        var invNorm = 1.0 / Math.Sqrt (ls);
        return new PtInt ((int)(point.x * invNorm), (int)(point.y * invNorm));
    }

    public static float Distance (PtInt point, PtInt other)
    {
        var dx = point.x - other.x;
        var dy = point.y - other.y;
        var ls = dx * dx + dy * dy;
        return (float)Math.Sqrt (ls);
    }

    public static float DistanceSquared (PtInt point, PtInt other)
    {
        var dx = point.x - other.x;
        var dy = point.y - other.y;
        return dx * dx + dy * dy;
    }

    public static PtInt Reflect (PtInt point, PtInt normal)
    {
        var dot = point.x * point.x + point.y * point.y;
        return new PtInt (
            (int)(point.x - 2.0f * dot * normal.x),
            (int)(point.y - 2.0f * dot * normal.y));
    }

    public static PtInt Ceiling (PtF value)
    {
        int x, y;
        checked {
            x = (int)Math.Ceiling (value.X);
            y = (int)Math.Ceiling (value.Y);
        }

        return new PtInt (x, y);
    }

    public static PtInt Round (PtF value)
    {
        int x, y;
        checked {
            x = (int)Math.Round (value.X);
            y = (int)Math.Round (value.Y);
        }

        return new PtInt (x, y);
    }

    public static PtInt Truncate (PtF value)
    {
        int x, y;
        checked {
            x = (int)value.X;
            y = (int)value.Y;
        }

        return new PtInt (x, y);
    }

    public static PtInt Add (PtInt pt, SzInt sz) => pt + sz;
    public static PtInt Add (PtInt pt, PtInt sz) => pt + sz;

    public static PtInt Subtract (PtInt pt, SzInt sz) => pt - sz;
    public static PtInt Subtract (PtInt pt, PtInt sz) => pt - sz;

    public static PtInt operator + (PtInt pt, SzInt sz) =>
        new PtInt (pt.X + sz.Width, pt.Y + sz.Height);
    public static PtInt operator + (PtInt pt, PtInt sz) =>
        new PtInt (pt.X + sz.X, pt.Y + sz.Y);

    public static PtInt operator - (PtInt pt, SzInt sz) =>
        new PtInt (pt.X - sz.Width, pt.Y - sz.Height);
    public static PtInt operator - (PtInt pt, PtInt sz) =>
        new PtInt (pt.X - sz.X, pt.Y - sz.Y);

    public static explicit operator SzInt (PtInt p) =>
        new SzInt (p.X, p.Y);
    public static implicit operator PtF (PtInt p) =>
        new  (p.X, p.Y);

    public static implicit operator Vector2 (PtInt point) =>
        new  (point.x, point.y);



    // public int32_t x
    private Int32 x;
    public Int32 X {
        readonly get => x;
        set => x = value;
    }

    // public int32_t y
    private Int32 y;
    public Int32 Y {
        readonly get => y;
        set => y = value;
    }

    public readonly bool Equals (PtInt obj) =>
        x == obj.x && y == obj.y;

    public readonly override bool Equals (object obj) =>
        obj is PtInt f && Equals (f);

    public static bool operator == (PtInt left, PtInt right) =>
        left.Equals (right);

    public static bool operator != (PtInt left, PtInt right) =>
        !left.Equals (right);

    public readonly override int GetHashCode ()
    {
        var hash = new HashCode ();
        hash.Add (x);
        hash.Add (y);
        return hash.ToHashCode ();
    }

}
