namespace d2;

// sk_color4f_t
[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 16)]
public readonly unsafe partial struct ColorF : IEquatable<ColorF>
{
    private const float EPSILON = 0.001f;

    [InlineArray(4)]
    public struct ColorFloat4Array
    {
        private float _element0;
        //public float this[int i]
        //{
        //    get => this[i];
        //    set => this[i] = value;
        //}
    }
    [FieldOffset(0)]
    private readonly ColorFloat4Array colorChans;
    [FieldOffset(0)]
    private readonly Single red;
    [FieldOffset(1)]
    private readonly Single green;
    [FieldOffset(2)]
    private readonly Single blue;
    [FieldOffset(3)]
    private readonly Single alpha;

    //public readonly byte Alpha => colorChans[3];// (byte)((color >> 24) & 0xff);
    //public readonly byte Red => colorChans[2];// (byte)((color >> 16) & 0xff);
    //public readonly byte Green => colorChans[1]; //(byte)((color >> 8) & 0xff);
    //public readonly byte Blue => colorChans[0]; //(byte)((color) & 0xff);

    public readonly Single R => red;
    public readonly Single G => green;
    public readonly Single B => blue;
    public readonly Single A => alpha;

    public readonly bool Equals(ColorF obj) =>
        R == obj.R && G == obj.G && B == obj.B && A == obj.A;

    public readonly override bool Equals(object obj) =>
        obj is ColorF f && Equals(f);

    public static bool operator ==(ColorF left, ColorF right) =>
        left.Equals(right);

    public static bool operator !=(ColorF left, ColorF right) =>
        !left.Equals(right);

    public readonly override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(R);
        hash.Add(G);
        hash.Add(B);
        hash.Add(A);
        return hash.ToHashCode();
    }

    public ColorF(double r, double g, double b, double a = 1)
    {
        red = (float)r;
        green = (float)g;
        blue = (float)b;
        alpha = (float)a;
    }

    public ColorF(float r, float g, float b, float a = 1f)
    {
        red = r;
        green = g;
        blue = b;
        alpha = a;
    }

    public readonly ColorF WithRed(float r) =>
        new(r, G, B, A);

    public readonly ColorF WithGreen(float g) =>
        new(R, g, B, A);

    public readonly ColorF WithBlue(float b) =>
        new(R, G, b, A);

    public readonly ColorF WithAlpha(float a) =>
        new(R, G, B, a);

    public readonly float Hue
    {
        get
        {
            ToHsv(out var h, out _, out _);
            return h;
        }
    }

    public readonly ColorF Clamp()
    {
        return new ColorF(Clamp(R), Clamp(G), Clamp(B), Clamp(A));

        static float Clamp(float v)
        {
            if (v > 1f)
                return 1f;
            if (v < 0f)
                return 0f;
            return v;
        }
    }

    public static ColorF FromHsl(float h, float s, float l, float a = 1f)
    {
        // convert from percentages
        h = h / 360f;
        s = s / 100f;
        l = l / 100f;

        // RGB results from 0 to 1
        var r = l;
        var g = l;
        var b = l;

        // HSL from 0 to 1
        if (Math.Abs(s) > EPSILON)
        {
            float v2;
            if (l < 0.5f)
                v2 = l * (1f + s);
            else
                v2 = (l + s) - (s * l);

            var v1 = 2f * l - v2;

            r = HueToRgb(v1, v2, h + (1f / 3f));
            g = HueToRgb(v1, v2, h);
            b = HueToRgb(v1, v2, h - (1f / 3f));
        }

        return new ColorF(r, g, b, a);
    }

    private static float HueToRgb(float v1, float v2, float vH)
    {
        if (vH < 0f)
            vH += 1f;
        if (vH > 1f)
            vH -= 1f;

        if ((6f * vH) < 1f)
            return (v1 + (v2 - v1) * 6f * vH);
        if ((2f * vH) < 1f)
            return (v2);
        if ((3f * vH) < 2f)
            return (v1 + (v2 - v1) * ((2f / 3f) - vH) * 6f);
        return (v1);
    }

    public static ColorF FromHsv(float h, float s, float v, float a = 1f)
    {
        // convert from percentages
        h = h / 360f;
        s = s / 100f;
        v = v / 100f;

        // RGB results from 0 to 1
        var r = v;
        var g = v;
        var b = v;

        // HSL from 0 to 1
        if (Math.Abs(s) <= EPSILON)
            return new(r, g, b, a);

        h = h * 6f;
        if (Math.Abs(h - 6f) < EPSILON)
            h = 0f; // H must be < 1

        var hInt = (int)h;
        var v1 = v * (1f - s);
        var v2 = v * (1f - s * (h - hInt));
        var v3 = v * (1f - s * (1f - (h - hInt)));

        if (hInt == 0)
            return new(r: v, g: v3, b: v1, a);
        else if (hInt == 1)
            return new(r: v2, g: v, b: v1, a);
        else if (hInt == 2)
            return new(r: v1, g: v, b: v3, a);
        else if (hInt == 3)
            return new(r: v1, g: v2, b: v, a);
        else if (hInt == 4)
            return new(r: v3, g: v1, b: v, a);
        else
            return new(r: v, g: v1, b: v2, a);

    }

    public readonly void ToHsl(out float h, out float s, out float l)
    {
        // RGB from 0 to 1
        var r = R;
        var g = G;
        var b = B;

        var min = Math.Min(Math.Min(r, g), b); // min value of RGB
        var max = Math.Max(Math.Max(r, g), b); // max value of RGB
        var delta = max - min; // delta RGB value

        // default to a gray, no chroma...
        h = 0f;
        s = 0f;
        l = (max + min) / 2f;

        // chromatic data...
        if (Math.Abs(delta) > EPSILON)
        {
            if (l < 0.5f)
                s = delta / (max + min);
            else
                s = delta / (2f - max - min);

            var deltaR = (((max - r) / 6f) + (delta / 2f)) / delta;
            var deltaG = (((max - g) / 6f) + (delta / 2f)) / delta;
            var deltaB = (((max - b) / 6f) + (delta / 2f)) / delta;

            if (Math.Abs(r - max) < EPSILON) // r == max
                h = deltaB - deltaG;
            else if (Math.Abs(g - max) < EPSILON) // g == max
                h = (1f / 3f) + deltaR - deltaB;
            else // b == max
                h = (2f / 3f) + deltaG - deltaR;

            if (h < 0f)
                h += 1f;
            if (h > 1f)
                h -= 1f;
        }

        // convert to percentages
        h = h * 360f;
        s = s * 100f;
        l = l * 100f;
    }

    public readonly void ToHsv(out float h, out float s, out float v)
    {
        // RGB from 0 to 1
        var r = R;
        var g = G;
        var b = B;

        var min = Math.Min(Math.Min(r, g), b); // min value of RGB
        var max = Math.Max(Math.Max(r, g), b); // max value of RGB
        var delta = max - min; // delta RGB value

        // default to a gray, no chroma...
        h = 0;
        s = 0;
        v = max;

        // chromatic data...
        if (Math.Abs(delta) > EPSILON)
        {
            s = delta / max;

            var deltaR = (((max - r) / 6f) + (delta / 2f)) / delta;
            var deltaG = (((max - g) / 6f) + (delta / 2f)) / delta;
            var deltaB = (((max - b) / 6f) + (delta / 2f)) / delta;

            if (Math.Abs(r - max) < EPSILON) // r == max
                h = deltaB - deltaG;
            else if (Math.Abs(g - max) < EPSILON) // g == max
                h = (1f / 3f) + deltaR - deltaB;
            else // b == max
                h = (2f / 3f) + deltaG - deltaR;

            if (h < 0f)
                h += 1f;
            if (h > 1f)
                h -= 1f;
        }

        // convert to percentages
        h = h * 360f;
        s = s * 100f;
        v = v * 100f;
    }

    public override readonly string ToString() => ((Color)this).ToString();


    public static ColorF FromRGBA(byte r, byte g, byte b, byte a = 255) => new Color(r, g, b, a).ToColorF();

    public static implicit operator ColorF(Color color) => color.ToColorF(); //SkiaApi.sk_color4f_from_color((uint)color, &colorF);

    public static explicit operator Color(ColorF color) => color.ToColor(); //SkiaApi.sk_color4f_to_color(&color);

    public static ColorF FromColor(Color bgra) => bgra.ToColorF();

    public Color ToColor() => new((byte)(R * 255).Clamp(0, 255), (byte)(G * 255).Clamp(0, 255), (byte)(B * 255).Clamp(0, 255), (byte)(A * 255).Clamp(0, 255));

}
