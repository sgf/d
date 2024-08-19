using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d2;

// sk_isize_t
[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct sizei : IEquatable<sizei>
{
    public static readonly sizei Empty;

    public sizei(int width, int height)
    {
        w = width;
        h = height;
    }

    public sizei(pointi pt)
    {
        w = pt.X;
        h = pt.Y;
    }

    public readonly bool IsEmpty => this == Empty;

    public readonly pointi ToPointI() => new(w, h);

    public readonly override string ToString() =>
        $"{{Width={w}, Height={h}}}";

    public static sizei Add(sizei sz1, sizei sz2) => sz1 + sz2;

    public static sizei Subtract(sizei sz1, sizei sz2) => sz1 - sz2;

    public static sizei operator +(sizei sz1, sizei sz2) =>
        new(sz1.Width + sz2.Width, sz1.Height + sz2.Height);

    public static sizei operator -(sizei sz1, sizei sz2) =>
        new(sz1.Width - sz2.Width, sz1.Height - sz2.Height);

    public static explicit operator pointi(sizei size) =>
        new(size.Width, size.Height);

    // public int32_t w
    private Int32 w;
    public Int32 Width
    {
        readonly get => w;
        set => w = value;
    }

    // public int32_t h
    private Int32 h;
    public Int32 Height
    {
        readonly get => h;
        set => h = value;
    }

    public readonly bool Equals(sizei obj) =>
        w == obj.w && h == obj.h;

    public readonly override bool Equals(object obj) =>
        obj is sizei f && Equals(f);

    public static bool operator ==(sizei left, sizei right) =>
        left.Equals(right);

    public static bool operator !=(sizei left, sizei right) =>
        !left.Equals(right);

    public readonly override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(w);
        hash.Add(h);
        return hash.ToHashCode();
    }

}
