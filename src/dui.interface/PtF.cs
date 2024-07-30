using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;

namespace d2;

// sk_point_t
[StructLayout (LayoutKind.Sequential)]
public unsafe partial struct PtF : IEquatable<PtF>
{
    public static readonly PtF Empty;

    public PtF (float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public readonly bool IsEmpty => this == Empty;

    public readonly float Length => (float)Math.Sqrt (x * x + y * y);

    public readonly float LengthSquared => x * x + y * y;

    public void Offset (PtF p)
    {
        x += p.x;
        y += p.y;
    }

    public void Offset (float dx, float dy)
    {
        x += dx;
        y += dy;
    }

    public readonly override string ToString () => $"{{X={x}, Y={y}}}";

    public static PtF Normalize (PtF point)
    {
        var ls = point.x * point.x + point.y * point.y;
        var invNorm = 1.0 / Math.Sqrt (ls);
        return new PtF ((float)(point.x * invNorm), (float)(point.y * invNorm));
    }

    public static float Distance (PtF point, PtF other)
    {
        var dx = point.x - other.x;
        var dy = point.y - other.y;
        var ls = dx * dx + dy * dy;
        return (float)Math.Sqrt (ls);
    }

    public static float DistanceSquared (PtF point, PtF other)
    {
        var dx = point.x - other.x;
        var dy = point.y - other.y;
        return dx * dx + dy * dy;
    }

    public static PtF Reflect (PtF point, PtF normal)
    {
        var dot = point.x * point.x + point.y * point.y;
        return new PtF (
            point.x - 2.0f * dot * normal.x,
            point.y - 2.0f * dot * normal.y);
    }

    public static PtF Add (PtF pt, SzInt sz) => pt + sz;
    public static PtF Add (PtF pt, SzF sz) => pt + sz;
    public static PtF Add (PtF pt, PtInt sz) => pt + sz;
    public static PtF Add (PtF pt, PtF sz) => pt + sz;

    public static PtF Subtract (PtF pt, SzInt sz) => pt - sz;
    public static PtF Subtract (PtF pt, SzF sz) => pt - sz;
    public static PtF Subtract (PtF pt, PtInt sz) => pt - sz;
    public static PtF Subtract (PtF pt, PtF sz) => pt - sz;

    public static PtF operator + (PtF pt, SzInt sz) =>
        new PtF (pt.x + sz.Width, pt.y + sz.Height);
    public static PtF operator + (PtF pt, SzF sz) =>
        new PtF (pt.x + sz.Width, pt.y + sz.Height);
    public static PtF operator + (PtF pt, PtInt sz) =>
        new PtF (pt.x + sz.X, pt.y + sz.Y);
    public static PtF operator + (PtF pt, PtF sz) =>
        new PtF (pt.x + sz.X, pt.y + sz.Y);

    public static PtF operator - (PtF pt, SzInt sz) =>
        new PtF (pt.X - sz.Width, pt.Y - sz.Height);
    public static PtF operator - (PtF pt, SzF sz) =>
        new PtF(pt.X - sz.Width, pt.Y - sz.Height);
    public static PtF operator - (PtF pt, PtInt sz) =>
        new PtF (pt.X - sz.X, pt.Y - sz.Y);
    public static PtF operator - (PtF pt, PtF sz) =>
        new PtF (pt.X - sz.X, pt.Y - sz.Y);

    public static implicit operator Vector2 (PtF point) =>
        new Vector2 (point.x, point.y);

    public static implicit operator PtF (Vector2 vector) =>
        new PtF (vector.X, vector.Y);

    private float x;
    public float X {
        readonly get => x;
        set => x = value;
    }

    private float y;
    public float Y {
        readonly get => y;
        set => y = value;
    }

    public readonly bool Equals (PtF obj) =>
        x == obj.x && y == obj.y;

    public readonly override bool Equals (object obj) =>
        obj is PtF f && Equals (f);

    public static bool operator == (PtF left, PtF right) =>
        left.Equals (right);

    public static bool operator != (PtF left, PtF right) =>
        !left.Equals (right);

    public readonly override int GetHashCode ()
    {
        var hash = new HashCode ();
        hash.Add (x);
        hash.Add (y);
        return hash.ToHashCode ();
    }

    //这个可能只能适用于double类型的HashCode计算
    //public override int GetHashCode ()
    //{
    //    return X.GetHashCode () ^ (Y.GetHashCode () * 397);
    //}

    public PtF Round ()
    {
        return new  ((float)Math.Round (X), (float)Math.Round (Y));
    }

    public void Deconstruct (out float x, out float y)
    {
        x = X;
        y = Y;
    }

    public static bool TryParse (string value, out PtF point)
    {
        if (!string.IsNullOrEmpty (value)) {
            string[] xy = value.Split (',');
            if (xy.Length == 2 
                && float.TryParse (xy[0], NumberStyles.Number, CultureInfo.InvariantCulture, out var x)
                && float.TryParse (xy[1], NumberStyles.Number, CultureInfo.InvariantCulture, out var y)) {
                point = new PtF (x, y);
                return true;
            }
        }

        point = default;
        return false;
    }


}
