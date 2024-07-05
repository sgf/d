using d2;

namespace d2;
//---------------------------------------------------------------------
// ENDIAN
//---------------------------------------------------------------------
#if IWORDS_BIG_ENDIAN
#define IWORDS_BIG_ENDIAN IPIXEL_BIG_ENDIAN
#endif

//---------------------------------------------------------------------
// misc structures
//---------------------------------------------------------------------

// 采样越界控制
enum IBOM
{
    IBOM_TRANSPARENT = 0,   // 越界时，使用透明色，即 IBITMAP::mask
    IBOM_REPEAT = 1,        // 越界时，重复边界颜色
    IBOM_WRAP = 2,          // 越界时，使用下一行颜色
    IBOM_MIRROR = 3,		// 越界时，使用镜像颜色
};

// 滤波模式
enum IPIXELFILTER
{
    IPIXEL_FILTER_BILINEAR = 0, // 滤波模式：双线性过滤采样
    IPIXEL_FILTER_NEAREST = 1,	// 滤波模式：最近采样
};

// 矩形申明
public struct IRECT
{
    int left;
    int top;
    int right;
    int bottom;
};


// 位图模式
[StructLayout(LayoutKind.Explicit)]
[BitObject(BitOrder.LeastSignificant)]
public unsafe partial  struct IMODE
{

    [FieldOffset(0)]
    [BitField("reserve1", 8, BitFieldType.Byte)]     // 保留
    [BitField("reserve2", 8, BitFieldType.Byte)]     // 保留
    [BitField("pixfmt", 6)]     // 颜色格式
    [BitField("fmtset", 1)]     //是否设置颜色格式
    [BitField("overflow", 2)]   // 越界采样模式，见 IBOM定义
    [BitField("filter", 2)]     // 过滤器
    [BitField("refbits", 1)]    // 是否引用数据
    [BitField("subpixel", 2)]   // 子像素模式
    [BitField(2)]   // padding
    private fixed byte _backingField[4];

    [FieldOffset(0)]
    public uint mode;
}

public const int
 IBLIT_NOCLIP	=16,
 IBLIT_ADDITIVE	=32;

//=====================================================================
// 32位定点数：高16位表示整数，低16位为小数
//=====================================================================
#ifndef CFIXED_16_16
#define CFIXED_16_16

#define cfixed_from_int(i)		(((cfixed)(i)) << 16)
#define cfixed_from_float(x)	((cfixed)((x) * 65536.0f))
#define cfixed_from_double(d)	((cfixed)((d) * 65536.0))
#define cfixed_to_int(f)		((f) >> 16)
#define cfixed_to_float(x)		((float)((x) / 65536.0f))
#define cfixed_to_double(f)		((double)((f) / 65536.0))
#define cfixed_const_1			(cfixed_from_int(1))
#define cfixed_const_e			((cfixed)(1))
#define cfixed_const_1_m_e		(cfixed_const_1 - cfixed_const_e)
#define cfixed_const_half		(cfixed_const_1 >> 1)
#define cfixed_frac(f)			((f) & cfixed_const_1_m_e)
#define cfixed_floor(f)			((f) & (~cfixed_const_1_m_e))
#define cfixed_ceil(f)			(cfixed_floor((f) + 0xffff))
#define cfixed_mul(x, y)		((cfixed)((((long)(x)) * (y)) >> 16))
#define cfixed_div(x, y)		((cfixed)((((long)(x)) << 16) / (y)))
#define cfixed_const_max		((long)0x7fffffff)
#define cfixed_const_min		(-((((long)1) << 31)))

#endif



//=====================================================================
// 扫描线操作
//=====================================================================

// 取得扫描线像素的函数定义
delegate void iBitmapFetchProc(in IBITMAP bmp, uint* card, int w, cfixed* pos, cfixed* step, byte* mask, IRECT* clip);

// 浮点数取得扫描线的函数定义
delegate void iBitmapFetchFloat(in IBITMAP bmp, uint* card, int w, float* pos, float* step, byte* mask, IRECT* clip);


// 取得扫描线的模式
public const int
 IBITMAP_FETCH_GENERAL_NEAREST = 0,
 IBITMAP_FETCH_GENERAL_BILINEAR = 1,
 IBITMAP_FETCH_TRANSLATE_NEAREST = 2,
 IBITMAP_FETCH_TRANSLATE_BILINEAR = 3,
 IBITMAP_FETCH_SCALE_NEAREST = 4,
 IBITMAP_FETCH_SCALE_BILINEAR = 5,
 IBITMAP_FETCH_REPEAT_TRANSLATE_NEAREST = 6,
 IBITMAP_FETCH_REPEAT_TRANSLATE_BILINEAR = 7,
 IBITMAP_FETCH_REPEAT_SCALE_NEAREST = 8,
 IBITMAP_FETCH_REPEAT_SCALE_BILINEAR = 9,
 IBITMAP_FETCH_WRAP_TRANSLATE_NEAREST = 10,
 IBITMAP_FETCH_WRAP_TRANSLATE_BILINEAR = 11,
 IBITMAP_FETCH_WRAP_SCALE_NEAREST = 12,
 IBITMAP_FETCH_WRAP_SCALE_BILINEAR = 13,
 IBITMAP_FETCH_MIRROR_TRANSLATE_NEAREST = 14,
 IBITMAP_FETCH_MIRROR_TRANSLATE_BILINEAR = 15,
 IBITMAP_FETCH_MIRROR_SCALE_NEAREST = 16,
 IBITMAP_FETCH_MIRROR_SCALE_BILINEAR = 17,


    IBITMAP_STACK_BUFFER=	16384;

//---------------------------------------------------------------------
// 通用部分
//---------------------------------------------------------------------
#define ibitmap_imode(bmp, _FIELD) (((IMODE*)&((bmp).mode))._FIELD)

#define ibitmap_imode_const(bmp, _FIELD) \
(((const IMODE*)&((bmp).mode))._FIELD)
    

/* draw pixel list with single color */
delegate void iPixelListSC(ref IBITMAP bmp, uint* xy, int count,
    uint color, int additive);

/* draw pixel list with different color */
delegate void iPixelListMC(ref IBITMAP bmp, uint* xycol, int count,
    int additive);

//=====================================================================
// GLYPH IMPLEMENTATION
//=====================================================================
public struct IGLYPHCHAR
{
    int w;
    int h;
    byte data[1];
}

public struct IGLYPHFONT
{
    int w;
    int h;
    byte* font;
}



//=====================================================================
// 像素合成
//=====================================================================

#define IPIXEL_COMPOSITE_SRC_A8R8G8B8	64
#define IPIXEL_COMPOSITE_DST_A8R8G8B8	128
#define IPIXEL_COMPOSITE_FORCE_32		256



//=====================================================================
// Inline Utilities
//=====================================================================
public static inline IRECT * ipixel_rect_set(IRECT* dst, int l, int t, int r, int b)
{
    dst.left = l;
    dst.top = t;
    dst.right = r;
    dst.bottom = b;
    return dst;
}

public static inline IRECT * ipixel_rect_copy(IRECT* dst, IRECT* src)
{
    dst.left = src.left;
    dst.top = src.top;
    dst.right = src.right;
    dst.bottom = src.bottom;
    return dst;
}

public static inline IRECT * ipixel_rect_offset(IRECT* self, int x, int y)
{
    self.left += x;
    self.top += y;
    self.right += x;
    self.bottom += y;
    return self;
}

public static inline int ipixel_rect_contains(const ref IRECT self, int x, int y)
{
    return (x >= self.left && y >= self.top &&
        x < self.right && y < self.bottom);
}

public static inline int ipixel_rect_intersects(const ref IRECT self, ref IRECT src)
{
    return !((src.right <= self.left) || (src.bottom <= self.top) ||
        (src.left >= self.right) || (src.top >= self.bottom));
}

public static inline IRECT * ipixel_rect_intersection(IRECT* dst, IRECT* src)
{
    int x1 = (dst.left > src.left) ? dst.left : src.left;
    int x2 = (dst.right < src.right) ? dst.right : src.right;
    int y1 = (dst.top > src.top) ? dst.top : src.top;
    int y2 = (dst.bottom < src.bottom) ? dst.bottom : src.bottom;
    if (x1 > x2 || y1 > y2)
    {
        dst.left = 0;
        dst.top = 0;
        dst.right = 0;
        dst.bottom = 0;
    }
    else
    {
        dst.left = x1;
        dst.top = y1;
        dst.right = x2;
        dst.bottom = y2;
    }
    return dst;
}

public static inline IRECT * ipixel_rect_union(IRECT* dst, IRECT* src)
{
    int x1 = (dst.left < src.left) ? dst.left : src.left;
    int x2 = (dst.right > src.right) ? dst.right : src.right;
    int y1 = (dst.top < src.top) ? dst.top : src.top;
    int y2 = (dst.bottom > src.bottom) ? dst.bottom : src.bottom;
    dst.left = x1;
    dst.top = y1;
    dst.right = x2;
    dst.bottom = y2;
    return dst;
}

public static inline cfixed cfixed_abs(cfixed x)
{
    return (x < 0) ? (-x) : x;
}

public static inline cfixed cfixed_min(cfixed x, cfixed y)
{
    return (x < y) ? x : y;
}

public static inline cfixed cfixed_max(cfixed x, cfixed y)
{
    return (x > y) ? x : y;
}

public static inline cfixed cfixed_inverse(cfixed x)
{
    return (cfixed)((((long)cfixed_const_1) * cfixed_const_1) / x);
}

public static inline int cfixed_within_epsilon(cfixed a, cfixed b, cfixed epsilon)
{
    cfixed c = a - b;
    if (c < 0) c = -c;
    return c <= epsilon;
}

public static inline int cfloat_within_epsilon(float a, float b, float epsilon)
{
    float c = a - b;
    if (c < 0) c = -c;
    return c <= epsilon;
}

public static inline int cfloat_to_int_ieee(float f)
{
    union { int intpart; float floatpart; }
    convert;
    int sign, mantissa, exponent, r;
    convert.floatpart = f;
    sign = (convert.intpart >> 31);
    mantissa = (convert.intpart & ((1 << 23) - 1)) | (1 << 23);
    exponent = ((convert.intpart & 0x7ffffffful) >> 23) - 127;
    r = ((uint)(mantissa) << 8) >> (31 - exponent);
    return ((r ^ sign) - sign) & ~(exponent >> 31);
}

public static inline int cfloat_ieee_enable()
{
    return (cfloat_to_int_ieee(1234.56f) == 1234);
}

public static inline cfixed cfixed_from_float_ieee(float f)
{
    return (cfixed)cfloat_to_int_ieee(f * 65536.0f);
}


#define IEPSILON_E4     ((float)1E-4)
#define IEPSILON_E5     ((float)1E-5)
#define IEPSILON_E6     ((float)1E-6)

#define CFIXED_EPSILON  ((cfixed)3)
#define CFLOAT_EPSILON  IEPSILON_E5

#define cfixed_is_same(a, b)  (cfixed_within_epsilon(a, b, CFIXED_EPSILON))
#define cfixed_is_zero(a)     cfixed_is_same(a, 0)
#define cfixed_is_one(a)      cfixed_is_same(a, cfixed_const_1)
#define cfixed_is_int(a)      cfixed_is_same(cfixed_frac(a), 0)

#define cfloat_is_same(a, b)  (cfloat_within_epsilon(a, b, CFLOAT_EPSILON))
#define cfloat_is_zero(a)     cfloat_is_same(a, 0.0f)
#define cfloat_is_one(a)      cfloat_is_same(a, 1.0f)


//=====================================================================
// HSV / HSL Color Space
//=====================================================================

// RGB . HSV
public static inline void ipixel_cvt_rgb_to_hsv(int r, int g, int b,
    float* H, float* S, float* V)
{
    int imin = (r < g) ? ((r < b) ? r : b) : ((g < b) ? g : b);
    int imax = (r > g) ? ((r > b) ? r : b) : ((g > b) ? g : b);
    int idelta = imax - imin;

    *V = imax * (1.0f / 255.0f);

    if (imax == 0)
    {
        *S = *H = 0.0f;
    }
    else
    {
        float h;
        *S = ((float)idelta) / ((float)imax);
        if (r == imax) h = ((float)(g - b)) / idelta;
        else if (g == imax) h = 2.0f + ((float)(b - r)) / idelta;
        else h = 4.0f + ((float)(r - g)) / idelta;
        h *= 60.0f;
        if (h < 0.0f) h += 360.0f;
        *H = h;
    }
}

// HSV . RGB
public static inline uint ipixel_cvt_hsv_to_rgb(float h, float s, float v)
{
    float r, g, b;
    int R, G, B;
    v = (1.0f < v) ? 1.0f : v;
    s = (1.0f < s) ? 1.0f : s;
    r = g = b = v;
    if (s != 0.0f)
    {
        float f, p, q, t;
        int i;
        while (h < 0.0f) h += 360.0f;
        while (h >= 360.0f) h -= 360.0f;
        h *= (1.0f / 60.0f);
        i = (int)h;
        f = h - i;
        p = v * (1.0f - s);
        q = v * (1.0f - s * f);
        t = v * (1.0f - s * (1.0f - f));
        switch (i)
        {
            case 0: r = v; g = t; b = p; break;
            case 1: r = q; g = v; b = p; break;
            case 2: r = p; g = v; b = t; break;
            case 3: r = p; g = q; b = v; break;
            case 4: r = t; g = p; b = v; break;
            case 5: r = v; g = p; b = q; break;
            case 6: r = v; g = t; b = p; break;
        }
    }
    R = (int)(r * 255.0f);
    G = (int)(g * 255.0f);
    B = (int)(b * 255.0f);
    return IRGBA_TO_A8R8G8B8(R, G, B, 0);
}

// RGB . HSL
public static inline void ipixel_cvt_rgb_to_hsl(int r, int g, int b,
    float* H, float* S, float* L)
{
    int minval = (r < g) ? ((r < b) ? r : b) : ((g < b) ? g : b);
    int maxval = (r > g) ? ((r > b) ? r : b) : ((g > b) ? g : b);
    float mdiff = (float)(maxval - minval);
    float msum = (float)(maxval + minval);
    *L = msum * (1.0f / 510.0f);
    if (maxval == minval)
    {
        *S = *H = 0.0f;
    }
    else
    {
        float mdiffinv = 1.0f / mdiff;
        float rnorm = (maxval - r) * mdiffinv;
        float gnorm = (maxval - g) * mdiffinv;
        float bnorm = (maxval - b) * mdiffinv;
        *S = (*L <= 0.5f) ? (mdiff / msum) : (mdiff / (510.0f - msum));
        if (r == maxval) *H = 60.0f * (6.0f + bnorm - gnorm);
        else if (g == maxval) *H = 60.0f * (2.0f + rnorm - bnorm);
        else *H = 60.0f * (4.0f + gnorm - rnorm);
        if (*H > 360.0f) *H -= 360.0f;
    }
}

// HUE . RGB
public static inline int ipixel_cvt_hue_to_rgb(float rm1, float rm2, float rh)
{
    int n;
    while (rh > 360.0f) rh -= 360.0f;
    while (rh < 0.0f) rh += 360.0f;
    if (rh < 60.0f) rm1 = rm1 + (rm2 - rm1) * rh / 60.0f;
    else if (rh < 180.0f) rm1 = rm2;
    else if (rh < 240.0f) rm1 = rm1 + (rm2 - rm1) * (240.0f - rh) / 60.0f;
    n = (int)(rm1 * 255);
    return (n > 255) ? 255 : ((n < 0) ? 0 : n);
}

// HSL . RGB
public static inline uint ipixel_cvt_hsl_to_rgb(float H, float S, float L)
{
    uint r, g, b;
    L = (1.0f < L) ? 1.0f : L;
    S = (1.0f < S) ? 1.0f : S;
    if (S == 0.0)
    {
        r = g = b = (uint)(255 * L);
    }
    else
    {
        float rm1, rm2;
        if (L <= 0.5f) rm2 = L + L * S;
        else rm2 = L + S - L * S;
        rm1 = 2.0f * L - rm2;
        r = ipixel_cvt_hue_to_rgb(rm1, rm2, H + 120.0f);
        g = ipixel_cvt_hue_to_rgb(rm1, rm2, H);
        b = ipixel_cvt_hue_to_rgb(rm1, rm2, H - 120.0f);
    }
    return IRGBA_TO_A8R8G8B8(r, g, b, 0);
}



#ifdef __cplusplus
}
#endif

#endif




//---------------------------------------------------------------------
// 位图操作
//---------------------------------------------------------------------

#define IFLAG_PIXFMT_SET	0x80

// 取得像素格式，见ibmbits.h中的 IPIX_FMT_xxx
int ibitmap_pixfmt_get(in IBITMAP bmp)
{
    IMODE mode;
    mode.mode = bmp.mode;
    if (mode.fmtset == 0) return -1;
    return mode.pixfmt;
}

// 设置像素格式
void ibitmap_pixfmt_set(ref IBITMAP bmp, int pixfmt)
{
    IMODE mode;
    if (ipixelfmt[pixfmt].bpp != (int)bmp.bpp)
    {
        assert(ipixelfmt[pixfmt].bpp == (int)bmp.bpp);
        abort();
    }
    mode.mode = bmp.mode;
    mode.pixfmt = (byte)(pixfmt & 63);
    mode.fmtset = 1;
    bmp.mode = mode.mode;
}

// 取得over flow模式
 IBOM ibitmap_overflow_get(in IBITMAP  bmp)
{
    IMODE mode;
    mode.mode = bmp.mode;
    return ( IBOM)mode.overflow;
}

// 设置over flow模式
void ibitmap_overflow_set(ref IBITMAP bmp,  IBOM of)
{
	IMODE mode;
mode.mode = bmp.mode;
mode.overflow = (byte)(of & 3);
bmp.mode = mode.mode;
}

// 取得颜色索引
iColorIndex* ibitmap_index_get(ref IBITMAP bmp)
{
    return (iColorIndex*)bmp.extra;
}

// 设置颜色索引
void ibitmap_index_set(ref IBITMAP bmp, iColorIndex* index)
{
    bmp.extra = index;
}

// 设置滤波器
void ibitmap_filter_set(ref IBITMAP bmp, IPIXELFILTER filter)
{
    IMODE mode;
    mode.mode = bmp.mode;
    mode.filter = (byte)(filter & 3);
    bmp.mode = mode.mode;
}


// 取得滤波器
IPIXELFILTER ibitmap_filter_get(in IBITMAP bmp)
{
    IMODE mode;
    mode.mode = bmp.mode;
    return (IPIXELFILTER)mode.filter;
}


// BLIT 裁剪
int ibitmap_clipex(ref IBITMAP dst, int* dx, int* dy, in IBITMAP src,
    int* sx, int* sy, int* sw, int* sh, ref IRECT clip, int flags)
{
    int clipdst[4], clipsrc[4], rect[4];
    int retval;
    if (clip == null)
    {
        clipdst[0] = 0;
        clipdst[1] = 0;
        clipdst[2] = (int)dst.w;
        clipdst[3] = (int)dst.h;
    }
    else
    {
        clipdst[0] = clip.left;
        clipdst[1] = clip.top;
        clipdst[2] = clip.right;
        clipdst[3] = clip.bottom;
    }
    clipsrc[0] = 0;
    clipsrc[1] = 0;
    clipsrc[2] = (int)src.w;
    clipsrc[3] = (int)src.h;
    rect[0] = *sx;
    rect[1] = *sy;
    rect[2] = *sx + *sw;
    rect[3] = *sy + *sh;
    retval = ibitmap_clip(clipdst, clipsrc, dx, dy, rect, flags);
    if (retval) return retval;
    *sx = rect[0];
    *sy = rect[1];
    *sw = rect[2] - rect[0];
    *sh = rect[3] - rect[1];
    return 0;
}

// 混色绘制
void ibitmap_blend(IBITMAP* dst, int dx, int dy, in IBITMAP src, int sx,
    int sy, int w, int h, uint color, ref IRECT clip, int flags)
{
    byte _buffer[IBITMAP_STACK_BUFFER];
    byte* buffer = _buffer;
    int operate = IPIXEL_BLEND_OP_BLEND;
    int retval = 0, flip = 0, sfmt, dfmt;
    iColorIndex* sindex;
    iColorIndex* dindex;

    if ((flags & IBLIT_NOCLIP) == 0)
    {
        retval = ibitmap_clipex(dst, &dx, &dy, src, &sx, &sy, &w, &h,
                                clip, flags);
        if (retval) return;
    }

    if (w * 4 > IBITMAP_STACK_BUFFER)
    {
        buffer = (byte*)icmalloc(w * 4);
        if (buffer == null) return;
    }

    if (flags & IBLIT_ADDITIVE)
        operate = IPIXEL_BLEND_OP_ADD;

    if (flags & IBLIT_HFLIP) flip |= IPIXEL_FLIP_HFLIP;
    if (flags & IBLIT_VFLIP) flip |= IPIXEL_FLIP_VFLIP;

    sfmt = ibitmap_pixfmt_guess(src);
    dfmt = ibitmap_pixfmt_guess(dst);

    sindex = (const iColorIndex*)(src.extra);
    dindex = (iColorIndex*)(dst.extra);

    if (sindex == null) sindex = _ipixel_src_index;
    if (dindex == null) dindex = _ipixel_dst_index;

    ipixel_blend(dfmt, dst.line[dy], (int)dst.pitch, dx, sfmt,
        src.line[sy], (int)src.pitch, sx, w, h, color,
        operate, flip, dindex, sindex, buffer);

    if (buffer != _buffer)
    {
        icfree(buffer);
    }
}

// 格式转换,/ 颜色转换
void ibitmap_convert(IBITMAP* dst, int dx, int dy, in IBITMAP src,
    int sx, int sy, int w, int h, ref IRECT clip, int flags)
{
    byte _buffer[IBITMAP_STACK_BUFFER];
    byte* buffer = _buffer;
    int retval = 0, flip = 0, sfmt, dfmt;
    iColorIndex* sindex;
    iColorIndex* dindex;

    if ((flags & IBLIT_NOCLIP) == 0)
    {
        retval = ibitmap_clipex(dst, &dx, &dy, src, &sx, &sy, &w, &h,
                                clip, flags);
        if (retval) return;
    }

    if (flags & IBLIT_HFLIP) flip |= IPIXEL_FLIP_HFLIP;
    if (flags & IBLIT_VFLIP) flip |= IPIXEL_FLIP_VFLIP;

    sfmt = ibitmap_pixfmt_guess(src);
    dfmt = ibitmap_pixfmt_guess(dst);

    sindex = (const iColorIndex*)(src.extra);
    dindex = (iColorIndex*)(dst.extra);

    if (sindex == null) sindex = _ipixel_src_index;
    if (dindex == null) dindex = _ipixel_dst_index;

    // do not need convert
    if (ipixelfmt[dfmt].type != IPIX_FMT_TYPE_INDEX && dfmt == sfmt)
    {
        int newflags = (flags & (IBLIT_HFLIP | IBLIT_VFLIP));
        ibitmap_blit(dst, dx, dy, src, sx, sy, w, h, newflags);
        return;
    }

    if (w * 4 > IBITMAP_STACK_BUFFER)
    {
        buffer = (byte*)icmalloc(w * 4);
        if (buffer == null) return;
    }

    // execute converting
    ipixel_convert(dfmt, dst.line[dy], (int)dst.pitch, dx, sfmt,
        src.line[sy], (int)src.pitch, sx, w, h, 0,
        flip, dindex, sindex, buffer);

    if (buffer != _buffer)
    {
        icfree(buffer);
    }
}


// 猜测颜色格式
int ibitmap_pixfmt_guess(in IBITMAP src)
{
    int fmt;
    fmt = ibitmap_pixfmt_get(src);
    if (fmt < 0)
    {
        if (src.bpp == 8) return IPIX_FMT_C8;
        else if (src.bpp == 15) return IPIX_FMT_X1R5G5B5;
        else if (src.bpp == 16) return IPIX_FMT_R5G6B5;
        else if (src.bpp == 24) return IPIX_FMT_R8G8B8;
        else if (src.bpp == 32) return IPIX_FMT_A8R8G8B8;
    }
    return fmt;
}


// 颜色转换，生成新的IBITMAP
IBITMAP* ibitmap_convfmt(int dfmt, in IBITMAP src, IRGB* spal,
	const IRGB* dpal)
{
    iColorIndex* sindex = null;
    iColorIndex* dindex = null;
    IBITMAP* dst = null;
    int sfmt, i;
    int w, h;

    sfmt = ibitmap_pixfmt_get(src);

    if (sfmt < 0)
    {
        if (src.bpp == 8) sfmt = IPIX_FMT_C8;
        else if (src.bpp == 15) sfmt = IPIX_FMT_X1R5G5B5;
        else if (src.bpp == 16) sfmt = IPIX_FMT_R5G6B5;
        else if (src.bpp == 24) sfmt = IPIX_FMT_R8G8B8;
        else if (src.bpp == 32) sfmt = IPIX_FMT_A8R8G8B8;
        else return null;
    }

    dst = ibitmap_create((int)src.w, (int)src.h, ipixelfmt[dfmt].bpp);
    if (dst == null) return null;

    ibitmap_pixfmt_set(dst, dfmt);

    w = (int)src.w;
    h = (int)src.h;

    if (ipixelfmt[sfmt].type == IPIX_FMT_TYPE_INDEX)
    {
        sindex = (iColorIndex*)icmalloc(sizeof(iColorIndex));
        if (sindex == null)
        {
            ibitmap_release(dst);
            return null;
        }
        if (spal == null) spal = _ipaletted;
        for (i = 0; i < 256; i++)
        {
            sindex.rgba[i] = IRGBA_TO_A8R8G8B8(spal[i].r, spal[i].g,
                spal[i].b, 255);
        }
    }

    if (ipixelfmt[dfmt].type == IPIX_FMT_TYPE_INDEX)
    {
        dindex = (iColorIndex*)icmalloc(sizeof(iColorIndex));
        if (dindex == null)
        {
            if (sindex != null) icfree(sindex);
            ibitmap_release(dst);
            return null;
        }
        if (dpal == null) dpal = _ipaletted;
        ipalette_to_index(dindex, dpal, 256);
    }

    // do not need convert
    if (ipixelfmt[dfmt].type != IPIX_FMT_TYPE_INDEX && dfmt == sfmt)
    {
        ibitmap_blit(dst, 0, 0, src, 0, 0, w, h, 0);
    }
    else
    {
        byte* buffer;
        buffer = (byte*)icmalloc(w * 4);
        if (buffer)
        {
            ipixel_blend(dfmt, dst.line[0], (int)dst.pitch, 0, sfmt,
                src.line[0], (int)src.pitch, 0, w, h, 0xffffffff,
                IPIXEL_BLEND_OP_COPY, 0, dindex, sindex, buffer);
            icfree(buffer);
        }
        else
        {
            ibitmap_release(dst);
            dst = null;
        }
    }

    if (sindex) icfree(sindex);
    if (dindex) icfree(dindex);

    return dst;
}


// 绘制矩形
void ibitmap_rectfill(IBITMAP* dst, int x, int y, int w, int h, uint c)
{
    int pixfmt = ibitmap_pixfmt_guess(dst);
    iHLineDrawProc proc;
    iColorIndex* index;
    proc = ipixel_get_hline_proc(pixfmt, 0, 0);
    index = (iColorIndex*)dst.extra;
    if (x >= (int)dst.w || y >= (int)dst.h) return;
    if (x < 0) w += x, x = 0;
    if (x + w >= (int)dst.w) w = (int)dst.w - x;
    if (w <= 0) return;
    if (y < 0) h += y, y = 0;
    if (y + h >= (int)dst.h) h = (int)dst.h - y;
    if (h <= 0) return;
    for (; h > 0; y++, h--)
    {
        proc(dst.line[y], x, w, c, null, index);
    }
}


// 引用BITMAP
IBITMAP* ibitmap_reference_new(void* ptr, int pitch, int w, int h, int fmt)
{
    IBITMAP* bitmap;
    int i;
    bitmap = (IBITMAP*)icmalloc(sizeof(IBITMAP));
    if (bitmap == null) return null;
    bitmap.pixel = (byte*)ptr;
    bitmap.line = (void**)icmalloc(sizeof(void*) * h);
    if (bitmap.line == null)
    {
        icfree(bitmap);
        return null;
    }
    for (i = 0; i < h; i++)
        bitmap.line[i] = (byte*)ptr + pitch * i;
    bitmap.bpp = ipixelfmt[fmt].bpp;
    bitmap.w = (unsigned)w;
    bitmap.h = (unsigned)h;
    bitmap.pitch = (uint)pitch;
    bitmap.mask = 0;
    bitmap.code = 0;
    bitmap.mode = 0;
    bitmap.extra = 0;
    ibitmap_imode(bitmap, refbits) = 1;
    ibitmap_pixfmt_set(bitmap, fmt);
    return bitmap;
}


// 删除引用
void ibitmap_reference_del(ref IBITMAP bmp)
{
    if (bmp)
    {
        if (ibitmap_imode(bmp, refbits) == 0)
        {
            assert(ibitmap_imode(bmp, refbits) != 0);
            abort();
            return;
        }
        if (bmp.line)
        {
            icfree(bmp.line);
            bmp.line = null;
        }
        bmp.pixel = null;
        bmp.mode = 0;
        icfree(bmp);
    }
}


// 调整引用
void ibitmap_reference_adjust(ref IBITMAP bmp, void* ptr, int pitch)
{
    int i;
    if (bmp.pixel == ptr && bmp.pitch == (uint)pitch)
        return;
    bmp.pixel = ptr;
    bmp.pitch = (uint)pitch;
    for (i = 0; i < (int)bmp.h; i++)
    {
        bmp.line[i] = (byte*)ptr + i * bmp.pitch;
    }
}


// 带遮罩的填充，alpha必须是一个8位格式的alpha位图(IPIX_FMT_A8)
void ibitmap_maskfill(IBITMAP* dst, int dx, int dy, IBITMAP* alpha,
    int sx, int sy, int sw, int sh, uint color, ref IRECT clip)
{
    iHLineDrawProc proc;
    int y;
    assert(alpha.bpp == 8);
    if (alpha.bpp != 8) return;
    if (ibitmap_clipex(dst, &dx, &dy, alpha, &sx, &sy, &sw, &sh,
                        clip, 0) != 0)
    {
        return;
    }
    proc = ipixel_get_hline_proc(ibitmap_pixfmt_guess(dst), 0, 0);
    for (y = 0; y < sh; y++)
    {
        byte* source = (byte*)alpha.line[sy + y] + sx;
        byte* dest = (byte*)dst.line[dy + y];
        proc(dest, dx, sw, color, source, null);
    }
}


// 截取IBITMAP 区域中的部分
IBITMAP* ibitmap_chop(in IBITMAP src, int x, int y, int w, int h)
{
    IBITMAP* bitmap;
    assert(w > 0 && h > 0);
    if (w <= 0 || h <= 0) return null;
    bitmap = ibitmap_create(w, h, src.bpp);
    assert(bitmap);
    if (bitmap == null) return null;
    ibitmap_pixfmt_set(bitmap, ibitmap_pixfmt_guess(src));
    ibitmap_blit(bitmap, 0, 0, src, x, y, w, h, 0);
    return bitmap;
}


//=====================================================================
// 扫描线存储或绘制
//=====================================================================
void ibitmap_scanline_store(ref IBITMAP bmp, int x, int y, int w, uint* card, ref IRECT clip, iStoreProc store)
{
    iColorIndex* index = (const iColorIndex*)bmp.extra;
    if (y < clip.top || y >= clip.bottom || w <= 0) return;
    if (x >= clip.right || x + w <= clip.left) return;
    if (x < clip.left)
    {
        int diff = clip.left - x;
        card += diff;
        x = clip.left;
        w -= diff;
    }
    if (x + w >= clip.right)
    {
        int diff = x + w - clip.right;
        w -= diff;
    }
    if (w < 0) return;
    store(bmp.line[y], card, x, w, index);
}

void ibitmap_scanline_blend(ref IBITMAP bmp, int x, int y, int w, uint* card, byte* cover, ref IRECT clip, iSpanDrawProc span)
{
    iColorIndex* index = (const iColorIndex*)bmp.extra;
    if (y < clip.top || y >= clip.bottom || w <= 0) return;
    if (x >= clip.right || x + w <= clip.left) return;
    if (x < clip.left)
    {
        int diff = clip.left - x;
        card += diff;
        if (cover) cover += diff;
        x = clip.left;
        w -= diff;
    }
    if (x + w >= clip.right)
    {
        int diff = x + w - clip.right;
        w -= diff;
    }
    if (w < 0) return;
    span(bmp.line[y], x, w, card, cover, index);
}


//=====================================================================
// 读点操作
//=====================================================================
public static inline int ipixel_overflow(int* X, int* Y, ref IRECT rect,

     IBOM mode)
{
    int x = *X;
    int y = *Y;
    if (x >= rect.left && y >= rect.top && x < rect.right &&
        y < rect.bottom)
        return 0;
    switch (mode)
    {
        case IBOM_TRANSPARENT:
            return -1;
            break;
        case IBOM_REPEAT:
            x = (x < rect.left) ? rect.left :
                (x >= rect.right) ? (rect.right - 1) : x;
            y = (y < rect.top) ? rect.top :
                (y >= rect.bottom) ? (rect.bottom - 1) : y;
            break;
        case IBOM_WRAP:
            {
                int w = rect.right - rect.left;
                int h = rect.bottom - rect.top;
                x = (x - rect.left) % w;
                y = (y - rect.top) % h;
                if (x < 0) x += w;
                if (y < 0) y += h;
                x += rect.left;
                y += rect.top;
                break;
            }
        case IBOM_MIRROR:
            {
                int w = rect.right - rect.left;
                int h = rect.bottom - rect.top;
                x -= rect.left;
                y -= rect.top;
                if (x < 0) x = (-x) % w;
                else if (x >= w) x = w - 1 - (x % w);
                if (y < 0) y = (-y) % h;
                else if (y >= h) y = h - 1 - (y % h);
                x += rect.left;
                y += rect.top;
                break;
            }
    }
    *X = x;
    *Y = y;
    return 0;
}


public static inline uint ipixel_biline_interp(uint tl, uint tr,
    uint bl, uint br, int distx, int disty)
{
    int distxy, distxiy, distixy, distixiy;
    uint f, r;

    distxy = distx * disty;
    distxiy = (distx << 8) - distxy;	/* distx * (256 - disty) */
    distixy = (disty << 8) - distxy;	/* disty * (256 - distx) */
    distixiy =
    256 * 256 - (disty << 8) -
    (distx << 8) + distxy;		/* (256 - distx) * (256 - disty) */

    /* Blue */
    r = (tl & 0x000000ff) * distixiy + (tr & 0x000000ff) * distxiy
      + (bl & 0x000000ff) * distixy + (br & 0x000000ff) * distxy;

    /* Green */
    f = (tl & 0x0000ff00) * distixiy + (tr & 0x0000ff00) * distxiy
      + (bl & 0x0000ff00) * distixy + (br & 0x0000ff00) * distxy;
    r |= f & 0xff000000;

    tl >>= 16;
    tr >>= 16;
    bl >>= 16;
    br >>= 16;
    r >>= 16;

    /* Red */
    f = (tl & 0x000000ff) * distixiy + (tr & 0x000000ff) * distxiy
      + (bl & 0x000000ff) * distixy + (br & 0x000000ff) * distxy;
    r |= f & 0x00ff0000;

    /* Alpha */
    f = (tl & 0x0000ff00) * distixiy + (tr & 0x0000ff00) * distxiy
      + (bl & 0x0000ff00) * distixy + (br & 0x0000ff00) * distxy;
    r |= f & 0xff000000;

    return r;
}

// 取得over flow模式
public static inline IBOM ibitmap_overflow_get_fast(in IBITMAP  bmp)
{
    IMODE mode;
mode.mode = bmp.mode;
return (IBOM)mode.overflow;
}

public static inline uint ibitmap_fetch_pixel(in IBITMAP bmp,
    int x, int y, ref IRECT clip, iFetchPixelProc get_pixel)
{

    IBOM mode = ibitmap_overflow_get_fast(bmp);
    const iColorIndex* index = (const iColorIndex*)bmp.extra;
    if (ipixel_overflow(&x, &y, clip, mode) != 0)
    {
        return (uint)bmp.mask;
    }
    return get_pixel(bmp.line[y], x, index);
}

public static inline uint ibitmap_fetch_pixel_nearest(in IBITMAP bmp,
    cfixed x, cfixed y, ref IRECT clip, iFetchPixelProc get_pixel)
{

    IBOM mode = ibitmap_overflow_get_fast(bmp);
    const iColorIndex* index = (const iColorIndex*)bmp.extra;
    int x0 = cfixed_to_int(x - cfixed_const_e);
    int y0 = cfixed_to_int(y - cfixed_const_e);
    if (ipixel_overflow(&x0, &y0, clip, mode) != 0)
    {
        return (uint)bmp.mask;
    }
    return get_pixel(bmp.line[y0], x0, index);
}

public static inline uint ibitmap_fetch_pixel_bilinear(in IBITMAP bmp,
    cfixed x, cfixed y, ref IRECT clip, iFetchPixelProc get_pixel)
{
    iColorIndex* index = (const iColorIndex*)bmp.extra;
    IBOM mode = ibitmap_overflow_get_fast(bmp);
    uint c00, c01, c10, c11;
    int distx, disty;
    int x1, y1, x2, y2;
    x1 = x - cfixed_const_1 / 2;
    y1 = y - cfixed_const_1 / 2;
    distx = (x1 >> 8) & 0xff;
    disty = (y1 >> 8) & 0xff;
    x1 = cfixed_to_int(x1);
    y1 = cfixed_to_int(y1);
    x2 = x1 + 1;
    y2 = y1 + 1;
    if (x1 >= clip.left && y1 >= clip.top && x2 < clip.right &&
        y2 < clip.bottom)
    {
        c00 = get_pixel(bmp.line[y1], x1, index);
        c01 = get_pixel(bmp.line[y1], x2, index);
        c10 = get_pixel(bmp.line[y2], x1, index);
        c11 = get_pixel(bmp.line[y2], x2, index);
    }
    else
    {
        if (mode == IBOM_TRANSPARENT)
        {
            if (x1 >= clip.right || y1 >= clip.bottom ||
                x2 < clip.left || y2 < clip.top)
                return (uint)bmp.mask;
        }
        c00 = ibitmap_fetch_pixel(bmp, x1, y1, clip, get_pixel);
        c01 = ibitmap_fetch_pixel(bmp, x2, y1, clip, get_pixel);
        c10 = ibitmap_fetch_pixel(bmp, x1, y1, clip, get_pixel);
        c11 = ibitmap_fetch_pixel(bmp, x2, y1, clip, get_pixel);
    }
    return ipixel_biline_interp(c00, c01, c10, c11, distx, disty);
}




//=====================================================================
// 扫描线获取函数
//=====================================================================
public static iBitmapFetchProc ibitmap_fetch_proc_table[IPIX_FMT_COUNT][18][2];
public static iBitmapFetchFloat ibitmap_fetch_float_table[IPIX_FMT_COUNT][2];

// 清空函数列表
public static void ibitmap_fetch_proc_table_clear(void)
{
    public static int inited = 0;
    if (inited == 0)
    {
        int i, j;
        for (i = 0; i < IPIX_FMT_COUNT; i++)
        {
            for (j = 0; j < 18; j++)
            {
                ibitmap_fetch_proc_table[i][j][0] = null;
                ibitmap_fetch_proc_table[i][j][1] = null;
            }
            ibitmap_fetch_float_table[i][0] = null;
            ibitmap_fetch_float_table[i][1] = null;
        }
        inited = 1;
    }
}


// 取得扫描线函数
iBitmapFetchProc ibitmap_scanline_get_proc(int pixfmt, int mode, int isdef)
{
    iBitmapFetchProc proc;
    public static int inited = 0;

    if (inited == 0)
    {
        ibitmap_fetch_proc_table_init();
        inited = 1;
    }

    assert(pixfmt >= 0 && pixfmt < IPIX_FMT_COUNT && mode >= 0 && mode < 18);

    if (pixfmt < 0 || pixfmt >= IPIX_FMT_COUNT || mode < 0 || mode >= 18)
        return null;

    proc = ibitmap_fetch_proc_table[pixfmt][mode][(isdef) ? 0 : 1];

    if (proc == null)
    {
        return ibitmap_fetch_general;
    }

    return proc;
}

// 取得浮点数扫描线函数
iBitmapFetchFloat ibitmap_scanline_get_float(int pixfmt, int isdefault)
{
    iBitmapFetchFloat proc;
    public static int inited = 0;

    if (inited == 0)
    {
        ibitmap_fetch_proc_table_init();
        inited = 1;
    }

    assert(pixfmt >= 0 && pixfmt < IPIX_FMT_COUNT);

    if (pixfmt < 0 || pixfmt >= IPIX_FMT_COUNT)
        return null;

    proc = ibitmap_fetch_float_table[pixfmt][(isdefault) ? 0 : 1];

    if (proc == null)
    {
        return ibitmap_fetch_general_float;
    }

    return proc;
}


// 设置扫描线函数
void ibitmap_scanline_set_proc(int pixfmt, int mode, iBitmapFetchProc proc)
{
    public static int inited = 0;
    if (inited == 0)
    {
        ibitmap_fetch_proc_table_init();
        inited = 1;
    }
    assert(pixfmt >= 0 && pixfmt < IPIX_FMT_COUNT && mode >= 0 && mode < 18);

    if (pixfmt < 0 || pixfmt >= IPIX_FMT_COUNT || mode < 0 || mode >= 18)
        return;

    ibitmap_fetch_proc_table[pixfmt][mode][1] = proc;
}

// 设置扫描线浮点函数
void ibitmap_scanline_set_float(int pixfmt, iBitmapFetchFloat proc)
{
    public static int inited = 0;
    if (inited == 0)
    {
        ibitmap_fetch_proc_table_init();
        inited = 1;
    }
    assert(pixfmt >= 0 && pixfmt < IPIX_FMT_COUNT);

    if (pixfmt < 0 || pixfmt >= IPIX_FMT_COUNT)
        return;

    ibitmap_fetch_float_table[pixfmt][1] = proc;
}


// 取得模式
int ibitmap_scanline_get_mode(in IBITMAP bmp, cfixed* src,
	const cfixed* step)
{
    IPIXELFILTER filter;
    IBOM overflow;
    int basetype;
    int index;

    overflow = (IBOM)ibitmap_imode_const(bmp, overflow);
    filter = (IPIXELFILTER)ibitmap_imode_const(bmp, filter);

    if (src[2] != cfixed_const_1 || step[1] != 0 || step[2] != 0)
    {
        if (filter == IPIXEL_FILTER_BILINEAR)
            return IBITMAP_FETCH_GENERAL_BILINEAR;
        return IBITMAP_FETCH_GENERAL_NEAREST;
    }

    basetype = 1;

    switch (overflow)
    {
        case IBOM_TRANSPARENT: index = IBITMAP_FETCH_TRANSLATE_NEAREST; break;
        case IBOM_REPEAT: index = IBITMAP_FETCH_REPEAT_TRANSLATE_NEAREST; break;
        case IBOM_WRAP: index = IBITMAP_FETCH_WRAP_TRANSLATE_NEAREST; break;
        case IBOM_MIRROR: index = IBITMAP_FETCH_MIRROR_TRANSLATE_NEAREST; break;
        default:
            if (filter == IPIXEL_FILTER_BILINEAR)
                return IBITMAP_FETCH_GENERAL_BILINEAR;
            return IBITMAP_FETCH_GENERAL_NEAREST;
    }

    index += basetype * 2;

    if (filter == IPIXEL_FILTER_BILINEAR) return index + 1;

    return index;
}


// 通用取扫描线
public static void ibitmap_fetch_general(in IBITMAP bmp, uint* card,
    int width, cfixed* source, cfixed* step,
     byte* mask, ref IRECT clip)
{
    cfixed u, v, w, du, dv, dw, x, y;
    iFetchPixelProc proc;
    IPIXELFILTER filter;
    IBOM mode;

    u = source[0];
    v = source[1];
    w = source[2];
    du = step[0];
    dv = step[1];
    dw = step[2];

    mode = ibitmap_overflow_get_fast(bmp);
    filter = (IPIXELFILTER)ibitmap_imode_const(bmp, filter);
    proc = ipixel_get_fetchpixel(ibitmap_imode_const(bmp, pixfmt), 0);

    if (filter == IPIXEL_FILTER_BILINEAR)
    {
        if (w == cfixed_const_1 && dw == 0)
        {
            if (mask == null)
            {
                for (; width > 0; u += du, v += dv, card++, width--)
                {
                    *card = ibitmap_fetch_pixel_bilinear(bmp,
                        u, v, clip, proc);
                }
            }
            else
            {
                for (; width > 0; u += du, v += dv, card++, width--)
                {
                    if (*mask++)
                    {
                        *card = ibitmap_fetch_pixel_bilinear(bmp,
                            u, v, clip, proc);
                    }
                }
            }
        }
        else
        {
            if (mask == null)
            {
                for (; width > 0; u += du, v += dv, w += dw, width--)
                {
                    if (w != 0)
                    {
                        x = cfixed_div(u, w);
                        y = cfixed_div(v, w);
                    }
                    else
                    {
                        x = 0, y = 0;
                    }
                    *card = ibitmap_fetch_pixel_bilinear(bmp,
                        x, y, clip, proc);
                    card++;
                }
            }
            else
            {
                for (; width > 0; u += du, v += dv, w += dw, width--)
                {
                    if (*mask++)
                    {
                        if (w != 0)
                        {
                            x = cfixed_div(u, w);
                            y = cfixed_div(v, w);
                        }
                        else
                        {
                            x = 0, y = 0;
                        }
                        *card = ibitmap_fetch_pixel_bilinear(bmp,
                            x, y, clip, proc);
                    }
                    card++;
                }
            }
        }
    }
    else if (filter == IPIXEL_FILTER_NEAREST)
    {
        if (w == cfixed_const_1 && dw == 0)
        {
            if (mask == null)
            {
                for (; width > 0; u += du, v += dv, card++, width--)
                {
                    *card = ibitmap_fetch_pixel_nearest(bmp, u, v,
                        clip, proc);
                }
            }
            else
            {
                for (; width > 0; u += du, v += dv, card++, width--)
                {
                    if (*mask++)
                    {
                        *card = ibitmap_fetch_pixel_nearest(bmp, u, v,
                            clip, proc);
                    }
                }
            }
        }
        else
        {
            if (mask == null)
            {
                for (; width > 0; u += du, v += dv, w += dw, width--)
                {
                    if (w != 0)
                    {
                        x = cfixed_div(u, w);
                        y = cfixed_div(v, w);
                    }
                    else
                    {
                        x = 0, y = 0;
                    }
                    *card = ibitmap_fetch_pixel_nearest(bmp, x, y,
                        clip, proc);
                    card++;
                }
            }
            else
            {
                for (; width > 0; u += du, v += dv, w += dw, width--)
                {
                    if (*mask++)
                    {
                        if (w != 0)
                        {
                            x = cfixed_div(u, w);
                            y = cfixed_div(v, w);
                        }
                        else
                        {
                            x = 0, y = 0;
                        }
                        *card = ibitmap_fetch_pixel_nearest(bmp, x, y,
                            clip, proc);
                    }
                    card++;
                }
            }
        }
    }
}

...这个貌似是通过宏批量生成的
// 通用读取扫描线
public static void ibitmap_fetch_general(in IBITMAP bmp, uint* card,
    int w, cfixed* source, cfixed* step, byte* mask,
	const ref IRECT clip);


// 通用取扫描线
public static void ibitmap_fetch_general_float(in IBITMAP bmp, uint* card,
    int width, float* source, float* step, byte* mask,
	const ref IRECT clip)
{
    float u, v, w, du, dv, dw, iw;
    iFetchPixelProc proc;
    IPIXELFILTER filter;
    cfixed x, y;
    IBOM mode;

    u = source[0];
    v = source[1];
    w = source[2];
    du = step[0];
    dv = step[1];
    dw = step[2];

    mode = ibitmap_overflow_get_fast(bmp);
    filter = (IPIXELFILTER)ibitmap_imode_const(bmp, filter);
    proc = ipixel_get_fetchpixel(ibitmap_imode_const(bmp, pixfmt), 0);

    if (cfloat_ieee_enable())
    {
        if (filter == IPIXEL_FILTER_BILINEAR)
        {
            if (w == 1.0f && dw == 0.0f)
            {
                if (mask == null)
                {
                    for (; width > 0; u += du, v += dv, card++, width--)
                    {
                        x = cfixed_from_float_ieee(u);
                        y = cfixed_from_float_ieee(v);
                        *card = ibitmap_fetch_pixel_bilinear(bmp,
                            x, y, clip, proc);
                    }
                }
                else
                {
                    for (; width > 0; u += du, v += dv, card++, width--)
                    {
                        if (*mask++)
                        {
                            x = cfixed_from_float_ieee(u);
                            y = cfixed_from_float_ieee(v);
                            *card = ibitmap_fetch_pixel_bilinear(bmp,
                                x, y, clip, proc);
                        }
                    }
                }
            }
            else
            {
                iw = (w == 0.0f) ? 0.0f : (65536.0f / w);
                for (; width > 0; width--)
                {
                    x = (cfixed)cfloat_to_int_ieee(u * iw);
                    y = (cfixed)cfloat_to_int_ieee(v * iw);
                    u += du, v += dv, w += dw;
                    iw = (w == 0.0f) ? 0.0f : (65536.0f / w);
                    *card = ibitmap_fetch_pixel_bilinear(bmp, x, y, clip,
                        proc);
                    card++;
                }
            }
        }
        else if (filter == IPIXEL_FILTER_NEAREST)
        {
            if (w == 1.0f && dw == 0.0f)
            {
                if (mask == null)
                {
                    for (; width > 0; u += du, v += dv, card++, width--)
                    {
                        x = cfixed_from_float_ieee(u);
                        y = cfixed_from_float_ieee(v);
                        *card = ibitmap_fetch_pixel_nearest(bmp, x, y,
                            clip, proc);
                    }
                }
                else
                {
                    for (; width > 0; u += du, v += dv, card++, width--)
                    {
                        if (*mask++)
                        {
                            x = cfixed_from_float_ieee(u);
                            y = cfixed_from_float_ieee(v);
                            *card = ibitmap_fetch_pixel_nearest(bmp, x, y,
                                clip, proc);
                        }
                    }
                }
            }
            else
            {
                iw = (w == 0.0f) ? 0.0f : (65536.0f / w);
                for (; width > 0; width--)
                {
                    x = (cfixed)cfloat_to_int_ieee(u * iw);
                    y = (cfixed)cfloat_to_int_ieee(v * iw);
                    u += du, v += dv, w += dw;
                    iw = (w == 0.0f) ? 0.0f : (65536.0f / w);
                    *card = ibitmap_fetch_pixel_nearest(bmp, x, y, clip,
                        proc);
                    card++;
                }
            }
        }
        return;
    }

    if (filter == IPIXEL_FILTER_BILINEAR)
    {
        if (w == 1.0f && dw == 0.0f)
        {
            if (mask == null)
            {
                for (; width > 0; u += du, v += dv, card++, width--)
                {
                    x = cfixed_from_float(u);
                    y = cfixed_from_float(v);
                    *card = ibitmap_fetch_pixel_bilinear(bmp, x, y,
                        clip, proc);
                }
            }
            else
            {
                for (; width > 0; u += du, v += dv, card++, width--)
                {
                    if (*mask++)
                    {
                        x = cfixed_from_float(u);
                        y = cfixed_from_float(v);
                        *card = ibitmap_fetch_pixel_bilinear(bmp, x, y,
                            clip, proc);
                    }
                }
            }
        }
        else
        {
            iw = (w == 0.0f) ? 0.0f : (65536.0f / w);
            for (; width > 0; u += du, v += dv, w += dw, card++, width--)
            {
                x = (cfixed)(u * iw);
                y = (cfixed)(v * iw);
                u += du, v += dv, w += dw;
                iw = (w == 0.0f) ? 0.0f : (65536.0f / w);
                *card = ibitmap_fetch_pixel_bilinear(bmp, x, y, clip, proc);
            }
        }
    }
    else if (filter == IPIXEL_FILTER_NEAREST)
    {
        if (w == 1.0f && dw == 0.0f)
        {
            if (mask == null)
            {
                for (; width > 0; u += du, v += dv, card++, width--)
                {
                    x = cfixed_from_float(u);
                    y = cfixed_from_float(v);
                    *card = ibitmap_fetch_pixel_nearest(bmp, x, y,
                        clip, proc);
                }
            }
            else
            {
                for (; width > 0; u += du, v += dv, card++, width--)
                {
                    if (*mask++)
                    {
                        x = cfixed_from_float(u);
                        y = cfixed_from_float(v);
                        *card = ibitmap_fetch_pixel_nearest(bmp, x, y,
                            clip, proc);
                    }
                }
            }
        }
        else
        {
            iw = (w == 0.0f) ? 0.0f : (65536.0f / w);
            for (; width > 0; u += du, v += dv, w += dw, card++, width--)
            {
                x = (cfixed)(u * iw);
                y = (cfixed)(v * iw);
                u += du, v += dv, w += dw;
                iw = (w == 0.0f) ? 0.0f : (65536.0f / w);
                *card = ibitmap_fetch_pixel_nearest(bmp, x, y, clip, proc);
            }
        }
    }
}


// 通用取得浮点扫描线
void ibitmap_scanline_float(in IBITMAP bmp, uint* card, int w,
	const float* srcvec, float* stepvec, byte* mask,
	const ref IRECT clip)
{
    iBitmapFetchFloat fscanline;
    fscanline = ibitmap_scanline_get_float(ibitmap_pixfmt_guess(bmp), 0);
    fscanline(bmp, card, w, srcvec, stepvec, mask, clip);
}

// 通用取得描线：使用浮点数内核的定点数接口
void ibitmap_scanline_fixed(in IBITMAP bmp, uint* card, int w,
	const cfixed* srcvec, cfixed* stepvec, byte* mask,
	const ref IRECT clip)
{
    float src[3], step[3];
    src[0] = cfixed_to_float(srcvec[0]);
    src[1] = cfixed_to_float(srcvec[1]);
    src[2] = cfixed_to_float(srcvec[2]);
    step[0] = cfixed_to_float(stepvec[0]);
    step[1] = cfixed_to_float(stepvec[1]);
    step[2] = cfixed_to_float(stepvec[2]);
    ibitmap_scanline_float(bmp, card, w, src, step, mask, clip);
}


//---------------------------------------------------------------------
// 快速取点函数
//---------------------------------------------------------------------

// 读取像素
public static inline uint ibitmap_fetch_pixel_int_A8R8G8B8(
    in IBITMAP bmp, int x, int y, ref IRECT clip)
{

     IBOM mode = ibitmap_overflow_get_fast(bmp);
if (ipixel_overflow(&x, &y, clip, mode) != 0)
{
    return (uint)bmp.mask;
}
return ((uint*)bmp.line[y])[x];
}

// 读取像素
public static inline uint ibitmap_fetch_pixel_nearest_A8R8G8B8(
    in IBITMAP bmp, cfixed x, cfixed y, ref IRECT clip)
{

     IBOM mode = ibitmap_overflow_get_fast(bmp);
int x0 = cfixed_to_int(x - cfixed_const_e);
int y0 = cfixed_to_int(y - cfixed_const_e);
if (ipixel_overflow(&x0, &y0, clip, mode) != 0)
{
    return (uint)bmp.mask;
}
return ((uint*)bmp.line[y0])[x0];
} 

// 读取像素
public static inline uint ibitmap_fetch_pixel_int_X8R8G8B8(
    in IBITMAP bmp, int x, int y, ref IRECT clip)
{

     IBOM mode = ibitmap_overflow_get_fast(bmp);
if (ipixel_overflow(&x, &y, clip, mode) != 0)
{
    return (uint)bmp.mask;
}
return ((uint*)bmp.line[y])[x] | 0xff000000;
}

// 读取像素
public static inline uint ibitmap_fetch_pixel_nearest_X8R8G8B8(
    in IBITMAP bmp, cfixed x, cfixed y, ref IRECT clip)
{

     IBOM mode = ibitmap_overflow_get_fast(bmp);
int x0 = cfixed_to_int(x - cfixed_const_e);
int y0 = cfixed_to_int(y - cfixed_const_e);
if (ipixel_overflow(&x0, &y0, clip, mode) != 0)
{
    return (uint)bmp.mask;
}
return ((uint*)bmp.line[y0])[x0] | 0xff000000;
} 

// 读取像素
public static inline uint ibitmap_fetch_pixel_int_R8G8B8(
    in IBITMAP bmp, int x, int y, ref IRECT clip)
{

     IBOM mode = ibitmap_overflow_get_fast(bmp);
if (ipixel_overflow(&x, &y, clip, mode) != 0)
{
    return (uint)bmp.mask;
}
return _ipixel_fetch(24, bmp.line[y], x) | 0xff000000;
}

// 读取像素
public static inline uint ibitmap_fetch_pixel_nearest_R8G8B8(
    in IBITMAP bmp, cfixed x, cfixed y, ref IRECT clip)
{

    IBOM mode = ibitmap_overflow_get_fast(bmp);
int x0 = cfixed_to_int(x - cfixed_const_e);
int y0 = cfixed_to_int(y - cfixed_const_e);
if (ipixel_overflow(&x0, &y0, clip, mode) != 0)
{
    return (uint)bmp.mask;
}
return _ipixel_fetch(24, bmp.line[y0], x0) | 0xff000000;
} 

public static inline uint ibitmap_fetch_pixel_bilinear_A8R8G8B8(
    in IBITMAP bmp, cfixed x, cfixed y, ref IRECT clip)
{
    IBOM mode = ibitmap_overflow_get_fast(bmp);
    uint c00, c01, c10, c11;
    int distx, disty;
    int x1, y1, x2, y2;
    x1 = x - cfixed_const_1 / 2;
    y1 = y - cfixed_const_1 / 2;
    distx = (x1 >> 8) & 0xff;
    disty = (y1 >> 8) & 0xff;
    x1 = cfixed_to_int(x1);
    y1 = cfixed_to_int(y1);
    x2 = x1 + 1;
    y2 = y1 + 1;
    if (x1 >= clip.left && y1 >= clip.top && x2 < clip.right &&
        y2 < clip.bottom)
    {
        byte* ptr1 = (byte*)bmp.line[y1];
        byte* ptr2 = (byte*)bmp.line[y2];
        c00 = _ipixel_fetch(32, ptr1, x1);
        c01 = _ipixel_fetch(32, ptr1, x2);
        c10 = _ipixel_fetch(32, ptr2, x1);
        c11 = _ipixel_fetch(32, ptr2, x2);
    }
    else
    {
        if (mode == IBOM_TRANSPARENT)
        {
            if (x1 >= clip.right || y1 >= clip.bottom ||
                x2 < clip.left || y2 < clip.top)
                return (uint)bmp.mask;
        }
        c00 = ibitmap_fetch_pixel_int_A8R8G8B8(bmp, x1, y1, clip);
        c01 = ibitmap_fetch_pixel_int_A8R8G8B8(bmp, x2, y1, clip);
        c10 = ibitmap_fetch_pixel_int_A8R8G8B8(bmp, x1, y1, clip);
        c11 = ibitmap_fetch_pixel_int_A8R8G8B8(bmp, x2, y1, clip);
    }
    return ipixel_biline_interp(c00, c01, c10, c11, distx, disty);
}

public static inline uint ibitmap_fetch_pixel_bilinear_X8R8G8B8(
    in IBITMAP bmp, cfixed x, cfixed y, ref IRECT clip)
{
    IBOM mode = ibitmap_overflow_get_fast(bmp);
    uint c00, c01, c10, c11;
    int distx, disty;
    int x1, y1, x2, y2;
    x1 = x - cfixed_const_1 / 2;
    y1 = y - cfixed_const_1 / 2;
    distx = (x1 >> 8) & 0xff;
    disty = (y1 >> 8) & 0xff;
    x1 = cfixed_to_int(x1);
    y1 = cfixed_to_int(y1);
    x2 = x1 + 1;
    y2 = y1 + 1;
    if (x1 >= clip.left && y1 >= clip.top && x2 < clip.right &&
        y2 < clip.bottom)
    {
        byte* ptr1 = (byte*)bmp.line[y1];
        byte* ptr2 = (byte*)bmp.line[y2];
        c00 = _ipixel_fetch(32, ptr1, x1);
        c01 = _ipixel_fetch(32, ptr1, x2);
        c10 = _ipixel_fetch(32, ptr2, x1);
        c11 = _ipixel_fetch(32, ptr2, x2);
        return ipixel_biline_interp(c00, c01, c10, c11, distx, disty) |
            0xff000000;
    }
    else
    {
        if (mode == IBOM_TRANSPARENT)
        {
            if (x1 >= clip.right || y1 >= clip.bottom ||
                x2 < clip.left || y2 < clip.top)
                return (uint)bmp.mask;
        }
        c00 = ibitmap_fetch_pixel_int_X8R8G8B8(bmp, x1, y1, clip);
        c01 = ibitmap_fetch_pixel_int_X8R8G8B8(bmp, x2, y1, clip);
        c10 = ibitmap_fetch_pixel_int_X8R8G8B8(bmp, x1, y1, clip);
        c11 = ibitmap_fetch_pixel_int_X8R8G8B8(bmp, x2, y1, clip);
    }
    return ipixel_biline_interp(c00, c01, c10, c11, distx, disty);
}


public static inline uint ibitmap_fetch_pixel_bilinear_R8G8B8(
    in IBITMAP bmp, cfixed x, cfixed y, ref IRECT clip)
{
    IBOM mode = ibitmap_overflow_get_fast(bmp);
    uint c00, c01, c10, c11;
    int distx, disty;
    int x1, y1, x2, y2;
    x1 = x - cfixed_const_1 / 2;
    y1 = y - cfixed_const_1 / 2;
    distx = (x1 >> 8) & 0xff;
    disty = (y1 >> 8) & 0xff;
    x1 = cfixed_to_int(x1);
    y1 = cfixed_to_int(y1);
    x2 = x1 + 1;
    y2 = y1 + 1;
    if (x1 >= clip.left && y1 >= clip.top && x2 < clip.right &&
        y2 < clip.bottom)
    {
        byte* ptr1 = (byte*)bmp.line[y1];
        byte* ptr2 = (byte*)bmp.line[y2];
        c00 = _ipixel_fetch(24, ptr1, x1);
        c01 = _ipixel_fetch(24, ptr1, x2);
        c10 = _ipixel_fetch(24, ptr2, x1);
        c11 = _ipixel_fetch(24, ptr2, x2);
        return ipixel_biline_interp(c00, c01, c10, c11, distx, disty) |
            0xff000000;
    }
    else
    {
        if (mode == IBOM_TRANSPARENT)
        {
            if (x1 >= clip.right || y1 >= clip.bottom ||
                x2 < clip.left || y2 < clip.top)
                return (uint)bmp.mask;
        }
        c00 = ibitmap_fetch_pixel_int_R8G8B8(bmp, x1, y1, clip);
        c01 = ibitmap_fetch_pixel_int_R8G8B8(bmp, x2, y1, clip);
        c10 = ibitmap_fetch_pixel_int_R8G8B8(bmp, x1, y1, clip);
        c11 = ibitmap_fetch_pixel_int_R8G8B8(bmp, x2, y1, clip);
    }
    return ipixel_biline_interp(c00, c01, c10, c11, distx, disty);
}

//---------------------------------------------------------------------
// 通用读点模板
//---------------------------------------------------------------------

// 通用读点的模板
#define IBITMAP_FETCH_PIXEL(fmt, bpp) \
public static inline uint ibitmap_fetch_pixel_int_##fmt( \
	in IBITMAP  bmp, int x, int y, IRECT *clip) \
{ \

    IBOM mode = ibitmap_overflow_get_fast(bmp); \
	const iColorIndex* _ipixel_src_index = (const iColorIndex*)bmp.extra; \
	uint c; \
	if (ipixel_overflow(&x, &y, clip, mode) != 0)
{ \
		return (uint)bmp.mask; \
	} \
	c = _ipixel_fetch(bpp, bmp.line[y], x); \
	_ipixel_src_index = _ipixel_src_index; \
	return IPIXEL_FROM(fmt, c); \
} \
public static inline uint ibitmap_fetch_pixel_nearest_##fmt( \
	in IBITMAP  bmp, cfixed x, cfixed y, IRECT *clip) \
{ \

    IBOM mode = ibitmap_overflow_get_fast(bmp); \
	const iColorIndex* _ipixel_src_index = (const iColorIndex*)bmp.extra; \
	int x0 = cfixed_to_int(x - cfixed_const_e); \
	int y0 = cfixed_to_int(y - cfixed_const_e); \
	uint c; \
	if (ipixel_overflow(&x0, &y0, clip, mode) != 0)
{ \
		return (uint)bmp.mask; \
	} \
	c = _ipixel_fetch(bpp, bmp.line[y0], x0); \
	_ipixel_src_index = _ipixel_src_index; \
	return IPIXEL_FROM(fmt, c); \
} \
public static inline uint ibitmap_fetch_pixel_bilinear_##fmt( \
	in IBITMAP  bmp, cfixed x, cfixed y, IRECT *clip) \
{ \
	const iColorIndex *_ipixel_src_index = (const iColorIndex*)bmp.extra; \
	IBOM mode = ibitmap_overflow_get_fast(bmp); \
	uint c00, c01, c10, c11; \
	int distx, disty; \
	int x1, y1, x2, y2; \
	x1 = x - cfixed_const_1 / 2; \
	y1 = y - cfixed_const_1 / 2; \
	distx = (x1 >> 8) & 0xff; \
	disty = (y1 >> 8) & 0xff; \
	x1 = cfixed_to_int(x1); \
	y1 = cfixed_to_int(y1); \
	x2 = x1 + 1; \
	y2 = y1 + 1; \
	if (x1 >= clip.left && y1 >= clip.top && x2 < clip.right && \
		y2 < clip.bottom) { \
		c00 = _ipixel_fetch(bpp, bmp.line[y1], x1); \
		c01 = _ipixel_fetch(bpp, bmp.line[y1], x2); \
		c10 = _ipixel_fetch(bpp, bmp.line[y2], x1); \
		c11 = _ipixel_fetch(bpp, bmp.line[y2], x2); \
		c00 = IPIXEL_FROM(fmt, c00); \
		c01 = IPIXEL_FROM(fmt, c01); \
		c10 = IPIXEL_FROM(fmt, c10); \
		c11 = IPIXEL_FROM(fmt, c11); \
	}	\

    else
{ \
		if (mode == IBOM_TRANSPARENT)
    { \
			if (x1 >= clip.right || y1 >= clip.bottom || \
				x2 < clip.left || y2 < clip.top) \
				return (uint)bmp.mask; \
		} \
		c00 = ibitmap_fetch_pixel_int_##fmt(bmp, x1, y1, clip); \
		c01 = ibitmap_fetch_pixel_int_##fmt(bmp, x2, y1, clip); \
		c10 = ibitmap_fetch_pixel_int_##fmt(bmp, x1, y1, clip); \
		c11 = ibitmap_fetch_pixel_int_##fmt(bmp, x2, y1, clip); \
	} \
	_ipixel_src_index = _ipixel_src_index; \
	return ipixel_biline_interp(c00, c01, c10, c11, distx, disty); \
}


// 通用取扫描线模板
#define IBITMAP_FETCH_GENERAL(fmt) \
public static void ibitmap_fetch_general_##fmt(in IBITMAP  bmp, uint *card, \
	int width, cfixed *source, cfixed *step, byte *mask,\
	const IRECT *clip) \
{ \
	cfixed u, v, w, du, dv, dw, x, y; \
	IPIXELFILTER filter; \
	IBOM mode; \
	u = source[0]; \
	v = source[1]; \
	w = source[2]; \
	du = step[0]; \
	dv = step[1]; \
	dw = step[2]; \
	mode = ibitmap_overflow_get_fast(bmp); \
	filter = (IPIXELFILTER)ibitmap_imode_const(bmp, filter); \
	if (filter == IPIXEL_FILTER_BILINEAR)
{ \
		if (w == cfixed_const_1 && dw == 0)
    { \
			for (; width > 0; u += du, v += dv, card++, width--)
        { \
				*card = ibitmap_fetch_pixel_bilinear_##fmt(	\
							bmp, u, v, clip); \
			} \
		}
else
{ \
			for (; width > 0; u += du, v += dv, w += dw, card++, width--)
    { \
				if (w != 0)
        { \
					x = cfixed_div(u, w); \
					y = cfixed_div(v, w); \
				}
        else
        {  \
					x = 0, y = 0; \
				} \
				*card = ibitmap_fetch_pixel_bilinear_##fmt(bmp, x, y, clip);\
			} \
		} \
	} \

    else if (filter == IPIXEL_FILTER_NEAREST)
{ \
		if (w == cfixed_const_1 && dw == 0)
    { \
			for (; width > 0; u += du, v += dv, card++, width--)
        { \
				*card = ibitmap_fetch_pixel_nearest_##fmt( \
							bmp, u, v, clip); \
			} \
		}
else
{ \
			for (; width > 0; u += du, v += dv, w += dw, card++, width--)
    { \
				if (w != 0)
        { \
					x = cfixed_div(u, w); \
					y = cfixed_div(v, w); \
				}
        else
        {  \
					x = 0, y = 0; \
				} \
				*card = ibitmap_fetch_pixel_nearest_##fmt(bmp, x, y, clip); \
			} \
		} \
	} \
}

IBITMAP_FETCH_PIXEL(A8B8G8R8, 32);
IBITMAP_FETCH_PIXEL(R8G8B8A8, 32);
IBITMAP_FETCH_PIXEL(B8G8R8A8, 32);
IBITMAP_FETCH_PIXEL(X8B8G8R8, 32);
IBITMAP_FETCH_PIXEL(R8G8B8X8, 32);
IBITMAP_FETCH_PIXEL(B8G8R8X8, 32);
IBITMAP_FETCH_PIXEL(B8G8R8, 24);
IBITMAP_FETCH_PIXEL(R5G6B5, 16);
IBITMAP_FETCH_PIXEL(B5G6R5, 16);
IBITMAP_FETCH_PIXEL(X1R5G5B5, 16);
IBITMAP_FETCH_PIXEL(X1B5G5R5, 16);
IBITMAP_FETCH_PIXEL(R5G5B5X1, 16);
IBITMAP_FETCH_PIXEL(B5G5R5X1, 16);
IBITMAP_FETCH_PIXEL(A1R5G5B5, 16);
IBITMAP_FETCH_PIXEL(A1B5G5R5, 16);
IBITMAP_FETCH_PIXEL(R5G5B5A1, 16);
IBITMAP_FETCH_PIXEL(B5G5R5A1, 16);
IBITMAP_FETCH_PIXEL(A4R4G4B4, 16);
IBITMAP_FETCH_PIXEL(A4B4G4R4, 16);
IBITMAP_FETCH_PIXEL(R4G4B4A4, 16);
IBITMAP_FETCH_PIXEL(B4G4R4A4, 16);
IBITMAP_FETCH_PIXEL(C8, 8);
IBITMAP_FETCH_PIXEL(A8, 8);
IBITMAP_FETCH_PIXEL(G8, 8);

IBITMAP_FETCH_GENERAL(A8R8G8B8);
IBITMAP_FETCH_GENERAL(A8B8G8R8);
IBITMAP_FETCH_GENERAL(R8G8B8A8);
IBITMAP_FETCH_GENERAL(B8G8R8A8);
IBITMAP_FETCH_GENERAL(X8R8G8B8);
IBITMAP_FETCH_GENERAL(X8B8G8R8);
IBITMAP_FETCH_GENERAL(R8G8B8X8);
IBITMAP_FETCH_GENERAL(B8G8R8X8);
IBITMAP_FETCH_GENERAL(R8G8B8);
IBITMAP_FETCH_GENERAL(B8G8R8);
IBITMAP_FETCH_GENERAL(R5G6B5);
IBITMAP_FETCH_GENERAL(B5G6R5);
IBITMAP_FETCH_GENERAL(X1R5G5B5);
IBITMAP_FETCH_GENERAL(X1B5G5R5);
IBITMAP_FETCH_GENERAL(R5G5B5X1);
IBITMAP_FETCH_GENERAL(B5G5R5X1);
IBITMAP_FETCH_GENERAL(A1R5G5B5);
IBITMAP_FETCH_GENERAL(A1B5G5R5);
IBITMAP_FETCH_GENERAL(R5G5B5A1);
IBITMAP_FETCH_GENERAL(B5G5R5A1);
IBITMAP_FETCH_GENERAL(A4R4G4B4);
IBITMAP_FETCH_GENERAL(A4B4G4R4);
IBITMAP_FETCH_GENERAL(R4G4B4A4);
IBITMAP_FETCH_GENERAL(B4G4R4A4);
IBITMAP_FETCH_GENERAL(C8);
IBITMAP_FETCH_GENERAL(A8);
IBITMAP_FETCH_GENERAL(G8);



// 平移的fetch
public static void ibitmap_fetch_translate_int(in IBITMAP bmp, uint* card,
    int width, cfixed* source, cfixed* step,
     byte* cover, ref IRECT clip)
{
    iColorIndex* index = (const iColorIndex*)bmp.extra;
    cfixed u, v, w, du, dv, dw;
    iFetchProc fetch;
    uint mask;
    int x, y, i, d;
    int overflow;
    int left;
    int right;

    u = source[0];
    v = source[1];
    w = source[2];
    du = step[0];
    dv = step[1];
    dw = step[2];

    if (!(cfixed_is_zero(dw) && cfixed_is_zero(dv) &&
        cfixed_is_one(du) && cfixed_is_one(w)))
    {
        assert(0);
        abort();
    }

    fetch = ipixel_get_fetch(ibitmap_pixfmt_guess(bmp), 0);

    x = cfixed_to_int(u - cfixed_const_e);
    y = cfixed_to_int(v - cfixed_const_e);
    overflow = (int)ibitmap_imode_const(bmp, overflow);
    mask = (uint)bmp.mask;

    left = clip.left;
    right = clip.right;

    switch (overflow)
    {
        case IBOM_TRANSPARENT:
            {
                if (y < clip.top || y >= clip.bottom ||
                    x >= right || x + width <= left)
                {
                    for (i = 0; i < width; i++) *card++ = mask;
                }
                else
                {
                    if (x < left)
                    {
                        int d = left - x;
                        for (i = 0; i < d; i++) *card++ = mask;
                        x += d;
                        width -= d;
                    }
                    if (x + width > right)
                    {
                        d = x + width - right;
                        for (i = 0; i < d; i++) card[width - 1 - i] = mask;
                        width -= d;
                    }
                    assert(x >= left && x + width <= right && width > 0);
                    fetch(bmp.line[y], x, width, card, index);
                }
            }
            break;
        case IBOM_REPEAT:
            {
                if (y < clip.top) y = clip.top;
                if (y >= clip.bottom) y = clip.bottom - 1;
                if (x + width > right)
                {
                    uint cc;
                    fetch(bmp.line[y], right - 1, 1, &cc, index);
                    d = x + width - right;
                    if (d > width) d = width;
                    for (i = 0; i < d; i++) card[width - 1 - i] = cc;
                    width -= d;
                }
                if (x < left)
                {
                    uint cc;
                    fetch(bmp.line[y], left, 1, &cc, index);
                    d = left - x;
                    if (d > width) d = width;
                    for (i = 0; i < d; i++) *card++ = cc;
                    x += d;
                    width -= d;
                }
                if (width > 0)
                {
                    fetch(bmp.line[y], x, width, card, index);
                }
            }
            break;
        case IBOM_WRAP:
            {
                int cw = right - left;
                int ch = clip.bottom - clip.top;
                if (y < clip.top || y >= clip.bottom)
                    y = clip.top + (y - clip.top) % ch;
                while (width > 0)
                {
                    int canread;
                    if (x < left || x >= right)
                        x = left + (x - left) % cw;
                    canread = right - x;
                    if (canread > width) canread = width;
                    fetch(bmp.line[y], x, canread, card, index);
                    x += canread;
                    card += canread;
                    width -= canread;
                }
            }
            break;
        case IBOM_MIRROR:
            {
                iBitmapFetchProc proc;
                proc = ibitmap_scanline_get_proc(ibitmap_pixfmt_guess(bmp),
                    IBITMAP_FETCH_GENERAL_NEAREST, 0);
                proc(bmp, card, w, source, step, cover, clip);
            }
            break;
    }
}


void ibitmap_fetch_scale_A8R8G8B8_nearest(in IBITMAP bmp, uint* card,
    int width, cfixed* source, cfixed* step,
     byte* cover, ref IRECT clip)
{
    int pixfmt = ibitmap_pixfmt_guess(bmp);
    int filter = ibitmap_imode_const(bmp, filter);
    int overflow = ibitmap_imode_const(bmp, overflow);
    cfixed u = source[0] - cfixed_const_e;
    cfixed v = source[1] - cfixed_const_e;
    cfixed du = step[0];
    uint* endup = card + width;
    int top = cfixed_to_int(v);

    if (filter != IPIXEL_FILTER_NEAREST || overflow > 1)
    {
        iBitmapFetchProc proc;
        proc = ibitmap_scanline_get_proc(pixfmt, filter ? 0 : 1, 0);
        proc(bmp, card, width, source, step, cover, clip);
        return;
    }

    if (overflow == IBOM_TRANSPARENT)
    {
        if (top < clip.top || top >= clip.bottom)
        {
            uint mask = (uint)bmp.mask;
            while (card < endup) *card++ = mask;
            return;
        }
    }
    else
    {
        if (top < clip.top) top = clip.top;
        else if (top >= clip.bottom) top = clip.bottom - 1;
    }

    if (du >= 0)
    {
        cfixed w = cfixed_from_int(clip.left);
        uint* line;
        uint color, mask;

        line = (uint*)bmp.line[top];

        mask = (pixfmt != IPIX_FMT_A8R8G8B8) ? 0xff000000 : 0;
        color = line[clip.left] | mask;

        if (overflow == IBOM_TRANSPARENT) color = (uint)bmp.mask;

        while (card < endup && u < w)
        {
            *card++ = color;
            u += du;
        }

        w = cfixed_from_int(clip.right);

        if (mask == 0)
        {
            while (card < endup && u < w)
            {
                *card++ = line[cfixed_to_int(u)];
                u += du;
            }
        }
        else
        {
            while (card < endup && u < w)
            {
                *card++ = line[cfixed_to_int(u)] | mask;
                u += du;
            }
        }

        color = line[clip.right - 1] | mask;
        if (overflow == IBOM_TRANSPARENT) color = (uint)bmp.mask;

        while (card < endup)
        {
            *card++ = color;
        }
    }
    else
    {
        cfixed w = cfixed_from_int(clip.right);
        uint* line;
        uint color, mask;

        line = (uint*)bmp.line[top];

        mask = (pixfmt != IPIX_FMT_A8R8G8B8) ? 0xff000000 : 0;
        color = line[clip.right - 1] | mask;

        if (overflow == IBOM_TRANSPARENT) color = (uint)bmp.mask;

        while (card < endup && u >= w)
        {
            *card++ = color;
            u += du;
        }

        w = cfixed_from_int(clip.left);

        if (mask == 0)
        {
            while (card < endup && u >= w)
            {
                *card++ = line[cfixed_to_int(u)];
                u += du;
            }
        }
        else
        {
            while (card < endup && u >= w)
            {
                *card++ = line[cfixed_to_int(u)] | mask;
                u += du;
            }
        }

        color = line[clip.left] | mask;
        if (overflow == IBOM_TRANSPARENT) color = (uint)bmp.mask;

        while (card < endup)
        {
            *card++ = color;
        }
    }
}

void ibitmap_fetch_scale_A8R8G8B8_bilinear(in IBITMAP bmp, uint* card,
    int width, cfixed* source, cfixed* step,
     byte* cover, ref IRECT clip)
{
    int pixfmt = ibitmap_pixfmt_guess(bmp);
    int filter = ibitmap_imode_const(bmp, filter);
    int overflow = ibitmap_imode_const(bmp, overflow);
    cfixed x = source[0] - cfixed_const_half;
    cfixed y = source[1] - cfixed_const_half;
    cfixed du = step[0];
    cfixed ux, x_top, x_bot, ux_top, ux_bot;
    uint* endup = card + width;
    uint* top_row;
    uint* bot_row;
    uint top_mask;
    uint bot_mask;
    uint zero[2];
    int x1, x2, y1, y2;
    int distx;
    int disty;

    if (filter != IPIXEL_FILTER_BILINEAR || overflow > 1)
    {
        iBitmapFetchProc proc;
        proc = ibitmap_scanline_get_proc(pixfmt, filter ? 0 : 1, 0);
        proc(bmp, card, width, source, step, cover, clip);
        return;
    }

    x_top = x_bot = x;
    ux = ux_top = ux_bot = du;

    y1 = cfixed_to_int(y);
    y2 = y1 + 1;

    zero[0] = zero[1] = (uint)bmp.mask;

    if (overflow == IBOM_TRANSPARENT)
    {
        if (y2 < clip.top || y1 >= clip.bottom)
        {
            uint mask = (uint)bmp.mask;
            while (card < endup) *card++ = mask;
            return;
        }
        if (y1 < clip.top || y1 >= clip.bottom)
        {
            top_row = zero;
            x_top = 0;
            ux_top = 0;
        }
        else
        {
            top_row = (const uint*)bmp.line[y1];
        }
        if (y2 < clip.top || y2 >= clip.bottom)
        {
            bot_row = zero;
            x_bot = 0;
            ux_bot = 0;
        }
        else
        {
            bot_row = (const uint*)bmp.line[y2];
        }
    }
    else
    {
        if (y1 < clip.top) y1 = clip.top;
        else if (y1 >= clip.bottom) y1 = clip.bottom - 1;
        if (y2 < clip.top) y2 = clip.top;
        else if (y2 >= clip.bottom) y2 = clip.bottom - 1;
        top_row = (const uint*)bmp.line[y1];
        bot_row = (const uint*)bmp.line[y2];
    }

    if (top_row == zero && bot_row == zero)
    {
        uint mask = (uint)bmp.mask;
        while (card < endup) *card++ = mask;
        return;
    }

    top_mask = bot_mask = 0;

    if (pixfmt == IPIX_FMT_X8R8G8B8)
    {
        top_mask = bot_mask = 0xff000000;
        if (top_row == zero) top_mask = 0;
        if (bot_row == zero) bot_mask = 0;
    }

    disty = (y >> 8) & 0xff;

    if (ux >= 0)
    {
        uint tl, tr, bl, br, cmask;
        cfixed w;

        if (overflow == IBOM_TRANSPARENT)
        {
            cmask = (uint)bmp.mask;
            tl = bl = cmask;
            if (top_row != zero) tr = top_row[clip.left] | top_mask;
            else tr = cmask;
            if (bot_row != zero) br = bot_row[clip.left] | bot_mask;
            else br = cmask;
            w = cfixed_from_int(clip.left - 1);
        }
        else
        {
            tl = tr = top_row[clip.left] | top_mask;
            bl = br = bot_row[clip.left] | bot_mask;
            cmask = ipixel_biline_interp(tl, tl, bl, bl, 0, disty);
            w = cfixed_from_int(clip.left);
        }

        // mask fill
        while (card < endup && x < w)
        {
            *card++ = cmask;
            x += ux;
            x_top += ux_top;
            x_bot += ux_bot;
        }

        // left edge fill
        w = cfixed_from_int(clip.left);
        while (card < endup && x < w)
        {
            distx = (x >> 8) & 0xff;
            *card++ = ipixel_biline_interp(tl, tr, bl, br, distx, disty);
            x += ux;
            x_top += ux_top;
            x_bot += ux_bot;
        }

        // main part
        w = cfixed_from_int(clip.right - 1);
        while (card < endup && x < w)
        {
            x1 = cfixed_to_int(x_top);
            x2 = cfixed_to_int(x_bot);
            distx = (x >> 8) & 0xff;
            tl = top_row[x1 + 0] | top_mask;
            tr = top_row[x1 + 1] | top_mask;
            bl = bot_row[x2 + 0] | bot_mask;
            br = bot_row[x2 + 1] | bot_mask;
            *card++ = ipixel_biline_interp(tl, tr, bl, br, distx, disty);
            x += ux;
            x_top += ux_top;
            x_bot += ux_bot;
        }

        if (overflow == IBOM_TRANSPARENT)
        {
            cmask = (uint)bmp.mask;
            tr = br = cmask;
            if (top_row != zero) tl = top_row[clip.right - 1] | top_mask;
            else tl = cmask;
            if (bot_row != zero) bl = bot_row[clip.right - 1] | bot_mask;
            else bl = cmask;
        }
        else
        {
            tl = tr = top_row[clip.right - 1] | top_mask;
            bl = br = bot_row[clip.right - 1] | bot_mask;
            cmask = ipixel_biline_interp(tl, tl, bl, bl, 0, disty);
        }

        // right edge
        w = cfixed_from_int(clip.right);
        while (card < endup && x < w)
        {
            distx = (x >> 8) & 0xff;
            *card++ = ipixel_biline_interp(tl, tr, bl, br, distx, disty);
            x += ux;
            x_top += ux_top;
            x_bot += ux_bot;
        }

        // left of scanline
        while (card < endup)
        {
            *card++ = cmask;
        }
    }
    else
    {
        uint tl, tr, bl, br, cmask;
        cfixed w;

        if (overflow == IBOM_TRANSPARENT)
        {
            cmask = (uint)bmp.mask;
            tr = br = cmask;
            if (top_row != zero) tl = top_row[clip.right - 1] | top_mask;
            else tl = cmask;
            if (bot_row != zero) bl = bot_row[clip.right - 1] | bot_mask;
            else bl = cmask;
        }
        else
        {
            tl = tr = top_row[clip.right - 1] | top_mask;
            bl = br = bot_row[clip.right - 1] | bot_mask;
            cmask = ipixel_biline_interp(tl, tl, bl, bl, 0, disty);
        }

        w = cfixed_from_int(clip.right);
        while (card < endup && x >= w)
        {
            *card++ = cmask;
            x += ux;
            x_top += ux_top;
            x_bot += ux_bot;
        }

        // right edge
        w = cfixed_from_int(clip.right - 1);
        while (card < endup && x >= w)
        {
            distx = (x >> 8) & 0xff;
            *card++ = ipixel_biline_interp(tl, tr, bl, br, distx, disty);
            x += ux;
            x_top += ux_top;
            x_bot += ux_bot;
        }

        // main part
        w = cfixed_from_int(clip.left);
        while (card < endup && x >= w)
        {
            x1 = cfixed_to_int(x_top);
            x2 = cfixed_to_int(x_bot);
            distx = (x >> 8) & 0xff;
            tl = top_row[x1 + 0] | top_mask;
            tr = top_row[x1 + 1] | top_mask;
            bl = bot_row[x2 + 0] | bot_mask;
            br = bot_row[x2 + 1] | bot_mask;
            *card++ = ipixel_biline_interp(tl, tr, bl, br, distx, disty);
            x += ux;
            x_top += ux_top;
            x_bot += ux_bot;
        }

        if (overflow == IBOM_TRANSPARENT)
        {
            cmask = (uint)bmp.mask;
            tl = bl = cmask;
            if (top_row != zero) tr = top_row[clip.left] | top_mask;
            else tr = cmask;
            if (bot_row != zero) br = bot_row[clip.left] | bot_mask;
            else br = cmask;
            w = cfixed_from_int(clip.left - 1);
        }
        else
        {
            tl = tr = top_row[clip.left] | top_mask;
            bl = br = bot_row[clip.left] | bot_mask;
            cmask = ipixel_biline_interp(tl, tl, bl, bl, 0, disty);
            w = cfixed_from_int(clip.left);
        }

        // left edge
        w = cfixed_from_int(clip.left - 1);
        while (card < endup && x >= w)
        {
            distx = (x >> 8) & 0xff;
            *card++ = ipixel_biline_interp(tl, tr, bl, br, distx, disty);
            x += ux;
            x_top += ux_top;
            x_bot += ux_bot;
        }

        // left of the scanline
        while (card < endup)
        {
            *card++ = cmask;
        }
    }
}


// 初始化函数查找表
public static void ibitmap_fetch_proc_table_init(void)
{
    public static int inited = 0;
    int fmt, i;
    if (inited != 0) return;
    ibitmap_fetch_proc_table_clear();
#define ibitmap_fetch_proc_table_init_general(fmt) do { \
    int z; \
				for (z = 0; z < 18; z++)
    { \
					ibitmap_fetch_proc_table[IPIX_FMT_##fmt][z][0] = \
						ibitmap_fetch_general_##fmt; \
					ibitmap_fetch_proc_table[IPIX_FMT_##fmt][z][1] = \
						ibitmap_fetch_general_##fmt; \
				}	\
			} while (0)
#if 1
	ibitmap_fetch_proc_table_init_general(A8R8G8B8);
	ibitmap_fetch_proc_table_init_general(A8B8G8R8);
	ibitmap_fetch_proc_table_init_general(R8G8B8A8);
	ibitmap_fetch_proc_table_init_general(B8G8R8A8);
	ibitmap_fetch_proc_table_init_general(X8R8G8B8);
	ibitmap_fetch_proc_table_init_general(X8B8G8R8);
	ibitmap_fetch_proc_table_init_general(R8G8B8X8);
	ibitmap_fetch_proc_table_init_general(B8G8R8X8);
	ibitmap_fetch_proc_table_init_general(R8G8B8);
	ibitmap_fetch_proc_table_init_general(B8G8R8);
	ibitmap_fetch_proc_table_init_general(R5G6B5);
	ibitmap_fetch_proc_table_init_general(B5G6R5);
	ibitmap_fetch_proc_table_init_general(X1R5G5B5);
	ibitmap_fetch_proc_table_init_general(X1B5G5R5);
	ibitmap_fetch_proc_table_init_general(R5G5B5X1);
	ibitmap_fetch_proc_table_init_general(B5G5R5X1);
	ibitmap_fetch_proc_table_init_general(A1R5G5B5);
	ibitmap_fetch_proc_table_init_general(A1B5G5R5);
	ibitmap_fetch_proc_table_init_general(R5G5B5A1);
	ibitmap_fetch_proc_table_init_general(B5G5R5A1);
	ibitmap_fetch_proc_table_init_general(A4R4G4B4);
	ibitmap_fetch_proc_table_init_general(A4B4G4R4);
	ibitmap_fetch_proc_table_init_general(R4G4B4A4);
	ibitmap_fetch_proc_table_init_general(B4G4R4A4);
	ibitmap_fetch_proc_table_init_general(C8);
	ibitmap_fetch_proc_table_init_general(A8);
	ibitmap_fetch_proc_table_init_general(G8);
	for (fmt = 0; fmt < 48; fmt++) {
		for (i = 0; i < 3; i++) {
			int pos = 2 + i * 4;
			ibitmap_fetch_proc_table[fmt][pos + 0][0] = 
				ibitmap_fetch_translate_int;
			ibitmap_fetch_proc_table[fmt][pos + 0][1] = 
				ibitmap_fetch_translate_int;
			ibitmap_fetch_proc_table[fmt][pos + 1][0] = 
				ibitmap_fetch_translate_int;
			ibitmap_fetch_proc_table[fmt][pos + 1][1] = 
				ibitmap_fetch_translate_int;
		}
	}
#if 1
	ibitmap_fetch_proc_table[IPIX_FMT_A8R8G8B8][4][0] =
		ibitmap_fetch_scale_A8R8G8B8_nearest;
	ibitmap_fetch_proc_table[IPIX_FMT_A8R8G8B8][4][1] =
		ibitmap_fetch_scale_A8R8G8B8_nearest;
	ibitmap_fetch_proc_table[IPIX_FMT_A8R8G8B8][5][0] =
		ibitmap_fetch_scale_A8R8G8B8_bilinear;
	ibitmap_fetch_proc_table[IPIX_FMT_A8R8G8B8][5][1] =
		ibitmap_fetch_scale_A8R8G8B8_bilinear;
	ibitmap_fetch_proc_table[IPIX_FMT_A8R8G8B8][8][0] =
		ibitmap_fetch_scale_A8R8G8B8_nearest;
	ibitmap_fetch_proc_table[IPIX_FMT_A8R8G8B8][8][1] =
		ibitmap_fetch_scale_A8R8G8B8_nearest;
	ibitmap_fetch_proc_table[IPIX_FMT_A8R8G8B8][9][0] =
		ibitmap_fetch_scale_A8R8G8B8_bilinear;
	ibitmap_fetch_proc_table[IPIX_FMT_A8R8G8B8][9][1] =
		ibitmap_fetch_scale_A8R8G8B8_bilinear;

	ibitmap_fetch_proc_table[IPIX_FMT_X8R8G8B8][4][0] =
		ibitmap_fetch_scale_A8R8G8B8_nearest;
	ibitmap_fetch_proc_table[IPIX_FMT_X8R8G8B8][4][1] =
		ibitmap_fetch_scale_A8R8G8B8_nearest;
	ibitmap_fetch_proc_table[IPIX_FMT_X8R8G8B8][5][0] =
		ibitmap_fetch_scale_A8R8G8B8_bilinear;
	ibitmap_fetch_proc_table[IPIX_FMT_X8R8G8B8][5][1] =
		ibitmap_fetch_scale_A8R8G8B8_bilinear;
	ibitmap_fetch_proc_table[IPIX_FMT_X8R8G8B8][8][0] =
		ibitmap_fetch_scale_A8R8G8B8_nearest;
	ibitmap_fetch_proc_table[IPIX_FMT_X8R8G8B8][8][1] =
		ibitmap_fetch_scale_A8R8G8B8_nearest;
	ibitmap_fetch_proc_table[IPIX_FMT_X8R8G8B8][9][0] =
		ibitmap_fetch_scale_A8R8G8B8_bilinear;
	ibitmap_fetch_proc_table[IPIX_FMT_X8R8G8B8][9][1] =
		ibitmap_fetch_scale_A8R8G8B8_bilinear;
#endif
#endif
#undef ibitmap_fetch_proc_table_init_general
    inited = 1;
}


//=====================================================================
// 批量写点
//=====================================================================
public struct iPixelListProcTable
{
    iPixelListSC pixel_list_sc, pixel_list_sc_default;
    iPixelListMC pixel_list_mc, pixel_list_mc_default;
};


#define IPIXEL_DOT_DRAW_PROC_X(fmt, bpp, mode) \
public static void ipixel_list_proc_sc_##fmt(ref IBITMAP  bmp, uint *xy, \
	int size, uint color, int add) \
{ \
	uint r1, g1, b1, a1, c1, r, g, b, a; \
	byte* ptr; \
	void** lines = bmp.line; \
	iColorIndex* _ipixel_src_index = (iColorIndex*)bmp.extra; \
	iColorIndex* _ipixel_dst_index = (iColorIndex*)bmp.extra; \
	IRGBA_FROM_A8R8G8B8(color, r, g, b, a); \
	if (a == 0) return; \
	if (add)
{ \
		r1 = g1 = b1 = a1 = 0; \
		IBLEND_ADDITIVE(r, g, b, a, r1, g1, b1, a1); \
		r = r1; g = g1; b = b1; a = a1; \
		for (; size > 0; size--, xy += 2)
    { \
			ptr = (byte*)lines[xy[1]]; \
			c1 = _ipixel_fetch(bpp, ptr, xy[0]); \
			IRGBA_FROM_PIXEL(fmt, c1, r1, g1, b1, a1); \
			r1 = ICLIP_256(r1 + r); \
			g1 = ICLIP_256(g1 + g); \
			b1 = ICLIP_256(b1 + b); \
			a1 = ICLIP_256(a1 + a); \
			c1 = IRGBA_TO_PIXEL(fmt, r1, g1, b1, a1); \
			_ipixel_store(bpp, ptr, xy[0], c1); \
		} \
		return; \
	}	\
	if (a == 255)
{ \
		c1 = IRGBA_TO_PIXEL(fmt, r, g, b, 255); \
		for (; size > 0; size--, xy += 2)
    { \
			ptr = (byte*)lines[xy[1]]; \
			_ipixel_store(bpp, ptr, xy[0], c1); \
		} \
	}	else { \
		for (; size > 0; size--, xy += 2) { \
			ptr = (byte*)lines[xy[1]]; \
			c1 = _ipixel_fetch(bpp, ptr, xy[0]); \
			IRGBA_FROM_PIXEL(fmt, c1, r1, g1, b1, a1); \
			IBLEND_##mode(r, g, b, a, r1, g1, b1, a1); \
			c1 = IRGBA_TO_PIXEL(fmt, r1, g1, b1, a1); \
			_ipixel_store(bpp, ptr, xy[0], c1); \
		} \
	} \
	_ipixel_src_index = _ipixel_src_index; \
	_ipixel_dst_index = _ipixel_dst_index; \
} \
public static void ipixel_list_proc_mc_##fmt(ref IBITMAP  bmp, uint *data, \
	int size, int additive) \
{ \
	uint r1, g1, b1, a1, c1, c, r, g, b, a; \
	byte *ptr; \
	void **lines = bmp.line; \
	iColorIndex *_ipixel_src_index = (iColorIndex*)bmp.extra; \
	iColorIndex *_ipixel_dst_index = (iColorIndex*)bmp.extra; \
	if (additive) { \
		for (; size > 0; size--, data += 3) { \
			ptr = (byte*)lines[data[1]]; \
			c = data[2]; \
			IRGBA_FROM_PIXEL(A8R8G8B8, c, r, g, b, a); \
			if (a != 0) { \
				c1 = _ipixel_fetch(bpp, ptr, data[0]); \
				IRGBA_FROM_PIXEL(fmt, c1, r1, g1, b1, a1); \
				IBLEND_ADDITIVE(r, g, b, a, r1, g1, b1, a1); \
				c1 = IRGBA_TO_PIXEL(fmt, r1, g1, b1, a1); \
				_ipixel_store(bpp, ptr, data[0], c1); \
			} \
		} \
		return; \
	} \
	for (; size > 0; size--, data += 3) { \
		ptr = (byte*)lines[data[1]]; \
		c = data[2]; \
		IRGBA_FROM_PIXEL(A8R8G8B8, c, r, g, b, a); \
		if (a == 255) { \
			c1 = IRGBA_TO_PIXEL(fmt, r, g, b, 0xff); \
			_ipixel_store(bpp, ptr, data[0], c1); \
		} \
		else if (a > 0) { \
			c1 = _ipixel_fetch(bpp, ptr, data[0]); \
			IRGBA_FROM_PIXEL(fmt, c1, r1, g1, b1, a1); \
			IBLEND_STATIC(r, g, b, a, r1, g1, b1, a1); \
			c1 = IRGBA_TO_PIXEL(fmt, r1, g1, b1, a1); \
			_ipixel_store(bpp, ptr, data[0], c1); \
		} \
	} \
	_ipixel_src_index = _ipixel_src_index; \
	_ipixel_dst_index = _ipixel_dst_index; \
}


IPIXEL_DOT_DRAW_PROC_X(A8R8G8B8, 32, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(A8B8G8R8, 32, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(R8G8B8A8, 32, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(B8G8R8A8, 32, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(X8R8G8B8, 32, STATIC)
IPIXEL_DOT_DRAW_PROC_X(X8B8G8R8, 32, STATIC)
IPIXEL_DOT_DRAW_PROC_X(R8G8B8X8, 32, STATIC)
IPIXEL_DOT_DRAW_PROC_X(B8G8R8X8, 32, STATIC)
IPIXEL_DOT_DRAW_PROC_X(P8R8G8B8, 32, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(R8G8B8, 24, STATIC)
IPIXEL_DOT_DRAW_PROC_X(B8G8R8, 24, STATIC)
IPIXEL_DOT_DRAW_PROC_X(R5G6B5, 16, STATIC)
IPIXEL_DOT_DRAW_PROC_X(B5G6R5, 16, STATIC)
IPIXEL_DOT_DRAW_PROC_X(X1R5G5B5, 16, STATIC)
IPIXEL_DOT_DRAW_PROC_X(X1B5G5R5, 16, STATIC)
IPIXEL_DOT_DRAW_PROC_X(R5G5B5X1, 16, STATIC)
IPIXEL_DOT_DRAW_PROC_X(B5G5R5X1, 16, STATIC)
IPIXEL_DOT_DRAW_PROC_X(A1R5G5B5, 16, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(A1B5G5R5, 16, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(R5G5B5A1, 16, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(B5G5R5A1, 16, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(X4R4G4B4, 16, STATIC)
IPIXEL_DOT_DRAW_PROC_X(X4B4G4R4, 16, STATIC)
IPIXEL_DOT_DRAW_PROC_X(R4G4B4X4, 16, STATIC)
IPIXEL_DOT_DRAW_PROC_X(B4G4R4X4, 16, STATIC)
IPIXEL_DOT_DRAW_PROC_X(A4R4G4B4, 16, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(A4B4G4R4, 16, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(R4G4B4A4, 16, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(B4G4R4A4, 16, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(C8, 8, STATIC)
IPIXEL_DOT_DRAW_PROC_X(G8, 8, STATIC)
IPIXEL_DOT_DRAW_PROC_X(A8, 8, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(R3G3B2, 8, STATIC)
IPIXEL_DOT_DRAW_PROC_X(B2G3R3, 8, STATIC)
IPIXEL_DOT_DRAW_PROC_X(X2R2G2B2, 8, STATIC)
IPIXEL_DOT_DRAW_PROC_X(X2B2G2R2, 8, STATIC)
IPIXEL_DOT_DRAW_PROC_X(R2G2B2X2, 8, STATIC)
IPIXEL_DOT_DRAW_PROC_X(B2G2R2X2, 8, STATIC)
IPIXEL_DOT_DRAW_PROC_X(A2R2G2B2, 8, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(A2B2G2R2, 8, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(R2G2B2A2, 8, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(B2G2R2A2, 8, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(X4C4, 8, STATIC)
IPIXEL_DOT_DRAW_PROC_X(X4G4, 8, STATIC)
IPIXEL_DOT_DRAW_PROC_X(X4A4, 8, NORMAL_FAST)
IPIXEL_DOT_DRAW_PROC_X(C4X4, 8, STATIC)
IPIXEL_DOT_DRAW_PROC_X(G4X4, 8, STATIC)
IPIXEL_DOT_DRAW_PROC_X(A4X4, 8, NORMAL_FAST)


#define ITABLE_ITEM(fmt) \
	{ ipixel_list_proc_sc_##fmt, ipixel_list_proc_sc_##fmt, \
	  ipixel_list_proc_mc_##fmt, ipixel_list_proc_mc_##fmt }

public static public struct iPixelListProcTable ipixel_list_proc_table[IPIX_FMT_COUNT] = 
{
	ITABLE_ITEM(A8R8G8B8),
	ITABLE_ITEM(A8B8G8R8),
	ITABLE_ITEM(R8G8B8A8),
	ITABLE_ITEM(B8G8R8A8),
	ITABLE_ITEM(X8R8G8B8),
	ITABLE_ITEM(X8B8G8R8),
	ITABLE_ITEM(R8G8B8X8),
	ITABLE_ITEM(B8G8R8X8),
	ITABLE_ITEM(P8R8G8B8),
	ITABLE_ITEM(R8G8B8),
	ITABLE_ITEM(B8G8R8),
	ITABLE_ITEM(R5G6B5),
	ITABLE_ITEM(B5G6R5),
	ITABLE_ITEM(X1R5G5B5),
	ITABLE_ITEM(X1B5G5R5),
	ITABLE_ITEM(R5G5B5X1),
	ITABLE_ITEM(B5G5R5X1),
	ITABLE_ITEM(A1R5G5B5),
	ITABLE_ITEM(A1B5G5R5),
	ITABLE_ITEM(R5G5B5A1),
	ITABLE_ITEM(B5G5R5A1),
	ITABLE_ITEM(X4R4G4B4),
	ITABLE_ITEM(X4B4G4R4),
	ITABLE_ITEM(R4G4B4X4),
	ITABLE_ITEM(B4G4R4X4),
	ITABLE_ITEM(A4R4G4B4),
	ITABLE_ITEM(A4B4G4R4),
	ITABLE_ITEM(R4G4B4A4),
	ITABLE_ITEM(B4G4R4A4),
	ITABLE_ITEM(C8),
	ITABLE_ITEM(G8),
	ITABLE_ITEM(A8),
	ITABLE_ITEM(R3G3B2),
	ITABLE_ITEM(B2G3R3),
	ITABLE_ITEM(X2R2G2B2),
	ITABLE_ITEM(X2B2G2R2),
	ITABLE_ITEM(R2G2B2X2),
	ITABLE_ITEM(B2G2R2X2),
	ITABLE_ITEM(A2R2G2B2),
	ITABLE_ITEM(A2B2G2R2),
	ITABLE_ITEM(R2G2B2A2),
	ITABLE_ITEM(B2G2R2A2),
	ITABLE_ITEM(X4C4),
	ITABLE_ITEM(X4G4),
	ITABLE_ITEM(X4A4),
	ITABLE_ITEM(C4X4),
	ITABLE_ITEM(G4X4),
	ITABLE_ITEM(A4X4),
};

#undef ITABLE_ITEM


iPixelListSC ibitmap_get_pixel_list_sc_proc(int pixfmt, int isdefault)
{
	assert(pixfmt >= 0 && pixfmt <= IPIX_FMT_A4X4);
	if (pixfmt < 0 || pixfmt > IPIX_FMT_A4X4) return null;
	if (isdefault == 0) return ipixel_list_proc_table[pixfmt].pixel_list_sc;
	return ipixel_list_proc_table[pixfmt].pixel_list_sc_default;
}

iPixelListMC ibitmap_get_pixel_list_mc_proc(int pixfmt, int isdefault)
{
	assert(pixfmt >= 0 && pixfmt <= IPIX_FMT_A4X4);
	if (pixfmt < 0 || pixfmt > IPIX_FMT_A4X4) return null;
	if (isdefault == 0) return ipixel_list_proc_table[pixfmt].pixel_list_mc;
	return ipixel_list_proc_table[pixfmt].pixel_list_mc_default;
}

void ibitmap_set_pixel_list_sc_proc(int pixfmt, iPixelListSC proc)
{
	assert(pixfmt >= 0 && pixfmt <= IPIX_FMT_A4X4);
	if (pixfmt < 0 || pixfmt > IPIX_FMT_A4X4) return;
	ipixel_list_proc_table[pixfmt].pixel_list_sc = proc;
}

void ibitmap_set_pixel_list_mc_proc(int pixfmt, iPixelListMC proc)
{
	assert(pixfmt >= 0 && pixfmt <= IPIX_FMT_A4X4);
	if (pixfmt < 0 || pixfmt > IPIX_FMT_A4X4) return;
	ipixel_list_proc_table[pixfmt].pixel_list_mc = proc;
}

void ibitmap_draw_pixel_list_sc(ref IBITMAP  bmp, uint *xy, int count,
	uint color, int additive)
{
	iPixelListSC proc;
	int pixfmt;
	pixfmt = ibitmap_pixfmt_guess(bmp);
	proc = ibitmap_get_pixel_list_sc_proc(pixfmt, 0);
	if (proc) {
		proc(bmp, xy, count, color, additive);
	}	else {
		assert(proc);
	}
}

void ibitmap_draw_pixel_list_mc(ref IBITMAP  bmp, uint *xyc, int count,
	int additive)
{
	iPixelListMC proc;
	int pixfmt;
	pixfmt = ibitmap_pixfmt_guess(bmp);
	proc = ibitmap_get_pixel_list_mc_proc(pixfmt, 0);
	if (proc) {
		proc(bmp, xyc, count, additive);
	}	else {
		assert(proc);
	}
}



//=====================================================================
// GLYPH IMPLEMENTATION
//=====================================================================
#define IDRAW_GLYPH_PROC(draw_pixel) { \
	byte*data = glyph.data; \
	byte*src; \
	int w = glyph.w; \
	int h = glyph.h; \
	int pitch = (w + 7) / 8; \
	int lgap = 0; \
	int d, i, j, xx, yy; \
	int shift; \
	int drawit; \
	if (clip) { \
		if (y < clip.top) { \
			d = clip.top - y; \
			h -= d; \
			if (h <= 0) return; \
			data += d * pitch; \
			y = clip.top; \
		} \
		if (y + h > clip.bottom) { \
			h = clip.bottom - y; \
			if (h <= 0) return; \
		} \
		if (x < clip.left) { \
			d = clip.left - x; \
			w -= d; \
			if (w <= 0) return; \
			data += d / 8; \
			lgap = d & 7; \
			x = clip.left; \
		} \
		if (x + w > clip.right) { \
			w = clip.right - x; \
			if (w <= 0) return; \
		} \
	} \
	while (h--) { \
		j = 0; \
		i = 0x80 >> lgap; \
		src = data; \
		d = *(src++); \
		shift = 7 - lgap; \
		yy = y; xx = x; \
		for (; ; ) { \
			drawit = (d & i) >> shift; \
			draw_pixel; \
			j++; \
			if (j >= w) break; \
			shift--; \
			i >>= 1; \
			if (i == 0) { \
				i = 0x80; \
				shift = 7; \
				d = *(src++); \
			} \
			xx++; \
		} \
		data += pitch; \
		y++; \
		yy++; \
	} \
}

void ibitmap_draw_glyph_font(IBITMAP *dst, IGLYPHbyte *glyph,
	const IRECT *clip, int x, int y, uint color, uint bk, 
	int additive, void *workmem)
{
	uint *xy1, *xy2;
	int p1, p2;
	xy1 = (uint*)workmem;
	xy2 = (uint*)workmem + glyph.w * glyph.h * 2;
	p1 = p2 = 0;
	if ((bk >> 24) == 0) {
		IDRAW_GLYPH_PROC( 
			{
				if (drawit) {
					xy1[p1++] = (uint)xx;
					xy1[p1++] = (uint)yy;
				}
			}
		);
		ibitmap_draw_pixel_list_sc(dst, xy1, p1 / 2, color, additive);
	}	else {
		IDRAW_GLYPH_PROC( 
			{
				if (drawit) {
					xy1[p1++] = (uint)xx;
					xy1[p1++] = (uint)yy;
				}	else {
					xy2[p2++] = (uint)xx;
					xy2[p2++] = (uint)yy;
				}
			}
		);
		ibitmap_draw_pixel_list_sc(dst, xy1, p1 / 2, color, additive);
		ibitmap_draw_pixel_list_sc(dst, xy2, p2 / 2, bk, additive);
	}
}

void ibitmap_draw_glyph(IBITMAP *dst, IGLYPHbyte *glyph, int x, int y,
	const IRECT *clip, uint color, uint bk, int add)
{
	byte _buffer[IBITMAP_STACK_BUFFER];
	byte *buffer = _buffer;
	int size = glyph.w * glyph.h * 16;
	if (size >= IBITMAP_STACK_BUFFER) {
		buffer = (byte*)icmalloc(size);
		if (buffer == null) return;
	}

	ibitmap_draw_glyph_font(dst, glyph, clip, x, y, color, bk, add, buffer);

	if (buffer != _buffer) {
		icfree(buffer);
	}
}

void ibitmap_draw_ascii(IBITMAP *dst, IGLYPHFONT *font, int x, int y,
	byte*string, IRECT *clip, uint col, uint bk, int add)
{
	byte _buffer[IBITMAP_STACK_BUFFER];
	byte *buffer = _buffer;
	int size = font.w * font.h * 16;
	int unit = ((font.w + 7) / 8) * font.h;
	IGLYPHbyte *glyph;
	IRECT rect;
	int left, i, p;

	if (size + unit + sizeof(IGLYPHCHAR) >= IBITMAP_STACK_BUFFER) {
		buffer = (byte*)icmalloc(size + unit + sizeof(IGLYPHCHAR));
		if (buffer == null) return;
	}

	glyph = (IGLYPHbyte*)(buffer + size);

	if (clip == null) {
		clip = &rect;
		rect.left = 0;
		rect.top = 0;
		rect.right = (int)dst.w;
		rect.bottom = (int)dst.h;
	}

	glyph.w = font.w;
	glyph.h = font.h;

	for (i = 0, left = x, p = 0; string[i]; i++) {
		int ch = (byte)string[i];
		if (ch == '\n') {
			x = left;
			y += font.h;
			p = 0;
		}
		else if (ch == '\r') {
			x = left;
			p = 0;
		}
		else if (ch == '\t') {
			int d = 8 - (p & 7);
			x += font.w * d;
			p += d;
		}
		else if (ch == ' ') {
			p += 1;
			x += font.w;
		}
		else {
			memcpy(glyph.data, &font.font[unit * ch], unit);
			ibitmap_draw_glyph_font(dst, glyph, clip, x, y, col, bk,
				add, buffer);
			x += font.w;
			p++;
		}
	}

	if (buffer != _buffer) {
		icfree(buffer);
	}
}


//=====================================================================
// 平滑缩放
//=====================================================================
public static int ifilter_shrink_x_c(byte *dstpix, byte *srcpix, 
	int height, int dstpitch, int srcpitch, int dstwidth, int srcwidth)
{
	int srcdiff = srcpitch - (srcwidth * 4);
	int dstdiff = dstpitch - (dstwidth * 4);
	int x, y;
	int xspace = 0x10000 * srcwidth / dstwidth;
	int xrecip = 0;
	long zrecip = 1;

	zrecip <<= 32;
	xrecip = (int)(zrecip / xspace);

	for (y = 0; y < height; y++) {
		uint accumulate[4] = { 0, 0, 0, 0 };
		int xcounter = xspace;
		for (x = 0; x < srcwidth; x++) {
			if (xcounter > 0x10000) {
				accumulate[0] += (uint) *srcpix++;
				accumulate[1] += (uint) *srcpix++;
				accumulate[2] += (uint) *srcpix++;
				accumulate[3] += (uint) *srcpix++;
				xcounter -= 0x10000;
			}	else {
				int xfrac = 0x10000 - xcounter;
				#define ismooth_putpix_x(n) { \
						*dstpix++ = (byte)(((accumulate[n] + ((srcpix[n] \
							* xcounter) >> 16)) * xrecip) >> 16); \
					}
				ismooth_putpix_x(0);
				ismooth_putpix_x(1);
				ismooth_putpix_x(2);
				ismooth_putpix_x(3);
				#undef ismooth_putpix_x
				accumulate[0] = (uint)((*srcpix++ * xfrac) >> 16);
				accumulate[1] = (uint)((*srcpix++ * xfrac) >> 16);
				accumulate[2] = (uint)((*srcpix++ * xfrac) >> 16);
				accumulate[3] = (uint)((*srcpix++ * xfrac) >> 16);
				xcounter = xspace - xfrac;
			}
		}
		srcpix += srcdiff;
		dstpix += dstdiff;
	}
	return 0;
}

public static int ifilter_shrink_y_c(byte *dstpix, byte *srcpix,
	int width, int dstpitch, int srcpitch, int dstheight, int srcheight)
{
	int srcdiff = srcpitch - (width * 4);
	int dstdiff = dstpitch - (width * 4);
	int x, y;
	int yspace = 0x10000 * srcheight / dstheight;
	int yrecip = 0;
	int ycounter = yspace;
	uint *templine;

	long zrecip = 1;
	zrecip <<= 32;
	yrecip = (int)(zrecip / yspace);

	templine = (uint*)icmalloc(dstpitch * 4);
	if (templine == null) return -1;

	memset(templine, 0, dstpitch * 4);

	for (y = 0; y < srcheight; y++) {
		uint *accumulate = templine;
		if (ycounter > 0x10000) {
			for (x = 0; x < width; x++) {
				*accumulate++ += (uint) *srcpix++;
				*accumulate++ += (uint) *srcpix++;
				*accumulate++ += (uint) *srcpix++;
				*accumulate++ += (uint) *srcpix++;
			}
			ycounter -= 0x10000;
		}	else {
			int yfrac = 0x10000 - ycounter;
			for (x = 0; x < width; x++) {
				#define ismooth_putpix_y() { \
					*dstpix++ = (byte) (((*accumulate++ + ((*srcpix++ * \
						ycounter) >> 16)) * yrecip) >> 16); \
				}
				ismooth_putpix_y();
				ismooth_putpix_y();
				ismooth_putpix_y();
				ismooth_putpix_y();
				#undef ismooth_putpix_y
			}
			dstpix += dstdiff;
			accumulate = templine;
			srcpix -= 4 * width;
			for (x = 0; x < width; x++) {
				*accumulate++ = (uint) ((*srcpix++ * yfrac) >> 16);
				*accumulate++ = (uint) ((*srcpix++ * yfrac) >> 16);
				*accumulate++ = (uint) ((*srcpix++ * yfrac) >> 16);
				*accumulate++ = (uint) ((*srcpix++ * yfrac) >> 16);
			}
			ycounter = yspace - yfrac;
		}
		srcpix += srcdiff;
	}

	icfree(templine);
	return 0;
}

public static int ifilter_expand_x_c(byte *dstpix, byte *srcpix, 
	int height, int dstpitch, int srcpitch, int dstwidth, int srcwidth)
{
	int dstdiff = dstpitch - (dstwidth * 4);
	int *xidx0, *xmult0, *xmult1;
	int x, y;
	int factorwidth = 4;

	xidx0 = (int*)icmalloc(dstwidth * 4);
	if (xidx0 == null) return -1;
	xmult0 = (int*)icmalloc(dstwidth * factorwidth);
	xmult1 = (int*)icmalloc(dstwidth * factorwidth);

	if (xmult0 == null || xmult1 == null) {
		icfree(xidx0);
		if (xmult0) icfree(xmult0);
		if (xmult1) icfree(xmult1);
		return -1;
	}

	for (x = 0; x < dstwidth; x++) {
		xidx0[x] = x * (srcwidth - 1) / dstwidth;
		xmult1[x] = 0x10000 * ((x * (srcwidth - 1)) % dstwidth) / dstwidth;
		xmult0[x] = 0x10000 - xmult1[x];
	}

	for (y = 0; y < height; y++) {
		const byte *srcrow0 = srcpix + y * srcpitch;
		for (x = 0; x < dstwidth; x++) {
			const byte *src = srcrow0 + xidx0[x] * 4;
			int xm0 = xmult0[x];
			int xm1 = xmult1[x];
			*dstpix++ = (byte)(((src[0] * xm0) + (src[4] * xm1)) >> 16);
			*dstpix++ = (byte)(((src[1] * xm0) + (src[5] * xm1)) >> 16);
			*dstpix++ = (byte)(((src[2] * xm0) + (src[6] * xm1)) >> 16);
			*dstpix++ = (byte)(((src[3] * xm0) + (src[7] * xm1)) >> 16);
		}
		dstpix += dstdiff;
	}

	icfree(xidx0);
	icfree(xmult0);
	icfree(xmult1);
	return 0;
}

public static int ifilter_expand_y_c(byte *dstpix, byte *srcpix, 
	int width, int dstpitch, int srcpitch, int dstheight, int srcheight)
{
	int x, y;
	for (y = 0; y < dstheight; y++) {
		int yidx0 = y * (srcheight - 1) / dstheight;
		const byte *s0 = srcpix + yidx0 * srcpitch;
		const byte *s1 = s0 + srcpitch;
		int ym1 = 0x10000 * ((y * (srcheight - 1)) % dstheight) / dstheight;
		int ym0 = 0x10000 - ym1;
		for (x = 0; x < width; x++) {
			*dstpix++ = (byte)(((*s0++ * ym0) + (*s1++ * ym1)) >> 16);
			*dstpix++ = (byte)(((*s0++ * ym0) + (*s1++ * ym1)) >> 16);
			*dstpix++ = (byte)(((*s0++ * ym0) + (*s1++ * ym1)) >> 16);
			*dstpix++ = (byte)(((*s0++ * ym0) + (*s1++ * ym1)) >> 16);
		}
		dstpix += dstpitch - 4 * width;
	}

	return 0;
}


// 平滑缩放
int ipixel_smooth_resize(byte *dstpix, byte *srcpix, int dstwidth,
	int srcwidth, int dstheight, int srcheight, int dstpitch, int srcpitch)
{
	byte *temp = null;

	if (srcwidth == dstwidth && srcheight == dstheight) {
		int size, y;
		for (y = 0, size = srcwidth * 4; y < dstheight; y++) {
			memcpy(dstpix + y * dstpitch, srcpix + y * srcpitch, size);
		}
		return 0;
	}

	temp = (byte*)icmalloc((int)srcwidth * dstheight * 4);

	if (temp == null) return -1;

	if (dstheight < srcheight) {
		if (ifilter_shrink_y_c(temp, srcpix, srcwidth, srcwidth * 4, 
			srcpitch, dstheight, srcheight) != 0) {
			icfree(temp);
			return -2;
		}
	}
	else if (dstheight > srcheight) {
		if (ifilter_expand_y_c(temp, srcpix, srcwidth, srcwidth * 4,
			srcpitch, dstheight, srcheight) != 0) {
			icfree(temp);
			return -3;
		}
	}
	else {
		int size, y;
		for (y = 0, size = srcwidth * 4; y < dstheight; y++) {
			memcpy(temp + y * size, srcpix + y * srcpitch, size);
		}
	}

	if (dstwidth < srcwidth) {
		if (ifilter_shrink_x_c(dstpix, temp, dstheight, dstpitch, 
			srcwidth * 4, dstwidth, srcwidth) != 0) {
			icfree(temp);
			return -4;
		}
	}
	else if (dstwidth > srcwidth) {
		if (ifilter_expand_x_c(dstpix, temp, dstheight, dstpitch, 
			srcwidth * 4, dstwidth, srcwidth) != 0) {
			icfree(temp);
			return -5;
		}
	}
	else {
		int size, y;
		for (y = 0, size = dstwidth * 4; y < dstheight; y++) {
			memcpy(dstpix + y * dstpitch, temp + y * size, size);
		}
	}

	icfree(temp);

	return 0;
}


// 平滑过渡：无裁剪
int ibitmap_smooth_resize(IBITMAP *dst, IRECT *rectdst, 
	in IBITMAP src, IRECT *rectsrc)
{
	byte *ss;
	byte *dd;
	int dstwidth, dstheight;
	int srcwidth, srcheight;
	int sfmt, dfmt;
	int retval;
	IBITMAP *tmp1, *tmp2;

	sfmt = ibitmap_pixfmt_guess(src);
	dfmt = ibitmap_pixfmt_guess(dst);

	dstwidth = rectdst.right - rectdst.left;
	srcwidth = rectsrc.right - rectsrc.left;
	dstheight = rectdst.bottom - rectdst.top;
	srcheight = rectsrc.bottom - rectsrc.top;

	#define _ilineptr(x, y) ((byte*)((x).line[y]))

	if (src.bpp == 32 && dst.bpp == 32 && sfmt == dfmt) {
		dd = _ilineptr(dst, rectdst.top) + rectdst.left * 4;
		ss = _ilineptr(src, rectsrc.top) + rectsrc.left * 4;
		retval = ipixel_smooth_resize(dd, ss, dstwidth, srcwidth, dstheight,
			srcheight, dst.pitch, src.pitch);
		return retval;
	}

	if (src.bpp == 32) {
		tmp1 = ibitmap_create(dstwidth, dstheight, 32);
		if (tmp1 == 0) return -20;
		ibitmap_pixfmt_set(tmp1, sfmt);
		dd = _ilineptr(tmp1, 0);
		ss = _ilineptr(src, rectsrc.top) + rectsrc.left * 4;
		retval = ipixel_smooth_resize(dd, ss, dstwidth, srcwidth, 
			dstheight, srcheight, tmp1.pitch, src.pitch);
		if (retval == 0) {
			ibitmap_convert(dst,  rectdst.left, rectdst.top, tmp1, 
				0, 0, dstwidth, dstheight, null, 0);
		}
		ibitmap_release(tmp1);
		return 0;
	}

	if (dst.bpp == 32) {
		tmp1 = ibitmap_create(srcwidth, srcheight, 32);
		if (tmp1 == null) return -30;
		ibitmap_pixfmt_set(tmp1, dfmt);
		ibitmap_convert(tmp1, 0, 0, src, rectsrc.left, rectsrc.top,
			srcwidth, srcheight, null, 0);
		dd = _ilineptr(dst, rectdst.top) + rectdst.left * 4;
		ss = _ilineptr(tmp1, 0);
		retval = ipixel_smooth_resize(dd, ss, dstwidth, srcwidth,
			dstheight, srcheight, dst.pitch, tmp1.pitch);
		ibitmap_release(tmp1);
		return retval;
	}

	tmp1 = ibitmap_create(dstwidth, dstheight, 32);
	tmp2 = ibitmap_create(srcwidth, srcheight, 32);

	if (tmp1 == null || tmp2 == null) {
		if (tmp1) ibitmap_release(tmp1);
		if (tmp2) ibitmap_release(tmp2);
		return -40;
	}

	ibitmap_pixfmt_set(tmp1, IPIX_FMT_A8R8G8B8);
	ibitmap_pixfmt_set(tmp2, IPIX_FMT_A8R8G8B8);

	ibitmap_convert(tmp2, 0, 0, src, rectsrc.left, rectsrc.top,
			srcwidth, srcheight, null, 0);

	dd = _ilineptr(tmp1, 0);
	ss = _ilineptr(tmp2, 0);

	retval = ipixel_smooth_resize(dd, ss, dstwidth, srcwidth,
		dstheight, srcheight, tmp1.pitch, tmp2.pitch);

	if (retval == 0) {
		ibitmap_convert(dst, rectdst.left, rectdst.top, tmp1, 
			0, 0, dstwidth, dstheight, null, 0);
	}

	ibitmap_release(tmp1);
	ibitmap_release(tmp2);

	return retval;
}



// 缩放裁剪
int ibitmap_clip_scale(const IRECT *clipdst, IRECT *clipsrc, 
	IRECT *bound_dst, IRECT *bound_src, int mode)
{
	int dcl = clipdst.left;
	int dct = clipdst.top;
	int dcr = clipdst.right;
	int dcb = clipdst.bottom;
	int scl = clipsrc.left;
	int sct = clipsrc.top;
	int scr = clipsrc.right;
	int scb = clipsrc.bottom;
	int dl = bound_dst.left;
	int dt = bound_dst.top;
	int dr = bound_dst.right;
	int db = bound_dst.bottom;
	int sl = bound_src.left;
	int st = bound_src.top;
	int sr = bound_src.right;
	int sb = bound_src.bottom;
	int dw = dr - dl;
	int dh = db - dt;
	int sw = sr - sl;
	int sh = sb - st;
	int hflip, vflip;
	float fx, fy;
	float ix, iy;
	int d;

    hflip = (mode & IBLIT_HFLIP)? 1 : 0;
    vflip = (mode & IBLIT_VFLIP)? 1 : 0;

	if (dw <= 0 || dh <= 0 || sw <= 0 || sh <= 0) 
		return -1;

	if (dl >= dcl && dt >= dct && dr <= dcr && db <= dcb &&
		sl >= scl && st >= sct && sr <= scr && sb <= scb)
		return 0;

	fx = ((float)dw) / sw;
	fy = ((float)dh) / sh;
	ix = ((float)sw) / dw;
	iy = ((float)sh) / dh;

	// check dest clip: left
	if (dl < dcl) {
		d = dcl - dl;
		dl = dcl;
		if (!hflip) sl += (int)(d * ix);
		else sr -= (int)(d * ix);
	}

	// check dest clip: top
	if (dt < dct) {
		d = dct - dt;
		dt = dct;
		if (!vflip) st += (int)(d * iy);
		else sb -= (int)(d * iy);
	}

	if (dl >= dr || dt >= db || sl >= sr || st >= sb)
		return -2;

	// check dest clip: right
	if (dr > dcr) {
		d = dr - dcr;
		dr = dcr;
		if (!hflip) sr -= (int)(d * ix);
		else sl += (int)(d * ix);
	}

	// check dest clip: bottom
	if (db > dcb) {
		d = db - dcb;
		db = dcb;
		if (!vflip) sb -= (int)(d * iy);
		else st += (int)(d * iy);
	}

	if (dl >= dr || dt >= db || sl >= sr || st >= sb)
		return -3;
	
	// check source clip: left
	if (sl < scl) {
		d = scl - sl;
		sl = scl;
		if (!hflip) dl += (int)(d * fx);
		else dr -= (int)(d * fx);
	}

	// check source clip: top
	if (st < sct) {
		d = sct - st;
		st = sct;
		if (!vflip) dt += (int)(d * fy);
		else db -= (int)(d * fy);
	}

	if (dl >= dr || dt >= db || sl >= sr || st >= sb)
		return -4;

	// check source clip: right
	if (sr > scr) {
		d = sr - scr;
		sr = scr;
		if (!hflip) dr -= (int)(d * fx);
		else dl += (int)(d * fx);
	}

	// check source clip: bottom
	if (sb > scb) {
		d = sb - scb;
		sb = scb;
		if (!vflip) db -= (int)(d * fy);
		else dt += (int)(d * fy);
	}

	if (dl >= dr || dt >= db || sl >= sr || st >= sb)
		return -5;

	bound_dst.left = dl;
	bound_dst.top = dt;
	bound_dst.right = dr;
	bound_dst.bottom = db;
	bound_src.left = sl;
	bound_src.top = st;
	bound_src.right = sr;
	bound_src.bottom = sb;

	return 0;
}

    
// 缩放绘制：接受IBLIT_MASK参数，不接受 IBLIT_HFLIP, IBLIT_VFLIP
int ibitmap_scale(IBITMAP *dst, IRECT *bound_dst, in IBITMAP src,
	const IRECT *bound_src, IRECT *clip, int mode)
{
	const iColorIndex *sindex;
	iColorIndex *dindex;
	IRECT dstclip;
	IRECT srcclip;
	IRECT dstrect;
	IRECT srcrect;
	uint mask;
	int dw, dh;
	int sw, sh;
	int sfmt;
	int dfmt;

	if (clip) {
		dstclip.left = clip.left;
		dstclip.top = clip.top;
		dstclip.right = clip.right;
		dstclip.bottom = clip.bottom;
	}	else {
		dstclip.left = 0;
		dstclip.top = 0;
		dstclip.right = (int)dst.w;
		dstclip.bottom = (int)dst.h;
	}

	srcclip.left = 0;
	srcclip.top = 0;
	srcclip.right = (int)src.w;
	srcclip.bottom = (int)src.h;

	if (bound_dst) {
		dstrect.left = bound_dst.left;
		dstrect.top = bound_dst.top;
		dstrect.right = bound_dst.right;
		dstrect.bottom = bound_dst.bottom;
	}	else {
		dstrect.left = dstclip.left;
		dstrect.top = dstclip.top;
		dstrect.right = dstclip.right;
		dstrect.bottom = dstclip.bottom;
	}

	if (bound_src) {
		srcrect.left = bound_src.left;
		srcrect.top = bound_src.top;
		srcrect.right = bound_src.right;
		srcrect.bottom = bound_src.bottom;
	}	else {
		srcrect.left = srcclip.left;
		srcrect.top = srcclip.top;
		srcrect.right = srcclip.right;
		srcrect.bottom = srcclip.bottom;
	}

	dw = dstrect.right - dstrect.left;
	dh = dstrect.bottom - dstrect.top;
	sw = srcrect.right - srcrect.left;
	sh = srcrect.bottom - srcrect.top;

	dfmt = ibitmap_pixfmt_guess(dst);
	sfmt = ibitmap_pixfmt_guess(src);

	// direct blit
	if (dw == sw && dh == sh && sfmt == dfmt) {
		int dx = dstrect.left;
		int dy = dstrect.top;
		int sx = srcrect.left;
		int sy = srcrect.top;
		mode &= ~IBLIT_CLIP;
		if ((mode & IBLIT_NOCLIP) == 0) {
			if (ibitmap_clipex(dst, &dx, &dy, src, &sx, &sy,
				&sw, &sh, clip, mode))
				return -100;
		}
		return ibitmap_blit(dst, dx,dy, src, sx, sy, sw, sh, mode);
	}

	sindex = (const iColorIndex*)(src.extra);
	dindex = (iColorIndex*)(dst.extra);

	if (sindex == null) sindex = _ipixel_src_index;
	if (dindex == null) dindex = _ipixel_dst_index;

	// direct convert pixel format
	if ((mode & IBLIT_MASK) == 0 && dw == sw && dh == sh && 0) {
		int dx = dstrect.left;
		int dy = dstrect.top;
		int sx = srcrect.left;
		int sy = srcrect.top;
		ibitmap_convert(dst, dx, dy, src, sx, sy, sw, sh, clip, mode);
		return 0;
	}

	// clip area
	if ((mode & IBLIT_NOCLIP) == 0) {
		if (ibitmap_clip_scale(&dstclip, &srcclip, &dstrect, &srcrect, mode))
			return -200;
		dw = dstrect.right - dstrect.left;
		dh = dstrect.bottom - dstrect.top;
		sw = srcrect.right - srcrect.left;
		sh = srcrect.bottom - srcrect.top;
	}

	// use bresenham algorithm for large picture
	if (dst.w >= 32767 || dst.h >= 32767 || 
		src.w >= 32767 || src.h >= 32767) {
		if (sfmt != dfmt) 
			return -300;
		// 2048 ms
		return ibitmap_stretch(dst, dstrect.left, dstrect.top, 
			dstrect.right - dstrect.left, dstrect.bottom - dstrect.top,
			src, srcrect.left, srcrect.top, srcrect.right - srcrect.left,
			srcrect.bottom - srcrect.top, mode);
	}
#if 1
	else if (sfmt == dfmt) {
		cfixed su, sv, du, dv;
		uint cc;
		int i, j;

		#define IBITMAP_SCALE_BITS(bpp, nbytes) { \
			for (j = 0; j < dh; sv += dv, j++) { \
				int srcline = cfixed_to_int(sv); \
				const byte *srcrow = ( byte*)src.line[srcline]; \
				byte *dstrow = (byte*)dst.line[dstrect.top + j]; \
				const byte *srcpix = srcrow; \
				byte *dstpix = dstrow + dstrect.left * nbytes; \
				if (mode & IBLIT_HFLIP) { \
					su = cfixed_from_int(srcrect.right - 1); \
				}	else { \
					su = cfixed_from_int(srcrect.left); \
				} \
				if (mode & IBLIT_MASK) { \
					for (i = dw; i > 0; su += du, i--) { \
						srcpix = srcrow + cfixed_to_int(su) * nbytes; \
						cc = _ipixel_fetch(bpp, srcpix, 0); \
						if (cc != mask) \
							_ipixel_store(bpp, dstpix, 0, cc);  \
						dstpix += nbytes; \
					} \
				}	else { \
					for (i = dw; i > 0; su += du, i--) { \
						srcpix = srcrow + cfixed_to_int(su) * nbytes; \
						cc = _ipixel_fetch(bpp, srcpix, 0); \
						_ipixel_store(bpp, dstpix, 0, cc);  \
						dstpix += nbytes; \
					} \
				} \
			} \
		}
		// 400
		if (mode & IBLIT_VFLIP) {
			sv = cfixed_from_int(srcrect.bottom - 1);
			dv = -cfixed_div(cfixed_from_int(sh), cfixed_from_int(dh));
		}	else {
			sv = cfixed_from_int(srcrect.top);
			dv = cfixed_div(cfixed_from_int(sh), cfixed_from_int(dh));
		}
		if (mode & IBLIT_HFLIP) {
			du = -cfixed_div(cfixed_from_int(sw), cfixed_from_int(dw));
		}	else {
			du = cfixed_div(cfixed_from_int(sw), cfixed_from_int(dw));
		}

		mask = (uint)src.mask;

		switch (src.bpp)
		{
		case  8: IBITMAP_SCALE_BITS( 8, 1); break;
		case 15:
		case 16: IBITMAP_SCALE_BITS(16, 2); break;
		case 24: IBITMAP_SCALE_BITS(24, 3); break;
		case 32: IBITMAP_SCALE_BITS(32, 4); break;
		}

		#undef IBITMAP_SCALE_BITS
	}
#endif
	else {
		const iColorIndex *_ipixel_src_index = sindex;
		iColorIndex *_ipixel_dst_index = dindex;
		cfixed su, sv, du, dv;
		int dstbytes, srcbytes;
		int i, j;

		if (mode & IBLIT_VFLIP) {
			sv = cfixed_from_int(srcrect.bottom - 1);
			dv = -cfixed_div(cfixed_from_int(sh), cfixed_from_int(dh));
		}	else {
			sv = cfixed_from_int(srcrect.top);
			dv = cfixed_div(cfixed_from_int(sh), cfixed_from_int(dh));
		}

		if (mode & IBLIT_HFLIP) {
			du = -cfixed_div(cfixed_from_int(sw), cfixed_from_int(dw));
		}	else {
			du = cfixed_div(cfixed_from_int(sw), cfixed_from_int(dw));
		}

		dstbytes = ipixelfmt[dfmt].pixelbyte;
		srcbytes = ipixelfmt[sfmt].pixelbyte;
		mask = (uint)src.mask;

		for (j = 0; j < dh; sv += dv, j++) {
			int srcline = cfixed_to_int(sv);
			int dstline = dstrect.top + j;
			const byte *srcrow = ( byte*)src.line[srcline];
			byte *dstrow = (byte*)dst.line[dstline];
			const byte *srcpix = null;
			byte *dstpix = dstrow + dstbytes * dstrect.left;
			uint cc, r, g, b, a, index;

			if (mode & IBLIT_HFLIP) {
				su = cfixed_from_int(srcrect.right - 1);
			}	else {
				su = cfixed_from_int(srcrect.left);
			}

			// 3480 ms
			for (i = 0; i < dw; su += du, dstpix += dstbytes, i++) {
				index = cfixed_to_int(su);
				switch (sfmt)
				{
				case IPIX_FMT_A8R8G8B8:
						srcpix = srcrow + index * 4;
						cc = _ipixel_fetch(32, srcpix, 0);
						IRGBA_FROM_A8R8G8B8(cc, r, g, b, a);
						break;
				case IPIX_FMT_A8B8G8R8:
						srcpix = srcrow + index * 4;
						cc = _ipixel_fetch(32, srcpix, 0);
						IRGBA_FROM_A8B8G8R8(cc, r, g, b, a);
						break;
				case IPIX_FMT_R8G8B8A8:
						srcpix = srcrow + index * 4;
						cc = _ipixel_fetch(32, srcpix, 0);
						IRGBA_FROM_R8G8B8A8(cc, r, g, b, a);
						break;
				case IPIX_FMT_B8G8R8A8:
						srcpix = srcrow + index * 4;
						cc = _ipixel_fetch(32, srcpix, 0);
						IRGBA_FROM_B8G8R8A8(cc, r, g, b, a);
						break;
				case IPIX_FMT_X8R8G8B8:
						srcpix = srcrow + index * 4;
						cc = _ipixel_fetch(32, srcpix, 0);
						IRGBA_FROM_X8R8G8B8(cc, r, g, b, a);
						break;
				case IPIX_FMT_X8B8G8R8:
						srcpix = srcrow + index * 4;
						cc = _ipixel_fetch(32, srcpix, 0);
						IRGBA_FROM_X8B8G8R8(cc, r, g, b, a);
						break;
				case IPIX_FMT_R8G8B8X8:
						srcpix = srcrow + index * 4;
						cc = _ipixel_fetch(32, srcpix, 0);
						IRGBA_FROM_R8G8B8X8(cc, r, g, b, a);
						break;
				case IPIX_FMT_B8G8R8X8:
						srcpix = srcrow + index * 4;
						cc = _ipixel_fetch(32, srcpix, 0);
						IRGBA_FROM_B8G8R8X8(cc, r, g, b, a);
						break;
				case IPIX_FMT_P8R8G8B8:
						srcpix = srcrow + index * 4;
						cc = _ipixel_fetch(32, srcpix, 0);
						IRGBA_FROM_P8R8G8B8(cc, r, g, b, a);
						break;
				case IPIX_FMT_R8G8B8:
						srcpix = srcrow + index * 3;
						cc = _ipixel_fetch(24, srcpix, 0);
						IRGBA_FROM_R8G8B8(cc, r, g, b, a);
						break;
				case IPIX_FMT_B8G8R8:
						srcpix = srcrow + index * 3;
						cc = _ipixel_fetch(24, srcpix, 0);
						IRGBA_FROM_B8G8R8(cc, r, g, b, a);
						break;
				case IPIX_FMT_R5G6B5:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_R5G6B5(cc, r, g, b, a);
						break;
				case IPIX_FMT_B5G6R5:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_B5G6R5(cc, r, g, b, a);
						break;
				case IPIX_FMT_X1R5G5B5:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_X1R5G5B5(cc, r, g, b, a);
						break;
				case IPIX_FMT_X1B5G5R5:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_X1B5G5R5(cc, r, g, b, a);
						break;
				case IPIX_FMT_R5G5B5X1:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_R5G5B5X1(cc, r, g, b, a);
						break;
				case IPIX_FMT_B5G5R5X1:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_B5G5R5X1(cc, r, g, b, a);
						break;
				case IPIX_FMT_A1R5G5B5:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_A1R5G5B5(cc, r, g, b, a);
						break;
				case IPIX_FMT_A1B5G5R5:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_A1B5G5R5(cc, r, g, b, a);
						break;
				case IPIX_FMT_R5G5B5A1:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_R5G5B5A1(cc, r, g, b, a);
						break;
				case IPIX_FMT_B5G5R5A1:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_B5G5R5A1(cc, r, g, b, a);
						break;
				case IPIX_FMT_X4R4G4B4:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_X4R4G4B4(cc, r, g, b, a);
						break;
				case IPIX_FMT_X4B4G4R4:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_X4B4G4R4(cc, r, g, b, a);
						break;
				case IPIX_FMT_R4G4B4X4:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_R4G4B4X4(cc, r, g, b, a);
						break;
				case IPIX_FMT_B4G4R4X4:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_B4G4R4X4(cc, r, g, b, a);
						break;
				case IPIX_FMT_A4R4G4B4:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_A4R4G4B4(cc, r, g, b, a);
						break;
				case IPIX_FMT_A4B4G4R4:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_A4B4G4R4(cc, r, g, b, a);
						break;
				case IPIX_FMT_R4G4B4A4:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_R4G4B4A4(cc, r, g, b, a);
						break;
				case IPIX_FMT_B4G4R4A4:
						srcpix = srcrow + index * 2;
						cc = _ipixel_fetch(16, srcpix, 0);
						IRGBA_FROM_B4G4R4A4(cc, r, g, b, a);
						break;
				case IPIX_FMT_C8:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_C8(cc, r, g, b, a);
						break;
				case IPIX_FMT_G8:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_G8(cc, r, g, b, a);
						break;
				case IPIX_FMT_A8:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_A8(cc, r, g, b, a);
						break;
				case IPIX_FMT_R3G3B2:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_R3G3B2(cc, r, g, b, a);
						break;
				case IPIX_FMT_B2G3R3:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_B2G3R3(cc, r, g, b, a);
						break;
				case IPIX_FMT_X2R2G2B2:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_X2R2G2B2(cc, r, g, b, a);
						break;
				case IPIX_FMT_X2B2G2R2:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_X2B2G2R2(cc, r, g, b, a);
						break;
				case IPIX_FMT_R2G2B2X2:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_R2G2B2X2(cc, r, g, b, a);
						break;
				case IPIX_FMT_B2G2R2X2:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_B2G2R2X2(cc, r, g, b, a);
						break;
				case IPIX_FMT_A2R2G2B2:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_A2R2G2B2(cc, r, g, b, a);
						break;
				case IPIX_FMT_A2B2G2R2:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_A2B2G2R2(cc, r, g, b, a);
						break;
				case IPIX_FMT_R2G2B2A2:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_R2G2B2A2(cc, r, g, b, a);
						break;
				case IPIX_FMT_B2G2R2A2:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_B2G2R2A2(cc, r, g, b, a);
						break;
				case IPIX_FMT_X4C4:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_X4C4(cc, r, g, b, a);
						break;
				case IPIX_FMT_X4G4:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_X4G4(cc, r, g, b, a);
						break;
				case IPIX_FMT_X4A4:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_X4A4(cc, r, g, b, a);
						break;
				case IPIX_FMT_C4X4:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_C4X4(cc, r, g, b, a);
						break;
				case IPIX_FMT_G4X4:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_G4X4(cc, r, g, b, a);
						break;
				case IPIX_FMT_A4X4:
						srcpix = srcrow + index * 1;
						cc = _ipixel_fetch(8, srcpix, 0);
						IRGBA_FROM_A4X4(cc, r, g, b, a);
						break;
				default: 
						r = g = b = a = cc = 0;
						break;
				}
				if (mask == cc && (mode & IBLIT_MASK)) continue;
				switch (dfmt)
				{
				case IPIX_FMT_A8R8G8B8:
						cc = IRGBA_TO_A8R8G8B8(r, g, b, a);
						_ipixel_store(32, dstpix, 0, cc);
						break;
				case IPIX_FMT_A8B8G8R8:
						cc = IRGBA_TO_A8B8G8R8(r, g, b, a);
						_ipixel_store(32, dstpix, 0, cc);
						break;
				case IPIX_FMT_R8G8B8A8:
						cc = IRGBA_TO_R8G8B8A8(r, g, b, a);
						_ipixel_store(32, dstpix, 0, cc);
						break;
				case IPIX_FMT_B8G8R8A8:
						cc = IRGBA_TO_B8G8R8A8(r, g, b, a);
						_ipixel_store(32, dstpix, 0, cc);
						break;
				case IPIX_FMT_X8R8G8B8:
						cc = IRGBA_TO_X8R8G8B8(r, g, b, a);
						_ipixel_store(32, dstpix, 0, cc);
						break;
				case IPIX_FMT_X8B8G8R8:
						cc = IRGBA_TO_X8B8G8R8(r, g, b, a);
						_ipixel_store(32, dstpix, 0, cc);
						break;
				case IPIX_FMT_R8G8B8X8:
						cc = IRGBA_TO_R8G8B8X8(r, g, b, a);
						_ipixel_store(32, dstpix, 0, cc);
						break;
				case IPIX_FMT_B8G8R8X8:
						cc = IRGBA_TO_B8G8R8X8(r, g, b, a);
						_ipixel_store(32, dstpix, 0, cc);
						break;
				case IPIX_FMT_P8R8G8B8:
						cc = IRGBA_TO_P8R8G8B8(r, g, b, a);
						_ipixel_store(32, dstpix, 0, cc);
						break;
				case IPIX_FMT_R8G8B8:
						cc = IRGBA_TO_R8G8B8(r, g, b, a);
						_ipixel_store(24, dstpix, 0, cc);
						break;
				case IPIX_FMT_B8G8R8:
						cc = IRGBA_TO_B8G8R8(r, g, b, a);
						_ipixel_store(24, dstpix, 0, cc);
						break;
				case IPIX_FMT_R5G6B5:
						cc = IRGBA_TO_R5G6B5(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_B5G6R5:
						cc = IRGBA_TO_B5G6R5(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_X1R5G5B5:
						cc = IRGBA_TO_X1R5G5B5(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_X1B5G5R5:
						cc = IRGBA_TO_X1B5G5R5(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_R5G5B5X1:
						cc = IRGBA_TO_R5G5B5X1(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_B5G5R5X1:
						cc = IRGBA_TO_B5G5R5X1(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_A1R5G5B5:
						cc = IRGBA_TO_A1R5G5B5(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_A1B5G5R5:
						cc = IRGBA_TO_A1B5G5R5(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_R5G5B5A1:
						cc = IRGBA_TO_R5G5B5A1(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_B5G5R5A1:
						cc = IRGBA_TO_B5G5R5A1(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_X4R4G4B4:
						cc = IRGBA_TO_X4R4G4B4(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_X4B4G4R4:
						cc = IRGBA_TO_X4B4G4R4(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_R4G4B4X4:
						cc = IRGBA_TO_R4G4B4X4(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_B4G4R4X4:
						cc = IRGBA_TO_B4G4R4X4(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_A4R4G4B4:
						cc = IRGBA_TO_A4R4G4B4(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_A4B4G4R4:
						cc = IRGBA_TO_A4B4G4R4(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_R4G4B4A4:
						cc = IRGBA_TO_R4G4B4A4(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_B4G4R4A4:
						cc = IRGBA_TO_B4G4R4A4(r, g, b, a);
						_ipixel_store(16, dstpix, 0, cc);
						break;
				case IPIX_FMT_C8:
						cc = IRGBA_TO_C8(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_G8:
						cc = IRGBA_TO_G8(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_A8:
						cc = IRGBA_TO_A8(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_R3G3B2:
						cc = IRGBA_TO_R3G3B2(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_B2G3R3:
						cc = IRGBA_TO_B2G3R3(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_X2R2G2B2:
						cc = IRGBA_TO_X2R2G2B2(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_X2B2G2R2:
						cc = IRGBA_TO_X2B2G2R2(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_R2G2B2X2:
						cc = IRGBA_TO_R2G2B2X2(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_B2G2R2X2:
						cc = IRGBA_TO_B2G2R2X2(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_A2R2G2B2:
						cc = IRGBA_TO_A2R2G2B2(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_A2B2G2R2:
						cc = IRGBA_TO_A2B2G2R2(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_R2G2B2A2:
						cc = IRGBA_TO_R2G2B2A2(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_B2G2R2A2:
						cc = IRGBA_TO_B2G2R2A2(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_X4C4:
						cc = IRGBA_TO_X4C4(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_X4G4:
						cc = IRGBA_TO_X4G4(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_X4A4:
						cc = IRGBA_TO_X4A4(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_C4X4:
						cc = IRGBA_TO_C4X4(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_G4X4:
						cc = IRGBA_TO_G4X4(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				case IPIX_FMT_A4X4:
						cc = IRGBA_TO_A4X4(r, g, b, a);
						_ipixel_store(8, dstpix, 0, cc);
						break;
				}
			}
		}
	}

	return 0;
}


// 安全 BLIT，支持不同像素格式
int ibitmap_blit2(IBITMAP *dst, int x, int y, in IBITMAP src,
	const IRECT *bound_src, IRECT *clip, int mode)
{
	int clipdst[4], clipsrc[4], bound[4];
	int pixfmt;

	if (dst == null || src == null) 
		return -100;

	if (bound_src == null) {
		bound[0] = 0;
		bound[1] = 0;
		bound[2] = (int)src.w;
		bound[3] = (int)src.h;
	}	else {
		bound[0] = bound_src.left;
		bound[1] = bound_src.top;
		bound[2] = bound_src.right;
		bound[3] = bound_src.bottom;
	}

	if ((mode & IBLIT_NOCLIP) == 0) {
		if (clip) {
			clipdst[0] = clip.left;
			clipdst[1] = clip.top;
			clipdst[2] = clip.right;
			clipdst[3] = clip.bottom;
		}	else {
			clipdst[0] = 0;
			clipdst[1] = 0;
			clipdst[2] = (int)dst.w;
			clipdst[3] = (int)dst.h;
		}
		clipsrc[0] = 0;
		clipsrc[1] = 0;
		clipsrc[2] = (int)src.w;
		clipsrc[3] = (int)src.h;
		if (ibitmap_clip(clipdst, clipsrc, &x, &y, bound, mode))
			return -1;
	}

	pixfmt = ibitmap_pixfmt_guess(dst);

	if (pixfmt != ibitmap_pixfmt_guess(src)) {
		IRECT bounddst;
		IRECT boundsrc;
		int w = bound[2] - bound[0];
		int h = bound[3] - bound[1];
		ipixel_rect_set(&bounddst, x, y, x + w, y + h);
		ipixel_rect_set(&boundsrc, bound[0], bound[1], bound[2], bound[3]);
		mode &= ~IBLIT_CLIP;
		mode |= IBLIT_NOCLIP;
		if ((mode & IBLIT_MASK) == 0) {
			ibitmap_convert(dst, x, y, src, bound[0], bound[1], w, h,
				clip, 0);
			return 0;
		}
		return ibitmap_scale(dst, &bounddst, src, &boundsrc, clip, mode);
	}

	mode &= ~IBLIT_CLIP;
	return ibitmap_blit(dst, x, y, src, bound[0], bound[1],
				bound[2] - bound[0], bound[3] - bound[1], mode);
}


// 重新采样
IBITMAP *ibitmap_resample(in IBITMAP src, IRECT *bound, 
	int newwidth, int newheight, int mode)
{
	IBITMAP *bitmap;
	IRECT srect;
	IRECT drect;

	bitmap = ibitmap_create(newwidth, newheight, src.bpp);

	if (bitmap == null)
		return null;

	ibitmap_pixfmt_set(bitmap, ibitmap_pixfmt_guess(src));

	if (bound == null) {
		bound = &srect;
		srect.left = 0;
		srect.top = 0;
		srect.right = (int)src.w;
		srect.bottom = (int)src.h;
	}

	drect.left = 0;
	drect.top = 0;
	drect.right = newwidth;
	drect.bottom = newheight;

	if (mode == 0) {
		ibitmap_smooth_resize(bitmap, &drect, src, &srect);
	}	
	else {
		ibitmap_scale(bitmap, &drect, src, &srect, null, 0);
	}

	return bitmap;
}


//---------------------------------------------------------------------
// 像素合成
//---------------------------------------------------------------------

// 位图合成
int ibitmap_composite(IBITMAP *dst, int dx, int dy, in IBITMAP src, 
	int sx, int sy, int w, int h, IRECT *clip, int op, int flags)
{
	byte _buffer[IBITMAP_STACK_BUFFER];
	byte *buffer = _buffer;
	int retval = 0, sfmt, dfmt;
	const iColorIndex *sindex;
	const iColorIndex *dindex;
	iPixelComposite composite;
	iFetchProc fetchsrc, fetchdst;
	iStoreProc storedst;
	const void *sline;
	uint *target;
	void *dline;
	int dpitch;
	int spitch;
	int flip, i;

	flip = flags & (IBLIT_HFLIP | IBLIT_VFLIP);

	if ((flags & IBLIT_NOCLIP) == 0) {
		retval = ibitmap_clipex(dst, &dx, &dy, src, &sx, &sy, &w, &h, 
								clip, flip);
		if (retval) return -1;
	}

	composite = ipixel_composite_get(op, 0);
	if (composite == null) return -2;

	if (w * 8 > IBITMAP_STACK_BUFFER) {
		buffer = (byte*)icmalloc(w * 4);
		if (buffer == null) return -3;
	}
	
	sline = (void*)src.line[sy];
	spitch = (int)src.pitch;
	dline = (void*)dst.line[dy];
	dpitch = (int)dst.pitch;

	if ((flip & IBLIT_VFLIP) != 0) {
		sline = src.line[sy + h - 1];
		spitch = -spitch;
	}

	sfmt = ibitmap_pixfmt_guess(src);
	dfmt = ibitmap_pixfmt_guess(dst);

	if ((flags & IPIXEL_COMPOSITE_SRC_A8R8G8B8) && sfmt == IPIX_FMT_X8R8G8B8)
		sfmt = IPIX_FMT_A8R8G8B8;
	if ((flags & IPIXEL_COMPOSITE_DST_A8R8G8B8) && dfmt == IPIX_FMT_X8R8G8B8)
		dfmt = IPIX_FMT_A8R8G8B8;

	if ((flags & IPIXEL_COMPOSITE_FORCE_32) &&
		(sfmt == dfmt) && (ipixelfmt[sfmt].bpp == 32)) {
		sfmt = IPIX_FMT_A8R8G8B8;
		dfmt = IPIX_FMT_A8R8G8B8;
	}

	fetchsrc = ipixel_get_fetch(sfmt, 0);
	fetchdst = ipixel_get_fetch(dfmt, 0);
	storedst = ipixel_get_store(dfmt, 0);

	sindex = (const iColorIndex*)(src.extra);
	dindex = (iColorIndex*)(dst.extra);
	target = (uint*)((byte*)buffer + w * 4);
	
	for (i = 0; i < h; i++) {
		const uint *card;
		if (sfmt == IPIX_FMT_A8R8G8B8 && (flip & IBLIT_HFLIP) == 0) {
			card = ((const uint*)sline) + sx;
		}	else {
			card = (const uint*)buffer;
			fetchsrc(sline, sx, w, (uint*)buffer, sindex);
			if (flip & IBLIT_HFLIP) {
				ipixel_card_reverse((uint*)buffer, w);
			}
		}
		if (dfmt == IPIX_FMT_A8R8G8B8) {
			composite((uint*)dline + dx, card, w);
		}	else {
			fetchdst(dline, dx, w, target, dindex);
			composite(target, card, w);
			storedst(dline, target, dx, w, dindex);
		}
		sline = (byte*)sline + spitch;
		dline = (byte*)dline + dpitch;
	}

	if (buffer != _buffer) {
		icfree(buffer);
		buffer = _buffer;
	}

	return 0;
}


