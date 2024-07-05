
namespace d2;

///**********************************************************************
// * MEMORY ALLOCATOR FUNCTIONS
// **********************************************************************/
//void* (* icmalloc) (size_t size) = malloc;
//void (* icfree) (void* ptr) = free;

public  unsafe partial struct IBITMAP() : IDisposable
{
    public int
     IBITMAP_BLITER_NORM = 0,
     IBITMAP_BLITER_MASK = 1,
     IBITMAP_BLITER_FLIP = 2,

     IBITMAP_FILLER = 3,

     IBITMAP_MALLOC = 4,
     IBITMAP_FREE = 5;

    public uint w;         /* width of the bitmap   */
    public uint h;         /* height of the bitmap  */
    public int bpp;       /* color depth of bitmap */
    public int pitch;     /* pitch of the bitmap   */
    public uint mask;      /* bitmap bit flags      */
    public uint mode;      /* additional mode data  */
    public uint code;      /* bitmap class code     */
    public void* pixel;             /* pixels data in bitmap */
    public void* extra;             /* extra data structure  */
    public void** line;             /* pointer to each line in bitmap */

    /**********************************************************************
     * BASIC BITMAP FUNCTIONS
     **********************************************************************/

    /*
     * ibitmap_create - create bitmap and return the pointer to public struct IBITMAP
     * width  - bitmap width
     * height - bitmap height
     * bpp    - bitmap color depth (8, 16, 24, 32)
     */
    ref IBITMAP ibitmap_create(uint w, uint h, int bpp)
    {


        byte* line;
        int pixelbyte = (bpp + 7) >> 3;
        int i;

        /* check for invalid parametes */
        Debug.Assert(bpp >= 8 && bpp <= 32 && w > 0 && h > 0);

        /* allocate memory for the IBITMAP public struct */
        var sp = ArrayPool<byte>.Shared.Rent(sizeof(IBITMAP));
        ref var bmp = ref MemoryMarshal.AsRef<IBITMAP>(sp);
        /* setup bmp public struct */
        bmp.w = w;
        bmp.h = h;
        bmp.bpp = bpp;
        bmp.pitch = (((int)w * pixelbyte + 7) >> 3) << 3;
        bmp.code = 0;

        /* allocate memory for pixel data */
        NativeMemoryArray<byte> sp1 = new(bmp.pitch * h);//ArrayPool<byte>.Shared.Rent(

        bmp.pixel = sp1.AsSpan();// (byte*)sp1;

        if (bmp.pixel == null)
        {
            icfree(bmp);
            return null;
        }

        bmp.mode = 0;
        bmp.mask = 0;
        bmp.extra = null;

        /* caculate the offset of each scanline */
        bmp.lines = sp1.AsMemoryList();
        bmp.line = (void**)icmalloc(sizeof(void*) * h);//这里似乎获取的是指针大小*高度. 指向行的指针*高度=指向行的指针*高度数量=的指针.
        if (bmp.line == null)
        {
            icfree(bmp.pixel);
            icfree(bmp);
            return null;
        }

        line = (byte*)bmp.pixel;
        for (i = 0; i < h; i++, line += bmp.pitch)
            bmp.line[i] = line;

        return bmp;
    }

    /*
     * ibitmap_release - release bitmap
     * bmp - pointer to the IBITMAP struct
     */
    void ibitmap_release(ref IBITMAP bmp)
    {
        Debug.Assert(bmp);
        if (bmp.pixel) icfree(bmp.pixel);
        bmp.pixel = null;
        if (bmp.line) icfree(bmp.line);
        bmp.line = null;
        bmp.w = 0;
        bmp.h = 0;
        bmp.pitch = 0;
        bmp.bpp = 0;
        icfree(bmp);
    }


    /*
     * ibitmap_clip - clip the rectangle from the src clip and dst clip then
     * caculate a new rectangle which is shared between dst and src cliprect:
     * clipdst  - dest clip array (left, top, right, bottom)
     * clipsrc  - source clip array (left, top, right, bottom)
     * (x, y)   - dest position
     * rectsrc  - source rect
     * mode     - check IBLIT.HFLIP or IBLIT.VFLIP
     * return zero for successful, return non-zero if there is no shared part
     */
    int ibitmap_clip(Span<int> clipdst, Span<int> clipsrc, int* x, int* y,
        Span<int> rectsrc, IBLIT mode)
    {
        var (dcl, dct, dcr, dcb) = (clipdst[0], clipdst[1], clipdst[2], clipdst[3]);       /* dest clip: left top right  bottom   */
        var (scl, sct, scr, scb) = (clipsrc[0], clipsrc[1], clipsrc[2], clipsrc[3]);       /* source clip: left  top  right bottom */
        var (dx, dy) = (*x, *y);                /* dest x,y position     */
        var (sl, st, sr, sb) = (rectsrc[0], rectsrc[1], rectsrc[2], rectsrc[3]); /* source rectangle: left top right bottom */
        int w, h, d;

        var hflip = mode.HasFlag(IBLIT.HFLIP);
        var vflip = mode.HasFlag(IBLIT.VFLIP);

        if (dcr <= dcl || dcb <= dct || scr <= scl || scb <= sct)
            return -1;

        if (sr <= scl || sb <= sct || sl >= scr || st >= scb)
            return -2;

        /* check dest clip: left */
        if (dx < dcl)
        {
            d = dcl - dx;
            dx = dcl;
            if (!hflip) sl += d;
            else sr -= d;
        }

        /* check dest clip: top */
        if (dy < dct)
        {
            d = dct - dy;
            dy = dct;
            if (!vflip) st += d;
            else sb -= d;
        }

        w = sr - sl;
        h = sb - st;

        if (w < 0 || h < 0)
            return -3;

        /* check dest clip: right */
        if (dx + w > dcr)
        {
            d = dx + w - dcr;
            if (!hflip) sr -= d;
            else sl += d;
        }

        /* check dest clip: bottom */
        if (dy + h > dcb)
        {
            d = dy + h - dcb;
            if (!vflip) sb -= d;
            else st += d;
        }

        if (sl >= sr || st >= sb)
            return -4;

        /* check source clip: left */
        if (sl < scl)
        {
            d = scl - sl;
            sl = scl;
            if (!hflip) dx += d;
        }

        /* check source clip: top */
        if (st < sct)
        {
            d = sct - st;
            st = sct;
            if (!vflip) dy += d;
        }

        if (sl >= sr || st >= sb)
            return -5;

        /* check source clip: right */
        if (sr > scr)
        {
            d = sr - scr;
            sr = scr;
            if (hflip) dx += d;
        }

        /* check source clip: bottom */
        if (sb > scb)
        {
            d = sb - scb;
            sb = scb;
            if (vflip) dy += d;
        }

        if (sl >= sr || st >= sb)
            return -6;

        *x = dx;
        *y = dy;
        (rectsrc[0], rectsrc[1], rectsrc[2], rectsrc[3]) = (sl, st, sr, sb);
        return 0;
    }


    /**********************************************************************
     * BLITTER DEFINITION
     **********************************************************************/


    /* set blitters interface to default blitters (c version) */
    public public static delegate*<byte*, int, byte*, int, int, int, int, int, int> ibitmap_blitn = &ibitmap_blitnc;
    public public static delegate*<byte*, int, byte*, int, int, int, int, int, uint, int> ibitmap_blitm = &ibitmap_blitmc;
    public public static delegate*<byte*, int, byte*, int, int, int, int, int, uint, IBLIT, int> ibitmap_blitf = &ibitmap_blitfc;

    //ibitmap_blitter_mask;
    // ibitmap_blitter_norm;
    // ibitmap_blitter_flip;

    public public static  delegate*<byte*, int, int, int, int, uint> ibitmap_filler;

    (int x, int y) Point;

    /**********************************************************************
     * BLITTER INTERFACE - CORE OPERATION
     **********************************************************************/

    /*
     * ibitmap_blit - blit from source bitmap to dest bitmap
     * returns zero for successful, others for error    
     * dst       - dest bitmap to draw on
     * x, y      - target position of dest bitmap to draw on
     * src       - source bitmap 
     * sx, sy    - source rectangle position in source bitmap
     * w, h      - source rectangle width and height in source bitmap
     * mode      - flags of IBLIT.CLIP, IBLIT.MASK, IBLIT.HFLIP, IBLIT.VFLIP...
     */
    int ibitmap_blit(ref IBITMAP dst, int x, int y,
        in IBITMAP src, int sx, int sy, int w, int h, IBLIT mode)
    {

        int pitch1, pitch2;
        int linesize, d1, d2;
        int pixsize, r;
        byte* pixel1;
        byte* pixel2;

        /* check whether parametes is error */
        //Debug.Assert(src && dst);
        Debug.Assert(src.bpp == dst.bpp);

        /* check whether need to clip rectangle */
        if (mode.HasFlag(IBLIT.CLIP))
        {

            Span<int> clipdst = [0, 0, (int)dst.w, (int)dst.h];
            Span<int> clipsrc = [0, 0, (int)src.w, (int)src.h];
            Span<int> rect = [sx, sy, sx + (int)w, sy + (int)h];
            r = ibitmap_clip(clipdst, clipsrc, &x, &y, rect, mode);
            if (r != 0) return 0;
            (sx, sy, w, h) = (rect[0], rect[1], rect[2] - rect[0], rect[3] - rect[1]);
        }

        /* get the size of one pixel */
        pixsize = (src.bpp + 7) >> 3;
        pitch1 = dst.pitch;
        pitch2 = src.pitch;

        /* choose linear offset */
        switch (pixsize)
        {
            case 1:
                linesize = w;
                d1 = x;
                d2 = sx;
                break;
            case 2:
                linesize = (w << 1);
                d1 = (x << 1);
                d2 = (sx << 1);
                break;
            case 3:
                linesize = (w << 1) + w;
                d1 = (x << 1) + x;
                d2 = (sx << 1) + sx;
                break;
            case 4:
                linesize = (w << 2);
                d1 = (x << 2);
                d2 = (sx << 2);
                break;
            default:
                linesize = w * pixsize;
                d1 = x * pixsize;
                d2 = sx * pixsize;
                break;
        }

        /* get the first scanlines of two bitmaps */
        pixel1 = (byte*)dst.line[y] + d1;
        pixel2 = (byte*)src.line[sy] + d2;

        /* detect whether blit with flip */
        if ((mode & (IBLIT.VFLIP | IBLIT.HFLIP)) != 0)
        {
            if (mode.HasFlag(IBLIT.VFLIP))
            {
                pixel2 = (byte*)src.line[sy + h - 1] + d2;
            }
            r = ibitmap_blitf(pixel1, pitch1, pixel2, pitch2, w, h,
                pixsize, linesize, src.mask, mode);
            if (r > 0) ibitmap_blitfc(pixel1, pitch1, pixel2, pitch2, w, h,
                pixsize, linesize, src.mask, mode);
            return 0;
        }

        /* check to use the normal blitter or the transparent blitter */
        if ((mode & IBLIT.MASK) == 0)
        {
            r = ibitmap_blitn(pixel1, pitch1, pixel2, pitch2, w, h,
                pixsize, linesize);
            if (r > 0) ibitmap_blitnc(pixel1, pitch1, pixel2, pitch2, w, h,
                pixsize, linesize);
        }
        else
        {
            r = ibitmap_blitm(pixel1, pitch1, pixel2, pitch2, w, h,
                pixsize, linesize, src.mask);
            if (r > 0) ibitmap_blitmc(pixel1, pitch1, pixel2, pitch2, w, h,
                pixsize, linesize, src.mask);
        }

        return 0;
    }


    /**********************************************************************
     * ibitmap_blitnc - default blitter for normal blit (no transparent)
     **********************************************************************/
    public static int ibitmap_blitnc(byte* dst, int pitch1, byte* src, int pitch2, int w, int h, int pixelbyte, int linesize)
    {

        byte* ss = (byte*)src;
        byte* sd = (byte*)dst;

        /* avoid never use warnings */
        if (pixelbyte == 1) linesize = w;

        /* copy each line */
        for (; h > 0; h--)
        {
            g.memcpy(sd, ss, linesize);
            sd += pitch1;
            ss += pitch2;
        }
        return 0;
    }

    public static uint endian = 0x11223344;// if (((byte*)&endian)[0] != 0x44)
    private public static bool IsBigEndian => (endian & 0x000000ff) != 0x44; //BitConverter.IsLittleEndian;
                                                                      //

    /**********************************************************************
     * ibitmap_blitmc - default blitter for transparent blit
     **********************************************************************/
    public static int ibitmap_blitmc(byte* dst, int pitch1, byte* src, int pitch2,
         int w, int h, int pixelbyte, int linesize, uint mask)
    {
        byte* p1, p2;
        uint key24, c;
        byte key08;
        ushort key16;
        uint imask, k;
        int inc1, inc2, i;
        int intsize, isize;

        /* caculate the inc offset of two array */
        inc1 = pitch1 - linesize;
        inc2 = pitch2 - linesize;

        intsize = sizeof(int);
        isize = sizeof(int);

        /* copying pixel for all color depth */
        switch (pixelbyte)
        {
            /* for 8 bits colors depth */
            case 1:
                key08 = (byte)mask;
                for (; h > 0; h--)
                {                 /* copy each scanline */
                    for (i = w; i > 0; i--)
                    {        /* copy each pixel */
                        if (*(byte*)src != key08) *dst = *src;
                        src++;
                        dst++;
                    }
                    dst += inc1;
                    src += inc2;
                }
                break;

            /* for 15/16 bits colors depth */
            case 2:
                key16 = (ushort)mask;
                for (; h > 0; h--)
                {                 /* copy each scanline */
                    for (i = w; i > 0; i--)
                    {  /* copy each pixel */
                        if (*(ushort*)src != key16)
                            *(ushort*)dst = *(ushort*)src;
                        src += 2;
                        dst += 2;
                    }
                    dst += inc1;
                    src += inc2;
                }
                break;

            /* for 24 bits colors depth */
            case 3:
                if (IsBigEndian) key24 = mask;


                else key24 = ((mask & 0xFFFF) << 8) | ((mask >> 16) & 0xFF);
                p1 = (byte*)dst;
                p2 = (byte*)src;

                /* 32/16 BIT: detect word-size in runtime instead of using */
                /* macros, in order to be compiled in any unknown platform */
                if (intsize == 4)
                {        /* 32/16 BIT PLATFORM */
                    for (; h > 0; h--)
                    {                 /* copy each scanline */
                        for (i = w; i > 0; i--)
                        {  /* copy each pixel */
                            c = (((uint)(*(ushort*)p2)) << 8);
                            if ((c | p2[2]) != key24)
                            {
                                p1[0] = p2[0];
                                p1[1] = p2[1];
                                p1[2] = p2[2];
                            }
                            p1 += 3;
                            p2 += 3;
                        }
                        p1 += inc1;
                        p2 += inc2;
                    }
                }


                else if (isize == 4)
                {    /* 64 BIT PLATFORM */
                    imask = (uint)(key24 & 0xffffffff);
                    for (; h > 0; h--)
                    {                 /* copy each scanline */
                        for (i = w; i > 0; i--)
                        {  /* copy each pixel */
                            k = (((uint)(*(ushort*)p2)) << 8);
                            if ((k | p2[2]) != imask)
                            {
                                p1[0] = p2[0];
                                p1[1] = p2[1];
                                p1[2] = p2[2];
                            }
                            p1 += 3;
                            p2 += 3;
                        }
                        p1 += inc1;
                        p2 += inc2;
                    }
                }
                break;

            /* for 32 bits colors depth */
            case 4:
                /* 32/16 BIT: detect word-size in runtime instead of using */
                /* macros, in order to be compiled in any unknown platform */
                if (intsize == 4)
                {             /* 32/16 BIT PLATFORM */
                    for (; h > 0; h--)
                    {                 /* copy each scanline */
                        for (i = w; i > 0; i--)
                        {  /* copy each pixel */
                            if (*(uint*)src != mask)
                                *(uint*)dst = *(uint*)src;
                            src += 4;
                            dst += 4;
                        }
                        dst += inc1;
                        src += inc2;
                    }
                }


                else if (isize == 4)
                {         /* 64 BIT PLATFORM */
                    imask = (uint)(mask & 0xffffffff);
                    for (; h > 0; h--)
                    {                 /* copy each scanline */
                        for (i = w; i > 0; i--)
                        {  /* copy each pixel */
                            if (*(uint*)src != imask)
                                *(uint*)dst = *(uint*)src;
                            src += 4;
                            dst += 4;
                        }
                        dst += inc1;
                        src += inc2;
                    }
                }
                break;
        }

        return 0;
    }


    /**********************************************************************
     * ibitmap_blitfc - default blitter for flip blit
     **********************************************************************/
    public static int ibitmap_blitfc(byte* dst, int pitch1, byte* src, int pitch2,
         int w, int h, int pixsize, int linesize, uint mask, IBLIT flag)
    {
        byte* p1, p2;
        uint key24, c;
        byte key08;
        ushort key16;
        uint imask, k;
        int inc1, inc2, i;
        int intsize, isize;
        var hasMask = flag.HasFlag(IBLIT.MASK);
        /* flip vertical without mask */
        if ((flag & (IBLIT.MASK | IBLIT.HFLIP)) == 0)
        {
            for (; h > 0; h--)
            {
                g.memcpy(dst, src, linesize);
                dst += pitch1;
                src -= pitch2;
            }
            return 0;
        }

        /* flip vertical with mask */
        if (hasMask && flag.HasFlag(IBLIT.HFLIP))
        {
            for (; h > 0; h--)
            {
                ibitmap_blitmc(dst, pitch1, src, pitch2, w, 1, pixsize,
                    linesize, mask);
                dst += pitch1;
                src -= pitch2;
            }
            return 0;
        }

        /* flip horizon */
        src += linesize - pixsize;
        inc1 = pitch1 - linesize;

        intsize = sizeof(int);
        isize = sizeof(int);

        /* horizon & vertical or horizon only */
        if (flag.HasFlag(IBLIT.VFLIP)) inc2 = -(pitch2 - linesize);
        else inc2 = pitch2 + linesize;

        /* copying pixel for all color depth */
        switch (pixsize)
        {
            /* flip in 8 bits colors depth */
            case 1:
                key08 = (byte)mask;
                for (; h > 0; h--)
                {
                    if (hasMask)
                    {
                        for (i = w; i > 0; i--)
                        {      /* copy each pixel with mask */
                            if (*(byte*)src != key08) *dst = *src;
                            dst++;
                            src--;
                        }
                    }
                    else
                    {
                        for (i = w; i > 0; i--)
                        {      /* copy pixels without mask  */
                            *dst = *src;
                            dst++;
                            src--;
                        }
                    }
                    dst += inc1;
                    src += inc2;
                }
                break;

            /* flip in 15/16 bits colors depth */
            case 2:
                key16 = (ushort)mask;
                for (; h > 0; h--)
                {
                    if (hasMask)
                    {
                        for (i = w; i > 0; i--)
                        {      /* copy each pixel with mask */
                            if (*(ushort*)src != key16)
                                *(ushort*)dst = *(ushort*)src;
                            dst += 2;
                            src -= 2;
                        }
                    }
                    else
                    {
                        for (i = w; i > 0; i--)
                        {      /* copy pixels without mask  */
                            *(ushort*)dst = *(ushort*)src;
                            dst += 2;
                            src -= 2;
                        }
                    }
                    dst += inc1;
                    src += inc2;
                }
                break;

            /* flip in 24 bits colors depth */
            case 3:
                if (IsBigEndian) key24 = mask;


                else key24 = ((mask & 0xFFFF) << 8) | ((mask >> 16) & 0xFF);
                p1 = (byte*)dst;
                p2 = (byte*)src;
                for (; h > 0; h--)
                {                   /* copy each scanline */
                    if (hasMask)
                    {
                        if (intsize == 4)
                        {   /* 32/16 BIT PLATFORM */
                            for (i = w; i > 0; i--)
                            {  /* copy each pixel with mask */
                                c = (((uint)(*(ushort*)p2)) << 8);
                                if ((c | p2[2]) != key24)
                                {
                                    p1[0] = p2[0];
                                    p1[1] = p2[1];
                                    p1[2] = p2[2];
                                }
                                p1 += 3;
                                p2 -= 3;
                            }
                        }


                        else if (isize == 4)
                        {  /* 64 BIT PLATFORM */
                            imask = (uint)(key24 & 0xffffffff);
                            for (i = w; i > 0; i--)
                            {  /* copy each pixel with mask */
                                k = (((uint)(*(ushort*)p2)) << 8);
                                if ((k | p2[2]) != imask)
                                {
                                    p1[0] = p2[0];
                                    p1[1] = p2[1];
                                    p1[2] = p2[2];
                                }
                                p1 += 3;
                                p2 -= 3;
                            }
                        }
                    }
                    else
                    {                     /* copy pixels without mask */
                        for (i = w; i > 0; i--)
                        {
                            p1[0] = p2[0];
                            p1[1] = p2[1];
                            p1[2] = p2[2];
                            p1 += 3;
                            p2 -= 3;
                        }
                    }
                    p1 += inc1;
                    p2 += inc2;
                }
                break;

            /* flip in 32 bits colors depth */
            case 4:
                for (; h > 0; h--)
                {
                    if (intsize == 4)
                    {       /* 32/16 BIT PLATFORM */
                        if (hasMask)
                        {
                            for (i = w; i > 0; i--)
                            {  /* copy each pixel with mask */
                                if (*(uint*)src != mask)
                                    *(uint*)dst = *(uint*)src;
                                dst += 4;
                                src -= 4;
                            }
                        }
                        else
                        {
                            for (i = w; i > 0; i--)
                            {  /* copy pixels without mask  */
                                *(uint*)dst = *(uint*)src;
                                dst += 4;
                                src -= 4;
                            }
                        }
                    }
                    else if (isize == 4)
                    {   /* 64 BIT PLATFORM */
                        imask = (uint)(mask & 0xffffffff);
                        if (hasMask)
                        {
                            for (i = w; i > 0; i--)
                            {  /* copy each pixel with mask */
                                if (*(uint*)src != imask)
                                    *(uint*)dst = *(uint*)src;
                                dst += 4;
                                src -= 4;
                            }
                        }
                        else
                        {
                            for (i = w; i > 0; i--)
                            {  /* copy pixels without mask  */
                                *(uint*)dst = *(uint*)src;
                                dst += 4;
                                src -= 4;
                            }
                        }
                    }
                    dst += inc1;
                    src += inc2;
                }
                break;
        }

        return 0;
    }



    /*
     * ibitmap_setmask - change mask(colorkey) of the bitmap
     * when blit with IBLIT.MASK, this value can be used as the key color to 
     * transparent. you can change bmp.mask directly without calling it.
     */
    int ibitmap_setmask(ref IBITMAP bmp, uint mask)
    {
        bmp.mask = mask;
        return 0;
    }


    /**********************************************************************
     * FILLER DEFINITION
     **********************************************************************/

    /* set filler interface to default filler (c version) */
    delegate*<byte*, int, int, int, int, uint, int> ibitmap_filling = &ibitmap_fillc;


    /* ibitmap_fill - fill the rectangle with given color 
     * returns zero for successful, others for error
     * dst     - dest bitmap to draw on
     * dx, dy  - target position of dest bitmap to draw on
     * w, h    - width and height of the rectangle to be filled
     * col     - indicate the color to fill the rectangle
     */
    int ibitmap_fill(ref IBITMAP dst, int dx, int dy, int w, int h,
        uint col, int noclip)
    {
        int pixsize, r;
        int delta;
        byte* pixel;

        //Debug.Assert(dst);
        if (noclip == 0)
        {
            if (dx >= (int)dst.w || dx + w <= 0 || w < 0) return 0;
            if (dy >= (int)dst.h || dy + h <= 0 || h < 0) return 0;
            if (dx < 0)
            {
                w += dx;
                dx = 0;
            }
            if (dy < 0)
            {
                h += dy;
                dy = 0;
            }
            if (dx + w >= (int)dst.w) w = (int)dst.w - dx;
            if (dy + h >= (int)dst.h) h = (int)dst.h - dy;
        }

        /* get pixel size */
        pixsize = (dst.bpp + 7) >> 3;

        /* choose linear offset */
        switch (pixsize)
        {
            case 1: delta = dx; break;
            case 2: delta = (dx << 1); break;
            case 3: delta = (dx << 1) + dx; break;
            case 4: delta = (dx << 2); break;
            default: delta = dx * pixsize; break;
        }

        /* get the first scanlines of the bitmap */
        pixel = (byte*)dst.line[dy] + delta;

        r = ibitmap_filling(pixel, dst.pitch, w, h, pixsize, col);
        if (r > 0) ibitmap_fillc(pixel, dst.pitch, w, h, pixsize, col);

        return 0;
    }


    /**********************************************************************
     * ibitmap_blitfc - default blitter for flip blit
     **********************************************************************/
    public static int ibitmap_fillc(byte* dst, int pitch, int w, int h, int pixsize,
         uint col)
    {
        ushort col16;
        byte col8, c1, c2, c3;
        uint* p32;
        uint k;
        uint* k32;
        int intsize, isize;
        int inc, i;

        intsize = sizeof(int);
        isize = sizeof(int);

        switch (pixsize)
        {
            /* fill for 8 bits color depth */
            case 1:
                col8 = (byte)(col & 0xff);
                for (; h > 0; h--)
                {
                    g.memset(dst, col8, w);
                    dst += pitch;
                }
                break;

            /* fill for 15/16 bits color depth */
            case 2:
                col16 = (ushort)(col & 0xffff);
                col = (col << 16) | (col & 0xffff);
                if (intsize == 4)
                {        /* 32/16 BIT PLATFORM */
                    for (; h > 0; h--)
                    {
                        p32 = (uint*)dst;
                        for (i = w >> 1; i > 0; i--) *p32++ = col;
                        if ((w & 1) > 0) *(ushort*)p32 = col16;
                        dst += pitch;
                    }
                }


                else if (isize == 4)
                {    /* 64 BIT PLATFORM */
                    k = (uint)(col & 0xffffffff);
                    for (; h > 0; h--)
                    {
                        k32 = (uint*)dst;
                        for (i = w >> 1; i > 0; i--) *k32++ = k;
                        if ((w & 1) > 0) *(ushort*)k32 = col16;
                        dst += pitch;
                    }
                }
                break;

            /* fill for 24 bits color depth */
            case 3:
                inc = pitch - ((w << 1) + w);
                if (IsBigEndian)
                {
                    c1 = (byte)((col >> 16) & 0xff);
                    c2 = (byte)((col >> 8) & 0xff);
                    c3 = (byte)(col & 0xff);
                }
                else
                {
                    c1 = (byte)(col & 0xff);
                    c2 = (byte)((col >> 8) & 0xff);
                    c3 = (byte)((col >> 16) & 0xff);
                }
                for (; h > 0; h--)
                {
                    for (i = w; i > 0; i--)
                    {
                        *dst++ = c1;
                        *dst++ = c2;
                        *dst++ = c3;
                    }
                    dst += inc;
                }
                break;

            /* fill for 32 bits color depth */
            case 4:
                if (intsize == 4)
                {        /* 32/16 BIT PLATFORM */
                    for (; h > 0; h--)
                    {
                        p32 = (uint*)dst;
                        for (i = w; i > 0; i--) *p32++ = col;
                        dst += pitch;
                    }
                }


                else if (isize == 4)
                {    /* 64 BIT PLATFORM */
                    k = (uint)(col & 0xffffffff);
                    for (; h > 0; h--)
                    {
                        k32 = (uint*)dst;
                        for (i = w; i > 0; i--) *k32++ = k;
                        dst += pitch;
                    }
                }
                break;
        }

        return 0;
    }

    /* ibitmap_stretch - copies a bitmap from a source rectangle into a 
     * destination rectangle, stretching or compressing the bitmap to fit 
     * the dimensions of the destination rectangle
     * returns zero for successful, others for invalid rectangle
     * dst       - dest bitmap to draw on
     * dx, dy    - target rectangle position of dest bitmap to draw on
     * dw, dh    - target rectangle width and height in dest bitmap
     * src       - source bitmap 
     * sx, sy    - source rectangle position in source bitmap
     * sw, sh    - source rectangle width and height in source bitmap
     * mode      - flags of IBLIT.MASK, IBLIT.HFLIP, IBLIT.VFLIP...
     * it uses bresenham like algorithm instead of fixed point or indexing 
     * to avoid integer size overflow and memory allocation, just use it 
     * when you don't have a stretch function.
     */
    int ibitmap_stretch(ref IBITMAP dst, int dx, int dy, int dw, int dh,
        in IBITMAP src, int sx, int sy, int sw, int sh, IBLIT mode)
    {
        int dstwidth, dstheight, dstwidth2, dstheight2, srcwidth2, srcheight2;
        int werr, herr, incx, incy, i, j, nbytes;
        uint mask, key24;
        int intsize, isize;

        /* check whether parametes is error */
        //Debug.Assert(src && dst);
        Debug.Assert(src.bpp == dst.bpp);

        if (src.bpp != dst.bpp)
            return -10;

        if (dw == sw && dh == sh)
        {
            mode |= IBLIT.CLIP;
            return ibitmap_blit(ref dst, dx, dy, src, sx, sy, sw, sh, mode);
        }

        if (dx < 0 || dx + dw > (int)dst.w || dy < 0 || dy + dh > (int)dst.h ||
            sx < 0 || sx + sw > (int)src.w || sy < 0 || sy + sh > (int)src.h ||
            sh <= 0 || sw <= 0 || dh <= 0 || dw <= 0)
            return -20;

        dstwidth = dw;
        dstheight = dh;
        dstwidth2 = dw * 2;
        dstheight2 = dh * 2;
        srcwidth2 = sw * 2;
        srcheight2 = sh * 2;

        if (!mode.HasFlag(IBLIT.VFLIP)) incy = 1;
        else
        {
            sy = sy + sh - 1;
            incy = -1;
        }

        herr = srcheight2 - dstheight2;
        nbytes = (src.bpp + 7) / 8;

        isize = sizeof(int);
        intsize = sizeof(int);
        mask = (uint)src.mask;

        for (j = 0; j < dstheight; j++)
        {
            byte* srcrow = (byte*)src.line[sy];
            byte* dstrow = (byte*)dst.line[dy];
            byte* srcpix = srcrow + nbytes * sx;
            byte* dstpix = dstrow + nbytes * dx;
            incx = nbytes;
            if (mode.HasFlag(IBLIT.HFLIP))
            {
                srcpix += (sw - 1) * nbytes;
                incx = -nbytes;
            }
            werr = srcwidth2 - dstwidth2;

            switch (nbytes)
            {
                case 1:
                    {
                        byte mask8;
                        if (!mode.HasFlag(IBLIT.MASK))
                        {
                            for (i = dstwidth; i > 0; i--)
                            {
                                *dstpix++ = *srcpix;
                                while (werr >= 0)
                                {
                                    srcpix += incx;
                                    werr -= dstwidth2;
                                }
                                werr += srcwidth2;
                            }
                        }
                        else
                        {
                            mask8 = (byte)(src.mask & 0xff);
                            for (i = dstwidth; i > 0; i--)
                            {
                                if (*srcpix != mask8) *dstpix = *srcpix;
                                dstpix++;
                                while (werr >= 0)
                                {
                                    srcpix += incx;
                                    werr -= dstwidth2;
                                }
                                werr += srcwidth2;
                            }
                        }
                    }
                    break;

                case 2:
                    {
                        ushort mask16;
                        if (!mode.HasFlag(IBLIT.MASK))
                        {
                            for (i = dstwidth; i > 0; i--)
                            {
                                *((ushort*)dstpix) = *((ushort*)srcpix);
                                dstpix += 2;
                                while (werr >= 0)
                                {
                                    srcpix += incx;
                                    werr -= dstwidth2;
                                }
                                werr += srcwidth2;
                            }
                        }
                        else
                        {
                            mask16 = (ushort)(src.mask & 0xffff);
                            for (i = dstwidth; i > 0; i--)
                            {
                                if (*((ushort*)srcpix) != mask16)
                                    *((ushort*)dstpix) = *((ushort*)srcpix);
                                dstpix += 2;
                                while (werr >= 0)
                                {
                                    srcpix += incx;
                                    werr -= dstwidth2;
                                }
                                werr += srcwidth2;
                            }
                        }
                    }
                    break;



                case 3:
                    if (IsBigEndian) key24 = mask;


                    else key24 = ((mask & 0xFFFF) << 8) | ((mask >> 16) & 0xFF);
                    if (!mode.HasFlag(IBLIT.MASK))
                    {
                        for (i = dstwidth; i > 0; i--)
                        {
                            dstpix[0] = srcpix[0];
                            dstpix[1] = srcpix[1];
                            dstpix[2] = srcpix[2];
                            dstpix += 3;
                            while (werr >= 0)
                            {
                                srcpix += incx;
                                werr -= dstwidth2;
                            }
                            werr += srcwidth2;
                        }
                    }
                    else if (intsize == 4)
                    {
                        uint intmask, k;
                        intmask = key24 & 0xffffff;
                        for (i = dstwidth; i > 0; i--)
                        {
                            k = (((uint)(*(ushort*)srcpix)) << 8);
                            if ((k | srcpix[2]) != intmask)
                            {
                                dstpix[0] = srcpix[0];
                                dstpix[1] = srcpix[1];
                                dstpix[2] = srcpix[2];
                            }
                            dstpix += 3;
                            while (werr >= 0)
                            {
                                srcpix += incx;
                                werr -= dstwidth2;
                            }
                            werr += srcwidth2;
                        }
                    }

                    else if (isize == 4)
                    {
                        uint imask, k;
                        imask = key24 & 0xffffff;
                        for (i = dstwidth; i > 0; i--)
                        {
                            k = (((uint)(*(ushort*)srcpix)) << 8);
                            if ((k | srcpix[2]) != imask)
                            {
                                dstpix[0] = srcpix[0];
                                dstpix[1] = srcpix[1];
                                dstpix[2] = srcpix[2];
                            }
                            dstpix += 3;
                            while (werr >= 0)
                            {
                                srcpix += incx;
                                werr -= dstwidth2;
                            }
                            werr += srcwidth2;
                        }
                    }
                    break;



                case 4:
                    if (intsize == 4)
                    {
                        uint maskint;
                        if (!mode.HasFlag(IBLIT.MASK))
                        {
                            for (i = dstwidth; i > 0; i--)
                            {
                                *((uint*)dstpix) =
                                                    *((uint*)srcpix);
                                dstpix += 4;
                                while (werr >= 0)
                                {
                                    srcpix += incx;
                                    werr -= dstwidth2;
                                }
                                werr += srcwidth2;
                            }
                        }
                        else
                        {
                            maskint = (uint)(src.mask);
                            for (i = dstwidth; i > 0; i--)
                            {
                                if (*((uint*)srcpix) != maskint)
                                    *((uint*)dstpix) =
                                                    *((uint*)srcpix);
                                dstpix += 4;
                                while (werr >= 0)
                                {
                                    srcpix += incx;
                                    werr -= dstwidth2;
                                }
                                werr += srcwidth2;
                            }
                        }
                    }


                    else if (isize == 4)
                    {
                        uint maskint;
                        if (!mode.HasFlag(IBLIT.MASK))
                        {
                            for (i = dstwidth; i > 0; i--)
                            {
                                *((uint*)dstpix) = *((uint*)srcpix);
                                dstpix += 4;
                                while (werr >= 0)
                                {
                                    srcpix += incx;
                                    werr -= dstwidth2;
                                }
                                werr += srcwidth2;
                            }
                        }
                        else
                        {
                            maskint = (uint)(src.mask);
                            for (i = dstwidth; i > 0; i--)
                            {
                                if (*((uint*)srcpix) != maskint)
                                    *((uint*)dstpix) =
                                                    *((uint*)srcpix);
                                dstpix += 4;
                                while (werr >= 0)
                                {
                                    srcpix += incx;
                                    werr -= dstwidth2;
                                }
                                werr += srcwidth2;
                            }
                        }
                    }


                    else
                    {
                        Debug.Assert(false);
                    }
                    break;
            }

            while (herr >= 0)
            {
                sy += incy, herr -= dstheight2;
            }

            herr += srcheight2;
            dy++;
        }

        return 0;
    }


    public void Dispose()
    {
        throw new NotImplementedException();
    }



    /**********************************************************************
     * Bitmap Basic Interface
     **********************************************************************/
    delegate*<byte*, int, in byte*, int, int, int, int, int, uint> ibitmap_blitter_mask;
    delegate*<byte*, int, in byte*, int, int, int, int, int> ibitmap_blitter_norm;
    delegate*<byte*, int, in byte*, int, int, int, int, int, uint, int> ibitmap_blitter_flip;
    delegate*<byte*, int, int, int, int, uint> ibitmap_filler;


    //ibitmap_blitn = &ibitmap_blitnc;
    //            ibitmap_blitm = &ibitmap_blitmc;
    //            ibitmap_blitf = &ibitmap_blitfc;
    //            ibitmap_filling = &ibitmap_fillc;
    //void* ibitmap_funcget(int functionid, int version)
    //{
    //    void* proc = null;
    //    switch (functionid)
    //    {
    //        case IBITMAP_BLITER_NORM:
    //            proc = (version) ? (void*)ibitmap_blitnc : (void*)ibitmap_blitn;
    //            break;
    //        case IBITMAP_BLITER_MASK:
    //            proc = (version) ? (void*)ibitmap_blitmc : (void*)ibitmap_blitm;
    //            break;
    //        case IBITMAP_BLITER_FLIP:
    //            proc = (version) ? (void*)ibitmap_blitfc : (void*)ibitmap_blitf;
    //            break;
    //        case IBITMAP_FILLER:
    //            proc = (version) ? (void*)ibitmap_fillc : (void*)ibitmap_filling;
    //            break;
    //    }
    //    return proc;
    //}


};

