using System.Globalization;

namespace d2;

public readonly struct Color : IEquatable<Color>
{
    public static readonly Color Empty;

    private readonly uint color;

    public Color(uint value) => color = value;

    public Color(byte red, byte green, byte blue, byte alpha) => color = (uint)((alpha << 24) | (red << 16) | (green << 8) | blue);

    public Color(byte red, byte green, byte blue) => color = (0xff000000u | (uint)(red << 16) | (uint)(green << 8) | blue);

    public readonly Color WithRed(byte red) => new Color(red, Green, Blue, Alpha);

    public readonly Color WithGreen(byte green) => new Color(Red, green, Blue, Alpha);

    public readonly Color WithBlue(byte blue) => new Color(Red, Green, blue, Alpha);

    public readonly Color WithAlpha(byte alpha) => new Color(Red, Green, Blue, alpha);

    public readonly byte Alpha => (byte)((color >> 24) & 0xff);
    public readonly byte Red => (byte)((color >> 16) & 0xff);
    public readonly byte Green => (byte)((color >> 8) & 0xff);
    public readonly byte Blue => (byte)((color) & 0xff);

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
        var r = colorf.Red * 255f;
        var g = colorf.Green * 255f;
        var b = colorf.Blue * 255f;

        return new Color((byte)r, (byte)g, (byte)b, a);
    }

    public static Color FromHsv(float h, float s, float v, byte a = 255)
    {
        var colorf = ColorF.FromHsv(h, s, v);

        // RGB results from 0 to 255
        var r = colorf.Red * 255f;
        var g = colorf.Green * 255f;
        var b = colorf.Blue * 255f;

        return new Color((byte)r, (byte)g, (byte)b, a);
    }

    public readonly void ToHsl(out float h, out float s, out float l)
    {
        // RGB from 0 to 255
        var r = Red / 255f;
        var g = Green / 255f;
        var b = Blue / 255f;

        var colorf = new ColorF(r, g, b);
        colorf.ToHsl(out h, out s, out l);
    }

    public readonly void ToHsv(out float h, out float s, out float v)
    {
        // RGB from 0 to 255
        var r = Red / 255f;
        var g = Green / 255f;
        var b = Blue / 255f;

        var colorf = new ColorF(r, g, b);
        colorf.ToHsv(out h, out s, out v);
    }

    public override readonly string ToString() =>
        $"#{Alpha:x2}{Red:x2}{Green:x2}{Blue:x2}";

    public readonly bool Equals(Color obj) =>
        obj.color == color;

    public override readonly bool Equals(object other) =>
        other is Color f && Equals(f);

    public static bool operator ==(Color left, Color right) =>
        left.Equals(right);

    public static bool operator !=(Color left, Color right) =>
        !left.Equals(right);

    public override readonly int GetHashCode() =>
        color.GetHashCode();

    public static implicit operator Color(uint color) =>
        new Color(color);

    public static explicit operator uint(Color color) =>
        color.color;

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

#if NETCOREAPP3_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
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
#else
			// clean up string
			hexString = hexString.Trim ();
			var startIndex = hexString[0] == '#' ? 1 : 0;

			var len = hexString.Length - startIndex;
			if (len == 3 || len == 4) {
				byte a;
				// parse [A]
				if (len == 4) {
					if (!byte.TryParse (string.Concat (new string (hexString[len - 4 + startIndex], 2)), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out a)) {
						// error
						color = SKColor.Empty;
						return false;
					}
				} else {
					a = 255;
				}

				// parse RGB
				if (!byte.TryParse (new string (hexString[len - 3 + startIndex], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r) ||
					!byte.TryParse (new string (hexString[len - 2 + startIndex], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g) ||
					!byte.TryParse (new string (hexString[len - 1 + startIndex], 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b)) {
					// error
					color = SKColor.Empty;
					return false;
				}

				// success
				color = new SKColor (r, g, b, a);
				return true;
			}

			if (len == 6 || len == 8) {
				// parse [AA]RRGGBB
				if (!uint.TryParse (hexString.Substring (startIndex), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var number)) {
					// error
					color = SKColor.Empty;
					return false;
				}

				// success
				color = (SKColor)number;

				// alpha was not provided, so use 255
				if (len == 6) {
					color = color.WithAlpha (255);
				}
				return true;
			}
#endif

        // error
        color = Color.Empty;
        return false;
    }
}