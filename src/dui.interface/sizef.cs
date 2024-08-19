using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;

namespace d2;

[StructLayout (LayoutKind.Sequential)]
public unsafe partial struct sizef : IEquatable<sizef>
{
    /// <summary> same as Zero </summary>
    public static readonly sizef Empty = Zero;
    public static readonly sizef Zero;

    public sizef (float size = 0)
    {
#if DEBUG
        if (float.IsNaN (size)) throw new ArgumentException ("NaN is not a valid value for size");
#endif
        w = size;
        h = size;
    }

    public sizef (float width, float height)
    {
#if DEBUG
        if (float.IsNaN (width)) throw new ArgumentException ("NaN is not a valid value for width");
        if (float.IsNaN (height)) throw new ArgumentException ("NaN is not a valid value for height");
#endif
        w = width;
        h = height;
    }

    public sizef (pointf pt)
    {
#if DEBUG
        if (float.IsNaN (pt.X)) throw new ArgumentException ("NaN is not a valid value for X");
        if (float.IsNaN (pt.Y)) throw new ArgumentException ("NaN is not a valid value for Y");
#endif
        w = pt.X;
        h = pt.Y;
    }

    public sizef (Vector2 vector)
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

    public readonly pointf ToPoint () => new (w, h);

    public readonly sizei ToSizeI ()
    {
        int w, h;
        checked {
            w = (int)this.w;
            h = (int)this.h;
        }

        return new sizei (w, h);
    }

    public readonly override string ToString () => $"{{Width={w}, Height={h}}}";

    public static sizef Add (sizef sz1, sizef sz2) => sz1 + sz2;

    public static sizef Subtract (sizef sz1, sizef sz2) => sz1 - sz2;

    public static sizef operator + (sizef sz1, sizef sz2) =>
        new (sz1.Width + sz2.Width, sz1.Height + sz2.Height);

    public static sizef operator - (sizef sz1, sizef sz2) =>
        new (sz1.Width - sz2.Width, sz1.Height - sz2.Height);

    public static explicit operator pointf (sizef size) =>
        new (size.Width, size.Height);
    public static implicit operator sizef (sizei size) =>
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

    public readonly bool Equals (sizef obj) =>
        w == obj.w && h == obj.h;

    public readonly override bool Equals (object obj) =>
        obj is sizef f && Equals (f);

    public static bool operator == (sizef left, sizef right) =>
        left.Equals (right);

    public static bool operator != (sizef left, sizef right) =>
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

    public static bool TryParse (string value, out sizef size)
    {
        if (!string.IsNullOrEmpty (value)) {
            string[] wh = value.Split (',');
            if (wh.Length == 2
                && float.TryParse (wh[0], NumberStyles.Number, CultureInfo.InvariantCulture, out var w)
                && float.TryParse (wh[1], NumberStyles.Number, CultureInfo.InvariantCulture, out var h)) {
                size = new sizef (w, h);
                return true;
            }
        }

        size = default;
        return false;
    }

}
