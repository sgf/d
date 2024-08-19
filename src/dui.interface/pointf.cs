using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;

namespace d2;

// sk_point_t
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct pointf : IEquatable<pointf>
{
    public static readonly pointf Empty;

    public pointf(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public readonly bool IsEmpty => this == Empty;

    public readonly float Length => (float)Math.Sqrt(x * x + y * y);

    public readonly float LengthSquared => x * x + y * y;

    public void Offset(pointf p)
    {
        x += p.x;
        y += p.y;
    }

    public void Offset(float dx, float dy)
    {
        x += dx;
        y += dy;
    }

    public readonly override string ToString() => $"{{X={x}, Y={y}}}";

    public static pointf Normalize(pointf point)
    {
        var ls = point.x * point.x + point.y * point.y;
        var invNorm = 1.0 / Math.Sqrt(ls);
        return new pointf((float)(point.x * invNorm), (float)(point.y * invNorm));
    }

    public static float Distance(pointf point, pointf other)
    {
        var dx = point.x - other.x;
        var dy = point.y - other.y;
        var ls = dx * dx + dy * dy;
        return (float)Math.Sqrt(ls);
    }

    public static float DistanceSquared(pointf point, pointf other)
    {
        var dx = point.x - other.x;
        var dy = point.y - other.y;
        return dx * dx + dy * dy;
    }

    public static pointf Reflect(pointf point, pointf normal)
    {
        var dot = point.x * point.x + point.y * point.y;
        return new(
            point.x - 2.0f * dot * normal.x,
            point.y - 2.0f * dot * normal.y);
    }

    public static pointf Add(pointf pt, sizei sz) => pt + sz;
    public static pointf Add(pointf pt, sizef sz) => pt + sz;
    public static pointf Add(pointf pt, pointi sz) => pt + sz;
    public static pointf Add(pointf pt, pointf sz) => pt + sz;

    public static pointf Subtract(pointf pt, sizei sz) => pt - sz;
    public static pointf Subtract(pointf pt, sizef sz) => pt - sz;
    public static pointf Subtract(pointf pt, pointi sz) => pt - sz;
    public static pointf Subtract(pointf pt, pointf sz) => pt - sz;

    public static pointf operator +(pointf pt, sizei sz) =>
        new (pt.x + sz.Width, pt.y + sz.Height);
    public static pointf operator +(pointf pt, sizef sz) =>
        new (pt.x + sz.Width, pt.y + sz.Height);
    public static pointf operator +(pointf pt, pointi sz) =>
        new (pt.x + sz.X, pt.y + sz.Y);
    public static pointf operator +(pointf pt, pointf sz) =>
        new (pt.x + sz.X, pt.y + sz.Y);

    public static pointf operator -(pointf pt, sizei sz) =>
        new (pt.X - sz.Width, pt.Y - sz.Height);
    public static pointf operator -(pointf pt, sizef sz) =>
        new (pt.X - sz.Width, pt.Y - sz.Height);
    public static pointf operator -(pointf pt, pointi sz) =>
        new (pt.X - sz.X, pt.Y - sz.Y);
    public static pointf operator -(pointf pt, pointf sz) =>
        new (pt.X - sz.X, pt.Y - sz.Y);

    public static implicit operator Vector2(pointf point) =>
        new (point.x, point.y);

    public static implicit operator pointf(Vector2 vector) =>
        new (vector.X, vector.Y);

    private float x;
    public float X
    {
        readonly get => x;
        set => x = value;
    }

    private float y;
    public float Y
    {
        readonly get => y;
        set => y = value;
    }

    public readonly bool Equals(pointf obj) =>
        x == obj.x && y == obj.y;

    public readonly override bool Equals(object obj) =>
        obj is pointf f && Equals(f);

    public static bool operator ==(pointf left, pointf right) =>
        left.Equals(right);

    public static bool operator !=(pointf left, pointf right) =>
        !left.Equals(right);

    public readonly override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(x);
        hash.Add(y);
        return hash.ToHashCode();
    }

    //这个可能只能适用于double类型的HashCode计算
    //public override int GetHashCode ()
    //{
    //    return X.GetHashCode () ^ (Y.GetHashCode () * 397);
    //}

    public pointf Round()
    {
        return new((float)Math.Round(X), (float)Math.Round(Y));
    }

    public void Deconstruct(out float x, out float y)
    {
        x = X;
        y = Y;
    }

    public static bool TryParse(string value, out pointf point)
    {
        if (!string.IsNullOrEmpty(value))
        {
            string[] xy = value.Split(',');
            if (xy.Length == 2
                && float.TryParse(xy[0], NumberStyles.Number, CultureInfo.InvariantCulture, out var x)
                && float.TryParse(xy[1], NumberStyles.Number, CultureInfo.InvariantCulture, out var y))
            {
                point = new (x, y);
                return true;
            }
        }

        point = default;
        return false;
    }


}
