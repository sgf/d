using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;

namespace d2;

[StructLayout (LayoutKind.Sequential)]
public unsafe partial struct SzF : IEquatable<SzF>
{
    /// <summary> same as Zero </summary>
    public static readonly SzF Empty = Zero;
    public static readonly SzF Zero;

    public SzF (float size = 0)
    {
#if DEBUG
        if (float.IsNaN (size)) throw new ArgumentException ("NaN is not a valid value for size");
#endif
        w = size;
        h = size;
    }

    public SzF (float width, float height)
    {
#if DEBUG
        if (float.IsNaN (width)) throw new ArgumentException ("NaN is not a valid value for width");
        if (float.IsNaN (height)) throw new ArgumentException ("NaN is not a valid value for height");
#endif
        w = width;
        h = height;
    }

    public SzF (PtF pt)
    {
#if DEBUG
        if (float.IsNaN (pt.X)) throw new ArgumentException ("NaN is not a valid value for X");
        if (float.IsNaN (pt.Y)) throw new ArgumentException ("NaN is not a valid value for Y");
#endif
        w = pt.X;
        h = pt.Y;
    }

    public SzF (Vector2 vector)
    {
#if DEBUG
        if (float.IsNaN (vector.X)) throw new ArgumentException ("NaN is not a valid value for X");
        if (float.IsNaN (vector.Y)) throw new ArgumentException ("NaN is not a valid value for Y");
#endif
        w = vector.X;
        h = vector.Y;
    }

    public readonly bool IsEmpty => IsZero;
    public readonly bool IsZero => this == Zero;

    public readonly PtF ToPoint () => new (w, h);

    public readonly SzInt ToSizeI ()
    {
        int w, h;
        checked {
            w = (int)this.w;
            h = (int)this.h;
        }

        return new SzInt (w, h);
    }

    public readonly override string ToString () => $"{{Width={w}, Height={h}}}";

    public static SzF Add (SzF sz1, SzF sz2) => sz1 + sz2;

    public static SzF Subtract (SzF sz1, SzF sz2) => sz1 - sz2;

    public static SzF operator + (SzF sz1, SzF sz2) =>
        new (sz1.Width + sz2.Width, sz1.Height + sz2.Height);

    public static SzF operator - (SzF sz1, SzF sz2) =>
        new (sz1.Width - sz2.Width, sz1.Height - sz2.Height);

    public static explicit operator PtF (SzF size) =>
        new (size.Width, size.Height);
    public static implicit operator SzF (SzInt size) =>
        new (size.Width, size.Height);

    private Single w;
    public Single Width {
        readonly get => w;
        set => w = value;
    }

    private Single h;
    public Single Height {
        readonly get => h;
        set => h = value;
    }

    public readonly bool Equals (SzF obj) =>
        w == obj.w && h == obj.h;

    public readonly override bool Equals (object obj) =>
        obj is SzF f && Equals (f);

    public static bool operator == (SzF left, SzF right) =>
        left.Equals (right);

    public static bool operator != (SzF left, SzF right) =>
        !left.Equals (right);

    public readonly override int GetHashCode ()
    {
        var hash = new HashCode ();
        hash.Add (w);
        hash.Add (h);
        return hash.ToHashCode ();
    }

    //这个可能只能适用于double类型的HashCode计算
    //public override int GetHashCode ()
    //{
    //    unchecked {
    //        return (_width.GetHashCode () * 397) ^ _height.GetHashCode ();
    //    }
    //}

    public void Deconstruct (out float width, out float height)
    {
        width = Width;
        height = Height;
    }

    public static bool TryParse (string value, out SzF size)
    {
        if (!string.IsNullOrEmpty (value)) {
            string[] wh = value.Split (',');
            if (wh.Length == 2
                && float.TryParse (wh[0], NumberStyles.Number, CultureInfo.InvariantCulture, out var w)
                && float.TryParse (wh[1], NumberStyles.Number, CultureInfo.InvariantCulture, out var h)) {
                size = new SzF (w, h);
                return true;
            }
        }

        size = default;
        return false;
    }

}
