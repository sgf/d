using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace d2;

// sk_ipoint_t
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct pointi : IEquatable<pointi>
{
    public static readonly pointi Empty;

    public pointi(sizei sz)
    {
        x = sz.Width;
        y = sz.Height;
    }

    public pointi(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public readonly bool IsEmpty => this == Empty;

    public readonly int Length => (int)Math.Sqrt(x * x + y * y);

    public readonly int LengthSquared => x * x + y * y;

    public void Offset(pointi p)
    {
        x += p.X;
        y += p.Y;
    }

    public void Offset(int dx, int dy)
    {
        x += dx;
        y += dy;
    }

    public readonly override string ToString() => $"{{X={x},Y={y}}}";

    public static pointi Normalize(pointi point)
    {
        var ls = point.x * point.x + point.y * point.y;
        var invNorm = 1.0 / Math.Sqrt(ls);
        return new pointi((int)(point.x * invNorm), (int)(point.y * invNorm));
    }

    public static float Distance(pointi point, pointi other)
    {
        var dx = point.x - other.x;
        var dy = point.y - other.y;
        var ls = dx * dx + dy * dy;
        return (float)Math.Sqrt(ls);
    }

    public static float DistanceSquared(pointi point, pointi other)
    {
        var dx = point.x - other.x;
        var dy = point.y - other.y;
        return dx * dx + dy * dy;
    }

    public static pointi Reflect(pointi point, pointi normal)
    {
        var dot = point.x * point.x + point.y * point.y;
        return new pointi(
            (int)(point.x - 2.0f * dot * normal.x),
            (int)(point.y - 2.0f * dot * normal.y));
    }

    public static pointi Ceiling(pointf value)
    {
        int x, y;
        checked
        {
            x = (int)Math.Ceiling(value.X);
            y = (int)Math.Ceiling(value.Y);
        }

        return new pointi(x, y);
    }

    public static pointi Round(pointf value)
    {
        int x, y;
        checked
        {
            x = (int)Math.Round(value.X);
            y = (int)Math.Round(value.Y);
        }

        return new pointi(x, y);
    }

    public static pointi Truncate(pointf value)
    {
        int x, y;
        checked
        {
            x = (int)value.X;
            y = (int)value.Y;
        }

        return new pointi(x, y);
    }

    public static pointi Add(pointi pt, sizei sz) => pt + sz;
    public static pointi Add(pointi pt, pointi sz) => pt + sz;

    public static pointi Subtract(pointi pt, sizei sz) => pt - sz;
    public static pointi Subtract(pointi pt, pointi sz) => pt - sz;

    public static pointi operator +(pointi pt, sizei sz) =>
        new(pt.X + sz.Width, pt.Y + sz.Height);
    public static pointi operator +(pointi pt, pointi sz) =>
        new(pt.X + sz.X, pt.Y + sz.Y);

    public static pointi operator -(pointi pt, sizei sz) =>
        new(pt.X - sz.Width, pt.Y - sz.Height);
    public static pointi operator -(pointi pt, pointi sz) =>
        new(pt.X - sz.X, pt.Y - sz.Y);

    public static explicit operator sizei(pointi p) =>
        new sizei(p.X, p.Y);
    public static implicit operator pointf(pointi p) =>
        new(p.X, p.Y);

    public static implicit operator Vector2(pointi point) =>
        new(point.x, point.y);



    // public int32_t x
    private Int32 x;
    public Int32 X
    {
        readonly get => x;
        set => x = value;
    }

    // public int32_t y
    private Int32 y;
    public Int32 Y
    {
        readonly get => y;
        set => y = value;
    }

    public readonly bool Equals(pointi obj) =>
        x == obj.x && y == obj.y;

    public readonly override bool Equals(object obj) =>
        obj is pointi f && Equals(f);

    public static bool operator ==(pointi left, pointi right) =>
        left.Equals(right);

    public static bool operator !=(pointi left, pointi right) =>
        !left.Equals(right);

    public readonly override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(x);
        hash.Add(y);
        return hash.ToHashCode();
    }

}
