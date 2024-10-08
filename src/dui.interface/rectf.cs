﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace d2;

// sk_rect_t

[DebuggerDisplay ("X/Left:{Left}, Y/Top:{Top}, Width:{Width}, Height:{Height}, Right:{Right}, Bottom:{Bottom}")]
//[TypeConverter (typeof (Converters.RectTypeConverter))]
[StructLayout (LayoutKind.Sequential)]
public unsafe partial struct rectf : IEquatable<rectf>
{
    public static readonly rectf Empty;
    //public static Rt Zero = new Rt ();

    public rectf (float left, float top, float right, float bottom)
    {
        this.left = left;
        this.right = right;
        this.top = top;
        this.bottom = bottom;
    }

    public rectf(float left, float top, (float width, float height) widthHeight)
    {
        this.left = left;
        this.right = left+widthHeight.width;
        this.top = top;
        this.bottom = top+ widthHeight.height;
    }

    public rectf (pointf loc, sizef sz) : this (loc.X, loc.Y, loc.X + sz.Width, loc.Y + sz.Height)
    {

    }

    public static rectf FromXYWH (float x, float y, float width, float height)
    => rectf.Create (x, y, width, height);

    public readonly pointf Mid => Ctr;
    public readonly float MidX => CtrX;
    public readonly float MidY => CtrY;
    public readonly pointf Ctr => new (CtrX, CtrY);
    public readonly float CtrX => left + (Width / 2f);
    public readonly float CtrY => top + (Height / 2f);

    public readonly float X => left;

    public readonly float Y => top;

    public readonly float Width => right - left;

    public readonly float Height => bottom - top;

    public readonly bool IsEmpty => this == Empty;

    public sizef Size {
        readonly get => new  (Width, Height);
        set {
            right = left + value.Width;
            bottom = top + value.Height;
        }
    }

    public pointf Location {
        readonly get => new pointf (left, top);
        set => this = rectf.Create (value, Size);
    }

    public readonly rectf Standardized {
        get {
            if (left > right) {
                if (top > bottom) {
                    return new rectf (right, bottom, left, top);
                } else {
                    return new rectf (right, top, left, bottom);
                }
            } else {
                if (top > bottom) {
                    return new rectf (left, bottom, right, top);
                } else {
                    return new rectf (left, top, right, bottom);
                }
            }
        }
    }

    public readonly rectf AspectFit (sizef size) => AspectResize (size, true);

    public readonly rectf AspectFill (sizef size) => AspectResize (size, false);

    private readonly rectf AspectResize (sizef size, bool fit)
    {
        if (size.Width == 0 || size.Height == 0 || Width == 0 || Height == 0)
            return Create (MidX, MidY, 0, 0);

        var aspectWidth = size.Width;
        var aspectHeight = size.Height;
        var imgAspect = aspectWidth / aspectHeight;
        var fullRectAspect = Width / Height;

        var compare = fit ? (fullRectAspect > imgAspect) : (fullRectAspect < imgAspect);
        if (compare) {
            aspectHeight = Height;
            aspectWidth = aspectHeight * imgAspect;
        } else {
            aspectWidth = Width;
            aspectHeight = aspectWidth / imgAspect;
        }
        var aspectLeft = MidX - (aspectWidth / 2f);
        var aspectTop = MidY - (aspectHeight / 2f);

        return Create (aspectLeft, aspectTop, aspectWidth, aspectHeight);
    }

    public static rectf Inflate (rectf rect, float x, float y)
    {
        var r = new rectf (rect.left, rect.top, rect.right, rect.bottom);
        r.Inflate (x, y);
        return r;
    }

    /// <summary> 膨胀,扩大 </summary>
    /// <param name="size"></param>
    public void Inflate (sizef size) =>
        Inflate (size.Width, size.Height);

    public void Inflate (float x, float y)
    {
        left -= x;
        top -= y;
        right += x;
        bottom += y;
    }

    public static rectf Intersect (rectf a, rectf b)
    {
        if (!a.IntersectsWithInclusive (b)) {
            return Empty;
        }
        return new rectf (
            Math.Max (a.left, b.left),
            Math.Max (a.top, b.top),
            Math.Min (a.right, b.right),
            Math.Min (a.bottom, b.bottom));
    }

    public void Intersect (rectf rect) =>
        this = Intersect (this, rect);

    public static rectf Union (rectf a, rectf b) =>
        new rectf (
            Math.Min (a.left, b.left),
            Math.Min (a.top, b.top),
            Math.Max (a.right, b.right),
            Math.Max (a.bottom, b.bottom));

    public void Union (rectf rect) =>
        this = Union (this, rect);

    public static implicit operator rectf (recti r) =>
        new (r.Left, r.Top, r.Right, r.Bottom);

    //public static implicit operator RtI (Rt  r) =>
    //    new (r.Left, r.Top, r.Right, r.Bottom);

    public readonly bool Contains (float x, float y) =>
        (x >= left) && (x < right) && (y >= top) && (y < bottom);

    public readonly bool Contains (pointf pt) =>
        Contains (pt.X, pt.Y);

    public readonly bool Contains (rectf rect) =>
        (left <= rect.left) && (right >= rect.right) &&
        (top <= rect.top) && (bottom >= rect.bottom);

    public readonly bool IntersectsWith (rectf rect) =>
        (left < rect.right) && (right > rect.left) && (top < rect.bottom) && (bottom > rect.top);

    public readonly bool IntersectsWithInclusive (rectf rect) =>
        (left <= rect.right) && (right >= rect.left) && (top <= rect.bottom) && (bottom >= rect.top);

    public void Offset (float x, float y)
    {
        left += x;
        top += y;
        right += x;
        bottom += y;
    }

    public void Offset (pointf pos) => Offset (pos.X, pos.Y);

    public readonly override string ToString () =>
        $"X/Left:{Left}, Y/Top:{Top}, Width:{Width}, Height:{Height}, Right:{Right}, Bottom:{Bottom}";

    public static rectf Create (pointf location, sizef size) =>
        Create (location.X, location.Y, size.Width, size.Height);

    public static rectf Create (sizef size) =>
        Create (pointf.Empty, size);

    public static rectf Create (float width, float height) =>
        new (pointf.Empty.X, pointf.Empty.Y, width, height);

    public static rectf Create (float x, float y, float width, float height) =>
        new (x, y, x + width, y + height);

    private Single left;
    public Single Left {
        readonly get => left;
        set => left = value;
    }

    private Single top;
    public Single Top {
        readonly get => top;
        set => top = value;
    }

    private Single right;
    public Single Right {
        readonly get => right;
        set => right = value;
    }

    private Single bottom;
    public Single Bottom {
        readonly get => bottom;
        set => bottom = value;
    }

    public readonly bool Equals (rectf obj) =>
        left == obj.left && top == obj.top && right == obj.right && bottom == obj.bottom;

    public readonly override bool Equals (object obj)
    {
        if (ReferenceEquals (null, obj))
            return false;
        return obj is rectf f && Equals (f);
    }

    public static bool operator == (rectf left, rectf right) =>
        left.Equals (right);//本质上是字节对比,因此可以优化为int64x2的直接对比,当然这还得增加 FieldOffset

    //public static bool operator == (Rt r1, Rt r2)
    //    => (r1.Location == r2.Location) && (r1.Size == r2.Size);

    public static bool operator != (rectf left, rectf right) =>
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

    //这个可能只能适用于double类型的HashCode计算
    //public override int GetHashCode ()
    //{
    //    unchecked {
    //        int hashCode = X.GetHashCode ();
    //        hashCode = (hashCode * 397) ^ Y.GetHashCode ();
    //        hashCode = (hashCode * 397) ^ Width.GetHashCode ();
    //        hashCode = (hashCode * 397) ^ Height.GetHashCode ();
    //        return hashCode;
    //    }
    //}

    //取整
    public rectf Round ()
        => new ((float)Math.Round (X), (float)Math.Round (Y), (float)Math.Round (Width), (float)Math.Round (Height));

    /// <summary>
    /// 结构
    /// </summary>
    public void Deconstruct (out float x_left, out float y_top,out float width, out float height, out float right, out float bottom)
    {
        x_left = X;
        y_top = Y;
        width = Width;
        height = Height;
        right = this.right;
        bottom = this.bottom;
    }

    public static bool TryParse (string value, out rectf rectangle)
    {
        if (!string.IsNullOrEmpty (value)) {
            string[] xywh = value.Split (',');
            if (xywh.Length == 4
                && float.TryParse (xywh[0], NumberStyles.Number, CultureInfo.InvariantCulture, out var x)
                && float.TryParse (xywh[1], NumberStyles.Number, CultureInfo.InvariantCulture, out var y)
                && float.TryParse (xywh[2], NumberStyles.Number, CultureInfo.InvariantCulture, out var w)
                && float.TryParse (xywh[3], NumberStyles.Number, CultureInfo.InvariantCulture, out var h)) {
                rectangle = new  (x, y, w, h);
                return true;
            }
        }

        rectangle = default;
        return false;
    }

}
