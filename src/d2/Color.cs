using PixelFarm.VectorMath;
using System.Globalization;

namespace d2;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 4)]
public unsafe readonly struct Color : IEquatable<Color>, IColor
{
    [InlineArray(4)]
    public struct Color32bbpArray
    {
        private byte _element0;
        //public byte this[int i]
        //{
        //    get => this[i];
        //    set => this[i] = value;
        //}
    }

    public static readonly Color Transparent = Empty;
    public static readonly Color Empty;
    public uint UIntValue => color;
    public readonly byte A => a;
    public readonly byte R => r;
    public readonly byte G => g;
    public readonly byte B => b;

    [FieldOffset(0)] private readonly uint color;
    [FieldOffset(0)] public readonly byte a;
    [FieldOffset(1)] public readonly byte r;
    [FieldOffset(2)] public readonly byte g;
    [FieldOffset(3)] public readonly byte b;
    [FieldOffset(0)] private readonly Color32bbpArray colorChans;

    //public readonly byte A => colorChans[3];// (byte)((color >> 24) & 0xff);
    //public readonly byte R => colorChans[2];// (byte)((color >> 16) & 0xff);
    //public readonly byte G => colorChans[1]; //(byte)((color >> 8) & 0xff);
    //public readonly byte B => colorChans[0]; //(byte)((color) & 0xff);

    public static implicit operator Color(uint color) => new(color);
    public static implicit operator uint(Color color) => color.color;


    public Color(uint value) => color = value;

    public Color(byte red, byte green, byte blue, byte alpha) => color = (uint)((alpha << 24) | (red << 16) | (green << 8) | blue);

    public Color(byte red, byte green, byte blue) => color = (0xff000000u | (uint)(red << 16) | (uint)(green << 8) | blue);

    public readonly Color WithRed(byte red) => new(red, G, B, A);

    public readonly Color WithGreen(byte green) => new(R, green, B, A);

    public readonly Color WithBlue(byte blue) => new(R, G, blue, A);

    public readonly Color WithAlpha(byte alpha) => new(R, G, B, alpha);


    public readonly float Hue
    {
        get
        {
            ToHsv(out var h, out _, out _);
            return h;
        }
    }

    public static Color FromHsl(float h, float s, float l, byte a = 255)
    {
        var colorf = ColorF.FromHsl(h, s, l);

        // RGB results from 0 to 255
        var r = colorf.R * 255f;
        var g = colorf.G * 255f;
        var b = colorf.B * 255f;

        return new Color((byte)r, (byte)g, (byte)b, a);
    }

    public static Color FromHsv(float h, float s, float v, byte a = 255)
    {
        var colorf = ColorF.FromHsv(h, s, v);

        // RGB results from 0 to 255
        var r = colorf.R * 255f;
        var g = colorf.G * 255f;
        var b = colorf.B * 255f;

        return new Color((byte)r, (byte)g, (byte)b, a);
    }

    public readonly void ToHsl(out float h, out float s, out float l)
    {
        // RGB from 0 to 255
        var r = R / 255f;
        var g = G / 255f;
        var b = B / 255f;

        var colorf = new ColorF(r, g, b);
        colorf.ToHsl(out h, out s, out l);
    }

    public readonly void ToHsv(out float h, out float s, out float v)
    {
        // RGB from 0 to 255
        var r = R / 255f;
        var g = G / 255f;
        var b = B / 255f;

        var colorf = new ColorF(r, g, b);
        colorf.ToHsv(out h, out s, out v);
    }

    public override readonly string ToString() => $"#{A:x2}{R:x2}{G:x2}{B:x2}";

    public readonly bool Equals(Color obj) => obj.color == color;

    public override readonly bool Equals(object other) => other is Color f && Equals(f);

    public static bool operator ==(Color left, Color right) => left.Equals(right);

    public static bool operator !=(Color left, Color right) => !left.Equals(right);

    public override readonly int GetHashCode() => color.GetHashCode();

    public static Color Parse(string hexString)
    {
        if (!TryParse(hexString, out var color))
            throw new ArgumentException("Invalid hexadecimal color string.", nameof(hexString));
        return color;
    }

    public static bool TryParse(string hexString, out Color color)
    {
        if (string.IsNullOrWhiteSpace(hexString))
        {
            // error
            color = Color.Empty;
            return false;
        }

        // clean up string
        var hexSpan = hexString.AsSpan().Trim().TrimStart('#');

        var len = hexSpan.Length;
        if (len == 3 || len == 4)
        {
            byte a;
            // parse [A]
            if (len == 4)
            {
                if (!byte.TryParse(hexSpan.Slice(0, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out a))
                {
                    // error
                    color = Color.Empty;
                    return false;
                }
                a = (byte)(a << 4 | a);
            }
            else
            {
                a = 255;
            }

            // parse RGB
            if (!byte.TryParse(hexSpan.Slice(len - 3, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r) ||
                !byte.TryParse(hexSpan.Slice(len - 2, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g) ||
                !byte.TryParse(hexSpan.Slice(len - 1, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
            {
                // error
                color = Color.Empty;
                return false;
            }

            // success
            color = new Color((byte)(r << 4 | r), (byte)(g << 4 | g), (byte)(b << 4 | b), a);
            return true;
        }

        if (len == 6 || len == 8)
        {
            // parse [AA]RRGGBB
            if (!uint.TryParse(hexSpan, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var number))
            {
                // error
                color = Color.Empty;
                return false;
            }

            // success
            color = (Color)number;

            // alpha was not provided, so use 255
            if (len == 6)
            {
                color = color.WithAlpha(255);
            }
            return true;
        }
        color = Color.Empty;
        return false;
    }
    public Vector3 ToVector3()
    {
        return new Vector3(R / 255.0f, G / 255.0f, B / 255.0f);
    }

    public Vector4 ToVector4()
    {
        return new Vector4(R / 255.0f, G / 255.0f, B / 255.0f, A / 255.0f);
    }

    public ColorF ToColorF()
    {
        var v = this.ToVector4();
        return new ColorF(v.x, v.y, v.z, v.w);
    }
}