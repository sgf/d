using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d2;


// sk_irect_t
[StructLayout (LayoutKind.Sequential)]
public unsafe partial struct recti : IEquatable<recti>
{
    public static readonly recti Empty;

    public recti (int left, int top, int right, int bottom)
    {
        this.left = left;
        this.right = right;
        this.top = top;
        this.bottom = bottom;
    }

    public readonly int MidX => left + (Width / 2);

    public readonly int MidY => top + (Height / 2);

    public readonly int Width => right - left;

    public readonly int Height => bottom - top;

    public readonly bool IsEmpty => this == Empty;

    public sizei Size {
        readonly get => new sizei (Width, Height);
        set {
            right = left + value.Width;
            bottom = top + value.Height;
        }
    }

    public pointi Location {
        readonly get => new pointi (left, top);
        set => this = recti.Create (value, Size);
    }

    public readonly recti Standardized {
        get {
            if (left > right) {
                if (top > bottom) {
                    return new recti (right, bottom, left, top);
                } else {
                    return new recti (right, top, left, bottom);
                }
            } else {
                if (top > bottom) {
                    return new recti (left, bottom, right, top);
                } else {
                    return new recti (left, top, right, bottom);
                }
            }
        }
    }

    public readonly recti AspectFit (sizei size) =>
        Floor (((rectf)this).AspectFit (size));

    public readonly recti AspectFill (sizei size) =>
        Floor (((rectf)this).AspectFill (size));

    public static recti Ceiling (rectf value) =>
        Ceiling (value, false);

    public static recti Ceiling (rectf value, bool outwards)
    {
        int x, y, r, b;
        checked {
            x = (int)(outwards && value.Width > 0 ? Math.Floor (value.Left) : Math.Ceiling (value.Left));
            y = (int)(outwards && value.Height > 0 ? Math.Floor (value.Top) : Math.Ceiling (value.Top));
            r = (int)(outwards && value.Width < 0 ? Math.Floor (value.Right) : Math.Ceiling (value.Right));
            b = (int)(outwards && value.Height < 0 ? Math.Floor (value.Bottom) : Math.Ceiling (value.Bottom));
        }

        return new recti (x, y, r, b);
    }

    public static recti Inflate (recti rect, int x, int y)
    {
        var r = new recti (rect.left, rect.top, rect.right, rect.bottom);
        r.Inflate (x, y);
        return r;
    }

    public void Inflate (sizei size) =>
        Inflate (size.Width, size.Height);

    public void Inflate (int width, int height)
    {
        left -= width;
        top -= height;
        right += width;
        bottom += height;
    }

    public static recti Intersect (recti a, recti b)
    {
        if (!a.IntersectsWithInclusive (b))
            return Empty;

        return new recti (
            Math.Max (a.left, b.left),
            Math.Max (a.top, b.top),
            Math.Min (a.right, b.right),
            Math.Min (a.bottom, b.bottom));
    }

    public void Intersect (recti rect) =>
        this = Intersect (this, rect);

    public static recti Round (rectf value)
    {
        int x, y, r, b;
        checked {
            x = (int)Math.Round (value.Left);
            y = (int)Math.Round (value.Top);
            r = (int)Math.Round (value.Right);
            b = (int)Math.Round (value.Bottom);
        }

        return new recti (x, y, r, b);
    }

    public static recti Floor (rectf value) => Floor (value, false);

    public static recti Floor (rectf value, bool inwards)
    {
        int x, y, r, b;
        checked {
            x = (int)(inwards && value.Width > 0 ? Math.Ceiling (value.Left) : Math.Floor (value.Left));
            y = (int)(inwards && value.Height > 0 ? Math.Ceiling (value.Top) : Math.Floor (value.Top));
            r = (int)(inwards && value.Width < 0 ? Math.Ceiling (value.Right) : Math.Floor (value.Right));
            b = (int)(inwards && value.Height < 0 ? Math.Ceiling (value.Bottom) : Math.Floor (value.Bottom));
        }

        return new recti (x, y, r, b);
    }

    public static recti Truncate (rectf value)
    {
        int x, y, r, b;
        checked {
            x = (int)value.Left;
            y = (int)value.Top;
            r = (int)value.Right;
            b = (int)value.Bottom;
        }

        return new recti (x, y, r, b);
    }

    public static recti Union (recti a, recti b) =>
        new recti (
            Math.Min (a.Left, b.Left),
            Math.Min (a.Top, b.Top),
            Math.Max (a.Right, b.Right),
            Math.Max (a.Bottom, b.Bottom));

    public void Union (recti rect) =>
        this = Union (this, rect);

    public readonly bool Contains (int x, int y) =>
        (x >= left) && (x < right) && (y >= top) && (y < bottom);

    public readonly bool Contains (pointi pt) =>
        Contains (pt.X, pt.Y);

    public readonly bool Contains (recti rect) =>
        (left <= rect.left) && (right >= rect.right) &&
        (top <= rect.top) && (bottom >= rect.bottom);

    public readonly bool IntersectsWith (recti rect) =>
        (left < rect.right) && (right > rect.left) && (top < rect.bottom) && (bottom > rect.top);

    public readonly bool IntersectsWithInclusive (recti rect) =>
        (left <= rect.right) && (right >= rect.left) && (top <= rect.bottom) && (bottom >= rect.top);

    public void Offset (int x, int y)
    {
        left += x;
        top += y;
        right += x;
        bottom += y;
    }

    public void Offset (pointi pos) => Offset (pos.X, pos.Y);

    public readonly override string ToString () =>
        $"{{Left={Left},Top={Top},Width={Width},Height={Height}}}";

    public static recti Create (sizei size) =>
        Create (pointi.Empty.X, pointi.Empty.Y, size.Width, size.Height);

    public static recti Create (pointi location, sizei size) =>
        Create (location.X, location.Y, size.Width, size.Height);

    public static recti Create (int width, int height) =>
        new recti (pointi.Empty.X, pointi.Empty.X, width, height);

    public static recti Create (int x, int y, int width, int height) =>
        new recti (x, y, x + width, y + height);


    // public int32_t left
    private Int32 left;
    public Int32 Left {
        readonly get => left;
        set => left = value;
    }

    // public int32_t top
    private Int32 top;
    public Int32 Top {
        readonly get => top;
        set => top = value;
    }

    // public int32_t right
    private Int32 right;
    public Int32 Right {
        readonly get => right;
        set => right = value;
    }

    // public int32_t bottom
    private Int32 bottom;
    public Int32 Bottom {
        readonly get => bottom;
        set => bottom = value;
    }

    public readonly bool Equals (recti obj) =>
        left == obj.left && top == obj.top && right == obj.right && bottom == obj.bottom;

    public readonly override bool Equals (object obj) =>
        obj is recti f && Equals (f);

    public static bool operator == (recti left, recti right) =>
        left.Equals (right);

    public static bool operator != (recti left, recti right) =>
        !left.Equals (right);

    public readonly override int GetHashCode ()
    {
        var hash = new HashCode ();
        hash.Add (left);
        hash.Add (top);
        hash.Add (right);
        hash.Add (bottom);
        return hash.ToHashCode ();
    }

}
