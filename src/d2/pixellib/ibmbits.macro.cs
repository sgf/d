using d2;
using ILGPU.Algorithms.Optimization.Optimizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d2.pixellib
{
    public class ibmbits.macro
    {
        

#define ICLIP_256(x) ((((255 - ((int)(x))) >> 31) | ((int)(x))) & 255)
#define ICLIP_FAST(x) ( (uint) ( (byte) ((x) | (0 - ((x) >> 8))) ) )
#define ICLIP_ZERO(x) ((-((int)(x)) >> 31) & ((int)(x)))

#define IPIXEL_FORMAT_BPP(pixfmt)      ipixelfmt[pixfmt].bpp
#define IPIXEL_FORMAT_TYPE(pixfmt)     ipixelfmt[pixfmt].type
#define IPIXEL_FORMAT_NAME(pixfmt)     ipixelfmt[pixfmt].name
#define IPIXEL_FORMAT_ALPHA(pixfmt)    ipixelfmt[pixfmt].alpha

#define IPIXEL_FORMAT_RMASK(pixfmt)    ipixelfmt[pixfmt].rmask
#define IPIXEL_FORMAT_GMASK(pixfmt)    ipixelfmt[pixfmt].gmask
#define IPIXEL_FORMAT_BMASK(pixfmt)    ipixelfmt[pixfmt].bmask
#define IPIXEL_FORMAT_AMASK(pixfmt)    ipixelfmt[pixfmt].amask

#define IPIXEL_FORMAT_RSHIFT(pixfmt)   ipixelfmt[pixfmt].rshift
#define IPIXEL_FORMAT_GSHIFT(pixfmt)   ipixelfmt[pixfmt].gshift
#define IPIXEL_FORMAT_BSHIFT(pixfmt)   ipixelfmt[pixfmt].bshift
#define IPIXEL_FORMAT_ASHIFT(pixfmt)   ipixelfmt[pixfmt].ashift

#define IPIXEL_FORMAT_RLOSS(pixfmt)    ipixelfmt[pixfmt].rloss
#define IPIXEL_FORMAT_GLOSS(pixfmt)    ipixelfmt[pixfmt].gloss
#define IPIXEL_FORMAT_BLOSS(pixfmt)    ipixelfmt[pixfmt].bloss
#define IPIXEL_FORMAT_ALOSS(pixfmt)    ipixelfmt[pixfmt].aloss


/**********************************************************************
 * BITS ACCESSING
 **********************************************************************/
public const int

 IPIXEL_ACCESS_MODE_NORMAL = 0,		/* fast mode, with lut */
 IPIXEL_ACCESS_MODE_ACCURATE = 1,		/* fast mode without lut */
 IPIXEL_ACCESS_MODE_BUILTIN = 2,		/* default accurate */


 IPIXEL_PROC_TYPE_FETCH = 0,
 IPIXEL_PROC_TYPE_STORE = 1,
 IPIXEL_PROC_TYPE_FETCHPIXEL = 2;



    /**********************************************************************
     * SPAN DRAWING
     **********************************************************************/

    /* span drawing proc */
    delegate void iSpanDrawProc(void* bits, int startx, int w, uint* card, byte* cover, iColorIndex* index);


    /* blending options */
    public const int
     IPIXEL_BLEND_OP_COPY = 0,
     IPIXEL_BLEND_OP_BLEND = 1,
     IPIXEL_BLEND_OP_SRCOVER = 2,
     IPIXEL_BLEND_OP_ADD = 3,

     IPIXEL_FLIP_NONE = 0,
     IPIXEL_FLIP_HFLIP = 1,
     IPIXEL_FLIP_VFLIP = 2;


    /**********************************************************************
     * HLINE
     **********************************************************************/

    /* draw hline routine */
    delegate void iHLineDrawProc(void* bits, int startx, int w, uint col, byte* cover, iColorIndex* index);


    /**********************************************************************
     * PIXEL BLIT
     **********************************************************************/
    public const int

    IPIXEL_BLIT_NORMAL = 0,
    IPIXEL_BLIT_MASK = 4;


    /* mask blit procedure: with or without mask (colorkey) */
    delegate int iBlitProc(void* dst, int dpitch, int dx, void* src,
        int spitch, int sx, int w, int h, uint mask, int flip);


    /**********************************************************************
     * CONVERTING
     **********************************************************************/

    /* pixel format converting procedure */
    delegate int iPixelCvt(void* dbits, int dpitch, int dx, void* sbits, int spitch, int sx, int w, int h, uint mask, int flip, iColorIndex* dindex, iColorIndex* sindex);

    public const int
     IPIXEL_CVT_FLAG = 8;



    /**********************************************************************
     * FREE FORMAT CONVERT
     **********************************************************************/

    /* free format reader procedure */
    delegate int iPixelFmtReader(ref iPixelFmt fmt,

            void* bits, int x, int w, uint* card);

    /* free format writer procedure */
    delegate int iPixelFmtWriter(ref iPixelFmt fmt, void* bits, int x, int w, uint* card);

    /* byte permute for 24/32 bits */
    delegate int iPixelFmtPermute(int dbpp, byte* dst, int w, int step,
            int sbpp, byte* src, int* pos, uint mask, int mode);



    /**********************************************************************
     * COMPOSITE
     **********************************************************************/

    /* pixel composite procedure */
    delegate void iPixelComposite(uint* dst, uint* src, int width);

    /* pixel composite operator */
    public const int
     IPIXEL_OP_SRC = 0,
     IPIXEL_OP_DST = 1,
     IPIXEL_OP_CLEAR = 2,
     IPIXEL_OP_BLEND = 3,
     IPIXEL_OP_ADD = 4,
     IPIXEL_OP_SUB = 5,
     IPIXEL_OP_SUB_INV = 6,
     IPIXEL_OP_XOR = 7,
     IPIXEL_OP_PLUS = 8,
     IPIXEL_OP_SRC_ATOP = 9,
     IPIXEL_OP_SRC_IN = 10,
     IPIXEL_OP_SRC_OUT = 11,
     IPIXEL_OP_SRC_OVER = 12,
     IPIXEL_OP_DST_ATOP = 13,
     IPIXEL_OP_DST_IN = 14,
     IPIXEL_OP_DST_OUT = 15,
     IPIXEL_OP_DST_OVER = 16,
     IPIXEL_OP_PREMUL_XOR = 17,
     IPIXEL_OP_PREMUL_PLUS = 18,
     IPIXEL_OP_PREMUL_SRC_OVER = 19,
     IPIXEL_OP_PREMUL_SRC_IN = 20,
     IPIXEL_OP_PREMUL_SRC_OUT = 21,
     IPIXEL_OP_PREMUL_SRC_ATOP = 22,
     IPIXEL_OP_PREMUL_DST_OVER = 23,
     IPIXEL_OP_PREMUL_DST_IN = 24,
     IPIXEL_OP_PREMUL_DST_OUT = 25,
     IPIXEL_OP_PREMUL_DST_ATOP = 26,
     IPIXEL_OP_PREMUL_BLEND = 27,
     IPIXEL_OP_ALLANON = 28,
     IPIXEL_OP_TINT = 29,
     IPIXEL_OP_DIFF = 30,
     IPIXEL_OP_DARKEN = 31,
     IPIXEL_OP_LIGHTEN = 32,
     IPIXEL_OP_SCREEN = 33,
     IPIXEL_OP_OVERLAY = 34;


    /**********************************************************************
     * MACRO: Pixel Fetching & Storing
     **********************************************************************/
#define _ipixel_fetch_8(ptr, offset)  (((const byte *)(ptr))[offset])
#define _ipixel_fetch_16(ptr, offset) (((const ushort*)(ptr))[offset])
#define _ipixel_fetch_32(ptr, offset) (((const uint*)(ptr))[offset])

#define _ipixel_fetch_24_lsb(ptr, offset) \
    ((((uint)(((byte*)(ptr)) + (offset)* 3)[0]) <<  0 ) | \
      (((uint)(((byte*)(ptr)) + (offset)* 3)[1]) <<  8 ) | \
      (((uint)(((byte*)(ptr)) + (offset)* 3)[2]) << 16 ))

#define _ipixel_fetch_24_msb(ptr, offset) \
    ((((uint)(((byte*)(ptr)) + (offset)* 3)[0]) << 16 ) | \
      (((uint)(((byte*)(ptr)) + (offset)* 3)[1]) <<  8 ) | \
      (((uint)(((byte*)(ptr)) + (offset)* 3)[2]) <<  0 ))

#define _ipixel_fetch_4_lsb(ptr, offset) \
    (((offset) & 1)? (_ipixel_fetch_8(ptr, (offset) >> 1) >> 4) : \
     (_ipixel_fetch_8(ptr, (offset) >> 1) & 0xf))

#define _ipixel_fetch_4_msb(ptr, offset) \
    (((offset) & 1)? (_ipixel_fetch_8(ptr, (offset) >> 1) & 0xf) : \
     (_ipixel_fetch_8(ptr, (offset) >> 1) >> 4))

#define _ipixel_fetch_1_lsb(ptr, offset) \
    ((_ipixel_fetch_8(ptr, (offset) >> 3) >> ((offset) & 7)) & 1)

#define _ipixel_fetch_1_msb(ptr, offset) \
    ((_ipixel_fetch_8(ptr, (offset) >> 3) >> (7 - ((offset) & 7))) & 1)


#define _ipixel_store_8(ptr, off, c)  (((byte *)(ptr))[off] = (byte)(c))
#define _ipixel_store_16(ptr, off, c) (((ushort*)(ptr))[off] = (ushort)(c))
#define _ipixel_store_32(ptr, off, c) (((uint*)(ptr))[off] = (uint)(c))

#define _ipixel_store_24_lsb(ptr, off, c) do { \
        ((byte*)(ptr))[(off) * 3 + 0] = (byte) (((c) >>  0) & 0xff); \
        ((byte*)(ptr))[(off) * 3 + 1] = (byte) (((c) >>  8) & 0xff); \
        ((byte*)(ptr))[(off) * 3 + 2] = (byte) (((c) >> 16) & 0xff); \
    } while (0)

#define _ipixel_store_24_msb(ptr, off, c)  do { \
    ((byte*)(ptr))[(off) * 3 + 0] = (byte)(((c) >> 16) & 0xff); \
        ((byte*)(ptr))[(off) * 3 + 1] = (byte)(((c) >> 8) & 0xff); \
        ((byte*)(ptr))[(off) * 3 + 2] = (byte)(((c) >> 0) & 0xff); \
    }   while (0)


    _ipixel_store_4_lsb(ptr, off, c)

        {
    do
    {
        uint __byte_off = (off) >> 1;
        uint __byte_value = _ipixel_fetch_8(ptr, __byte_off);
        uint __v4 = (c) & 0xf;
        if ((off) & 1)
        {
            __byte_value = ((__byte_value) & 0x0f) | ((__v4) << 4);
        }
        else
        {
            __byte_value = ((__byte_value) & 0xf0) | ((__v4));
        }
        _ipixel_store_8(ptr, __byte_off, __byte_value);
    } while (0)

    }

#define _ipixel_store_4_msb(ptr, off, c) do { \
uint __byte_off = (off) >> 1; \
        uint __byte_value = _ipixel_fetch_8(ptr, __byte_off); \
        uint __v4 = (c) & 0xf; \
        if ((off) & 1)
{ \
            __byte_value = ((__byte_value) & 0xf0) | ((__v4)); \
        }
else
{ \
            __byte_value = ((__byte_value) & 0x0f) | ((__v4) << 4); \
        } \
        _ipixel_store_8(ptr, __byte_off, __byte_value); \
    }   while (0)

#define _ipixel_store_1_lsb(ptr, off, c) do { \
    uint __byte_off = (off) >> 3; \
        uint __byte_bit = (off) & 7; \
        uint __byte_value = _ipixel_fetch_8(ptr, __byte_off); \
        __byte_value &= ((~(1 << __byte_bit)) & 0xff); \
        __byte_value |= ((c) & 1) << __byte_bit; \
        _ipixel_store_8(ptr, __byte_off, __byte_value); \
    }   while (0)

#define _ipixel_store_1_msb(ptr, off, c) do { \
    uint __byte_off = (off) >> 3; \
        uint __byte_bit = 7 - ((off) & 7); \
        uint __byte_value = _ipixel_fetch_8(ptr, __byte_off); \
        __byte_value &= ((~(1 << __byte_bit)) & 0xff); \
        __byte_value |= ((c) & 1) << __byte_bit; \
        _ipixel_store_8(ptr, __byte_off, __byte_value); \
    }   while (0)



#define IFETCH24_LSB(a) ( \
    (((uint)(((byte*)(a))[0]))) | \
	(((uint)(((byte*)(a))[1])) << 8) | \
	(((uint)(((byte*)(a))[2])) << 16) )

#define IFETCH24_MSB(a) ( \
	(((uint)(((byte*)(a))[0])) << 16) | \
	(((uint)(((byte*)(a))[1])) << 8) | \
	(((uint)(((byte*)(a))[2]))) )

#define ISTORE24_LSB(a, c) do { \
		((byte*)(a))[0] = (byte)(((c)) & 0xff); \
		((byte*)(a))[1] = (byte)(((c) >> 8) & 0xff); \
		((byte*)(a))[2] = (byte)(((c) >> 16) & 0xff); \
	}	while (0)

#define ISTORE24_MSB(a, c) do { \
    ((byte*)(a))[0] = (byte)(((c) >> 16) & 0xff); \
		((byte*)(a))[1] = (byte)(((c) >> 8) & 0xff); \
		((byte*)(a))[2] = (byte)(((c)) & 0xff); \
	}	while (0)



#if IPIXEL_BIG_ENDIAN
#define _ipixel_fetch_24 _ipixel_fetch_24_msb
#define _ipixel_store_24 _ipixel_store_24_msb
#define _ipixel_fetch_4  _ipixel_fetch_4_msb
#define _ipixel_store_4  _ipixel_store_4_msb
#define _ipixel_fetch_1  _ipixel_fetch_1_msb
#define _ipixel_store_1  _ipixel_store_1_msb
#define IFETCH24         IFETCH24_MSB
#define ISTORE24         ISTORE24_MSB
#else
#define _ipixel_fetch_24 _ipixel_fetch_24_lsb
#define _ipixel_store_24 _ipixel_store_24_lsb
#define _ipixel_fetch_4 _ipixel_fetch_4_lsb
#define _ipixel_store_4 _ipixel_store_4_lsb
#define _ipixel_fetch_1 _ipixel_fetch_1_lsb
#define _ipixel_store_1 _ipixel_store_1_lsb
#define IFETCH24         IFETCH24_LSB
#define ISTORE24         ISTORE24_LSB
#endif

#define _ipixel_fetch(nbits, ptr, off) _ipixel_fetch_##nbits(ptr, off)
#define _ipixel_store(nbits, ptr, off, c) _ipixel_store_##nbits(ptr, off, c)



    /**********************************************************************
     * MACRO: Pixel Assemble & Disassemble 
     **********************************************************************/

    /* assemble 32 bits */
#define _ipixel_asm_8888(a, b, c, d) ((uint)( \
    ((uint)(a) << 24) | \
            ((uint)(b) << 16) | \
            ((uint)(c) << 8) | \
            ((uint)(d) << 0)))

/* assemble 24 bits */
#define _ipixel_asm_888(a, b, c) ((uint)( \
            ((uint)(a) << 16) | \
            ((uint)(b) << 8) | \
            ((uint)(c) << 0)))

/* assemble 16 bits */
#define _ipixel_asm_1555(a, b, c, d) ((ushort)( \
            ((ushort)((a) & 0x80) << 8) | \
            ((ushort)((b) & 0xf8) << 7) | \
            ((ushort)((c) & 0xf8) << 2) | \
            ((ushort)((d) & 0xf8) >> 3)))

#define _ipixel_asm_5551(a, b, c, d) ((ushort)( \
            ((ushort)((a) & 0xf8) << 8) | \
            ((ushort)((b) & 0xf8) << 3) | \
            ((ushort)((c) & 0xf8) >> 2) | \
            ((ushort)((d) & 0x80) >> 7)))

#define _ipixel_asm_565(a, b, c)  ((ushort)( \
            ((ushort)((a) & 0xf8) << 8) | \
            ((ushort)((b) & 0xfc) << 3) | \
            ((ushort)((c) & 0xf8) >> 3)))

#define _ipixel_asm_4444(a, b, c, d) ((ushort)( \
            ((ushort)((a) & 0xf0) << 8) | \
            ((ushort)((b) & 0xf0) << 4) | \
            ((ushort)((c) & 0xf0) << 0) | \
            ((ushort)((d) & 0xf0) >> 4)))

/* assemble 8 bits */
#define _ipixel_asm_332(a, b, c) ((byte)( \
            ((byte)((a) & 0xe0) >> 0) | \
            ((byte)((b) & 0xe0) >> 3) | \
            ((byte)((c) & 0xf0) >> 6)))

#define _ipixel_asm_233(a, b, c) ((byte)( \
            ((byte)((a) & 0xc0) >> 0) | \
            ((byte)((b) & 0xe0) >> 2) | \
            ((byte)((c) & 0xe0) >> 5)))

#define _ipixel_asm_2222(a, b, c, d) ((byte)( \
            ((byte)((a) & 0xc0) >> 0) | \
            ((byte)((b) & 0xc0) >> 2) | \
            ((byte)((c) & 0xc0) >> 4) | \
            ((byte)((d) & 0xc0) >> 6)))

/* assemble 4 bits */
#define _ipixel_asm_1111(a, b, c, d) ((byte)( \
            ((byte)((a) & 0x80) >> 4) | \
            ((byte)((a) & 0x80) >> 5) | \
            ((byte)((a) & 0x80) >> 6) | \
            ((byte)((a) & 0x80) >> 7)))

#define _ipixel_asm_121(a, b, c) ((byte)( \
            ((byte)((a) & 0x80) >> 4) | \
            ((byte)((b) & 0xc0) >> 5) | \
            ((byte)((c) & 0xc0) >> 7)))


/* disassemble 32 bits */
#define _ipixel_disasm_8888(x, a, b, c, d) do { \
            (a) = ((x) >> 24) & 0xff; \
            (b) = ((x) >> 16) & 0xff; \
            (c) = ((x) >> 8) & 0xff; \
            (d) = ((x) >> 0) & 0xff; \
        }   while (0)

#define _ipixel_disasm_888X(x, a, b, c) do { \
    (a) = ((x) >> 24) & 0xff; \
            (b) = ((x) >> 16) & 0xff; \
            (c) = ((x) >> 8) & 0xff; \
        }   while (0)

#define _ipixel_disasm_X888(x, a, b, c) do { \
    (a) = ((x) >> 16) & 0xff; \
            (b) = ((x) >> 8) & 0xff; \
            (c) = ((x) >> 0) & 0xff; \
        }   while (0)

    /* disassemble 24 bits */
#define _ipixel_disasm_888(x, a, b, c) do { \
    (a) = ((x) >> 16) & 0xff; \
            (b) = ((x) >> 8) & 0xff; \
            (c) = ((x) >> 0) & 0xff; \
        }   while (0)

    /* disassemble 16 bits */
#define _ipixel_disasm_1555(x, a, b, c, d) do { \
    (a) = _ipixel_scale_1[(x) >> 15]; \
            (b) = _ipixel_scale_5[((x) >> 10) & 0x1f]; \
            (c) = _ipixel_scale_5[((x) >> 5) & 0x1f]; \
            (d) = _ipixel_scale_5[((x) >> 0) & 0x1f]; \
        }   while (0)

#define _ipixel_disasm_X555(x, a, b, c) do { \
    (a) = _ipixel_scale_5[((x) >> 10) & 0x1f]; \
            (b) = _ipixel_scale_5[((x) >> 5) & 0x1f]; \
            (c) = _ipixel_scale_5[((x) >> 0) & 0x1f]; \
        }   while (0)

#define _ipixel_disasm_5551(x, a, b, c, d) do { \
    (a) = _ipixel_scale_5[((x) >> 11) & 0x1f]; \
            (b) = _ipixel_scale_5[((x) >> 6) & 0x1f]; \
            (c) = _ipixel_scale_5[((x) >> 1) & 0x1f]; \
            (d) = _ipixel_scale_1[((x) >> 0) & 0x01]; \
        }   while (0)

#define _ipixel_disasm_555X(x, a, b, c) do { \
    (a) = _ipixel_scale_5[((x) >> 11) & 0x1f]; \
            (b) = _ipixel_scale_5[((x) >> 6) & 0x1f]; \
            (c) = _ipixel_scale_5[((x) >> 1) & 0x1f]; \
        }   while (0)

#define _ipixel_disasm_565(x, a, b, c) do { \
    (a) = _ipixel_scale_5[((x) >> 11) & 0x1f]; \
            (b) = _ipixel_scale_6[((x) >> 5) & 0x3f]; \
            (c) = _ipixel_scale_5[((x) >> 0) & 0x1f]; \
        }   while (0)

#define _ipixel_disasm_4444(x, a, b, c, d) do { \
    (a) = _ipixel_scale_4[((x) >> 12) & 0xf]; \
            (b) = _ipixel_scale_4[((x) >> 8) & 0xf]; \
            (c) = _ipixel_scale_4[((x) >> 4) & 0xf]; \
            (d) = _ipixel_scale_4[((x) >>  0) & 0xf]; \
        }   while (0)

#define _ipixel_disasm_X444(x, a, b, c) do { \
            (a) = _ipixel_scale_4[((x) >>  8) & 0xf]; \
            (b) = _ipixel_scale_4[((x) >>  4) & 0xf]; \
            (c) = _ipixel_scale_4[((x) >>  0) & 0xf]; \
        }   while (0)

#define _ipixel_disasm_444X(x, a, b, c) do { \
            (a) = _ipixel_scale_4[((x) >> 12) & 0xf]; \
            (b) = _ipixel_scale_4[((x) >>  8) & 0xf]; \
            (c) = _ipixel_scale_4[((x) >>  4) & 0xf]; \
        }   while (0)

/* disassemble 8 bits */
#define _ipixel_disasm_233(x, a, b, c) do { \
            (a) = _ipixel_scale_2[((x) >>  6) & 3]; \
            (b) = _ipixel_scale_3[((x) >>  3) & 7]; \
            (c) = _ipixel_scale_3[((x) >>  0) & 7]; \
        }   while (0)

#define _ipixel_disasm_332(x, a, b, c) do { \
            (a) = _ipixel_scale_3[((x) >>  5) & 7]; \
            (b) = _ipixel_scale_3[((x) >>  2) & 7]; \
            (c) = _ipixel_scale_2[((x) >>  0) & 3]; \
        }   while (0)

#define _ipixel_disasm_2222(x, a, b, c, d) do { \
            (a) = _ipixel_scale_2[((x) >>  6) & 3]; \
            (b) = _ipixel_scale_2[((x) >>  4) & 3]; \
            (c) = _ipixel_scale_2[((x) >>  2) & 3]; \
            (d) = _ipixel_scale_2[((x) >>  0) & 3]; \
        }   while (0)

#define _ipixel_disasm_X222(x, a, b, c) do { \
            (a) = _ipixel_scale_2[((x) >>  4) & 3]; \
            (b) = _ipixel_scale_2[((x) >>  2) & 3]; \
            (c) = _ipixel_scale_2[((x) >>  0) & 3]; \
        }   while (0)

#define _ipixel_disasm_222X(x, a, b, c) do { \
            (a) = _ipixel_scale_2[((x) >>  6) & 3]; \
            (b) = _ipixel_scale_2[((x) >>  4) & 3]; \
            (c) = _ipixel_scale_2[((x) >>  2) & 3]; \
        }   while (0)

/* disassemble 4 bits */
#define _ipixel_disasm_1111(x, a, b, c, d) do { \
            (a) = _ipixel_scale_1[((x) >> 3) & 1]; \
            (b) = _ipixel_scale_1[((x) >> 2) & 1]; \
            (c) = _ipixel_scale_1[((x) >> 1) & 1]; \
            (d) = _ipixel_scale_1[((x) >> 0) & 1]; \
        }   while (0)

#define _ipixel_disasm_X111(x, a, b, c) do { \
            (a) = _ipixel_scale_1[((x) >> 2) & 1]; \
            (b) = _ipixel_scale_1[((x) >> 1) & 1]; \
            (c) = _ipixel_scale_1[((x) >> 0) & 1]; \
        }   while (0)

#define _ipixel_disasm_111X(x, a, b, c) do { \
            (a) = _ipixel_scale_1[((x) >> 3) & 1]; \
            (b) = _ipixel_scale_1[((x) >> 2) & 1]; \
            (c) = _ipixel_scale_1[((x) >> 1) & 1]; \
        }   while (0)

#define _ipixel_disasm_121(x, a, b, c) do { \
            (a) = _ipixel_scale_1[((x) >> 3) & 1]; \
            (b) = _ipixel_scale_2[((x) >> 1) & 3]; \
            (c) = _ipixel_scale_1[((x) >> 0) & 1]; \
        }   while (0)

/* assemble 10 bits */
#define _ipixel_asm_2101010(a, b, c, d) ((uint)( \
			((uint)((a) & 0xc0) << 30) | \
			(((uint)_ipixel_8_to_10(b)) << 20) | \
			(((uint)_ipixel_8_to_10(c)) << 10) | \
			(((uint)_ipixel_8_to_10(d)) << 0)))

#define _ipixel_asm_1010102(a, b, c, d) ((uint)( \
			(((uint)_ipixel_8_to_10(a)) << 22) | \
			(((uint)_ipixel_8_to_10(b)) << 12) | \
			(((uint)_ipixel_8_to_10(c)) <<  2) | \
			((uint)((d) & 0xc0) >> 6)))

/* disassemble 10 bits */
#define _ipixel_disasm_2101010(x, a, b, c, d) do { \
			(a) = _ipixel_scale_2[((x) >> 30) & 3]; \
			(b) = ((x) >> 22) & 0xff; \
			(c) = ((x) >> 12) & 0xff; \
			(d) = ((x) >>  2) & 0xff; \
		}	while (0)

#define _ipixel_disasm_1010102(x, a, b, c, d) do { \
			(a) = ((x) >> 24) & 0xff; \
			(b) = ((x) >> 14) & 0xff; \
			(c) = ((x) >>  4) & 0xff; \
			(d) = _ipixel_scale_2[(x) & 3]; \
		}	while (0)


/**********************************************************************
 * MACRO: Color Convertion
 **********************************************************************/
#define _ipixel_norm(color) (((color) >> 7) + (color))
#define _ipixel_unnorm(color) ((((color) << 8) - (color)) >> 8)
#define _imul_y_div_255(x, y) (((x) * _ipixel_norm(y)) >> 8)
#define _ipixel_fast_div_255(x) (((x) + (((x) + 257) >> 8)) >> 8)
#define _ipixel_8_to_9(x) (((color) >> 7) + (color << 1))
#define _ipixel_8_to_10(x) (((color) >> 6) + (color << 2))

#define _ipixel_clamp_to_0(x) ((-((int)(x)) >> 31) & ((int)(x)))
#define _ipixel_clamp_to_255(x) \
        ((((255 - ((int)(x))) >> 31) | ((int)(x))) & 255)

#define _ipixel_to_gray(r, g, b) \
        ((19595 * (r) + 38469 * (g) + 7472 * (b)) >> 16)

#define _ipixel_to_pmul(c, r, g, b, a) do { \
            uint __a = (a); \
            uint __b = (__a); \
            uint __X1 = __a; \
            uint __X2 = ((r) * __b); \
            uint __X3 = ((g) * __b); \
            uint __X4 = ((b) * __b); \
            __X2 = (__X2 + (__X2 >> 8)) >> 8; \
            __X3 = (__X3 + (__X3 >> 8)) >> 8; \
            __X4 = (__X4 + (__X4 >> 8)) >> 8; \
            c = _ipixel_asm_8888(__X1, __X2, __X3, __X4); \
        }   while (0)

#define _ipixel_from_pmul(c, r, g, b, a) do { \
            uint __SA = ((c) >> 24); \
            uint __FA = (__SA); \
            (a) = __SA; \
            if (__FA > 0) { \
                (r) = ((((c) >> 16) & 0xff) * 255) / __FA; \
                (g) = ((((c) >>  8) & 0xff) * 255) / __FA; \
                (b) = ((((c) >>  0) & 0xff) * 255) / __FA; \
            }    else { \
                (r) = 0; (g) = 0; (b) = 0; \
            }    \
        }   while (0)

#define _ipixel_RGBA_to_P8R8G8B8(r, g, b, a) ( \
        (((a)) << 24) | \
        ((((r) * _ipixel_norm(a)) >> 8) << 16) | \
        ((((g) * _ipixel_norm(a)) >> 8) <<  8) | \
        ((((b) * _ipixel_norm(a)) >> 8) <<  0))

#define _ipixel_R8G8B8_to_R5G5B5(c) \
    ((((c) >> 3) & 0x001f) | (((c) >> 6) & 0x03e0) | (((c) >> 9) & 0x7c00))


#define _ipixel_R5G5B5_to_ent(index, c) ((index).ent[c])
#define _ipixel_A8R8G8B8_from_index(index, c) ((index).rgba[c])


#define _ipixel_R8G8B8_to_ent(index, c) \
        _ipixel_R5G5B5_to_ent(index, _ipixel_R8G8B8_to_R5G5B5(c))
#define _ipixel_RGB_to_ent(index, r, g, b) \
        _ipixel_R5G5B5_to_ent(index, _ipixel_asm_1555(0, r, g, b))

#define _ipixel_RGB_from_index(index, c, r, g, b) do { \
            uint __rgba = _ipixel_A8R8G8B8_from_index(index, c); \
            _ipixel_disasm_X888(__rgba, r, g, b);  \
        } while (0)

#define _ipixel_RGBA_from_index(index, c, r, g, b, a) do { \
            uint __rgba = _ipixel_A8R8G8B8_from_index(index, c); \
            _ipixel_disasm_8888(__rgba, a, r, g, b); \
        } while (0)

#define ISPLIT_ARGB(c, a, r, g, b) do { \
            (a) = (((uint)(c)) >> 24); \
            (r) = (((uint)(c)) >> 16) & 0xff; \
            (g) = (((uint)(c)) >>  8) & 0xff; \
            (b) = (((uint)(c))      ) & 0xff; \
        } while (0)

#define ISPLIT_RGB(c, r, g, b) do { \
            (r) = (((uint)(c)) >> 16) & 0xff; \
            (g) = (((uint)(c)) >>  8) & 0xff; \
            (b) = (((uint)(c))      ) & 0xff; \
        } while (0)

#define _ipixel_fetch_bpp(bpp, ptr, pos, c) do { \
            switch (bpp) { \
            case  1: c = _ipixel_fetch( 1, ptr, pos); break; \
            case  4: c = _ipixel_fetch( 4, ptr, pos); break; \
            case  8: c = _ipixel_fetch( 8, ptr, pos); break; \
            case 16: c = _ipixel_fetch(16, ptr, pos); break; \
            case 24: c = _ipixel_fetch(24, ptr, pos); break; \
            case 32: c = _ipixel_fetch(32, ptr, pos); break; \
            default: c = 0; break; \
            } \
        } while (0)

#define _ipixel_store_bpp(bpp, ptr, pos, c) do { \
            switch (bpp) { \
            case  1: _ipixel_store( 1, ptr, pos, c); break; \
            case  4: _ipixel_store( 4, ptr, pos, c); break; \
            case  8: _ipixel_store( 8, ptr, pos, c); break; \
            case 16: _ipixel_store(16, ptr, pos, c); break; \
            case 24: _ipixel_store(24, ptr, pos, c); break; \
            case 32: _ipixel_store(32, ptr, pos, c); break; \
            } \
        } while (0)

#define _ipixel_load_card_lsb(ptr, r, g, b, a) do { \
			(a) = ((byte*)(ptr))[3]; \
			(r) = ((byte*)(ptr))[2]; \
			(g) = ((byte*)(ptr))[1]; \
			(b) = ((byte*)(ptr))[0]; \
		} while (0)

#define _ipixel_load_card_msb(ptr, r, g, b, a) do { \
			(a) = ((byte*)(ptr))[0]; \
			(r) = ((byte*)(ptr))[1]; \
			(g) = ((byte*)(ptr))[2]; \
			(b) = ((byte*)(ptr))[3]; \
		} while (0)


#if IPIXEL_BIG_ENDIAN
	#define _ipixel_load_card	_ipixel_load_card_msb
#else
	#define _ipixel_load_card	_ipixel_load_card_lsb
#endif

#if IPIXEL_BIG_ENDIAN
	#define _ipixel_card_alpha	0
#else
	#define _ipixel_card_alpha	3
#endif


/**********************************************************************
 * LIN's LOOP UNROLL MACROs: 
 * Actually Duff's unroll macro isn't compatible with vc7 because
 * of non-standard usage of 'switch' & 'for' statement. 
 * the macros below are standard implementation of loop unroll
 **********************************************************************/
#ifndef ILINS_LOOP_QUATRO
#define ILINS_LOOP_QUATRO(actionx1, actionx2, actionx4, width) do { \
    uint __width = (uint)(width);    \
    uint __increment = __width >> 2; \
    for (; __increment > 0; __increment--) { actionx4; }    \
    if (__width & 2) { actionx2; } \
    if (__width & 1) { actionx1; } \
}   while (0)
#endif

#ifndef ILINS_LOOP_DOUBLE
#define ILINS_LOOP_DOUBLE(actionx1, actionx2, width) do { \
	uint __width = (uint)(width); \
	uint __increment = __width >> 1; \
	for (; __increment > 0; __increment--) { actionx2; } \
	if (__width & 1) { actionx1; }  \
}	while (0)
#endif

#ifndef ILINS_LOOP_ONCE
#define ILINS_LOOP_ONCE(action, width) do { \
    uint __width = (uint)(width); \
    for (; __width > 0; __width--) { action; } \
}   while (0)
#endif



/**********************************************************************
 * MACRO: PIXEL DISASSEMBLE
 **********************************************************************/

/* pixel format: 32 bits */
#define IRGBA_FROM_A8R8G8B8(c, r, g, b, a) _ipixel_disasm_8888(c, a, r, g, b)
#define IRGBA_FROM_A8B8G8R8(c, r, g, b, a) _ipixel_disasm_8888(c, a, b, g, r)
#define IRGBA_FROM_R8G8B8A8(c, r, g, b, a) _ipixel_disasm_8888(c, r, g, b, a)
#define IRGBA_FROM_B8G8R8A8(c, r, g, b, a) _ipixel_disasm_8888(c, b, g, r, a)
#define IRGBA_FROM_X8R8G8B8(c, r, g, b, a) do { \
        _ipixel_disasm_X888(c, r, g, b); (a) = 255; } while (0)
#define IRGBA_FROM_X8B8G8R8(c, r, g, b, a) do { \
        _ipixel_disasm_X888(c, b, g, r); (a) = 255; } while (0)
#define IRGBA_FROM_R8G8B8X8(c, r, g, b, a) do { \
        _ipixel_disasm_888X(c, r, g, b); (a) = 255; } while (0)
#define IRGBA_FROM_B8G8R8X8(c, r, g, b, a) do { \
        _ipixel_disasm_888X(c, b, g, r); (a) = 255; } while (0)
#define IRGBA_FROM_P8R8G8B8(c, r, g, b, a) do { \
        _ipixel_from_pmul(c, r, g, b, a); } while (0)

/* pixel format: 24 bits */
#define IRGBA_FROM_R8G8B8(c, r, g, b, a) do { \
        _ipixel_disasm_888(c, r, g, b); (a) = 255; } while (0)
#define IRGBA_FROM_B8G8R8(c, r, g, b, a) do { \
        _ipixel_disasm_888(c, b, g, r); (a) = 255; } while (0)

/* pixel format: 16 bits */
#define IRGBA_FROM_R5G6B5(c, r, g, b, a) do { \
        _ipixel_disasm_565(c, r, g, b); (a) = 255; } while (0)
#define IRGBA_FROM_B5G6R5(c, r, g, b, a) do { \
        _ipixel_disasm_565(c, b, g, r); (a) = 255; } while (0)
#define IRGBA_FROM_X1R5G5B5(c, r, g, b, a) do { \
        _ipixel_disasm_X555(c, r, g, b); (a) = 255; } while (0)
#define IRGBA_FROM_X1B5G5R5(c, r, g, b, a) do { \
        _ipixel_disasm_X555(c, b, g, r); (a) = 255; } while (0)
#define IRGBA_FROM_R5G5B5X1(c, r, g, b, a) do { \
        _ipixel_disasm_555X(c, r, g, b); (a) = 255; } while (0)
#define IRGBA_FROM_B5G5R5X1(c, r, g, b, a) do { \
        _ipixel_disasm_555X(c, b, g, r); (a) = 255; } while (0)
#define IRGBA_FROM_A1R5G5B5(c, r, g, b, a) _ipixel_disasm_1555(c, a, r, g, b)
#define IRGBA_FROM_A1B5G5R5(c, r, g, b, a) _ipixel_disasm_1555(c, a, b, g, r)
#define IRGBA_FROM_R5G5B5A1(c, r, g, b, a) _ipixel_disasm_5551(c, r, g, b, a)
#define IRGBA_FROM_B5G5R5A1(c, r, g, b, a) _ipixel_disasm_5551(c, b, g, r, a)
#define IRGBA_FROM_X4R4G4B4(c, r, g, b, a) do { \
        _ipixel_disasm_X444(c, r, g, b); (a) = 255; } while (0)
#define IRGBA_FROM_X4B4G4R4(c, r, g, b, a) do { \
        _ipixel_disasm_X444(c, b, g, r); (a) = 255; } while (0)
#define IRGBA_FROM_R4G4B4X4(c, r, g, b, a) do { \
        _ipixel_disasm_444X(c, r, g, b); (a) = 255; } while (0)
#define IRGBA_FROM_B4G4R4X4(c, r, g, b, a) do { \
        _ipixel_disasm_444X(c, b, g, r); (a) = 255; } while (0)
#define IRGBA_FROM_A4R4G4B4(c, r, g, b, a) _ipixel_disasm_4444(c, a, r, g, b)
#define IRGBA_FROM_A4B4G4R4(c, r, g, b, a) _ipixel_disasm_4444(c, a, b, g, r)
#define IRGBA_FROM_R4G4B4A4(c, r, g, b, a) _ipixel_disasm_4444(c, r, g, b, a)
#define IRGBA_FROM_B4G4R4A4(c, r, g, b, a) _ipixel_disasm_4444(c, b, g, r, a)

/* pixel format: 8 bits */
#define IRGBA_FROM_C8(c, r, g, b, a) do { \
        _ipixel_RGBA_from_index(_ipixel_src_index, c, r, g, b, a); } while (0)
#define IRGBA_FROM_G8(c, r, g, b, a) do { \
        (r) = (g) = (b) = (c); (a) = 255; } while (0)
#define IRGBA_FROM_A8(c, r, g, b, a) do { \
        (r) = (g) = (b) = 255; (a) = (c); } while (0)

#define IRGBA_FROM_R3G3B2(c, r, g, b, a) do { \
        _ipixel_disasm_332(c, r, g, b); (a) = 255; } while (0)
#define IRGBA_FROM_B2G3R3(c, r, g, b, a) do { \
        _ipixel_disasm_233(c, b, g, r); (a) = 255; } while (0)
#define IRGBA_FROM_X2R2G2B2(c, r, g, b, a) do { \
        _ipixel_disasm_X222(c, r, g, b); (a) = 255; } while (0)
#define IRGBA_FROM_X2B2G2R2(c, r, g, b, a) do { \
        _ipixel_disasm_X222(c, b, g, r); (a) = 255; } while (0) 
#define IRGBA_FROM_R2G2B2X2(c, r, g, b, a) do { \
        _ipixel_disasm_222X(c, r, g, b); (a) = 255; } while (0)
#define IRGBA_FROM_B2G2R2X2(c, r, g, b, a) do { \
        _ipixel_disasm_222X(c, b, g, r); (a) = 255; } while (0)
#define IRGBA_FROM_A2R2G2B2(c, r, g, b, a) _ipixel_disasm_2222(c, a, r, g, b)
#define IRGBA_FROM_A2B2G2R2(c, r, g, b, a) _ipixel_disasm_2222(c, a, b, g, r)
#define IRGBA_FROM_R2G2B2A2(c, r, g, b, a) _ipixel_disasm_2222(c, r, g, b, a)
#define IRGBA_FROM_B2G2R2A2(c, r, g, b, a) _ipixel_disasm_2222(c, b, g, r, a)

#define IRGBA_FROM_X4C4(c, r, g, b, a) do { \
        _ipixel_RGBA_from_index(_ipixel_src_index, c, r, g, b, a); \
        } while (0)
#define IRGBA_FROM_X4G4(c, r, g, b, a) do { \
        (r) = (g) = (b) = _ipixel_scale_4[c]; (a) = 255; } while (0)
#define IRGBA_FROM_X4A4(c, r, g, b, a) do { \
        (r) = (g) = (b) = 255; (a) = _ipixel_scale_4[c]; } while (0)
#define IRGBA_FROM_C4X4(c, r, g, b, a) do { \
        _ipixel_RGBA_from_index(_ipixel_src_index, (c) >> 4, r, g, b, a); \
        } while (0)
#define IRGBA_FROM_G4X4(c, r, g, b, a) do { \
        (r) = (g) = (b) = _ipixel_scale_4[(c) >> 4]; (a) = 255; } while (0)
#define IRGBA_FROM_A4X4(c, r, g, b, a) do { \
        (r) = (g) = (b) = 255; (a) = _ipixel_scale_4[(c) >> 4]; } while (0)

/* pixel format: 4 bits */
#define IRGBA_FROM_C4(c, r, g, b, a) IRGBA_FROM_X4C4(c, r, g, b, a)
#define IRGBA_FROM_G4(c, r, g, b, a) IRGBA_FROM_X4G4(c, r, g, b, a)
#define IRGBA_FROM_A4(c, r, g, b, a) IRGBA_FROM_X4A4(c, r, g, b, a)
#define IRGBA_FROM_R1G2B1(c, r, g, b, a) do { \
        _ipixel_disasm_121(c, r, g, b); (a) = 255; } while (0)
#define IRGBA_FROM_B1G2R1(c, r, g, b, a) do { \
        _ipixel_disasm_121(c, b, g, r); (a) = 255; } while (0)
#define IRGBA_FROM_A1R1G1B1(c, r, g, b, a) _ipixel_disasm_1111(c, a, r, g, b)
#define IRGBA_FROM_A1B1G1R1(c, r, g, b, a) _ipixel_disasm_1111(c, a, b, g, r)
#define IRGBA_FROM_R1G1B1A1(c, r, g, b, a) _ipixel_disasm_1111(c, r, g, b, a)
#define IRGBA_FROM_B1G1R1A1(c, r, g, b, a) _ipixel_disasm_1111(c, b, g, r, a)
#define IRGBA_FROM_X1R1G1B1(c, r, g, b, a) do { \
        _ipixel_disasm_X111(c, r, g, b); (a) = 255; } while (0)
#define IRGBA_FROM_X1B1G1R1(c, r, g, b, a) do { \
        _ipixel_disasm_X111(c, b, g, r); (a) = 255; } while (0)
#define IRGBA_FROM_R1G1B1X1(c, r, g, b, a) do { \
        _ipixel_disasm_111X(c, r, g, b); (a) = 255; } while (0)
#define IRGBA_FROM_B1G1R1X1(c, r, g, b, a) do { \
        _ipixel_disasm_111X(c, b, g, r); (a) = 255; } while (0)

/* pixel format: 1 bit */
#define IRGBA_FROM_C1(c, r, g, b, a) do { \
        _ipixel_RGBA_from_index(_ipixel_src_index, c, r, g, b, a); \
        } while (0)
#define IRGBA_FROM_G1(c, r, g, b, a) do { \
        (r) = (g) = (b) = _ipixel_scale_1[c]; (a) = 255; } while (0)
#define IRGBA_FROM_A1(c, r, g, b, a) do { \
        (r) = (g) = (b) = 255; (a) = _ipixel_scale_1[c]; } while (0)

/* pixel format: 10-10-10-2 bit */
#define IRGBA_FROM_A2R10G10B10(c, r, g, b, a) \
		_ipixel_disasm_2101010(c, a, r, g, b)
#define IRGBA_FROM_A2B10G10R10(c, r, g, b, a) \
		_ipixel_disasm_2101010(c, a, b, g, r)
#define IRGBA_FROM_R10G10B10A2(c, r, g, b, a) \
		_ipixel_disasm_1010102(c, r, g, b, a)
#define IRGBA_FROM_B10G10R10A2(c, r, g, b, a) \
		_ipixel_disasm_1010102(c, b, g, r, a)


/**********************************************************************
 * MACRO: PIXEL ASSEMBLE
 **********************************************************************/

/* pixel format: 32 bits */
#define IRGBA_TO_A8R8G8B8(r, g, b, a)  _ipixel_asm_8888(a, r, g, b)
#define IRGBA_TO_A8B8G8R8(r, g, b, a)  _ipixel_asm_8888(a, b, g, r)
#define IRGBA_TO_R8G8B8A8(r, g, b, a)  _ipixel_asm_8888(r, g, b, a)
#define IRGBA_TO_B8G8R8A8(r, g, b, a)  _ipixel_asm_8888(b, g, r, a)
#define IRGBA_TO_X8R8G8B8(r, g, b, a)  _ipixel_asm_8888(0, r, g, b)
#define IRGBA_TO_X8B8G8R8(r, g, b, a)  _ipixel_asm_8888(0, b, g, r)
#define IRGBA_TO_R8G8B8X8(r, g, b, a)  _ipixel_asm_8888(r, g, b, 0)
#define IRGBA_TO_B8G8R8X8(r, g, b, a)  _ipixel_asm_8888(b, g, r, 0)
#define IRGBA_TO_P8R8G8B8(r, g, b, a)  _ipixel_RGBA_to_P8R8G8B8(r, g, b, a)

/* pixel format: 24 bits */
#define IRGBA_TO_R8G8B8(r, g, b, a)   _ipixel_asm_888(r, g, b)
#define IRGBA_TO_B8G8R8(r, g, b, a)   _ipixel_asm_888(b, g, r)

/* pixel format: 16 bits */
#define IRGBA_TO_R5G6B5(r, g, b, a)   _ipixel_asm_565(r, g, b)
#define IRGBA_TO_B5G6R5(r, g, b, a)   _ipixel_asm_565(b, g, r)
#define IRGBA_TO_X1R5G5B5(r, g, b, a) _ipixel_asm_1555(0, r, g, b)
#define IRGBA_TO_X1B5G5R5(r, g, b, a) _ipixel_asm_1555(0, b, g, r)
#define IRGBA_TO_R5G5B5X1(r, g, b, a) _ipixel_asm_5551(r, g, b, 0)
#define IRGBA_TO_B5G5R5X1(r, g, b, a) _ipixel_asm_5551(b, g, r, 0)
#define IRGBA_TO_A1R5G5B5(r, g, b, a) _ipixel_asm_1555(a, r, g, b)
#define IRGBA_TO_A1B5G5R5(r, g, b, a) _ipixel_asm_1555(a, b, g, r)
#define IRGBA_TO_R5G5B5A1(r, g, b, a) _ipixel_asm_5551(r, g, b, a)
#define IRGBA_TO_B5G5R5A1(r, g, b, a) _ipixel_asm_5551(b, g, r, a)
#define IRGBA_TO_X4R4G4B4(r, g, b, a) _ipixel_asm_4444(0, r, g, b)
#define IRGBA_TO_X4B4G4R4(r, g, b, a) _ipixel_asm_4444(0, b, g, r)
#define IRGBA_TO_R4G4B4X4(r, g, b, a) _ipixel_asm_4444(r, g, b, 0)
#define IRGBA_TO_B4G4R4X4(r, g, b, a) _ipixel_asm_4444(b, g, r, 0)
#define IRGBA_TO_A4R4G4B4(r, g, b, a) _ipixel_asm_4444(a, r, g, b)
#define IRGBA_TO_A4B4G4R4(r, g, b, a) _ipixel_asm_4444(a, b, g, r)
#define IRGBA_TO_R4G4B4A4(r, g, b, a) _ipixel_asm_4444(r, g, b, a)
#define IRGBA_TO_B4G4R4A4(r, g, b, a) _ipixel_asm_4444(b, g, r, a)

/* pixel format: 8 bits */
#define IRGBA_TO_C8(r, g, b, a) \
        _ipixel_RGB_to_ent(_ipixel_dst_index, r, g, b)
#define IRGBA_TO_G8(r, g, b, a)       _ipixel_to_gray(r, g, b)
#define IRGBA_TO_A8(r, g, b, a)       (a)
#define IRGBA_TO_R3G3B2(r, g, b, a)   _ipixel_asm_332(r, g, b)
#define IRGBA_TO_B2G3R3(r, g, b, a)   _ipixel_asm_233(b, g, r)
#define IRGBA_TO_X2R2G2B2(r, g, b, a) _ipixel_asm_2222(0, r, g, b)
#define IRGBA_TO_X2B2G2R2(r, g, b, a) _ipixel_asm_2222(0, b, g, r)
#define IRGBA_TO_R2G2B2X2(r, g, b, a) _ipixel_asm_2222(r, g, b, 0)
#define IRGBA_TO_B2G2R2X2(r, g, b, a) _ipixel_asm_2222(b, g, r, 0)
#define IRGBA_TO_A2R2G2B2(r, g, b, a) _ipixel_asm_2222(a, r, g, b)
#define IRGBA_TO_A2B2G2R2(r, g, b, a) _ipixel_asm_2222(a, b, g, r)
#define IRGBA_TO_R2G2B2A2(r, g, b, a) _ipixel_asm_2222(r, g, b, a)
#define IRGBA_TO_B2G2R2A2(r, g, b, a) _ipixel_asm_2222(b, g, r, a)
#define IRGBA_TO_X4C4(r, g, b, a)     (IRGBA_TO_C8(r, g, b, a) & 0xf)
#define IRGBA_TO_X4G4(r, g, b, a)     (IRGBA_TO_G8(r, g, b, a) >>  4)
#define IRGBA_TO_X4A4(r, g, b, a)     (IRGBA_TO_A8(r, g, b, a) >>  4)
#define IRGBA_TO_C4X4(r, g, b, a)     ((IRGBA_TO_C8(r, g, b, a) & 0xf) << 4)
#define IRGBA_TO_G4X4(r, g, b, a)     (IRGBA_TO_G8(r, g, b, a) & 0xf0)
#define IRGBA_TO_A4X4(r, g, b, a)     (IRGBA_TO_A8(r, g, b, a) & 0xf0)

/* pixel format: 4 bits */
#define IRGBA_TO_C4(r, g, b, a)       IRGBA_TO_X4C4(r, g, b, a)
#define IRGBA_TO_G4(r, g, b, a)       IRGBA_TO_X4G4(r, g, b, a)
#define IRGBA_TO_A4(r, g, b, a)       IRGBA_TO_X4A4(r, g, b, a)
#define IRGBA_TO_R1G2B1(r, g, b, a)   _ipixel_asm_121(r, g, b)
#define IRGBA_TO_B1G2R1(r, g, b, a)   _ipixel_asm_121(b, g, r)
#define IRGBA_TO_A1R1G1B1(r, g, b, a) _ipixel_asm_1111(a, r, g, b)
#define IRGBA_TO_A1B1G1R1(r, g, b, a) _ipixel_asm_1111(a, b, g, r)
#define IRGBA_TO_R1G1B1A1(r, g, b, a) _ipixel_asm_1111(r, g, b, a)
#define IRGBA_TO_B1G1R1A1(r, g, b, a) _ipixel_asm_1111(b, g, r, a)

#define IRGBA_TO_X1R1G1B1(r, g, b, a) _ipixel_asm_1111(0, r, g, b)
#define IRGBA_TO_X1B1G1R1(r, g, b, a) _ipixel_asm_1111(0, b, g, r)
#define IRGBA_TO_R1G1B1X1(r, g, b, a) _ipixel_asm_1111(r, g, b, 0)
#define IRGBA_TO_B1G1R1X1(r, g, b, a) _ipixel_asm_1111(b, g, r, 0)

/* pixel format: 1 bit */
#define IRGBA_TO_C1(r, g, b, a)       (IRGBA_TO_C8(r, g, b, a) & 1)
#define IRGBA_TO_G1(r, g, b, a)       (IRGBA_TO_G8(r, g, b, a) >> 7)
#define IRGBA_TO_A1(r, g, b, a)       (IRGBA_TO_A8(r, g, b, a) >> 7)

/* pixel format: 10-10-10-2 bit */
#define IRGBA_TO_A2R10G10B10(r, g, b, a) _ipixel_asm_2101010(a, r, g, b)
#define IRGBA_TO_A2B10G10R10(r, g, b, a) _ipixel_asm_2101010(a, b, g, r)
#define IRGBA_TO_R10G10B10A2(r, g, b, a) _ipixel_asm_1010102(r, g, b, a)
#define IRGBA_TO_B10G10R10A2(r, g, b, a) _ipixel_asm_1010102(b, g, r, a)


/**********************************************************************
 * MACRO: PIXEL FORMAT BPP (bits per pixel)
 **********************************************************************/
#define IPIX_FMT_BPP_A8R8G8B8         32
#define IPIX_FMT_BPP_A8B8G8R8         32
#define IPIX_FMT_BPP_R8G8B8A8         32
#define IPIX_FMT_BPP_B8G8R8A8         32
#define IPIX_FMT_BPP_X8R8G8B8         32
#define IPIX_FMT_BPP_X8B8G8R8         32
#define IPIX_FMT_BPP_R8G8B8X8         32
#define IPIX_FMT_BPP_B8G8R8X8         32
#define IPIX_FMT_BPP_P8R8G8B8         32
#define IPIX_FMT_BPP_R8G8B8           24
#define IPIX_FMT_BPP_B8G8R8           24
#define IPIX_FMT_BPP_R5G6B5           16
#define IPIX_FMT_BPP_B5G6R5           16
#define IPIX_FMT_BPP_X1R5G5B5         16
#define IPIX_FMT_BPP_X1B5G5R5         16
#define IPIX_FMT_BPP_R5G5B5X1         16
#define IPIX_FMT_BPP_B5G5R5X1         16
#define IPIX_FMT_BPP_A1R5G5B5         16
#define IPIX_FMT_BPP_A1B5G5R5         16
#define IPIX_FMT_BPP_R5G5B5A1         16
#define IPIX_FMT_BPP_B5G5R5A1         16
#define IPIX_FMT_BPP_X4R4G4B4         16
#define IPIX_FMT_BPP_X4B4G4R4         16
#define IPIX_FMT_BPP_R4G4B4X4         16
#define IPIX_FMT_BPP_B4G4R4X4         16
#define IPIX_FMT_BPP_A4R4G4B4         16
#define IPIX_FMT_BPP_A4B4G4R4         16
#define IPIX_FMT_BPP_R4G4B4A4         16
#define IPIX_FMT_BPP_B4G4R4A4         16
#define IPIX_FMT_BPP_C8               8
#define IPIX_FMT_BPP_G8               8
#define IPIX_FMT_BPP_A8               8
#define IPIX_FMT_BPP_R2G3B2           7
#define IPIX_FMT_BPP_B2G3R2           7
#define IPIX_FMT_BPP_X2R2G2B2         8
#define IPIX_FMT_BPP_X2B2G2R2         8
#define IPIX_FMT_BPP_R2G2B2X2         8
#define IPIX_FMT_BPP_B2G2R2X2         8
#define IPIX_FMT_BPP_A2R2G2B2         8
#define IPIX_FMT_BPP_A2B2G2R2         8
#define IPIX_FMT_BPP_R2G2B2A2         8
#define IPIX_FMT_BPP_B2G2R2A2         8
#define IPIX_FMT_BPP_X4C4             8
#define IPIX_FMT_BPP_X4G4             8
#define IPIX_FMT_BPP_X4A4             8
#define IPIX_FMT_BPP_C4X4             8
#define IPIX_FMT_BPP_G4X4             8
#define IPIX_FMT_BPP_A4X4             8
#define IPIX_FMT_BPP_C4               4
#define IPIX_FMT_BPP_G4               4
#define IPIX_FMT_BPP_A4               4
#define IPIX_FMT_BPP_R1G2B1           4
#define IPIX_FMT_BPP_B1G2R1           4
#define IPIX_FMT_BPP_A1R1G1B1         4
#define IPIX_FMT_BPP_A1B1G1R1         4
#define IPIX_FMT_BPP_R1G1B1A1         4
#define IPIX_FMT_BPP_B1G1R1A1         4
#define IPIX_FMT_BPP_X1R1G1B1         4
#define IPIX_FMT_BPP_X1B1G1R1         4
#define IPIX_FMT_BPP_R1G1B1X1         4
#define IPIX_FMT_BPP_B1G1R1X1         4
#define IPIX_FMT_BPP_C1               1
#define IPIX_FMT_BPP_G1               1
#define IPIX_FMT_BPP_A1               1
#define IPIX_FMT_BPP_A2R10G10B10      32
#define IPIX_FMT_BPP_A2B10G10R10      32
#define IPIX_FMT_BPP_R10G10B10A2      32
#define IPIX_FMT_BPP_B10G10R10A2      32


/**********************************************************************
 * MACRO: PIXEL CONVERTION (A8R8G8B8 . *, * . A8R8G8B8)
 **********************************************************************/
/* pixel convertion look-up-tables
 * using those table to speed-up 16bits.32bits, 8bits.32bits converting.
 * this is much faster than binary shifting. But look-up-tables must be 
 * initialized by ipixel_lut_init or calling ipixel_get_fetch(0, 0)
 */ 

extern uint _ipixel_cvt_lut_R5G6B5[512];
extern uint _ipixel_cvt_lut_B5G6R5[512];
extern uint _ipixel_cvt_lut_X1R5G5B5[512];
extern uint _ipixel_cvt_lut_X1B5G5R5[512];
extern uint _ipixel_cvt_lut_R5G5B5X1[512];
extern uint _ipixel_cvt_lut_B5G5R5X1[512];
extern uint _ipixel_cvt_lut_A1R5G5B5[512];
extern uint _ipixel_cvt_lut_A1B5G5R5[512];
extern uint _ipixel_cvt_lut_R5G5B5A1[512];
extern uint _ipixel_cvt_lut_B5G5R5A1[512];
extern uint _ipixel_cvt_lut_X4R4G4B4[512];
extern uint _ipixel_cvt_lut_X4B4G4R4[512];
extern uint _ipixel_cvt_lut_R4G4B4X4[512];
extern uint _ipixel_cvt_lut_B4G4R4X4[512];
extern uint _ipixel_cvt_lut_A4R4G4B4[512];
extern uint _ipixel_cvt_lut_A4B4G4R4[512];
extern uint _ipixel_cvt_lut_R4G4B4A4[512];
extern uint _ipixel_cvt_lut_B4G4R4A4[512];
extern uint _ipixel_cvt_lut_R3G3B2[256];
extern uint _ipixel_cvt_lut_B2G3R3[256];
extern uint _ipixel_cvt_lut_X2R2G2B2[256];
extern uint _ipixel_cvt_lut_X2B2G2R2[256];
extern uint _ipixel_cvt_lut_R2G2B2X2[256];
extern uint _ipixel_cvt_lut_B2G2R2X2[256];
extern uint _ipixel_cvt_lut_A2R2G2B2[256];
extern uint _ipixel_cvt_lut_A2B2G2R2[256];
extern uint _ipixel_cvt_lut_R2G2B2A2[256];
extern uint _ipixel_cvt_lut_B2G2R2A2[256];


/* look-up-tables accessing */
#if IPIXEL_BIG_ENDIAN
#define IPIXEL_CVT_LUT_2(fmt, x) \
		(	_ipixel_cvt_lut_##fmt[((x) & 0xff) + 256] | \
			_ipixel_cvt_lut_##fmt[((x) >>   8) +   0] )
#else
#define IPIXEL_CVT_LUT_2(fmt, x) \
		(	_ipixel_cvt_lut_##fmt[((x) & 0xff) +   0] | \
			_ipixel_cvt_lut_##fmt[((x) >>   8) + 256] )
#endif

#define IPIXEL_CVT_LUT_1(fmt, x) _ipixel_cvt_lut_##fmt[(x)]


/* convert from 32 bits */
#define IPIXEL_FROM_A8R8G8B8(x) (x) 
#define IPIXEL_FROM_A8B8G8R8(x) \
		((((x) & 0xff00ff00) | \
		(((x) & 0xff0000) >> 16) | \
		(((x) & 0xff) << 16)))
#define IPIXEL_FROM_R8G8B8A8(x) \
		((((x) & 0xff) << 24) | (((x) & 0xffffff00) >> 8))
#define IPIXEL_FROM_B8G8R8A8(x) \
		(	(((x) & 0x000000ff) << 24) | \
			(((x) & 0x0000ff00) <<  8) | \
			(((x) & 0x00ff0000) >>  8) | \
			(((x) & 0xff000000) >> 24))
#define IPIXEL_FROM_X8R8G8B8(x) ((x) | 0xff000000)
#define IPIXEL_FROM_X8B8G8R8(x) \
		(IPIXEL_FROM_A8B8G8R8(x) | 0xff000000)
#define IPIXEL_FROM_R8G8B8X8(x) (((x) >> 8) | 0xff000000)
#define IPIXEL_FROM_B8G8R8X8(x) \
		(	(((x) & 0x0000ff00) <<  8) | \
			(((x) & 0x00ff0000) >>  8) | \
			(((x) & 0xff000000) >> 24) | 0xff000000 )
#define IPIXEL_FROM_P8R8G8B8(x) ( ((x) >> 24) == 0 ? (0) : \
		((((((x) >> 16) & 0xff) * 255) / ((x) >> 24)) << 16) | \
		((((((x) >>  8) & 0xff) * 255) / ((x) >> 24)) << 8) | \
		((((((x) >>  0) & 0xff) * 255) / ((x) >> 24)) << 0) | \
		((x) & 0xff000000) )

/* convert from 24 bits */
#define IPIXEL_FROM_R8G8B8(x) ((x) | 0xff000000)
#define IPIXEL_FROM_B8G8R8(x) \
		(	(((x) & 0x0000ff00) | \
			(((x) & 0xff0000) >> 16) | \
			(((x) & 0xff) << 16)) | 0xff000000 )

/* convert from 16 bits */
#define IPIXEL_FROM_R5G6B5(x) IPIXEL_CVT_LUT_2(R5G6B5, x)
#define IPIXEL_FROM_B5G6R5(x) IPIXEL_CVT_LUT_2(B5G6R5, x)
#define IPIXEL_FROM_A1R5G5B5(x) IPIXEL_CVT_LUT_2(A1R5G5B5, x)
#define IPIXEL_FROM_A1B5G5R5(x) IPIXEL_CVT_LUT_2(A1B5G5R5, x)
#define IPIXEL_FROM_R5G5B5A1(x) IPIXEL_CVT_LUT_2(R5G5B5A1, x)
#define IPIXEL_FROM_B5G5R5A1(x) IPIXEL_CVT_LUT_2(B5G5R5A1, x)
#define IPIXEL_FROM_X1R5G5B5(x) IPIXEL_CVT_LUT_2(X1R5G5B5, x)
#define IPIXEL_FROM_X1B5G5R5(x) IPIXEL_CVT_LUT_2(X1B5G5R5, x)
#define IPIXEL_FROM_R5G5B5X1(x) IPIXEL_CVT_LUT_2(R5G5B5X1, x)
#define IPIXEL_FROM_B5G5R5X1(x) IPIXEL_CVT_LUT_2(B5G5R5X1, x)
#define IPIXEL_FROM_A4R4G4B4(x) IPIXEL_CVT_LUT_2(A4R4G4B4, x)
#define IPIXEL_FROM_A4B4G4R4(x) IPIXEL_CVT_LUT_2(A4B4G4R4, x)
#define IPIXEL_FROM_R4G4B4A4(x) IPIXEL_CVT_LUT_2(R4G4B4A4, x)
#define IPIXEL_FROM_B4G4R4A4(x) IPIXEL_CVT_LUT_2(B4G4R4A4, x)
#define IPIXEL_FROM_X4R4G4B4(x) IPIXEL_CVT_LUT_2(X4R4G4B4, x)
#define IPIXEL_FROM_X4B4G4R4(x) IPIXEL_CVT_LUT_2(X4B4G4R4, x)
#define IPIXEL_FROM_R4G4B4X4(x) IPIXEL_CVT_LUT_2(R4G4B4X4, x)
#define IPIXEL_FROM_B4G4R4X4(x) IPIXEL_CVT_LUT_2(B4G4R4X4, x)

/* convert from 8 bits */
#define IPIXEL_FROM_C8(x) _ipixel_A8R8G8B8_from_index(_ipixel_src_index, x)
#define IPIXEL_FROM_G8(x) _ipixel_asm_8888(255, x, x, x)
#define IPIXEL_FROM_A8(x) _ipixel_asm_8888(x, 0, 0, 0)
#define IPIXEL_FROM_R3G3B2(x) IPIXEL_CVT_LUT_1(R3G3B2, x)
#define IPIXEL_FROM_B2G3R3(x) IPIXEL_CVT_LUT_1(B2G3R3, x)
#define IPIXEL_FROM_X2R2G2B2(x) IPIXEL_CVT_LUT_1(X2R2G2B2, x)
#define IPIXEL_FROM_X2B2G2R2(x) IPIXEL_CVT_LUT_1(X2B2G2R2, x)
#define IPIXEL_FROM_R2G2B2X2(x) IPIXEL_CVT_LUT_1(R2G2B2X2, x)
#define IPIXEL_FROM_B2G2R2X2(x) IPIXEL_CVT_LUT_1(B2G2R2X2, x)
#define IPIXEL_FROM_A2R2G2B2(x) IPIXEL_CVT_LUT_1(A2R2G2B2, x)
#define IPIXEL_FROM_A2B2G2R2(x) IPIXEL_CVT_LUT_1(A2B2G2R2, x)
#define IPIXEL_FROM_R2G2B2A2(x) IPIXEL_CVT_LUT_1(R2G2B2A2, x)
#define IPIXEL_FROM_B2G2R2A2(x) IPIXEL_CVT_LUT_1(B2G2R2A2, x)
#define IPIXEL_FROM_X4C4(x) IPIXEL_FROM_C8(x)
#define IPIXEL_FROM_X4G4(x) IPIXEL_FROM_G8(_ipixel_scale_4[x])
#define IPIXEL_FROM_X4A4(x) IPIXEL_FROM_A8(_ipixel_scale_4[x])
#define IPIXEL_FROM_C4X4(x) IPIXEL_FROM_X4C4(((x) >> 4))
#define IPIXEL_FROM_G4X4(x) IPIXEL_FROM_X4G4(((x) >> 4))
#define IPIXEL_FROM_A4X4(x) IPIXEL_FROM_X4A4(((x) >> 4))

/* convert from 4 bits */
#define IPIXEL_FROM_C4(x) IPIXEL_FROM_X4C4(x)
#define IPIXEL_FROM_G4(x) IPIXEL_FROM_X4G4(x)
#define IPIXEL_FROM_A4(x) IPIXEL_FROM_X4A4(x)
#define IPIXEL_FROM_R1G2B1(x) \
		(	(_ipixel_scale_1[((x) >> 3)] << 16) | \
			(_ipixel_scale_2[((x) & 6) >> 1] << 8) | \
			(_ipixel_scale_1[((x) >> 0) & 1] << 0) | 0xff000000 )
#define IPIXEL_FROM_B1G2R1(x) \
		(	(_ipixel_scale_1[((x) >> 3)] << 0) | \
			(_ipixel_scale_2[((x) & 6) >> 1] << 8) | \
			(_ipixel_scale_1[((x) >> 0) & 1] << 16) | 0xff000000 )
#define IPIXEL_FROM_1111(x, s1, s2, s3, s4) \
		(	(_ipixel_scale_1[((x) >> 3) & 1] << (s1)) | \
			(_ipixel_scale_1[((x) >> 2) & 1] << (s2)) | \
			(_ipixel_scale_1[((x) >> 1) & 1] << (s3)) | \
			(_ipixel_scale_1[((x) >> 0) & 1] << (s4)))
#define IPIXEL_FROM_A1R1G1B1(x) IPIXEL_FROM_1111(x, 24, 16, 8, 0)
#define IPIXEL_FROM_A1B1G1R1(x) IPIXEL_FROM_1111(x, 24, 0, 8, 16)
#define IPIXEL_FROM_R1G1B1A1(x) IPIXEL_FROM_1111(x, 16, 8, 0, 24)
#define IPIXEL_FROM_B1G1R1A1(x) IPIXEL_FROM_1111(x, 0, 8, 16, 24)
#define IPIXEL_FROM_X111(x, sr, sg, sb) \
		(	(_ipixel_scale_1[((x) >> (sr)) & 1] << 16) | \
			(_ipixel_scale_1[((x) >> (sg)) & 1] <<  8) | \
			(_ipixel_scale_1[((x) >> (sb)) & 1] <<  0) | 0xff000000 )
#define IPIXEL_FROM_X1R1G1B1(x) IPIXEL_FROM_X111(x, 2, 1, 0)
#define IPIXEL_FROM_X1B1G1R1(x) IPIXEL_FROM_X111(x, 0, 1, 2)
#define IPIXEL_FROM_R1G1B1X1(x) IPIXEL_FROM_X111(x, 3, 2, 1)
#define IPIXEL_FROM_B1G1R1X1(x) IPIXEL_FROM_X111(x, 1, 2, 3)

/* convert from 1 bits */
#define IPIXEL_FROM_C1(x) IPIXEL_FROM_C8(x)
#define IPIXEL_FROM_G1(x) IPIXEL_FROM_G8(_ipixel_scale_1[(x)])
#define IPIXEL_FROM_A1(x) IPIXEL_FROM_A8(_ipixel_scale_1[(x)])


/* pixel convert to 32 bits */
#define IPIXEL_TO_A8R8G8B8(x) (x)
#define IPIXEL_TO_A8B8G8R8(x) \
			(	((x) & 0xff00ff00) | \
				(((x) >> 16) & 0xff) | (((x) & 0xff) << 16) )
#define IPIXEL_TO_R8G8B8A8(x) \
			(	(((x) & 0xffffff) << 8) | (((x) & 0xff000000) >> 24) )
#define IPIXEL_TO_B8G8R8A8(x) \
		(	(((x) & 0xff) << 24) | \
			(((x) & 0xff00) << 8) | \
			(((x) & 0xff0000) >> 8) | \
			(((x) & 0xff000000) >> 24) )

#define IPIXEL_TO_X8R8G8B8(x) ((x) & 0xffffff)

#define IPIXEL_TO_X8B8G8R8(x) \
		(	(((x) & 0xff) << 16) | \
			((x) & 0xff00) | \
			(((x) & 0xff0000) >> 16) )

#define IPIXEL_TO_R8G8B8X8(x) ((x) << 8)

#define IPIXEL_TO_B8G8R8X8(x) \
		(	(((x) & 0xff) << 24) | \
			(((x) & 0xff00) << 8) | \
			(((x) & 0xff0000) >> 8) )

#define IPIXEL_TO_P8R8G8B8(x) \
		IRGBA_TO_P8R8G8B8(	(((x) & 0xff0000) >> 16), \
							(((x) & 0xff00) >> 8), \
							(((x) & 0xff)), \
							(((x) & 0xff000000) >> 24) )

/* pixel convert to 24 bits */
#define IPIXEL_TO_R8G8B8(x) ((x) & 0xffffff)

#define IPIXEL_TO_B8G8R8(x) \
		(	(((x) & 0xff) << 16) | \
			((x) & 0xff00) | \
			(((x) & 0xff0000) >> 16) )

/* pixel convert to 16 bits */
#define IPIXEL_TO_R5G6B5(x) \
		(	(((x) & 0xf80000) >> 8) | \
			(((x) & 0xfc00) >> 5) | \
			(((x) & 0xf8) >> 3) )

#define IPIXEL_TO_B5G6R5(x) \
		(	(((x) & 0xf8) << 8) | \
			(((x) & 0xfc00) >> 5) | \
			(((x) & 0xf80000) >> 19) )

#define IPIXEL_TO_X1R5G5B5(x) \
		(	(((x) & 0xf80000) >> 9) | \
			(((x) & 0xf800) >> 6) | \
			(((x) & 0xf8) >> 3) )

#define IPIXEL_TO_X1B5G5R5(x) \
		(	(((x) & 0xf8) << 7) | \
			(((x) & 0xf800) >> 6) | \
			(((x) & 0xf80000) >> 19) )

#define IPIXEL_TO_R5G5B5X1(x) \
		(	(((x) & 0xf80000) >> 8) | \
			(((x) & 0xf800) >> 5) | \
			(((x) & 0xf8) >> 2) )

#define IPIXEL_TO_B5G5R5X1(x) \
		(	(((x) & 0xf8) << 8) | \
			(((x) & 0xf800) >> 5) | \
			(((x) & 0xf80000) >> 18) )

#define IPIXEL_TO_A1R5G5B5(x) \
		(	(((x) & 0x80000000) >> 16) | \
			(((x) & 0xf80000) >> 9) | \
			(((x) & 0xf800) >> 6) | \
			(((x) & 0xf8) >> 3) )

#define IPIXEL_TO_A1B5G5R5(x) \
		(	(((x) & 0x80000000) >> 16) | \
			(((x) & 0xf8) << 7) | \
			(((x) & 0xf800) >> 6) | \
			(((x) & 0xf80000) >> 19) )

#define IPIXEL_TO_R5G5B5A1(x) \
		(	(((x) & 0xf80000) >> 8) | \
			(((x) & 0xf800) >> 5) | \
			(((x) & 0xf8) >> 2) | \
			(((x) & 0x80000000) >> 31) )

#define IPIXEL_TO_B5G5R5A1(x) \
		(	(((x) & 0xf8) << 8) | \
			(((x) & 0xf800) >> 5) | \
			(((x) & 0xf80000) >> 18) | \
			(((x) & 0x80000000) >> 31) )

#define IPIXEL_TO_X4R4G4B4(x) \
		(	(((x) & 0xf00000) >> 12) | \
			(((x) & 0xf000) >> 8) | \
			(((x) & 0xf0) >> 4) )

#define IPIXEL_TO_X4B4G4R4(x) \
		(	(((x) & 0xf0) << 4) | \
			(((x) & 0xf000) >> 8) | \
			(((x) & 0xf00000) >> 20) )

#define IPIXEL_TO_R4G4B4X4(x) \
		(	(((x) & 0xf00000) >> 8) | \
			(((x) & 0xf000) >> 4) | \
			((x) & 0xf0) )

#define IPIXEL_TO_B4G4R4X4(x) \
		(	(((x) & 0xf0) << 8) | \
			(((x) & 0xf000) >> 4) | \
			(((x) & 0xf00000) >> 16) )

#define IPIXEL_TO_A4R4G4B4(x) \
		(	(((x) & 0xf0000000) >> 16) | \
			(((x) & 0xf00000) >> 12) | \
			(((x) & 0xf000) >> 8) | \
			(((x) & 0xf0) >> 4) )

#define IPIXEL_TO_A4B4G4R4(x) \
		(	(((x) & 0xf0000000) >> 16) | \
			(((x) & 0xf0) << 4) | \
			(((x) & 0xf000) >> 8) | \
			(((x) & 0xf00000) >> 20) )

#define IPIXEL_TO_R4G4B4A4(x) \
		(	(((x) & 0xf00000) >> 8) | \
			(((x) & 0xf000) >> 4) | \
			((x) & 0xf0) | \
			(((x) & 0xf0000000) >> 28) )

#define IPIXEL_TO_B4G4R4A4(x) \
		(	(((x) & 0xf0) << 8) | \
			(((x) & 0xf000) >> 4) | \
			(((x) & 0xf00000) >> 16) | \
			(((x) & 0xf0000000) >> 28) )

/* pixel convert to 8 bits */
#define IPIXEL_TO_C8(x) _ipixel_RGB_to_ent(_ipixel_dst_index, \
			(((x) & 0xff0000) >> 16), \
			(((x) & 0xff00) >> 8), \
			(((x) & 0xff)) )


#define IPIXEL_TO_G8(x) _ipixel_to_gray( \
			(((x) & 0xff0000) >> 16), \
			(((x) & 0xff00) >> 8), \
			(((x) & 0xff)) )

#define IPIXEL_TO_A8(x) (((x) & 0xff000000) >> 24)

#define IPIXEL_TO_R3G3B2(x) \
		(	(((x) & 0xe00000) >> 16) | \
			(((x) & 0xe000) >> 11) | \
			(((x) & 0xc0) >> 6) )

#define IPIXEL_TO_B2G3R3(x) \
		(	((x) & 0xc0) | \
			(((x) & 0xe000) >> 10) | \
			(((x) & 0xe00000) >> 21) )

#define IPIXEL_TO_X2R2G2B2(x) \
		(	(((x) & 0xc00000) >> 18) | \
			(((x) & 0xc000) >> 12) | \
			(((x) & 0xc0) >> 6) )

#define IPIXEL_TO_X2B2G2R2(x) \
		(	(((x) & 0xc0) >> 2) | \
			(((x) & 0xc000) >> 12) | \
			(((x) & 0xc00000) >> 22) )

#define IPIXEL_TO_R2G2B2X2(x) \
		(	(((x) & 0xc00000) >> 16) | \
			(((x) & 0xc000) >> 10) | \
			(((x) & 0xc0) >> 4) )

#define IPIXEL_TO_B2G2R2X2(x) \
		(	((x) & 0xc0) | \
			(((x) & 0xc000) >> 10) | \
			(((x) & 0xc00000) >> 20) )

#define IPIXEL_TO_A2R2G2B2(x) \
		(	(((x) & 0xc0000000) >> 24) | \
			(((x) & 0xc00000) >> 18) | \
			(((x) & 0xc000) >> 12) | \
			(((x) & 0xc0) >> 6) )

#define IPIXEL_TO_A2B2G2R2(x) \
		(	(((x) & 0xc0000000) >> 24) | \
			(((x) & 0xc0) >> 2) | \
			(((x) & 0xc000) >> 12) | \
			(((x) & 0xc00000) >> 22) )

#define IPIXEL_TO_R2G2B2A2(x) \
		(	(((x) & 0xc00000) >> 16) | \
			(((x) & 0xc000) >> 10) | \
			(((x) & 0xc0) >> 4) | \
			(((x) & 0xc0000000) >> 30) )

#define IPIXEL_TO_B2G2R2A2(x) \
		(	((x) & 0xc0) | \
			(((x) & 0xc000) >> 10) | \
			(((x) & 0xc00000) >> 20) | \
			(((x) & 0xc0000000) >> 30) )

#define IPIXEL_TO_X4C4(x) (IPIXEL_TO_C8(x) & 0xf)
#define IPIXEL_TO_X4G4(x) (IPIXEL_TO_G8(x) >> 4)
#define IPIXEL_TO_X4A4(x) (IPIXEL_TO_A8(x) >> 4)
#define IPIXEL_TO_C4X4(x) ((IPIXEL_TO_C8(x) & 0xf) << 4)
#define IPIXEL_TO_G4X4(x) (IPIXEL_TO_G8(x) & 0xf0)
#define IPIXEL_TO_A4X4(x) (IPIXEL_TO_A8(x) & 0xf0)

/* pixel convert to 4 bits */
#define IPIXEL_TO_C4(x) IPIXEL_TO_X4C4(x)
#define IPIXEL_TO_G4(x) IPIXEL_TO_X4G4(x)
#define IPIXEL_TO_A4(x) IPIXEL_TO_X4A4(x)

#define IPIXEL_TO_R1G2B1(x) \
		(	(((x) & 0x800000) >> 20) | \
			(((x) & 0xc000) >> 13) | \
			(((x) & 0x80) >> 7) )

#define IPIXEL_TO_B1G2R1(x) \
		(	(((x) & 0x80) >> 4) | \
			(((x) & 0xc000) >> 13) | \
			(((x) & 0x800000) >> 23) )

#define IPIXEL_TO_A1R1G1B1(x) \
		(	(((x) & 0x80000000) >> 28) | \
			(((x) & 0x800000) >> 21) | \
			(((x) & 0x8000) >> 14) | \
			(((x) & 0x80) >> 7) )

#define IPIXEL_TO_A1B1G1R1(x) \
		(	(((x) & 0x80000000) >> 28) | \
			(((x) & 0x80) >> 5) | \
			(((x) & 0x8000) >> 14) | \
			(((x) & 0x800000) >> 23) )

#define IPIXEL_TO_R1G1B1A1(x) \
		(	(((x) & 0x800000) >> 20) | \
			(((x) & 0x8000) >> 13) | \
			(((x) & 0x80) >> 6) | \
			(((x) & 0x80000000) >> 31) )

#define IPIXEL_TO_B1G1R1A1(x) \
		(	(((x) & 0x80) >> 4) | \
			(((x) & 0x8000) >> 13) | \
			(((x) & 0x800000) >> 22) | \
			(((x) & 0x80000000) >> 31) )

#define IPIXEL_TO_X1R1G1B1(x) \
		(	(((x) & 0x800000) >> 21) | \
			(((x) & 0x8000) >> 14) | \
			(((x) & 0x80) >> 7) )

#define IPIXEL_TO_X1B1G1R1(x) \
		(	(((x) & 0x80) >> 5) | \
			(((x) & 0x8000) >> 14) | \
			(((x) & 0x800000) >> 23) )

#define IPIXEL_TO_R1G1B1X1(x) \
		(	(((x) & 0x800000) >> 20) | \
			(((x) & 0x8000) >> 13) | \
			(((x) & 0x80) >> 6) )

#define IPIXEL_TO_B1G1R1X1(x) \
		(	(((x) & 0x80) >> 4) | \
			(((x) & 0x8000) >> 13) | \
			(((x) & 0x800000) >> 22) )

/* pixel convert to 1 bit */
#define IPIXEL_TO_C1(x) (IPIXEL_TO_C8(x) & 1)
#define IPIXEL_TO_G1(x) (IPIXEL_TO_G8(x) >> 7)
#define IPIXEL_TO_A1(x) (IPIXEL_TO_A8(x) >> 7)


/**********************************************************************
 * MACRO: PIXEL FORMAT UTILITY
 **********************************************************************/
#define IRGBA_FROM_PIXEL(fmt, c, r, g, b, a) IRGBA_FROM_##fmt(c, r, g, b, a)
#define IRGBA_TO_PIXEL(fmt, r, g, b, a) IRGBA_TO_##fmt(r, g, b, a)
#define IPIXEL_FROM(fmt, c) IPIXEL_FROM_##fmt(c)
#define IPIXEL_TO(fmt, c) IPIXEL_TO_##fmt(c)
#define IPIX_FMT_NBITS(fmt) IPIX_FMT_BPP_##fmt

#define IRGBA_FROM_COLOR(c, fmt, r, g, b, a) do { \
	r = ((((ICOLORD)(c)) & (fmt).rmask) >> (fmt).rshift) << (fmt).rloss; \
	g = ((((ICOLORD)(c)) & (fmt).gmask) >> (fmt).gshift) << (fmt).gloss; \
	b = ((((ICOLORD)(c)) & (fmt).bmask) >> (fmt).bshift) << (fmt).bloss; \
	a = ((((ICOLORD)(c)) & (fmt).amask) >> (fmt).ashift) << (fmt).aloss; \
}	while (0)

#define IRGB_FROM_COLOR(c, fmt, r, g, b) do { \
	r = ((((ICOLORD)(c)) & (fmt).rmask) >> (fmt).rshift) << (fmt).rloss; \
	g = ((((ICOLORD)(c)) & (fmt).gmask) >> (fmt).gshift) << (fmt).gloss; \
	b = ((((ICOLORD)(c)) & (fmt).bmask) >> (fmt).bshift) << (fmt).bloss; \
}	while (0)

#define IRGBA_TO_COLOR(fmt, r, g, b, a) ( \
	(((r) >> (fmt).rloss) << (fmt).rshift) | \
	(((g) >> (fmt).gloss) << (fmt).gshift) | \
	(((b) >> (fmt).bloss) << (fmt).bshift) | \
	(((a) >> (fmt).aloss) << (fmt).ashift))

#define IRGB_TO_COLOR(fmt, r, g, b) ( \
	(((r) >> (fmt).rloss) << (fmt).rshift) | \
	(((g) >> (fmt).gloss) << (fmt).gshift) | \
	(((b) >> (fmt).bloss) << (fmt).bshift))


#define IRGBA_DISEMBLE(fmt, c, r, g, b, a) do { \
		switch (fmt) { \
        case IPIX_FMT_A8R8G8B8: IRGBA_FROM_A8R8G8B8(c, r, g, b, a); break; \
        case IPIX_FMT_A8B8G8R8: IRGBA_FROM_A8B8G8R8(c, r, g, b, a); break; \
        case IPIX_FMT_R8G8B8A8: IRGBA_FROM_R8G8B8A8(c, r, g, b, a); break; \
        case IPIX_FMT_B8G8R8A8: IRGBA_FROM_B8G8R8A8(c, r, g, b, a); break; \
        case IPIX_FMT_X8R8G8B8: IRGBA_FROM_X8R8G8B8(c, r, g, b, a); break; \
        case IPIX_FMT_X8B8G8R8: IRGBA_FROM_X8B8G8R8(c, r, g, b, a); break; \
        case IPIX_FMT_R8G8B8X8: IRGBA_FROM_R8G8B8X8(c, r, g, b, a); break; \
        case IPIX_FMT_B8G8R8X8: IRGBA_FROM_B8G8R8X8(c, r, g, b, a); break; \
        case IPIX_FMT_P8R8G8B8: IRGBA_FROM_P8R8G8B8(c, r, g, b, a); break; \
        case IPIX_FMT_R8G8B8: IRGBA_FROM_R8G8B8(c, r, g, b, a); break; \
        case IPIX_FMT_B8G8R8: IRGBA_FROM_B8G8R8(c, r, g, b, a); break; \
        case IPIX_FMT_R5G6B5: IRGBA_FROM_R5G6B5(c, r, g, b, a); break; \
        case IPIX_FMT_B5G6R5: IRGBA_FROM_B5G6R5(c, r, g, b, a); break; \
        case IPIX_FMT_X1R5G5B5: IRGBA_FROM_X1R5G5B5(c, r, g, b, a); break; \
        case IPIX_FMT_X1B5G5R5: IRGBA_FROM_X1B5G5R5(c, r, g, b, a); break; \
        case IPIX_FMT_R5G5B5X1: IRGBA_FROM_R5G5B5X1(c, r, g, b, a); break; \
        case IPIX_FMT_B5G5R5X1: IRGBA_FROM_B5G5R5X1(c, r, g, b, a); break; \
        case IPIX_FMT_A1R5G5B5: IRGBA_FROM_A1R5G5B5(c, r, g, b, a); break; \
        case IPIX_FMT_A1B5G5R5: IRGBA_FROM_A1B5G5R5(c, r, g, b, a); break; \
        case IPIX_FMT_R5G5B5A1: IRGBA_FROM_R5G5B5A1(c, r, g, b, a); break; \
        case IPIX_FMT_B5G5R5A1: IRGBA_FROM_B5G5R5A1(c, r, g, b, a); break; \
        case IPIX_FMT_X4R4G4B4: IRGBA_FROM_X4R4G4B4(c, r, g, b, a); break; \
        case IPIX_FMT_X4B4G4R4: IRGBA_FROM_X4B4G4R4(c, r, g, b, a); break; \
        case IPIX_FMT_R4G4B4X4: IRGBA_FROM_R4G4B4X4(c, r, g, b, a); break; \
        case IPIX_FMT_B4G4R4X4: IRGBA_FROM_B4G4R4X4(c, r, g, b, a); break; \
        case IPIX_FMT_A4R4G4B4: IRGBA_FROM_A4R4G4B4(c, r, g, b, a); break; \
        case IPIX_FMT_A4B4G4R4: IRGBA_FROM_A4B4G4R4(c, r, g, b, a); break; \
        case IPIX_FMT_R4G4B4A4: IRGBA_FROM_R4G4B4A4(c, r, g, b, a); break; \
        case IPIX_FMT_B4G4R4A4: IRGBA_FROM_B4G4R4A4(c, r, g, b, a); break; \
        case IPIX_FMT_C8: IRGBA_FROM_C8(c, r, g, b, a); break; \
        case IPIX_FMT_G8: IRGBA_FROM_G8(c, r, g, b, a); break; \
        case IPIX_FMT_A8: IRGBA_FROM_A8(c, r, g, b, a); break; \
        case IPIX_FMT_R3G3B2: IRGBA_FROM_R3G3B2(c, r, g, b, a); break; \
        case IPIX_FMT_B2G3R3: IRGBA_FROM_B2G3R3(c, r, g, b, a); break; \
        case IPIX_FMT_X2R2G2B2: IRGBA_FROM_X2R2G2B2(c, r, g, b, a); break; \
        case IPIX_FMT_X2B2G2R2: IRGBA_FROM_X2B2G2R2(c, r, g, b, a); break; \
        case IPIX_FMT_R2G2B2X2: IRGBA_FROM_R2G2B2X2(c, r, g, b, a); break; \
        case IPIX_FMT_B2G2R2X2: IRGBA_FROM_B2G2R2X2(c, r, g, b, a); break; \
        case IPIX_FMT_A2R2G2B2: IRGBA_FROM_A2R2G2B2(c, r, g, b, a); break; \
        case IPIX_FMT_A2B2G2R2: IRGBA_FROM_A2B2G2R2(c, r, g, b, a); break; \
        case IPIX_FMT_R2G2B2A2: IRGBA_FROM_R2G2B2A2(c, r, g, b, a); break; \
        case IPIX_FMT_B2G2R2A2: IRGBA_FROM_B2G2R2A2(c, r, g, b, a); break; \
        case IPIX_FMT_X4C4: IRGBA_FROM_X4C4(c, r, g, b, a); break; \
        case IPIX_FMT_X4G4: IRGBA_FROM_X4G4(c, r, g, b, a); break; \
        case IPIX_FMT_X4A4: IRGBA_FROM_X4A4(c, r, g, b, a); break; \
        case IPIX_FMT_C4X4: IRGBA_FROM_C4X4(c, r, g, b, a); break; \
        case IPIX_FMT_G4X4: IRGBA_FROM_G4X4(c, r, g, b, a); break; \
        case IPIX_FMT_A4X4: IRGBA_FROM_A4X4(c, r, g, b, a); break; \
        case IPIX_FMT_C4: IRGBA_FROM_C4(c, r, g, b, a); break; \
        case IPIX_FMT_G4: IRGBA_FROM_G4(c, r, g, b, a); break; \
        case IPIX_FMT_A4: IRGBA_FROM_A4(c, r, g, b, a); break; \
        case IPIX_FMT_R1G2B1: IRGBA_FROM_R1G2B1(c, r, g, b, a); break; \
        case IPIX_FMT_B1G2R1: IRGBA_FROM_B1G2R1(c, r, g, b, a); break; \
        case IPIX_FMT_A1R1G1B1: IRGBA_FROM_A1R1G1B1(c, r, g, b, a); break; \
        case IPIX_FMT_A1B1G1R1: IRGBA_FROM_A1B1G1R1(c, r, g, b, a); break; \
        case IPIX_FMT_R1G1B1A1: IRGBA_FROM_R1G1B1A1(c, r, g, b, a); break; \
        case IPIX_FMT_B1G1R1A1: IRGBA_FROM_B1G1R1A1(c, r, g, b, a); break; \
        case IPIX_FMT_X1R1G1B1: IRGBA_FROM_X1R1G1B1(c, r, g, b, a); break; \
        case IPIX_FMT_X1B1G1R1: IRGBA_FROM_X1B1G1R1(c, r, g, b, a); break; \
        case IPIX_FMT_R1G1B1X1: IRGBA_FROM_R1G1B1X1(c, r, g, b, a); break; \
        case IPIX_FMT_B1G1R1X1: IRGBA_FROM_B1G1R1X1(c, r, g, b, a); break; \
        case IPIX_FMT_C1: IRGBA_FROM_C1(c, r, g, b, a); break; \
        case IPIX_FMT_G1: IRGBA_FROM_G1(c, r, g, b, a); break; \
        case IPIX_FMT_A1: IRGBA_FROM_A1(c, r, g, b, a); break; \
		default: (r) = (g) = (b) = (a) = 0; break; \
		} \
    }   while (0)


#define IRGBA_ASSEMBLE(fmt, c, r, g, b, a) do { \
		switch (fmt) { \
        case IPIX_FMT_A8R8G8B8: c = IRGBA_TO_A8R8G8B8(r, g, b, a); break; \
        case IPIX_FMT_A8B8G8R8: c = IRGBA_TO_A8B8G8R8(r, g, b, a); break; \
        case IPIX_FMT_R8G8B8A8: c = IRGBA_TO_R8G8B8A8(r, g, b, a); break; \
        case IPIX_FMT_B8G8R8A8: c = IRGBA_TO_B8G8R8A8(r, g, b, a); break; \
        case IPIX_FMT_X8R8G8B8: c = IRGBA_TO_X8R8G8B8(r, g, b, a); break; \
        case IPIX_FMT_X8B8G8R8: c = IRGBA_TO_X8B8G8R8(r, g, b, a); break; \
        case IPIX_FMT_R8G8B8X8: c = IRGBA_TO_R8G8B8X8(r, g, b, a); break; \
        case IPIX_FMT_B8G8R8X8: c = IRGBA_TO_B8G8R8X8(r, g, b, a); break; \
        case IPIX_FMT_P8R8G8B8: c = IRGBA_TO_P8R8G8B8(r, g, b, a); break; \
        case IPIX_FMT_R8G8B8: c = IRGBA_TO_R8G8B8(r, g, b, a); break; \
        case IPIX_FMT_B8G8R8: c = IRGBA_TO_B8G8R8(r, g, b, a); break; \
        case IPIX_FMT_R5G6B5: c = IRGBA_TO_R5G6B5(r, g, b, a); break; \
        case IPIX_FMT_B5G6R5: c = IRGBA_TO_B5G6R5(r, g, b, a); break; \
        case IPIX_FMT_X1R5G5B5: c = IRGBA_TO_X1R5G5B5(r, g, b, a); break; \
        case IPIX_FMT_X1B5G5R5: c = IRGBA_TO_X1B5G5R5(r, g, b, a); break; \
        case IPIX_FMT_R5G5B5X1: c = IRGBA_TO_R5G5B5X1(r, g, b, a); break; \
        case IPIX_FMT_B5G5R5X1: c = IRGBA_TO_B5G5R5X1(r, g, b, a); break; \
        case IPIX_FMT_A1R5G5B5: c = IRGBA_TO_A1R5G5B5(r, g, b, a); break; \
        case IPIX_FMT_A1B5G5R5: c = IRGBA_TO_A1B5G5R5(r, g, b, a); break; \
        case IPIX_FMT_R5G5B5A1: c = IRGBA_TO_R5G5B5A1(r, g, b, a); break; \
        case IPIX_FMT_B5G5R5A1: c = IRGBA_TO_B5G5R5A1(r, g, b, a); break; \
        case IPIX_FMT_X4R4G4B4: c = IRGBA_TO_X4R4G4B4(r, g, b, a); break; \
        case IPIX_FMT_X4B4G4R4: c = IRGBA_TO_X4B4G4R4(r, g, b, a); break; \
        case IPIX_FMT_R4G4B4X4: c = IRGBA_TO_R4G4B4X4(r, g, b, a); break; \
        case IPIX_FMT_B4G4R4X4: c = IRGBA_TO_B4G4R4X4(r, g, b, a); break; \
        case IPIX_FMT_A4R4G4B4: c = IRGBA_TO_A4R4G4B4(r, g, b, a); break; \
        case IPIX_FMT_A4B4G4R4: c = IRGBA_TO_A4B4G4R4(r, g, b, a); break; \
        case IPIX_FMT_R4G4B4A4: c = IRGBA_TO_R4G4B4A4(r, g, b, a); break; \
        case IPIX_FMT_B4G4R4A4: c = IRGBA_TO_B4G4R4A4(r, g, b, a); break; \
        case IPIX_FMT_C8: c = IRGBA_TO_C8(r, g, b, a); break; \
        case IPIX_FMT_G8: c = IRGBA_TO_G8(r, g, b, a); break; \
        case IPIX_FMT_A8: c = IRGBA_TO_A8(r, g, b, a); break; \
        case IPIX_FMT_R3G3B2: c = IRGBA_TO_R3G3B2(r, g, b, a); break; \
        case IPIX_FMT_B2G3R3: c = IRGBA_TO_B2G3R3(r, g, b, a); break; \
        case IPIX_FMT_X2R2G2B2: c = IRGBA_TO_X2R2G2B2(r, g, b, a); break; \
        case IPIX_FMT_X2B2G2R2: c = IRGBA_TO_X2B2G2R2(r, g, b, a); break; \
        case IPIX_FMT_R2G2B2X2: c = IRGBA_TO_R2G2B2X2(r, g, b, a); break; \
        case IPIX_FMT_B2G2R2X2: c = IRGBA_TO_B2G2R2X2(r, g, b, a); break; \
        case IPIX_FMT_A2R2G2B2: c = IRGBA_TO_A2R2G2B2(r, g, b, a); break; \
        case IPIX_FMT_A2B2G2R2: c = IRGBA_TO_A2B2G2R2(r, g, b, a); break; \
        case IPIX_FMT_R2G2B2A2: c = IRGBA_TO_R2G2B2A2(r, g, b, a); break; \
        case IPIX_FMT_B2G2R2A2: c = IRGBA_TO_B2G2R2A2(r, g, b, a); break; \
        case IPIX_FMT_X4C4: c = IRGBA_TO_X4C4(r, g, b, a); break; \
        case IPIX_FMT_X4G4: c = IRGBA_TO_X4G4(r, g, b, a); break; \
        case IPIX_FMT_X4A4: c = IRGBA_TO_X4A4(r, g, b, a); break; \
        case IPIX_FMT_C4X4: c = IRGBA_TO_C4X4(r, g, b, a); break; \
        case IPIX_FMT_G4X4: c = IRGBA_TO_G4X4(r, g, b, a); break; \
        case IPIX_FMT_A4X4: c = IRGBA_TO_A4X4(r, g, b, a); break; \
        case IPIX_FMT_C4: c = IRGBA_TO_C4(r, g, b, a); break; \
        case IPIX_FMT_G4: c = IRGBA_TO_G4(r, g, b, a); break; \
        case IPIX_FMT_A4: c = IRGBA_TO_A4(r, g, b, a); break; \
        case IPIX_FMT_R1G2B1: c = IRGBA_TO_R1G2B1(r, g, b, a); break; \
        case IPIX_FMT_B1G2R1: c = IRGBA_TO_B1G2R1(r, g, b, a); break; \
        case IPIX_FMT_A1R1G1B1: c = IRGBA_TO_A1R1G1B1(r, g, b, a); break; \
        case IPIX_FMT_A1B1G1R1: c = IRGBA_TO_A1B1G1R1(r, g, b, a); break; \
        case IPIX_FMT_R1G1B1A1: c = IRGBA_TO_R1G1B1A1(r, g, b, a); break; \
        case IPIX_FMT_B1G1R1A1: c = IRGBA_TO_B1G1R1A1(r, g, b, a); break; \
        case IPIX_FMT_X1R1G1B1: c = IRGBA_TO_X1R1G1B1(r, g, b, a); break; \
        case IPIX_FMT_X1B1G1R1: c = IRGBA_TO_X1B1G1R1(r, g, b, a); break; \
        case IPIX_FMT_R1G1B1X1: c = IRGBA_TO_R1G1B1X1(r, g, b, a); break; \
        case IPIX_FMT_B1G1R1X1: c = IRGBA_TO_B1G1R1X1(r, g, b, a); break; \
        case IPIX_FMT_C1: c = IRGBA_TO_C1(r, g, b, a); break; \
        case IPIX_FMT_G1: c = IRGBA_TO_G1(r, g, b, a); break; \
        case IPIX_FMT_A1: c = IRGBA_TO_A1(r, g, b, a); break; \
		default: c = 0; break; \
		} \
    }   while (0)

#define IRGBA_FMT_TO(fmt, cc, r, g, b, a) do { \
		uint __X1 = ((r) >> (fmt).rloss) << (fmt).rshift; \
		uint __X2 = ((g) >> (fmt).gloss) << (fmt).gshift; \
		uint __X3 = ((b) >> (fmt).bloss) << (fmt).bshift; \
		uint __X4 = ((a) >> (fmt).aloss) << (fmt).ashift; \
		(cc) = __X1 | __X2 | __X3 | __X4; \
	} while (0)

#define IRGBA_FMT_FROM(fmt, cc, r, g, b, a) do { \
		const byte *__rscale = &_ipixel_bit_scale[(fmt).rloss][0]; \
		const byte *__gscale = &_ipixel_bit_scale[(fmt).gloss][0]; \
		const byte *__bscale = &_ipixel_bit_scale[(fmt).bloss][0]; \
		const byte *__ascale = &_ipixel_bit_scale[(fmt).aloss][0]; \
		(r) = __rscale[((cc) & ((fmt).rmask)) >> ((fmt).rshift)]; \
		(g) = __gscale[((cc) & ((fmt).gmask)) >> ((fmt).gshift)]; \
		(b) = __bscale[((cc) & ((fmt).bmask)) >> ((fmt).bshift)]; \
		(a) = __ascale[((cc) & ((fmt).amask)) >> ((fmt).ashift)]; \
	} while (0)

#define IRGBA_FMT_DISEMBLE(fmt, c, r, g, b, a) do { \
		if ((fmt).format != IPIX_FMT_PACKED) { \
			IRGBA_DISEMBLE((fmt).format, c, r, g, b, a); \
		}	else { \
			IRGBA_FMT_FROM(fmt, c, r, g, b, a); \
		} \
	} while (0)

#define IRGBA_FMT_ASSEMBLE(fmt, c, r, g, b, a) do { \
		if ((fmt).format != IPIX_FMT_PACKED) { \
			IRGBA_ASSEMBLE((fmt).format, c, r, g, b, a); \
		}	else { \
			IRGBA_FMT_TO(fmt, c, r, g, b, a); \
		} \
	} while (0)



/**********************************************************************
 * MACRO: PIXEL FILLING FAST MACRO
 **********************************************************************/
#define _ipixel_fill_32(__bits, __startx, __size, __cc) do { \
		uint *__ptr = (uint*)(__bits) + (__startx); \
		size_t __width = (size_t)(__size); \
		for (; __width > 0; __width--) { *__ptr++ = (uint)(__cc); } \
	}	while (0)

#define _ipixel_fill_24(__bits, __startx, __size, __cc) do { \
		byte *__ptr = (byte*)(__bits) + (__startx) * 3; \
		byte *__dst = __ptr; \
		int __cnt; \
		for (__cnt = 12; __size > 0 && __cnt > 0; __size--, __cnt--) { \
			_ipixel_store(24, __ptr, 0, __cc); \
			__ptr += 3; \
		} \
		for (; __size >= 4; __size -= 4) { \
			((uint*)__ptr)[0] = ((const uint*)__dst)[0]; \
			((uint*)__ptr)[1] = ((const uint*)__dst)[1]; \
			((uint*)__ptr)[2] = ((const uint*)__dst)[2]; \
			__ptr += 12; \
		} \
		for (; __size > 0; __size--) { \
			_ipixel_store(24, __ptr, 0, __cc); \
			__ptr += 3; \
		} \
	}	while (0)

#define _ipixel_fill_16(__bits, __startx, __size, __cc) do { \
		ushort *__ptr = ((ushort*)(__bits)) + (__startx); \
		ushort __c1 = (ushort)((__cc) & 0xffff); \
		size_t __width = (size_t)(__size); \
		for (; __width > 0; __width--) { *__ptr++ = __c1; } \
	}	while (0)

#define _ipixel_fill_8(__bits, __startx, __size, __cc) do { \
		byte *__ptr = ((byte*)(__bits)) + (__startx); \
		byte __c1 = (byte)((__cc) & 0xff); \
		size_t __width = (size_t)(__size); \
		for (; __width > 0; __width--) { *__ptr++ = __c1; } \
	}	while (0)

#define _ipixel_fill_4(__bits, __startx, __size, __cc) do { \
		int __pos = (__startx); \
		int __cnt; \
		for (__cnt = (__size); __cnt > 0; __pos++, __cnt--) { \
			_ipixel_store_4(__bits, __pos, __cc); \
		} \
	}	while (0)

#define _ipixel_fill_1(__bits, __startx, __size, __cc) do { \
		int __pos = (__startx); \
		int __cnt; \
		for (__cnt = (__size); __cnt > 0; __pos++, __cnt--) { \
			_ipixel_store_1(__bits, __pos, __cc); \
		} \
	}	while (0)


#define _ipixel_fill(bpp, bits, startx, size, color) \
		_ipixel_fill_##bpp(bits, startx, size, color)



/**********************************************************************
 * MACRO: PIXEL BLENDING
 **********************************************************************/
/* blend onto a public static surface (no alpha channel) */
#define IBLEND_STATIC(sr, sg, sb, sa, dr, dg, db, da) do { \
		int SA = _ipixel_norm(sa); \
		(dr) = (((((int)(sr)) - ((int)(dr))) * SA) >> 8) + (dr); \
		(dg) = (((((int)(sg)) - ((int)(dg))) * SA) >> 8) + (dg); \
		(db) = (((((int)(sb)) - ((int)(db))) * SA) >> 8) + (db); \
		(da) = 255; \
	}	while (0)

/* blend onto a normal surface (with alpha channel) */
#define IBLEND_NORMAL(sr, sg, sb, sa, dr, dg, db, da) do { \
		int SA = _ipixel_norm(sa); \
		int DA = _ipixel_norm(da); \
		int FA = DA + (((256 - DA) * SA) >> 8); \
		SA = (FA != 0)? ((SA << 8) / FA) : (0); \
		(da) = _ipixel_unnorm(FA); \
		(dr) = (((((int)(sr)) - ((int)(dr))) * SA) >> 8) + (dr); \
		(dg) = (((((int)(sg)) - ((int)(dg))) * SA) >> 8) + (dg); \
		(db) = (((((int)(sb)) - ((int)(db))) * SA) >> 8) + (db); \
	}	while (0)


/* blend onto a normal surface (with alpha channel) in a fast way */
/* looking up alpha values from lut instead of div. calculation */
/* lut must be inited by ipixel_lut_init() */
#define IBLEND_NORMAL_FAST(sr, sg, sb, sa, dr, dg, db, da) do { \
		uint __lutpos = (((da) & 0xf8) << 4) | (((sa) & 0xfc) >> 1); \
		int SA = ipixel_blend_lut[(__lutpos) + 0]; \
		int FA = ipixel_blend_lut[(__lutpos) + 1]; \
		SA = _ipixel_norm((SA)); \
		(da) = FA; \
		(dr) = (((((int)(sr)) - ((int)(dr))) * SA) >> 8) + (dr); \
		(dg) = (((((int)(sg)) - ((int)(dg))) * SA) >> 8) + (dg); \
		(db) = (((((int)(sb)) - ((int)(db))) * SA) >> 8) + (db); \
	}	while (0)

/* premultiplied src over */
#define IBLEND_SRCOVER(sr, sg, sb, sa, dr, dg, db, da) do { \
		uint SA = 255 - (sa); \
		(dr) = (dr) * SA; \
		(dg) = (dg) * SA; \
		(db) = (db) * SA; \
		(da) = (da) * SA; \
		(dr) = _ipixel_fast_div_255(dr) + (sr); \
		(dg) = _ipixel_fast_div_255(dg) + (sg); \
		(db) = _ipixel_fast_div_255(db) + (sb); \
		(da) = _ipixel_fast_div_255(da) + (sa); \
	}	while (0)

/* additive blend */
#define IBLEND_ADDITIVE(sr, sg, sb, sa, dr, dg, db, da) do { \
		int XA = _ipixel_norm(sa); \
		int XR = (sr) * XA; \
		int XG = (sg) * XA; \
		int XB = (sb) * XA; \
		XA = (sa) + (da); \
		XR = (XR >> 8) + (dr); \
		XG = (XG >> 8) + (dg); \
		XB = (XB >> 8) + (db); \
		(dr) = ICLIP_256(XR); \
		(dg) = ICLIP_256(XG); \
		(db) = ICLIP_256(XB); \
		(da) = ICLIP_256(XA); \
	}	while (0)


/* premutiplied 32bits blending: 
   dst = src + (255 - src.alpha) * dst / 255 */
#define IBLEND_PARGB(color_dst, color_src) do { \
		uint __A = 255 - ((color_src) >> 24); \
		uint __DST_RB = (color_dst) & 0xff00ff; \
		uint __DST_AG = ((color_dst) >> 8) & 0xff00ff; \
		__DST_RB *= __A; \
		__DST_AG *= __A; \
		__DST_RB += __DST_RB >> 8; \
		__DST_AG += __DST_AG >> 8; \
		__DST_RB >>= 8; \
		__DST_AG &= 0xff00ff00; \
		__A = (__DST_RB & 0xff00ff) | __DST_AG; \
		(color_dst) = __A + (color_src); \
	}	while (0)


/* premutiplied 32bits blending (with coverage): 
   tmp = src * coverage / 255,
   dst = tmp + (255 - tmp.alpha) * dst / 255 */
#define IBLEND_PARGB_COVER(color_dst, color_src, coverage) do { \
		uint __r1 = (color_src) & 0xff00ff; \
		uint __r2 = ((color_src) >> 8) & 0xff00ff; \
		uint __r3 = _ipixel_norm(coverage); \
		uint __r4; \
		__r1 *= __r3; \
		__r2 *= __r3; \
		__r3 = (color_dst) & 0xff00ff; \
		__r4 = ((color_dst) >> 8) & 0xff00ff; \
		__r1 = ((__r1) >> 8) & 0xff00ff; \
		__r2 = (__r2) & 0xff00ff00; \
		__r1 = __r1 | __r2; \
		__r2 = 255 - (__r2 >> 24); \
		__r3 *= __r2; \
		__r4 *= __r2; \
		__r3 = ((__r3 + (__r3 >> 8)) >> 8) & 0xff00ff; \
		__r4 = (__r4 + (__r4 >> 8)) & 0xff00ff00; \
		(color_dst) = (__r3 | __r4) + (__r1); \
	}	while (0)


/* compositing */
#define IBLEND_COMPOSITE(sr, sg, sb, sa, dr, dg, db, da, FS, FD) do { \
		(dr) = _ipixel_mullut[(FS)][(sr)] + _ipixel_mullut[(FD)][(dr)]; \
		(dg) = _ipixel_mullut[(FS)][(sg)] + _ipixel_mullut[(FD)][(dg)]; \
		(db) = _ipixel_mullut[(FS)][(sb)] + _ipixel_mullut[(FD)][(db)]; \
		(da) = _ipixel_mullut[(FS)][(sa)] + _ipixel_mullut[(FD)][(da)]; \
	}	while (0)

/* premultiply: src atop */
#define IBLEND_OP_SRC_ATOP(sr, sg, sb, sa, dr, dg, db, da) do { \
		uint FS = (da); \
		uint FD = 255 - (sa); \
		IBLEND_COMPOSITE(sr, sg, sb, sa, dr, dg, db, da, FS, FD); \
	}	while (0)

/* premultiply: src in */
#define IBLEND_OP_SRC_IN(sr, sg, sb, sa, dr, dg, db, da) do { \
		uint FS = (da); \
		IBLEND_COMPOSITE(sr, sg, sb, sa, dr, dg, db, da, FS, 0); \
	}	while (0)

/* premultiply: src out */
#define IBLEND_OP_SRC_OUT(sr, sg, sb, sa, dr, dg, db, da) do { \
		uint FS = 255 - (da); \
		IBLEND_COMPOSITE(sr, sg, sb, sa, dr, dg, db, da, FS, 0); \
	}	while (0)

/* premultiply: src over */
#define IBLEND_OP_SRC_OVER(sr, sg, sb, sa, dr, dg, db, da) do { \
		uint FD = 255 - (sa); \
		IBLEND_COMPOSITE(sr, sg, sb, sa, dr, dg, db, da, 255, FD); \
	}	while (0)

/* premultiply: dst atop */
#define IBLEND_OP_DST_ATOP(sr, sg, sb, sa, dr, dg, db, da) do { \
		uint FS = 255 - (da); \
		uint FD = (sa); \
		IBLEND_COMPOSITE(sr, sg, sb, sa, dr, dg, db, da, FS, FD); \
	}	while (0)

/* premultiply: dst in */
#define IBLEND_OP_DST_IN(sr, sg, sb, sa, dr, dg, db, da) do { \
		uint FD = (sa); \
		IBLEND_COMPOSITE(sr, sg, sb, sa, dr, dg, db, da, 0, FD); \
	}	while (0)

/* premultiply: dst out */
#define IBLEND_OP_DST_OUT(sr, sg, sb, sa, dr, dg, db, da) do { \
		uint FD = 255 - (sa); \
		IBLEND_COMPOSITE(sr, sg, sb, sa, dr, dg, db, da, 0, FD); \
	}	while (0)

/* premultiply: dst over */
#define IBLEND_OP_DST_OVER(sr, sg, sb, sa, dr, dg, db, da) do { \
		uint FS = 255 - (da); \
		IBLEND_COMPOSITE(sr, sg, sb, sa, dr, dg, db, da, FS, 255); \
	}	while (0)

/* premultiply: xor */
#define IBLEND_OP_XOR(sr, sg, sb, sa, dr, dg, db, da) do { \
		uint FS = 255 - (da); \
		uint FD = 255 - (sa); \
		IBLEND_COMPOSITE(sr, sg, sb, sa, dr, dg, db, da, FS, FD); \
	}	while (0)

/* premultiply: plus */
#define IBLEND_OP_PLUS(sr, sg, sb, sa, dr, dg, db, da) do { \
		(dr) = ICLIP_256((sr) + (dr)); \
		(dg) = ICLIP_256((sg) + (dg)); \
		(db) = ICLIP_256((sb) + (db)); \
		(da) = ICLIP_256((sa) + (da)); \
	}	while (0)

    }
}

	#define IPIXEL_LUT_INIT(fmt, nbytes) \
		ipixel_lut_##nbytes##_to_4(IPIX_FMT_##fmt, IPIX_FMT_A8R8G8B8, \
			_ipixel_cvt_lut_##fmt)

	IPIXEL_LUT_INIT(R5G6B5, 2);
	IPIXEL_LUT_INIT(B5G6R5, 2);
	IPIXEL_LUT_INIT(X1R5G5B5, 2);
	IPIXEL_LUT_INIT(X1B5G5R5, 2);
	IPIXEL_LUT_INIT(R5G5B5X1, 2);
	IPIXEL_LUT_INIT(B5G5R5X1, 2);
	IPIXEL_LUT_INIT(A1R5G5B5, 2);
	IPIXEL_LUT_INIT(A1B5G5R5, 2);
	IPIXEL_LUT_INIT(R5G5B5A1, 2);
	IPIXEL_LUT_INIT(B5G5R5A1, 2);
	IPIXEL_LUT_INIT(X4R4G4B4, 2);
	IPIXEL_LUT_INIT(X4B4G4R4, 2);
	IPIXEL_LUT_INIT(R4G4B4X4, 2);
	IPIXEL_LUT_INIT(B4G4R4X4, 2);
	IPIXEL_LUT_INIT(A4R4G4B4, 2);
	IPIXEL_LUT_INIT(A4B4G4R4, 2);
	IPIXEL_LUT_INIT(R4G4B4A4, 2);
	IPIXEL_LUT_INIT(B4G4R4A4, 2);
	IPIXEL_LUT_INIT(R3G3B2, 1);
	IPIXEL_LUT_INIT(B2G3R3, 1);
	IPIXEL_LUT_INIT(X2R2G2B2, 1);
	IPIXEL_LUT_INIT(X2B2G2R2, 1);
	IPIXEL_LUT_INIT(R2G2B2X2, 1);
	IPIXEL_LUT_INIT(B2G2R2X2, 1);
	IPIXEL_LUT_INIT(A2R2G2B2, 1);
	IPIXEL_LUT_INIT(A2B2G2R2, 1);
	IPIXEL_LUT_INIT(R2G2B2A2, 1);
	IPIXEL_LUT_INIT(B2G2R2A2, 1);

	#undef IPIXEL_LUT_INIT




IFETCH_PROC(B8G8R8, 24)

IFETCH_PROC(R5G6B5, 16)
IFETCH_PROC(B5G6R5, 16)
IFETCH_PROC(X1R5G5B5, 16)
IFETCH_PROC(X1B5G5R5, 16)
IFETCH_PROC(R5G5B5X1, 16)
IFETCH_PROC(B5G5R5X1, 16)
IFETCH_PROC(A1R5G5B5, 16)
IFETCH_PROC(A1B5G5R5, 16)
IFETCH_PROC(R5G5B5A1, 16)
IFETCH_PROC(B5G5R5A1, 16)
IFETCH_PROC(X4R4G4B4, 16)
IFETCH_PROC(X4B4G4R4, 16)
IFETCH_PROC(R4G4B4X4, 16)
IFETCH_PROC(B4G4R4X4, 16)
IFETCH_PROC(A4R4G4B4, 16)
IFETCH_PROC(A4B4G4R4, 16)
IFETCH_PROC(R4G4B4A4, 16)
IFETCH_PROC(B4G4R4A4, 16)
IFETCH_PROC(C8, 8)
IFETCH_PROC(G8, 8)
IFETCH_PROC(A8, 8)
IFETCH_PROC(R3G3B2, 8)
IFETCH_PROC(B2G3R3, 8)
IFETCH_PROC(X2R2G2B2, 8)
IFETCH_PROC(X2B2G2R2, 8)
IFETCH_PROC(R2G2B2X2, 8)
IFETCH_PROC(B2G2R2X2, 8)
IFETCH_PROC(A2R2G2B2, 8)
IFETCH_PROC(A2B2G2R2, 8)
IFETCH_PROC(R2G2B2A2, 8)
IFETCH_PROC(B2G2R2A2, 8)
IFETCH_PROC(X4C4, 8)
IFETCH_PROC(X4G4, 8)
IFETCH_PROC(X4A4, 8)
IFETCH_PROC(C4X4, 8)
IFETCH_PROC(G4X4, 8)
IFETCH_PROC(A4X4, 8)
IFETCH_PROC(C4, 4)
IFETCH_PROC(G4, 4)
IFETCH_PROC(A4, 4)
IFETCH_PROC(R1G2B1, 4)
IFETCH_PROC(B1G2R1, 4)
IFETCH_PROC(A1R1G1B1, 4)
IFETCH_PROC(A1B1G1R1, 4)
IFETCH_PROC(R1G1B1A1, 4)
IFETCH_PROC(B1G1R1A1, 4)
IFETCH_PROC(X1R1G1B1, 4)
IFETCH_PROC(X1B1G1R1, 4)
IFETCH_PROC(R1G1B1X1, 4)
IFETCH_PROC(B1G1R1X1, 4)
IFETCH_PROC(C1, 1)
IFETCH_PROC(G1, 1)
IFETCH_PROC(A1, 1)


ISTORE_PROC(B8G8R8A8, 32)

ISTORE_PROC(X8B8G8R8, 32)

ISTORE_PROC(B8G8R8X8, 32)
ISTORE_PROC(P8R8G8B8, 32)

ISTORE_PROC(X1B5G5R5, 16)
ISTORE_PROC(R5G5B5X1, 16)
ISTORE_PROC(B5G5R5X1, 16)

ISTORE_PROC(A1R5G5B5, 16)
ISTORE_PROC(A1B5G5R5, 16)
ISTORE_PROC(R5G5B5A1, 16)
ISTORE_PROC(B5G5R5A1, 16)
ISTORE_PROC(X4R4G4B4, 16)
ISTORE_PROC(X4B4G4R4, 16)
ISTORE_PROC(R4G4B4X4, 16)
ISTORE_PROC(B4G4R4X4, 16)

ISTORE_PROC(A4R4G4B4, 16)
ISTORE_PROC(A4B4G4R4, 16)


ISTORE_PROC(B4G4R4A4, 16)
ISTORE_PROC(C8, 8)
ISTORE_PROC(G8, 8)
ISTORE_PROC(A8, 8)
ISTORE_PROC(R3G3B2, 8)
ISTORE_PROC(B2G3R3, 8)
ISTORE_PROC(X2R2G2B2, 8)
ISTORE_PROC(X2B2G2R2, 8)
ISTORE_PROC(R2G2B2X2, 8)
ISTORE_PROC(B2G2R2X2, 8)
ISTORE_PROC(A2R2G2B2, 8)
ISTORE_PROC(A2B2G2R2, 8)
ISTORE_PROC(R2G2B2A2, 8)
ISTORE_PROC(B2G2R2A2, 8)
ISTORE_PROC(X4C4, 8)
ISTORE_PROC(X4G4, 8)
ISTORE_PROC(X4A4, 8)
ISTORE_PROC(C4X4, 8)
ISTORE_PROC(G4X4, 8)
ISTORE_PROC(A4X4, 8)
ISTORE_PROC(C4, 4)
ISTORE_PROC(G4, 4)
ISTORE_PROC(A4, 4)
ISTORE_PROC(R1G2B1, 4)
ISTORE_PROC(B1G2R1, 4)
ISTORE_PROC(A1R1G1B1, 4)
ISTORE_PROC(A1B1G1R1, 4)
ISTORE_PROC(R1G1B1A1, 4)
ISTORE_PROC(B1G1R1A1, 4)
ISTORE_PROC(X1R1G1B1, 4)
ISTORE_PROC(X1B1G1R1, 4)
ISTORE_PROC(R1G1B1X1, 4)
ISTORE_PROC(B1G1R1X1, 4)
ISTORE_PROC(C1, 1)
ISTORE_PROC(G1, 1)
ISTORE_PROC(A1, 1)


IFETCH_PIXEL(R8G8B8X8, 32)
IFETCH_PIXEL(B8G8R8X8, 32)
IFETCH_PIXEL(P8R8G8B8, 32)

IFETCH_PIXEL(R8G8B8, 24)
IFETCH_PIXEL(B8G8R8, 24)
IFETCH_PIXEL(R5G6B5, 16)
IFETCH_PIXEL(B5G6R5, 16)
IFETCH_PIXEL(X1R5G5B5, 16)
IFETCH_PIXEL(X1B5G5R5, 16)
IFETCH_PIXEL(R5G5B5X1, 16)
IFETCH_PIXEL(B5G5R5X1, 16)
IFETCH_PIXEL(A1R5G5B5, 16)
IFETCH_PIXEL(A1B5G5R5, 16)
IFETCH_PIXEL(R5G5B5A1, 16)
IFETCH_PIXEL(B5G5R5A1, 16)
IFETCH_PIXEL(X4R4G4B4, 16)
IFETCH_PIXEL(X4B4G4R4, 16)
IFETCH_PIXEL(R4G4B4X4, 16)
IFETCH_PIXEL(B4G4R4X4, 16)
IFETCH_PIXEL(A4R4G4B4, 16)
IFETCH_PIXEL(A4B4G4R4, 16)
IFETCH_PIXEL(R4G4B4A4, 16)
IFETCH_PIXEL(B4G4R4A4, 16)
IFETCH_PIXEL(C8, 8)
IFETCH_PIXEL(G8, 8)
IFETCH_PIXEL(A8, 8)
IFETCH_PIXEL(R3G3B2, 8)
IFETCH_PIXEL(B2G3R3, 8)
IFETCH_PIXEL(X2R2G2B2, 8)
IFETCH_PIXEL(X2B2G2R2, 8)
IFETCH_PIXEL(R2G2B2X2, 8)
IFETCH_PIXEL(B2G2R2X2, 8)
IFETCH_PIXEL(A2R2G2B2, 8)
IFETCH_PIXEL(A2B2G2R2, 8)
IFETCH_PIXEL(R2G2B2A2, 8)
IFETCH_PIXEL(B2G2R2A2, 8)
IFETCH_PIXEL(X4C4, 8)
IFETCH_PIXEL(X4G4, 8)
IFETCH_PIXEL(X4A4, 8)
IFETCH_PIXEL(C4X4, 8)
IFETCH_PIXEL(G4X4, 8)
IFETCH_PIXEL(A4X4, 8)
IFETCH_PIXEL(C4, 4)
IFETCH_PIXEL(G4, 4)
IFETCH_PIXEL(A4, 4)
IFETCH_PIXEL(R1G2B1, 4)
IFETCH_PIXEL(B1G2R1, 4)
IFETCH_PIXEL(A1R1G1B1, 4)
IFETCH_PIXEL(A1B1G1R1, 4)
IFETCH_PIXEL(R1G1B1A1, 4)
IFETCH_PIXEL(B1G1R1A1, 4)
IFETCH_PIXEL(X1R1G1B1, 4)
IFETCH_PIXEL(X1B1G1R1, 4)
IFETCH_PIXEL(R1G1B1X1, 4)
IFETCH_PIXEL(B1G1R1X1, 4)
IFETCH_PIXEL(C1, 1)
IFETCH_PIXEL(G1, 1)
IFETCH_PIXEL(A1, 1)


#define ITABLE_ITEM(fmt) { \
		_ifetch_proc_##fmt, _ifetch_proc_##fmt, \
		_istore_proc_##fmt, _istore_proc_##fmt, \
		_ifetch_pixel_##fmt, _ifetch_pixel_##fmt }


/**********************************************************************
 * FETCHING STORING LOOK UP TABLE
 **********************************************************************/
#define IFETCH_LUT_2(sfmt) \
uint _ipixel_cvt_lut_##sfmt[256 * 2]; \
public static void _ifetch_proc_lut_##sfmt(const void *bits, int x, \
    int w, uint *buffer, iColorIndex *idx) \
{ \
	const byte *input = ( byte*)bits + (x << 1); \
	uint c1, c2; \
	for (; w > 0; w--) { \
		c1 = _ipixel_cvt_lut_##sfmt[*input++ +   0]; \
		c2 = _ipixel_cvt_lut_##sfmt[*input++ + 256]; \
		*buffer++ = c1 | c2; \
	} \
} \
public static uint _ifetch_pixel_lut_##sfmt(const void *bits, \
    int offset, iColorIndex *idx) \
{ \
	const byte *input = ( byte*)bits + (offset << 1); \
	uint c1, c2; \
	c1 = _ipixel_cvt_lut_##sfmt[*input++ +   0]; \
	c2 = _ipixel_cvt_lut_##sfmt[*input++ + 256]; \
	return c1 | c2; \
}


#define IFETCH_LUT_1(sfmt) \
uint _ipixel_cvt_lut_##sfmt[256]; \
public static void _ifetch_proc_lut_##sfmt(const void *bits, int x, \
    int w, uint *buffer, iColorIndex *idx) \
{ \
	const byte *input = ( byte*)bits + x; \
	uint c1; \
	for (; w > 0; w--) { \
		c1 = _ipixel_cvt_lut_##sfmt[*input++]; \
		*buffer++ = c1; \
	} \
} \
public static uint _ifetch_pixel_lut_##sfmt(const void *bits, \
    int offset, iColorIndex *idx) \
{ \
	const byte *input = ( byte*)bits + offset; \
	return _ipixel_cvt_lut_##sfmt[*input]; \
}

#define IFETCH_LUT_MAIN(sfmt, nbytes) \
	IFETCH_LUT_##nbytes(sfmt)


IFETCH_LUT_MAIN(R5G6B5, 2)
IFETCH_LUT_MAIN(B5G6R5, 2)
IFETCH_LUT_MAIN(X1R5G5B5, 2)
IFETCH_LUT_MAIN(X1B5G5R5, 2)
IFETCH_LUT_MAIN(R5G5B5X1, 2)
IFETCH_LUT_MAIN(B5G5R5X1, 2)
IFETCH_LUT_MAIN(A1R5G5B5, 2)
IFETCH_LUT_MAIN(A1B5G5R5, 2)
IFETCH_LUT_MAIN(R5G5B5A1, 2)
IFETCH_LUT_MAIN(B5G5R5A1, 2)
IFETCH_LUT_MAIN(X4R4G4B4, 2)
IFETCH_LUT_MAIN(X4B4G4R4, 2)
IFETCH_LUT_MAIN(R4G4B4X4, 2)
IFETCH_LUT_MAIN(B4G4R4X4, 2)
IFETCH_LUT_MAIN(A4R4G4B4, 2)
IFETCH_LUT_MAIN(A4B4G4R4, 2)
IFETCH_LUT_MAIN(R4G4B4A4, 2)
IFETCH_LUT_MAIN(B4G4R4A4, 2)
IFETCH_LUT_MAIN(R3G3B2, 1)
IFETCH_LUT_MAIN(B2G3R3, 1)
IFETCH_LUT_MAIN(X2R2G2B2, 1)
IFETCH_LUT_MAIN(X2B2G2R2, 1)
IFETCH_LUT_MAIN(R2G2B2X2, 1)
IFETCH_LUT_MAIN(B2G2R2X2, 1)
IFETCH_LUT_MAIN(A2R2G2B2, 1)
IFETCH_LUT_MAIN(A2B2G2R2, 1)
IFETCH_LUT_MAIN(R2G2B2A2, 1)
IFETCH_LUT_MAIN(B2G2R2A2, 1)


#define ITABLE_ITEM(fmt) \
{
    IPIX_FMT_##fmt, _ifetch_proc_lut_##fmt, _ifetch_pixel_lut_##fmt }




/**********************************************************************
 * SPAN DRAWING
 **********************************************************************/
/* span blending for 8, 16, 24, 32 bits */
#define IPIXEL_SPAN_DRAW_PROC_N(fmt, bpp, nbytes, mode) \
public static void ipixel_span_draw_proc_##fmt##_0(void *bits, \
	int offset, int w, uint *card, byte *cover, \
	const iColorIndex *_ipixel_src_index) \
{ \
	byte *dst = ((byte*)bits) + offset * nbytes; \
	uint cc, r1, g1, b1, a1, r2, g2, b2, a2, inc; \
	if (cover == null) { \
		for (inc = w; inc > 0; inc--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			if (a1 == 255) { \
				cc = IRGBA_TO_PIXEL(fmt, r1, g1, b1, 255); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			else if (a1 > 0) { \
				cc = _ipixel_fetch(bpp, dst, 0); \
				IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
				IBLEND_##mode(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			card++; \
			dst += nbytes; \
		} \
	}	else { \
		for (inc = w; inc > 0; inc--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			cc = *cover++; \
			r2 = a1 + cc; \
			if (r2 == 510) { \
				cc = IRGBA_TO_PIXEL(fmt, r1, g1, b1, 255); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			else if (r2 > 0 && cc > 0) { \
				a1 = _imul_y_div_255(a1, cc); \
				cc = _ipixel_fetch(bpp, dst, 0); \
				IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
				IBLEND_##mode(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			card++; \
			dst += nbytes; \
		} \
	} \
	cc = a1 + a2 + r1 + r2 + g1 + g2 + b1 + b2; \
} \
public static void ipixel_span_draw_proc_##fmt##_1(void *bits, \
	int offset, int w, uint *card, byte *cover, \
	const iColorIndex *_ipixel_src_index) \
{ \
	byte *dst = ((byte*)bits) + offset * nbytes; \
	uint cc, r1, g1, b1, a1, r2, g2, b2, a2, inc; \
	if (cover == null) { \
		for (inc = w; inc > 0; inc--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			if (a1 == 255) { \
				cc = IRGBA_TO_PIXEL(fmt, r1, g1, b1, 255); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			else if (a1 > 0) { \
				cc = _ipixel_fetch(bpp, dst, 0); \
				IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
				IBLEND_SRCOVER(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			card++; \
			dst += nbytes; \
		} \
	}	else { \
		for (inc = w; inc > 0; inc--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			cc = *cover++; \
			r2 = a1 + cc; \
			if (r2 == 510) { \
				cc = IRGBA_TO_PIXEL(fmt, r1, g1, b1, 255); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			else if (r2 > 0 && cc > 0) { \
				a1 = _imul_y_div_255(a1, cc); \
				cc = _ipixel_fetch(bpp, dst, 0); \
				IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
				r1 = _imul_y_div_255(r1, cc); \
				g1 = _imul_y_div_255(g1, cc); \
				b1 = _imul_y_div_255(b1, cc); \
				IBLEND_SRCOVER(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			card++; \
			dst += nbytes; \
		} \
	} \
	cc = a1 + a2 + r1 + r2 + g1 + g2 + b1 + b2; \
} \
public static void ipixel_span_draw_proc_##fmt##_2(void *bits, \
	int offset, int w, uint *card, byte *cover, \
	const iColorIndex *_ipixel_src_index) \
{ \
	byte *dst = ((byte*)bits) + offset * nbytes; \
	uint cc, r1, g1, b1, a1, r2, g2, b2, a2, inc; \
	if (cover == null) { \
		for (inc = w; inc > 0; inc--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			if (a1 > 0) { \
				cc = _ipixel_fetch(bpp, dst, 0); \
				IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
				IBLEND_ADDITIVE(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			card++; \
			dst += nbytes; \
		} \
	}	else { \
		for (inc = w; inc > 0; inc--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			cc = *cover++; \
			if (a1 > 0 && cc > 0) { \
				a1 = _imul_y_div_255(a1, cc); \
				cc = _ipixel_fetch(bpp, dst, 0); \
				IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
				IBLEND_ADDITIVE(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			card++; \
			dst += nbytes; \
		} \
	} \
	cc = a1 + a2 + r1 + r2 + g1 + g2 + b1 + b2; \
}

/* span blending for 8 bits without palette */
#define IPIXEL_SPAN_DRAW_PROC_1(fmt, bpp, nbytes, mode) \
public static void ipixel_span_draw_proc_##fmt##_0(void *bits, \
	int offset, int w, uint *card, byte *cover, \
	const iColorIndex *_ipixel_src_index) \
{ \
	byte *dst = ((byte*)bits) + (offset); \
	uint cc, r1, g1, b1, a1, r2, g2, b2, a2, inc; \
	if (cover == null) { \
		for (inc = w; inc > 0; inc--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			if (a1 == 255) { \
				cc = IRGBA_TO_PIXEL(fmt, r1, g1, b1, 255); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			else if (a1 > 0) { \
				r1 = dst[0]; \
				cc = _ipixel_cvt_lut_##fmt[r1]; \
				IRGBA_FROM_PIXEL(A8R8G8B8, cc, r2, g2, b2, a2); \
				IBLEND_##mode(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			card++; \
			dst++; \
		} \
	}	else { \
		for (inc = w; inc > 0; inc--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			cc = *cover++; \
			r2 = a1 + cc; \
			if (r2 == 510) { \
				cc = IRGBA_TO_PIXEL(fmt, r1, g1, b1, 255); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			else if (r2 > 0 && cc > 0) { \
				a1 = _imul_y_div_255(a1, cc); \
				r1 = dst[0]; \
				cc = _ipixel_cvt_lut_##fmt[r1]; \
				IRGBA_FROM_PIXEL(A8R8G8B8, cc, r2, g2, b2, a2); \
				IBLEND_##mode(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			card++; \
			dst++; \
		} \
	} \
	cc = a1 + a2 + r1 + r2 + g1 + g2 + b1 + b2; \
} \
public static void ipixel_span_draw_proc_##fmt##_1(void *bits, \
	int offset, int w, uint *card, byte *cover, \
	const iColorIndex *_ipixel_src_index) \
{ \
	byte *dst = ((byte*)bits) + (offset); \
	uint cc, r1, g1, b1, a1, r2, g2, b2, a2, inc; \
	if (cover == null) { \
		for (inc = w; inc > 0; inc--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			if (a1 == 255) { \
				cc = IRGBA_TO_PIXEL(fmt, r1, g1, b1, 255); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			else if (a1 > 0) { \
				r1 = dst[0]; \
				cc = _ipixel_cvt_lut_##fmt[r1]; \
				IRGBA_FROM_PIXEL(A8R8G8B8, cc, r2, g2, b2, a2); \
				IBLEND_SRCOVER(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			card++; \
			dst++; \
		} \
	}	else { \
		for (inc = w; inc > 0; inc--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			cc = *cover++; \
			r2 = a1 + cc; \
			if (r2 == 510) { \
				cc = IRGBA_TO_PIXEL(fmt, r1, g1, b1, 255); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			else if (r2 > 0 && cc > 0) { \
				a1 = _imul_y_div_255(a1, cc); \
				r1 = dst[0]; \
				cc = _ipixel_cvt_lut_##fmt[r1]; \
				IRGBA_FROM_PIXEL(A8R8G8B8, cc, r2, g2, b2, a2); \
				r1 = _imul_y_div_255(r1, cc); \
				g1 = _imul_y_div_255(g1, cc); \
				b1 = _imul_y_div_255(b1, cc); \
				IBLEND_SRCOVER(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			card++; \
			dst++; \
		} \
	} \
	cc = a1 + a2 + r1 + r2 + g1 + g2 + b1 + b2; \
} \
public static void ipixel_span_draw_proc_##fmt##_2(void *bits, \
	int offset, int w, uint *card, byte *cover, \
	const iColorIndex *_ipixel_src_index) \
{ \
	byte *dst = ((byte*)bits) + (offset); \
	uint cc, r1, g1, b1, a1, r2, g2, b2, a2, inc; \
	if (cover == null) { \
		for (inc = w; inc > 0; inc--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			if (a1 > 0) { \
				r1 = dst[0]; \
				cc = _ipixel_cvt_lut_##fmt[r1]; \
				IRGBA_FROM_PIXEL(A8R8G8B8, cc, r2, g2, b2, a2); \
				IBLEND_ADDITIVE(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			card++; \
			dst++; \
		} \
	}	else { \
		for (inc = w; inc > 0; inc--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			cc = *cover++; \
			if (a1 > 0 && cc > 0) { \
				a1 = _imul_y_div_255(a1, cc); \
				r1 = dst[0]; \
				cc = _ipixel_cvt_lut_##fmt[r1]; \
				IRGBA_FROM_PIXEL(A8R8G8B8, cc, r2, g2, b2, a2); \
				IBLEND_ADDITIVE(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
			card++; \
			dst++; \
		} \
	} \
	cc = a1 + a2 + r1 + r2 + g1 + g2 + b1 + b2; \
}

/* span blending for 8/4/1 bits with or without palette */
#define IPIXEL_SPAN_DRAW_PROC_X(fmt, bpp, nbytes, mode, init) \
public static void ipixel_span_draw_proc_##fmt##_0(void *bits, \
	int offset, int w, uint *card, byte *cover, \
	const iColorIndex *_ipixel_src_index) \
{ \
	uint cc, r1, g1, b1, a1, r2, g2, b2, a2, inc; \
	byte *dst = (byte*)bits; \
	init; \
	if (cover == null) { \
		for (inc = offset; w > 0; inc++, w--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			if (a1 == 255) { \
				cc = IRGBA_TO_PIXEL(fmt, r1, g1, b1, 255); \
				_ipixel_store(bpp, dst, inc, cc); \
			} \
			else if (a1 > 0) { \
				cc = _ipixel_fetch(bpp, dst, inc); \
				IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
				IBLEND_##mode(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, inc, cc); \
			} \
			card++; \
		} \
	}	else { \
		for (inc = offset; w > 0; inc++, w--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			cc = *cover++; \
			r2 = a1 + cc; \
			if (r2 == 510) { \
				cc = IRGBA_TO_PIXEL(fmt, r1, g1, b1, 255); \
				_ipixel_store(bpp, dst, inc, cc); \
			} \
			else if (r2 > 0 && cc > 0) { \
				a1 = _imul_y_div_255(a1, cc); \
				cc = _ipixel_fetch(bpp, dst, inc); \
				IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
				IBLEND_##mode(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, inc, cc); \
			} \
			card++; \
		} \
	} \
	cc = a1 + a2 + r1 + r2 + g1 + g2 + b1 + b2; \
} \
public static void ipixel_span_draw_proc_##fmt##_1(void *bits, \
	int offset, int w, uint *card, byte *cover, \
	const iColorIndex *_ipixel_src_index) \
{ \
	uint cc, r1, g1, b1, a1, r2, g2, b2, a2, inc; \
	byte *dst = (byte*)bits; \
	init; \
	if (cover == null) { \
		for (inc = offset; w > 0; inc++, w--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			if (a1 == 255) { \
				cc = IRGBA_TO_PIXEL(fmt, r1, g1, b1, 255); \
				_ipixel_store(bpp, dst, inc, cc); \
			} \
			else if (a1 > 0) { \
				cc = _ipixel_fetch(bpp, dst, inc); \
				IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
				IBLEND_SRCOVER(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, inc, cc); \
			} \
			card++; \
		} \
	}	else { \
		for (inc = offset; w > 0; inc++, w--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			cc = *cover++; \
			r2 = a1 + cc; \
			if (r2 == 510) { \
				cc = IRGBA_TO_PIXEL(fmt, r1, g1, b1, 255); \
				_ipixel_store(bpp, dst, inc, cc); \
			} \
			else if (r2 > 0 && cc > 0) { \
				a1 = _imul_y_div_255(a1, cc); \
				cc = _ipixel_fetch(bpp, dst, inc); \
				IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
				r1 = _imul_y_div_255(r1, cc); \
				g1 = _imul_y_div_255(g1, cc); \
				b1 = _imul_y_div_255(b1, cc); \
				IBLEND_SRCOVER(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, inc, cc); \
			} \
			card++; \
		} \
	} \
	cc = a1 + a2 + r1 + r2 + g1 + g2 + b1 + b2; \
} \
public static void ipixel_span_draw_proc_##fmt##_2(void *bits, \
	int offset, int w, uint *card, byte *cover, \
	const iColorIndex *_ipixel_src_index) \
{ \
	uint cc, r1, g1, b1, a1, r2, g2, b2, a2, inc; \
	byte *dst = (byte*)bits; \
	init; \
	if (cover == null) { \
		for (inc = offset; w > 0; inc++, w--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			if (a1 > 0) { \
				cc = _ipixel_fetch(bpp, dst, inc); \
				IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
				IBLEND_ADDITIVE(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, inc, cc); \
			} \
			card++; \
		} \
	}	else { \
		for (inc = offset; w > 0; inc++, w--) { \
			_ipixel_load_card(card, r1, g1, b1, a1); \
			cc = *cover++; \
			if (r1 > 0 && cc > 0) { \
				a1 = _imul_y_div_255(a1, cc); \
				cc = _ipixel_fetch(bpp, dst, inc); \
				IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
				IBLEND_ADDITIVE(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, inc, cc); \
			} \
			card++; \
		} \
	} \
}

/* span blending for 4/1 bits without palette */
#define IPIXEL_SPAN_DRAW_PROC_BITS(fmt, bpp, nbytes, mode) \
		IPIXEL_SPAN_DRAW_PROC_X(fmt, bpp, nbytes, mode, {}) 

/* span blending for 8/4/1 bits with palette */
#define IPIXEL_SPAN_DRAW_PROC_PAL(fmt, bpp, nbytes, mode) \
		IPIXEL_SPAN_DRAW_PROC_X(fmt, bpp, nbytes, mode, \
			const iColorIndex *_ipixel_dst_index = _ipixel_src_index)

/* span blending main */
#define IPIXEL_SPAN_DRAW_MAIN(type, fmt, bpp, nbytes, mode) \
	IPIXEL_SPAN_DRAW_PROC_##type(fmt, bpp, nbytes, mode) 

/* span blending procedures declare */
IPIXEL_SPAN_DRAW_MAIN(N, A8R8G8B8, 32, 4, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(N, A8B8G8R8, 32, 4, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(N, R8G8B8A8, 32, 4, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(N, B8G8R8A8, 32, 4, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(N, X8R8G8B8, 32, 4, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, X8B8G8R8, 32, 4, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, R8G8B8X8, 32, 4, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, B8G8R8X8, 32, 4, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, P8R8G8B8, 32, 4, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(N, R8G8B8, 24, 3, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, B8G8R8, 24, 3, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, R5G6B5, 16, 2, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, B5G6R5, 16, 2, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, X1R5G5B5, 16, 2, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, X1B5G5R5, 16, 2, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, R5G5B5X1, 16, 2, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, B5G5R5X1, 16, 2, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, A1R5G5B5, 16, 2, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(N, A1B5G5R5, 16, 2, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(N, R5G5B5A1, 16, 2, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(N, B5G5R5A1, 16, 2, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(N, X4R4G4B4, 16, 2, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, X4B4G4R4, 16, 2, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, R4G4B4X4, 16, 2, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, B4G4R4X4, 16, 2, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, A4R4G4B4, 16, 2, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(N, A4B4G4R4, 16, 2, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(N, R4G4B4A4, 16, 2, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(N, B4G4R4A4, 16, 2, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(PAL, C8, 8, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, G8, 8, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, A8, 8, 1, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(1, R3G3B2, 8, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(1, B2G3R3, 8, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(1, X2R2G2B2, 8, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(1, X2B2G2R2, 8, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(1, R2G2B2X2, 8, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(1, B2G2R2X2, 8, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(1, A2R2G2B2, 8, 1, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(1, A2B2G2R2, 8, 1, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(1, R2G2B2A2, 8, 1, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(1, B2G2R2A2, 8, 1, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(PAL, X4C4, 8, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, X4G4, 8, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, X4A4, 8, 1, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(PAL, C4X4, 8, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, G4X4, 8, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(N, A4X4, 8, 1, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(PAL, C4, 4, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(BITS, G4, 4, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(BITS, A4, 4, 1, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(BITS, R1G2B1, 4, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(BITS, B1G2R1, 4, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(BITS, A1R1G1B1, 4, 1, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(BITS, A1B1G1R1, 4, 1, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(BITS, R1G1B1A1, 4, 1, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(BITS, B1G1R1A1, 4, 1, NORMAL_FAST)
IPIXEL_SPAN_DRAW_MAIN(BITS, X1R1G1B1, 4, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(BITS, X1B1G1R1, 4, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(BITS, R1G1B1X1, 4, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(BITS, B1G1R1X1, 4, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(PAL, C1, 1, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(BITS, G1, 1, 1, STATIC)
IPIXEL_SPAN_DRAW_MAIN(BITS, A1, 1, 1, NORMAL_FAST)






#define ITABLE_ITEM(fmt) { \
ipixel_span_draw_proc_##fmt##_0, ipixel_span_draw_proc_##fmt##_1, \
	ipixel_span_draw_proc_##fmt##_2, ipixel_span_draw_proc_##fmt##_0, \
	ipixel_span_draw_proc_##fmt##_1, ipixel_span_draw_proc_##fmt##_2 }

/**********************************************************************
 * MACRO: HLINE ROUTINE
 **********************************************************************/
/* hline filling: 8/16/24/32 bits without palette */
#define IPIXEL_HLINE_DRAW_PROC_N(fmt, bpp, nbytes, mode) \
public static void ipixel_hline_draw_proc_##fmt##_0(void *bits, \
	int offset, int w, uint color, byte *cover, \
	const iColorIndex *idx) \
{ \
	byte *dst = ((byte*)bits) + offset * nbytes; \
	uint r1, g1, b1, a1, r2, g2, b2, a2, cc, cx, cz; \
	IRGBA_FROM_A8R8G8B8(color, r1, g1, b1, a1); \
	if (a1 == 0) return; \
	cz = IRGBA_TO_PIXEL(fmt, r1, g1, b1, a1); \
	if (cover == null) { \
		if (a1 == 255) { \
			_ipixel_fill(bpp, dst, 0, w, cz); \
		} \
		else if (a1 > 0) { \
			for (; w > 0; dst += nbytes, w--) { \
				cc = _ipixel_fetch(bpp, dst, 0); \
				IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
				IBLEND_##mode(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
		} \
	}	else { \
		if (a1 == 255) { \
			for (; w > 0; dst += nbytes, w--) { \
				a1 = *cover++; \
				if (a1 == 255) { \
					_ipixel_store(bpp, dst, 0, cz); \
				} \
				else if (a1 > 0) { \
					cc = _ipixel_fetch(bpp, dst, 0); \
					IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
					IBLEND_##mode(r1, g1, b1, a1, r2, g2, b2, a2); \
					cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
					_ipixel_store(bpp, dst, 0, cc); \
				} \
			} \
		}	\
		else if (a1 > 0) { \
			a1 = _ipixel_norm(a1); \
			for (; w > 0; dst += nbytes, w--) { \
				cx = *cover++; \
				if (cx > 0) { \
					cx = (cx * a1) >> 8; \
					cc = _ipixel_fetch(bpp, dst, 0); \
					IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
					IBLEND_##mode(r1, g1, b1, cx, r2, g2, b2, a2); \
					cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
					_ipixel_store(bpp, dst, 0, cc); \
				} \
			} \
		} \
	} \
	cc = a1 + a2 + r1 + r2 + g1 + g2 + b1 + b2; \
} \
public static void ipixel_hline_draw_proc_##fmt##_1(void *bits, \
	int offset, int w, uint color, byte *cover, \
	const iColorIndex *idx) \
{ \
	byte *dst = ((byte*)bits) + offset * nbytes; \
	uint r1, g1, b1, a1, r2, g2, b2, a2, cc, cx, cz; \
	uint r3, g3, b3; \
	IRGBA_FROM_A8R8G8B8(color, r1, g1, b1, a1); \
	if (a1 == 0) return; \
	cz = IRGBA_TO_PIXEL(fmt, r1, g1, b1, a1); \
	if (cover == null) { \
		if (a1 == 255) { \
			_ipixel_fill(bpp, dst, 0, w, cz); \
		} \
		else if (a1 > 0) { \
			for (; w > 0; dst += nbytes, w--) { \
				cc = _ipixel_fetch(bpp, dst, 0); \
				IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
				IBLEND_SRCOVER(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, 0, cc); \
			} \
		} \
	}	else { \
		if (a1 == 255) { \
			for (; w > 0; dst += nbytes, w--) { \
				a1 = *cover++; \
				if (a1 == 255) { \
					_ipixel_store(bpp, dst, 0, cz); \
				} \
				else if (a1 > 0) { \
					cc = _ipixel_fetch(bpp, dst, 0); \
					IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
					r3 = _imul_y_div_255(r1, a1); \
					g3 = _imul_y_div_255(g1, a1); \
					b3 = _imul_y_div_255(b1, a1); \
					IBLEND_SRCOVER(r3, g3, b3, a1, r2, g2, b2, a2); \
					cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
					_ipixel_store(bpp, dst, 0, cc); \
				} \
			} \
		}	\
		else if (a1 > 0) { \
			a1 = _ipixel_norm(a1); \
			for (; w > 0; dst += nbytes, w--) { \
				cx = *cover++; \
				if (cx > 0) { \
					r3 = _imul_y_div_255(r1, cx); \
					g3 = _imul_y_div_255(g1, cx); \
					b3 = _imul_y_div_255(b1, cx); \
					cx = (cx * a1) >> 8; \
					cc = _ipixel_fetch(bpp, dst, 0); \
					IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
					IBLEND_SRCOVER(r3, g3, b3, cx, r2, g2, b2, a2); \
					cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
					_ipixel_store(bpp, dst, 0, cc); \
				} \
			} \
		} \
	} \
	cc = a1 + a2 + r1 + r2 + g1 + g2 + b1 + b2; \
} \
public static void ipixel_hline_draw_proc_##fmt##_2(void *bits, \
	int offset, int w, uint color, byte *cover, \
	const iColorIndex *idx) \
{ \
	byte *dst = ((byte*)bits) + offset * nbytes; \
	uint r1, g1, b1, a1, r2, g2, b2, a2, cc, cx; \
	IRGBA_FROM_A8R8G8B8(color, r1, g1, b1, a1); \
	if (a1 == 0) return; \
	if (cover == null) { \
		r2 = g2 = b2 = a2 = 0; \
		IBLEND_ADDITIVE(r1, g1, b1, a1, r2, g2, a2, b2); \
		r1 = r2; g1 = g2; b1 = b2; a1 = a2; \
		for (; w > 0; dst += nbytes, w--) { \
			cc = _ipixel_fetch(bpp, dst, 0); \
			IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
			r2 += r1, g2 += g1, b2 += b1, a2 += a1; \
			r2 = ICLIP_256(r2); \
			g2 = ICLIP_256(g2); \
			b2 = ICLIP_256(b2); \
			a2 = ICLIP_256(a2); \
			cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
			_ipixel_store(bpp, dst, 0, cc); \
		} \
	}	else { \
		if (a1 == 255) { \
			for (; w > 0; dst += nbytes, w--) { \
				a1 = *cover++; \
				if (a1 > 0) { \
					cc = _ipixel_fetch(bpp, dst, 0); \
					IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
					IBLEND_ADDITIVE(r1, g1, b1, a1, r2, g2, b2, a2); \
					cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
					_ipixel_store(bpp, dst, 0, cc); \
				} \
			} \
		}	\
		else if (a1 > 0) { \
			a1 = _ipixel_norm(a1); \
			for (; w > 0; dst += nbytes, w--) { \
				cx = *cover++; \
				if (cx > 0) { \
					cx = (cx * a1) >> 8; \
					cc = _ipixel_fetch(bpp, dst, 0); \
					IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
					IBLEND_ADDITIVE(r1, g1, b1, cx, r2, g2, b2, a2); \
					cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
					_ipixel_store(bpp, dst, 0, cc); \
				} \
			} \
		} \
	} \
}

/* hline filling: 8/4/1 bits with or without palette */
#define IPIXEL_HLINE_DRAW_PROC_X(fmt, bpp, nbytes, mode, init) \
public static void ipixel_hline_draw_proc_##fmt##_0(void *bits, \
	int offset, int w, uint col, byte *cover, \
	const iColorIndex *_ipixel_src_index) \
{ \
	byte *dst = ((byte*)bits); \
	uint r1, g1, b1, a1, r2, g2, b2, a2, cc, cx, cz; \
	init; \
	IRGBA_FROM_A8R8G8B8(col, r1, g1, b1, a1); \
	if (a1 == 0) return; \
	cz = IRGBA_TO_PIXEL(fmt, r1, g1, b1, a1); \
	if (cover == null) { \
		if (a1 == 255) { \
			_ipixel_fill(bpp, dst, offset, w, cz); \
		} \
		else if (a1 > 0) { \
			for (; w > 0; offset++, w--) { \
				cc = _ipixel_fetch(bpp, dst, offset); \
				IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
				IBLEND_##mode(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, offset, cc); \
			} \
		} \
	}	else { \
		if (a1 == 255) { \
			for (; w > 0; offset++, w--) { \
				a1 = *cover++; \
				if (a1 == 255) { \
					_ipixel_store(bpp, dst, offset, cz); \
				} \
				else if (a1 > 0) { \
					cc = _ipixel_fetch(bpp, dst, offset); \
					IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
					IBLEND_##mode(r1, g1, b1, a1, r2, g2, b2, a2); \
					cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
					_ipixel_store(bpp, dst, offset, cc); \
				} \
			} \
		}	\
		else if (a1 > 0) { \
			a1 = _ipixel_norm(a1); \
			for (; w > 0; offset++, w--) { \
				cx = (*cover++ * a1) >> 8; \
				if (cx == 255) { \
					cc = IRGBA_TO_PIXEL(fmt, r1, g1, b1, 255); \
					_ipixel_store(bpp, dst, offset, cc); \
				} \
				else if (cx > 0) { \
					cc = _ipixel_fetch(bpp, dst, offset); \
					IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
					IBLEND_##mode(r1, g1, b1, cx, r2, g2, b2, a2); \
					cc = IRGBA_TO_PIXEL(fmt, r1, g1, b1, 255); \
					_ipixel_store(bpp, dst, offset, cc); \
				} \
			} \
		} \
	} \
	cc = a1 + a2 + r1 + r2 + g1 + g2 + b1 + b2; \
} \
public static void ipixel_hline_draw_proc_##fmt##_1(void *bits, \
	int offset, int w, uint col, byte *cover, \
	const iColorIndex *_ipixel_src_index) \
{ \
	byte *dst = ((byte*)bits); \
	uint r1, g1, b1, a1, r2, g2, b2, a2, cc, cx, cz; \
	uint r3, g3, b3; \
	init; \
	IRGBA_FROM_A8R8G8B8(col, r1, g1, b1, a1); \
	if (a1 == 0) return; \
	cz = IRGBA_TO_PIXEL(fmt, r1, g1, b1, a1); \
	if (cover == null) { \
		if (a1 == 255) { \
			_ipixel_fill(bpp, dst, offset, w, cz); \
		} \
		else if (a1 > 0) { \
			for (; w > 0; offset++, w--) { \
				cc = _ipixel_fetch(bpp, dst, offset); \
				IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
				IBLEND_SRCOVER(r1, g1, b1, a1, r2, g2, b2, a2); \
				cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
				_ipixel_store(bpp, dst, offset, cc); \
			} \
		} \
	}	else { \
		if (a1 == 255) { \
			for (; w > 0; offset++, w--) { \
				a1 = *cover++; \
				if (a1 == 255) { \
					_ipixel_store(bpp, dst, offset, cz); \
				} \
				else if (a1 > 0) { \
					cc = _ipixel_fetch(bpp, dst, offset); \
					IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
					r3 = _imul_y_div_255(r1, a1); \
					g3 = _imul_y_div_255(g1, a1); \
					b3 = _imul_y_div_255(b1, a1); \
					IBLEND_SRCOVER(r3, g3, b3, a1, r2, g2, b2, a2); \
					cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
					_ipixel_store(bpp, dst, offset, cc); \
				} \
			} \
		}	\
		else if (a1 > 0) { \
			a1 = _ipixel_norm(a1); \
			for (; w > 0; offset++, w--) { \
				cx = *cover++; \
				if (cx + a1 == 255 + 256) { \
					cc = IRGBA_TO_PIXEL(fmt, r1, g1, b1, 255); \
					_ipixel_store(bpp, dst, offset, cc); \
				} \
				else if (cx > 0) { \
					r3 = _imul_y_div_255(r1, cx); \
					g3 = _imul_y_div_255(g1, cx); \
					b3 = _imul_y_div_255(b1, cx); \
					cx = (cx * a1) >> 8; \
					cc = _ipixel_fetch(bpp, dst, offset); \
					IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
					IBLEND_SRCOVER(r1, g1, b1, cx, r2, g2, b2, a2); \
					cc = IRGBA_TO_PIXEL(fmt, r1, g1, b1, 255); \
					_ipixel_store(bpp, dst, offset, cc); \
				} \
			} \
		} \
	} \
	cc = a1 + a2 + r1 + r2 + g1 + g2 + b1 + b2; \
} \
public static void ipixel_hline_draw_proc_##fmt##_2(void *bits, \
	int offset, int w, uint col, byte *cover, \
	const iColorIndex *_ipixel_src_index) \
{ \
	byte *dst = ((byte*)bits); \
	uint r1, g1, b1, a1, r2, g2, b2, a2, cc, cx; \
	init; \
	IRGBA_FROM_A8R8G8B8(col, r1, g1, b1, a1); \
	if (a1 == 0) return; \
	if (cover == null) { \
		r2 = g2 = b2 = a2 = 0; \
		IBLEND_ADDITIVE(r1, g1, b1, a1, r2, g2, a2, b2); \
		r1 = r2; g1 = g2; b1 = b2; a1 = a2; \
		for (; w > 0; dst += nbytes, w--) { \
			cc = _ipixel_fetch(bpp, dst, offset); \
			IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
			r2 += r1, g2 += g1, b2 += b1, a2 += a1; \
			r2 = ICLIP_256(r2); \
			g2 = ICLIP_256(g2); \
			b2 = ICLIP_256(b2); \
			a2 = ICLIP_256(a2); \
			cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
			_ipixel_store(bpp, dst, offset, cc); \
		} \
	}	else { \
		if (a1 == 255) { \
			for (; w > 0; offset++, w--) { \
				a1 = *cover++; \
				if (a1 > 0) { \
					cc = _ipixel_fetch(bpp, dst, offset); \
					IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
					IBLEND_ADDITIVE(r1, g1, b1, a1, r2, g2, b2, a2); \
					cc = IRGBA_TO_PIXEL(fmt, r2, g2, b2, a2); \
					_ipixel_store(bpp, dst, offset, cc); \
				} \
			} \
		}	\
		else if (a1 > 0) { \
			a1 = _ipixel_norm(a1); \
			for (; w > 0; offset++, w--) { \
				cx = *cover++; \
				if (cx > 0) { \
					cx = (cx * a1) >> 8; \
					cc = _ipixel_fetch(bpp, dst, offset); \
					IRGBA_FROM_PIXEL(fmt, cc, r2, g2, b2, a2); \
					IBLEND_ADDITIVE(r1, g1, b1, cx, r2, g2, b2, a2); \
					cc = IRGBA_TO_PIXEL(fmt, r1, g1, b1, 255); \
					_ipixel_store(bpp, dst, offset, cc); \
				} \
			} \
		} \
	} \
	cc = a1 + a2 + r1 + r2 + g1 + g2 + b1 + b2; \
}

/* hline filling: 8/4/1 bits without palette */
#define IPIXEL_HLINE_DRAW_PROC_BITS(fmt, bpp, nbytes, mode) \
		IPIXEL_HLINE_DRAW_PROC_X(fmt, bpp, nbytes, mode, {})

/* hline filling: 8/4/1 bits with palette */
#define IPIXEL_HLINE_DRAW_PROC_PAL(fmt, bpp, nbytes, mode) \
		IPIXEL_HLINE_DRAW_PROC_X(fmt, bpp, nbytes, mode,  \
				const iColorIndex *_ipixel_dst_index = _ipixel_src_index) 

/* hline filling: main macro */
#define IPIXEL_HLINE_DRAW_MAIN(type, fmt, bpp, nbytes, mode) \
	IPIXEL_HLINE_DRAW_PROC_##type(fmt, bpp, nbytes, mode) 


#if 1
IPIXEL_HLINE_DRAW_MAIN(N, A8R8G8B8, 32, 4, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(N, A8B8G8R8, 32, 4, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(N, R8G8B8A8, 32, 4, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(N, B8G8R8A8, 32, 4, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(N, X8R8G8B8, 32, 4, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, X8B8G8R8, 32, 4, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, R8G8B8X8, 32, 4, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, B8G8R8X8, 32, 4, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, P8R8G8B8, 32, 4, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(N, R8G8B8, 24, 3, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, B8G8R8, 24, 3, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, R5G6B5, 16, 2, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, B5G6R5, 16, 2, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, X1R5G5B5, 16, 2, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, X1B5G5R5, 16, 2, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, R5G5B5X1, 16, 2, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, B5G5R5X1, 16, 2, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, A1R5G5B5, 16, 2, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(N, A1B5G5R5, 16, 2, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(N, R5G5B5A1, 16, 2, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(N, B5G5R5A1, 16, 2, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(N, X4R4G4B4, 16, 2, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, X4B4G4R4, 16, 2, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, R4G4B4X4, 16, 2, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, B4G4R4X4, 16, 2, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, A4R4G4B4, 16, 2, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(N, A4B4G4R4, 16, 2, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(N, R4G4B4A4, 16, 2, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(N, B4G4R4A4, 16, 2, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(PAL, C8, 8, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, G8, 8, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, A8, 8, 1, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(N, R3G3B2, 8, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, B2G3R3, 8, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, X2R2G2B2, 8, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, X2B2G2R2, 8, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, R2G2B2X2, 8, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, B2G2R2X2, 8, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, A2R2G2B2, 8, 1, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(N, A2B2G2R2, 8, 1, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(N, R2G2B2A2, 8, 1, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(N, B2G2R2A2, 8, 1, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(PAL, X4C4, 8, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, X4G4, 8, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, X4A4, 8, 1, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(PAL, C4X4, 8, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, G4X4, 8, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(N, A4X4, 8, 1, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(PAL, C4, 4, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(BITS, G4, 4, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(BITS, A4, 4, 1, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(BITS, R1G2B1, 4, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(BITS, B1G2R1, 4, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(BITS, A1R1G1B1, 4, 1, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(BITS, A1B1G1R1, 4, 1, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(BITS, R1G1B1A1, 4, 1, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(BITS, B1G1R1A1, 4, 1, NORMAL_FAST)
IPIXEL_HLINE_DRAW_MAIN(BITS, X1R1G1B1, 4, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(BITS, X1B1G1R1, 4, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(BITS, R1G1B1X1, 4, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(BITS, B1G1R1X1, 4, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(PAL, C1, 1, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(BITS, G1, 1, 1, STATIC)
IPIXEL_HLINE_DRAW_MAIN(BITS, A1, 1, 1, NORMAL_FAST)
#endif

#undef IPIXEL_HLINE_DRAW_MAIN
#undef IPIXEL_HLINE_DRAW_PROC_PAL
#undef IPIXEL_HLINE_DRAW_PROC_BITS
#undef IPIXEL_HLINE_DRAW_PROC_X
#undef IPIXEL_HLINE_DRAW_PROC_N





#define ITABLE_ITEM(fmt) { \
	ipixel_hline_draw_proc_##fmt##_0, ipixel_hline_draw_proc_##fmt##_1, \
	ipixel_hline_draw_proc_##fmt##_2, ipixel_hline_draw_proc_##fmt##_0, \
	ipixel_hline_draw_proc_##fmt##_1, ipixel_hline_draw_proc_##fmt##_2 }




	#define IPIXEL_BLEND_LOOP(work) do { \
			for (k = 0; k < h; sline += spitch, dline += dpitch, k++) { \
				const uint *src = ((const uint*)sline); \
				uint *dst = ((uint*)dline); \
				fetch(src, sx, w, buffer, sindex); \
				work; \
			} \
		}	while (0)





/**********************************************************************
 * MACRO: BLITING ROUTINE
 **********************************************************************/
/* normal blit in 32/16/8 bits */
#define IPIXEL_BLIT_PROC_N(nbits, nbytes, INTTYPE) \
public static int ipixel_blit_proc_##nbits(void *dbits, int dpitch, int dx,  \
	const void *sbits, int spitch, int sx, int w, int h, uint mask, \
	int flip) \
{ \
	int y, x; \
	if (flip & IPIXEL_FLIP_VFLIP) { \
		sbits = ( byte*)sbits + spitch * (h - 1); \
		spitch = -spitch; \
	} \
	if ((flip & IPIXEL_FLIP_HFLIP) == 0) { \
		int size = w * nbytes; \
		for (y = 0; y < h; y++) { \
			ipixel_memcpy((INTTYPE*)dbits + dx, (const INTTYPE*)sbits + sx, size); \
			dbits = (byte*)dbits + dpitch; \
			sbits = ( byte*)sbits + spitch; \
		} \
	}	else { \
		for (y = 0; y < h; y++) { \
			const INTTYPE *src = (const INTTYPE*)sbits + sx + w - 1; \
			INTTYPE *dst = (INTTYPE*)dbits + dx; \
			for (x = w; x > 0; x--) *dst++ = *src--; \
			dbits = (byte*)dbits + dpitch; \
			sbits = ( byte*)sbits + spitch; \
		} \
	} \
	return 0; \
} 

/* normal blit in 24/4/1 bits */
#define IPIXEL_BLIT_PROC_BITS(nbits) \
public static int ipixel_blit_proc_##nbits(void *dbits, int dpitch, int dx, \
	const void *sbits, int spitch, int sx, int w, int h, uint mask, \
	int flip) \
{ \
	int y, x1, x2, sx0, sxd, endx; \
	if (flip & IPIXEL_FLIP_VFLIP) { \
		sbits = ( byte*)sbits + spitch * (h - 1); \
		spitch = -spitch; \
	} \
	if (flip & IPIXEL_FLIP_HFLIP) { \
		sx0 = sx + w - 1; \
		sxd = -1; \
	}	else { \
		sx0 = sx; \
		sxd = 1; \
	} \
	endx = dx + w; \
	for (y = 0; y < h; y++) { \
		uint cc; \
		for (x1 = dx, x2 = sx0; x1 < endx; x1++, x2 += sxd) { \
			cc = _ipixel_fetch(nbits, sbits, x2); \
			_ipixel_store(nbits, dbits, x1, cc); \
		} \
		dbits = (byte*)dbits + dpitch; \
		sbits = ( byte*)sbits + spitch; \
	} \
	return 0; \
}


/* mask blit in 32/16/8 bits */
#define IPIXEL_BLIT_MASK_PROC_N(nbits, nbytes, INTTYPE) \
public static int ipixel_blit_mask_proc_##nbits(void *dbits, int dpitch, \
	int dx, void *sbits, int spitch, int sx, int w, int h, \
	uint mask, int flip) \
{ \
	INTTYPE cmask = (INTTYPE)mask; \
	int y; \
	if (flip & IPIXEL_FLIP_VFLIP) { \
		sbits = ( byte*)sbits + spitch * (h - 1); \
		spitch = -spitch; \
	} \
	if ((flip & IPIXEL_FLIP_HFLIP) == 0) { \
		for (y = 0; y < h; y++) { \
			const INTTYPE *src = (const INTTYPE*)sbits + sx; \
			INTTYPE *dst = (INTTYPE*)dbits + dx; \
			INTTYPE *dstend = dst + w; \
			for (; dst < dstend; src++, dst++) { \
				if (src[0] != cmask) dst[0] = src[0]; \
			} \
			dbits = (byte*)dbits + dpitch; \
			sbits = ( byte*)sbits + spitch; \
		} \
	}	else { \
		for (y = 0; y < h; y++) { \
			const INTTYPE *src = (const INTTYPE*)sbits + sx + w - 1; \
			INTTYPE *dst = (INTTYPE*)dbits + dx; \
			INTTYPE *dstend = dst + w; \
			for (; dst < dstend; src--, dst++) { \
				if (src[0] != cmask) dst[0] = src[0]; \
			} \
			dbits = (byte*)dbits + dpitch; \
			sbits = ( byte*)sbits + spitch; \
		} \
	} \
	return 0; \
}

/* mask blit in 24/4/1 bits */
#define IPIXEL_BLIT_MASK_PROC_BITS(nbits) \
public static int ipixel_blit_mask_proc_##nbits(void *dbits, int dpitch, \
	int dx, void *sbits, int spitch, int sx, int w, int h, \
	uint mask, int flip) \
{ \
	int y, x1, x2, sx0, sxd, endx; \
	if (flip & IPIXEL_FLIP_VFLIP) { \
		sbits = ( byte*)sbits + spitch * (h - 1); \
		spitch = -spitch; \
	} \
	if (flip & IPIXEL_FLIP_HFLIP) { \
		sx0 = sx + w - 1; \
		sxd = -1; \
	}	else { \
		sx0 = sx; \
		sxd = 1; \
	} \
	endx = dx + w; \
	for (y = 0; y < h; y++) { \
		uint cc; \
		for (x1 = dx, x2 = sx0; x1 < endx; x1++, x2 += sxd) { \
			cc = _ipixel_fetch(nbits, sbits, x2); \
			if (cc != mask) _ipixel_store(nbits, dbits, x1, cc); \
		} \
		dbits = (byte*)dbits + dpitch; \
		sbits = ( byte*)sbits + spitch; \
	} \
	return 0; \
}


/* normal bliter */
IPIXEL_BLIT_PROC_N(32, 4, uint);
IPIXEL_BLIT_PROC_N(16, 2, ushort);
IPIXEL_BLIT_PROC_N(8, 1, byte);

IPIXEL_BLIT_PROC_BITS(24);
IPIXEL_BLIT_PROC_BITS(4);
IPIXEL_BLIT_PROC_BITS(1);

/* mask bliter */
IPIXEL_BLIT_MASK_PROC_N(32, 4, uint);
IPIXEL_BLIT_MASK_PROC_N(16, 2, ushort);
IPIXEL_BLIT_MASK_PROC_N(8, 1, byte);

IPIXEL_BLIT_MASK_PROC_BITS(24);
IPIXEL_BLIT_MASK_PROC_BITS(4);
IPIXEL_BLIT_MASK_PROC_BITS(1);


#undef IPIXEL_BLIT_PROC_N
#undef IPIXEL_BLIT_PROC_BITS
#undef IPIXEL_BLIT_MASK_PROC_N
#undef IPIXEL_BLIT_MASK_PROC_BITS



#define ITABLE_ITEM(bpp) { \
	ipixel_blit_proc_##bpp, ipixel_blit_proc_##bpp, \
	ipixel_blit_mask_proc_##bpp, ipixel_blit_mask_proc_##bpp }




#define IPIXEL_COMPOSITE_NORMAL(name, opname) \
public static void ipixel_comp_##name(uint *dst, uint *src, int w)\
{ \
	uint sr, sg, sb, sa, dr, dg, db, da; \
	for (; w > 0; dst++, src++, w--) { \
		_ipixel_load_card(src, sr, sg, sb, sa); \
		_ipixel_load_card(dst, dr, dg, db, da); \
		sr = _ipixel_mullut[sa][sr]; \
		sg = _ipixel_mullut[sa][sg]; \
		sb = _ipixel_mullut[sa][sb]; \
		dr = _ipixel_mullut[da][dr]; \
		dg = _ipixel_mullut[da][dg]; \
		db = _ipixel_mullut[da][db]; \
		IBLEND_OP_##opname(sr, sg, sb, sa, dr, dg, db, da); \
		dr = _ipixel_divlut[da][dr]; \
		dg = _ipixel_divlut[da][dg]; \
		db = _ipixel_divlut[da][db]; \
		dst[0] = IRGBA_TO_A8R8G8B8(dr, dg, db, da); \
	} \
}

#define IPIXEL_COMPOSITE_PREMUL(name, opname) \
public static void ipixel_comp_##name(uint *dst, uint *src, int w)\
{ \
	uint sr, sg, sb, sa, dr, dg, db, da; \
	for (; w > 0; dst++, src++, w--) { \
		_ipixel_load_card(src, sr, sg, sb, sa); \
		_ipixel_load_card(dst, dr, dg, db, da); \
		IBLEND_OP_##opname(sr, sg, sb, sa, dr, dg, db, da); \
		dst[0] = IRGBA_TO_A8R8G8B8(dr, dg, db, da); \
	} \
}

IPIXEL_COMPOSITE_NORMAL(xor, XOR);
IPIXEL_COMPOSITE_NORMAL(plus, PLUS);
IPIXEL_COMPOSITE_NORMAL(src_atop, SRC_ATOP);
IPIXEL_COMPOSITE_NORMAL(src_in, SRC_IN);
IPIXEL_COMPOSITE_NORMAL(src_out, SRC_OUT);
IPIXEL_COMPOSITE_NORMAL(src_over, SRC_OVER);
IPIXEL_COMPOSITE_NORMAL(dst_atop, DST_ATOP);
IPIXEL_COMPOSITE_NORMAL(dst_in, DST_IN);
IPIXEL_COMPOSITE_NORMAL(dst_out, DST_OUT);
IPIXEL_COMPOSITE_NORMAL(dst_over, DST_OVER);

IPIXEL_COMPOSITE_PREMUL(pre_xor, XOR);
IPIXEL_COMPOSITE_PREMUL(pre_plus, PLUS);
IPIXEL_COMPOSITE_PREMUL(pre_src_atop, SRC_ATOP);
IPIXEL_COMPOSITE_PREMUL(pre_src_in, SRC_IN);
IPIXEL_COMPOSITE_PREMUL(pre_src_out, SRC_OUT);
IPIXEL_COMPOSITE_PREMUL(pre_src_over, SRC_OVER);
IPIXEL_COMPOSITE_PREMUL(pre_dst_atop, DST_ATOP);
IPIXEL_COMPOSITE_PREMUL(pre_dst_in, DST_IN);
IPIXEL_COMPOSITE_PREMUL(pre_dst_out, DST_OUT);
IPIXEL_COMPOSITE_PREMUL(pre_dst_over, DST_OVER);

#undef IPIXEL_COMPOSITE_NORMAL
#undef IPIXEL_COMPOSITE_PREMUL

