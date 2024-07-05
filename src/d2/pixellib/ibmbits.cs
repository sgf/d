using ComputeSharp;

namespace d2;

/* pixel format: 32 bits */
public int
 IPIX_FMT_A8R8G8B8 = 0,
 IPIX_FMT_A8B8G8R8 = 1,
 IPIX_FMT_R8G8B8A8 = 2,
 IPIX_FMT_B8G8R8A8 = 3,
 IPIX_FMT_X8R8G8B8 = 4,
 IPIX_FMT_X8B8G8R8 = 5,
 IPIX_FMT_R8G8B8X8 = 6,
 IPIX_FMT_B8G8R8X8 = 7,
 IPIX_FMT_P8R8G8B8 = 8,

/* pixel format: 24 bits */
 IPIX_FMT_R8G8B8 = 9,
 IPIX_FMT_B8G8R8 = 10;

/* pixel format: 16 bits */
public int
 IPIX_FMT_R5G6B5 = 11,
 IPIX_FMT_B5G6R5 = 12,
 IPIX_FMT_X1R5G5B5 = 13,
 IPIX_FMT_X1B5G5R5 = 14,
 IPIX_FMT_R5G5B5X1 = 15,
 IPIX_FMT_B5G5R5X1 = 16,
 IPIX_FMT_A1R5G5B5 = 17,
 IPIX_FMT_A1B5G5R5 = 18,
 IPIX_FMT_R5G5B5A1 = 19,
 IPIX_FMT_B5G5R5A1 = 20,
 IPIX_FMT_X4R4G4B4 = 21,
 IPIX_FMT_X4B4G4R4 = 22,
 IPIX_FMT_R4G4B4X4 = 23,
 IPIX_FMT_B4G4R4X4 = 24,
 IPIX_FMT_A4R4G4B4 = 25,
 IPIX_FMT_A4B4G4R4 = 26,
 IPIX_FMT_R4G4B4A4 = 27,
 IPIX_FMT_B4G4R4A4 = 28;

/* pixel format: 8 bits */
public int
 IPIX_FMT_C8 = 29,
 IPIX_FMT_G8 = 30,
 IPIX_FMT_A8 = 31,
 IPIX_FMT_R3G3B2 = 32,
 IPIX_FMT_B2G3R3 = 33,
 IPIX_FMT_X2R2G2B2 = 34,
 IPIX_FMT_X2B2G2R2 = 35,
 IPIX_FMT_R2G2B2X2 = 36,
 IPIX_FMT_B2G2R2X2 = 37,
 IPIX_FMT_A2R2G2B2 = 38,
 IPIX_FMT_A2B2G2R2 = 39,
 IPIX_FMT_R2G2B2A2 = 40,
 IPIX_FMT_B2G2R2A2 = 41,
 IPIX_FMT_X4C4 = 42,
 IPIX_FMT_X4G4 = 43,
 IPIX_FMT_X4A4 = 44,
 IPIX_FMT_C4X4 = 45,
 IPIX_FMT_G4X4 = 46,
 IPIX_FMT_A4X4 = 47;

/* pixel format: 4 bits */
public int
 IPIX_FMT_C4 = 48,
 IPIX_FMT_G4 = 49,
 IPIX_FMT_A4 = 50,
 IPIX_FMT_R1G2B1 = 51,
 IPIX_FMT_B1G2R1 = 52,
 IPIX_FMT_A1R1G1B1 = 53,
 IPIX_FMT_A1B1G1R1 = 54,
 IPIX_FMT_R1G1B1A1 = 55,
 IPIX_FMT_B1G1R1A1 = 56,
 IPIX_FMT_X1R1G1B1 = 57,
 IPIX_FMT_X1B1G1R1 = 58,
 IPIX_FMT_R1G1B1X1 = 59,
 IPIX_FMT_B1G1R1X1 = 60;

/* pixel format: 1 bit */

public int
 IPIX_FMT_C1 = 61,
 IPIX_FMT_G1 = 62,
 IPIX_FMT_A1 = 63,

IPIX_FMT_COUNT = 64,

 /* arbitrary packed format */
 IPIX_FMT_PACKED = 65,

 /* pixel format types */
 IPIX_FMT_TYPE_ARGB = 0,
 IPIX_FMT_TYPE_RGB = 1,
 IPIX_FMT_TYPE_INDEX = 2,
 IPIX_FMT_TYPE_ALPHA = 3,
 IPIX_FMT_TYPE_GRAY = 4,
 IPIX_FMT_TYPE_PREMUL = 5;




/**********************************************************************
 * Global Structures
 **********************************************************************/

/* pixel format descriptor */
public struct IPIXELFMT
{
    int format;              /* pixel format code       */
    int bpp;                 /* bits per pixel          */
    int alpha;               /* 0: no alpha, 1: alpha   */
    int type;                /* IPIX_FMT_TYPE_...       */
    int pixelbyte;           /* nbytes per pixel        */
    byte* name;        /* name string             */
    uint rmask;           /* red component mask      */
    uint gmask;           /* green component mask    */
    uint bmask;           /* blue component mask     */
    uint amask;           /* alpha component mask    */
    int rshift;              /* red component shift     */
    int gshift;              /* green component shift   */
    int bshift;              /* blue component shift    */
    int ashift;              /* alpha component shift   */
    int rloss;               /* red component loss      */
    int gloss;               /* green component loss    */
    int bloss;               /* blue component loss     */
    int aloss;               /* alpha component loss    */
};

/* RGB descript */
public struct IRGB
{
    byte r, g, b;
    byte reserved;
};

/* color index: used to lookup colors */
public struct iColorIndex
{
    int color;                   /* how many colors can be used */
    uint rgba[256];           /* C8 . A8R8G8B8 lookup       */
    byte ent[32768];    /* X1R5G5B5 . C8 lookup       */
};


/* scanline fetch: fetching from different pixel formats to A8R8GB8 */
delegate void iFetchProc(void* bits, int x, int w, uint* buffer, ref iColorIndex idx);

/* scanline store: storing from A8R8G8B8 to other formats */
delegate void iStoreProc(void* bits, uint* buffer, int x, int w, ref iColorIndex idx);

/* pixel fetch: fetching color from diferent pixel format to A8R8G8B8 */
delegate uint iFetchPixelProc(void* bits, int offset, ref iColorIndex idx);



/* pixel format declaration */
 public  IPIXELFMT ipixelfmt[IPIX_FMT_COUNT];

/* color component bit scale */
 uint _ipixel_scale_1[2];
 uint _ipixel_scale_2[4];
 uint _ipixel_scale_3[8];
 uint _ipixel_scale_4[16];
 uint _ipixel_scale_5[32];
 uint _ipixel_scale_6[64];

/* default index */
 iColorIndex * _ipixel_src_index;
 iColorIndex * _ipixel_dst_index;

/* default palette */
 IRGB _ipaletted[256];

/* pixel alpha lookup table: initialized by ipixel_lut_init() */
 byte ipixel_blend_lut[2048 * 2];

/* multiplication & division table: initialized by ipixel_lut_init() */
 byte _ipixel_mullut[256][256];  /* [x][y] = x * y / 255 */
 byte _ipixel_divlut[256][256];  /* [x][y] = y * 255 / x */

/* color component scale for 0-8 bits */
 byte _ipixel_bit_scale[9][256];


/* vim: set ts=4 sw=4 tw=0 noet :*/



/**********************************************************************
 * GLOBAL VARIABLES
 **********************************************************************/

/* pixel format declare */
 public struct IPIXELFMT ipixelfmt[64] =
{
    { IPIX_FMT_A8R8G8B8, 32, 1, 0, 4, "IPIX_FMT_A8R8G8B8",
      0xff0000, 0xff00, 0xff, 0xff000000, 16, 8, 0, 24, 0, 0, 0, 0 },
    { IPIX_FMT_A8B8G8R8, 32, 1, 0, 4, "IPIX_FMT_A8B8G8R8",
      0xff, 0xff00, 0xff0000, 0xff000000, 0, 8, 16, 24, 0, 0, 0, 0 },
    { IPIX_FMT_R8G8B8A8, 32, 1, 0, 4, "IPIX_FMT_R8G8B8A8",
      0xff000000, 0xff0000, 0xff00, 0xff, 24, 16, 8, 0, 0, 0, 0, 0 },
    { IPIX_FMT_B8G8R8A8, 32, 1, 0, 4, "IPIX_FMT_B8G8R8A8",
      0xff00, 0xff0000, 0xff000000, 0xff, 8, 16, 24, 0, 0, 0, 0, 0 },
    { IPIX_FMT_X8R8G8B8, 32, 0, 1, 4, "IPIX_FMT_X8R8G8B8",
      0xff0000, 0xff00, 0xff, 0x0, 16, 8, 0, 0, 0, 0, 0, 8 },
    { IPIX_FMT_X8B8G8R8, 32, 0, 1, 4, "IPIX_FMT_X8B8G8R8",
      0xff, 0xff00, 0xff0000, 0x0, 0, 8, 16, 0, 0, 0, 0, 8 },
    { IPIX_FMT_R8G8B8X8, 32, 0, 1, 4, "IPIX_FMT_R8G8B8X8",
      0xff000000, 0xff0000, 0xff00, 0x0, 24, 16, 8, 0, 0, 0, 0, 8 },
    { IPIX_FMT_B8G8R8X8, 32, 0, 1, 4, "IPIX_FMT_B8G8R8X8",
      0xff00, 0xff0000, 0xff000000, 0x0, 8, 16, 24, 0, 0, 0, 0, 8 },
    { IPIX_FMT_P8R8G8B8, 32, 1, 5, 4, "IPIX_FMT_P8R8G8B8",
      0xff0000, 0xff00, 0xff, 0x0, 16, 8, 0, 0, 0, 0, 0, 8 },
    { IPIX_FMT_R8G8B8, 24, 0, 1, 3, "IPIX_FMT_R8G8B8",
      0xff0000, 0xff00, 0xff, 0x0, 16, 8, 0, 0, 0, 0, 0, 8 },
    { IPIX_FMT_B8G8R8, 24, 0, 1, 3, "IPIX_FMT_B8G8R8",
      0xff, 0xff00, 0xff0000, 0x0, 0, 8, 16, 0, 0, 0, 0, 8 },
    { IPIX_FMT_R5G6B5, 16, 0, 1, 2, "IPIX_FMT_R5G6B5",
      0xf800, 0x7e0, 0x1f, 0x0, 11, 5, 0, 0, 3, 2, 3, 8 },
    { IPIX_FMT_B5G6R5, 16, 0, 1, 2, "IPIX_FMT_B5G6R5",
      0x1f, 0x7e0, 0xf800, 0x0, 0, 5, 11, 0, 3, 2, 3, 8 },
    { IPIX_FMT_X1R5G5B5, 16, 0, 1, 2, "IPIX_FMT_X1R5G5B5",
      0x7c00, 0x3e0, 0x1f, 0x0, 10, 5, 0, 0, 3, 3, 3, 8 },
    { IPIX_FMT_X1B5G5R5, 16, 0, 1, 2, "IPIX_FMT_X1B5G5R5",
      0x1f, 0x3e0, 0x7c00, 0x0, 0, 5, 10, 0, 3, 3, 3, 8 },
    { IPIX_FMT_R5G5B5X1, 16, 0, 1, 2, "IPIX_FMT_R5G5B5X1",
      0xf800, 0x7c0, 0x3e, 0x0, 11, 6, 1, 0, 3, 3, 3, 8 },
    { IPIX_FMT_B5G5R5X1, 16, 0, 1, 2, "IPIX_FMT_B5G5R5X1",
      0x3e, 0x7c0, 0xf800, 0x0, 1, 6, 11, 0, 3, 3, 3, 8 },
    { IPIX_FMT_A1R5G5B5, 16, 1, 0, 2, "IPIX_FMT_A1R5G5B5",
      0x7c00, 0x3e0, 0x1f, 0x8000, 10, 5, 0, 15, 3, 3, 3, 7 },
    { IPIX_FMT_A1B5G5R5, 16, 1, 0, 2, "IPIX_FMT_A1B5G5R5",
      0x1f, 0x3e0, 0x7c00, 0x8000, 0, 5, 10, 15, 3, 3, 3, 7 },
    { IPIX_FMT_R5G5B5A1, 16, 1, 0, 2, "IPIX_FMT_R5G5B5A1",
      0xf800, 0x7c0, 0x3e, 0x1, 11, 6, 1, 0, 3, 3, 3, 7 },
    { IPIX_FMT_B5G5R5A1, 16, 1, 0, 2, "IPIX_FMT_B5G5R5A1",
      0x3e, 0x7c0, 0xf800, 0x1, 1, 6, 11, 0, 3, 3, 3, 7 },
    { IPIX_FMT_X4R4G4B4, 16, 0, 1, 2, "IPIX_FMT_X4R4G4B4",
      0xf00, 0xf0, 0xf, 0x0, 8, 4, 0, 0, 4, 4, 4, 8 },
    { IPIX_FMT_X4B4G4R4, 16, 0, 1, 2, "IPIX_FMT_X4B4G4R4",
      0xf, 0xf0, 0xf00, 0x0, 0, 4, 8, 0, 4, 4, 4, 8 },
    { IPIX_FMT_R4G4B4X4, 16, 0, 1, 2, "IPIX_FMT_R4G4B4X4",
      0xf000, 0xf00, 0xf0, 0x0, 12, 8, 4, 0, 4, 4, 4, 8 },
    { IPIX_FMT_B4G4R4X4, 16, 0, 1, 2, "IPIX_FMT_B4G4R4X4",
      0xf0, 0xf00, 0xf000, 0x0, 4, 8, 12, 0, 4, 4, 4, 8 },
    { IPIX_FMT_A4R4G4B4, 16, 1, 0, 2, "IPIX_FMT_A4R4G4B4",
      0xf00, 0xf0, 0xf, 0xf000, 8, 4, 0, 12, 4, 4, 4, 4 },
    { IPIX_FMT_A4B4G4R4, 16, 1, 0, 2, "IPIX_FMT_A4B4G4R4",
      0xf, 0xf0, 0xf00, 0xf000, 0, 4, 8, 12, 4, 4, 4, 4 },
    { IPIX_FMT_R4G4B4A4, 16, 1, 0, 2, "IPIX_FMT_R4G4B4A4",
      0xf000, 0xf00, 0xf0, 0xf, 12, 8, 4, 0, 4, 4, 4, 4 },
    { IPIX_FMT_B4G4R4A4, 16, 1, 0, 2, "IPIX_FMT_B4G4R4A4",
      0xf0, 0xf00, 0xf000, 0xf, 4, 8, 12, 0, 4, 4, 4, 4 },
    { IPIX_FMT_C8, 8, 0, 2, 1, "IPIX_FMT_C8",
      0x0, 0x0, 0x0, 0x0, 0, 0, 0, 0, 8, 8, 8, 8 },
    { IPIX_FMT_G8, 8, 0, 3, 1, "IPIX_FMT_G8",
      0x0, 0xff, 0x0, 0x0, 0, 0, 0, 0, 8, 0, 8, 8 },
    { IPIX_FMT_A8, 8, 1, 4, 1, "IPIX_FMT_A8",
      0x0, 0x0, 0x0, 0xff, 0, 0, 0, 0, 8, 8, 8, 0 },
    { IPIX_FMT_R3G3B2, 8, 0, 1, 1, "IPIX_FMT_R3G3B2",
      0xe0, 0x1c, 0x3, 0x0, 5, 2, 0, 0, 5, 5, 6, 8 },
    { IPIX_FMT_B2G3R3, 8, 0, 1, 1, "IPIX_FMT_B2G3R3",
      0x7, 0x38, 0xc0, 0x0, 0, 3, 6, 0, 5, 5, 6, 8 },
    { IPIX_FMT_X2R2G2B2, 8, 0, 1, 1, "IPIX_FMT_X2R2G2B2",
      0x30, 0xc, 0x3, 0x0, 4, 2, 0, 0, 6, 6, 6, 8 },
    { IPIX_FMT_X2B2G2R2, 8, 0, 1, 1, "IPIX_FMT_X2B2G2R2",
      0x3, 0xc, 0x30, 0x0, 0, 2, 4, 0, 6, 6, 6, 8 },
    { IPIX_FMT_R2G2B2X2, 8, 0, 1, 1, "IPIX_FMT_R2G2B2X2",
      0xc0, 0x30, 0xc, 0x0, 6, 4, 2, 0, 6, 6, 6, 8 },
    { IPIX_FMT_B2G2R2X2, 8, 0, 1, 1, "IPIX_FMT_B2G2R2X2",
      0xc, 0x30, 0xc0, 0x0, 2, 4, 6, 0, 6, 6, 6, 8 },
    { IPIX_FMT_A2R2G2B2, 8, 1, 0, 1, "IPIX_FMT_A2R2G2B2",
      0x30, 0xc, 0x3, 0xc0, 4, 2, 0, 6, 6, 6, 6, 6 },
    { IPIX_FMT_A2B2G2R2, 8, 1, 0, 1, "IPIX_FMT_A2B2G2R2",
      0x3, 0xc, 0x30, 0xc0, 0, 2, 4, 6, 6, 6, 6, 6 },
    { IPIX_FMT_R2G2B2A2, 8, 1, 0, 1, "IPIX_FMT_R2G2B2A2",
      0xc0, 0x30, 0xc, 0x3, 6, 4, 2, 0, 6, 6, 6, 6 },
    { IPIX_FMT_B2G2R2A2, 8, 1, 0, 1, "IPIX_FMT_B2G2R2A2",
      0xc, 0x30, 0xc0, 0x3, 2, 4, 6, 0, 6, 6, 6, 6 },
    { IPIX_FMT_X4C4, 8, 0, 2, 1, "IPIX_FMT_X4C4",
      0x0, 0x0, 0x0, 0x0, 0, 0, 0, 0, 8, 8, 8, 8 },
    { IPIX_FMT_X4G4, 8, 0, 3, 1, "IPIX_FMT_X4G4",
      0x0, 0xf, 0x0, 0x0, 0, 0, 0, 0, 8, 4, 8, 8 },
    { IPIX_FMT_X4A4, 8, 1, 4, 1, "IPIX_FMT_X4A4",
      0x0, 0x0, 0x0, 0xf, 0, 0, 0, 0, 8, 8, 8, 4 },
    { IPIX_FMT_C4X4, 8, 0, 2, 1, "IPIX_FMT_C4X4",
      0x0, 0x0, 0x0, 0x0, 0, 0, 0, 0, 8, 8, 8, 8 },
    { IPIX_FMT_G4X4, 8, 0, 3, 1, "IPIX_FMT_G4X4",
      0x0, 0xf0, 0x0, 0x0, 0, 4, 0, 0, 8, 4, 8, 8 },
    { IPIX_FMT_A4X4, 8, 1, 4, 1, "IPIX_FMT_A4X4",
      0x0, 0x0, 0x0, 0xf0, 0, 0, 0, 4, 8, 8, 8, 4 },
    { IPIX_FMT_C4, 4, 0, 2, 0, "IPIX_FMT_C4",
      0x0, 0x0, 0x0, 0x0, 0, 0, 0, 0, 8, 8, 8, 8 },
    { IPIX_FMT_G4, 4, 0, 3, 0, "IPIX_FMT_G4",
      0x0, 0xf, 0x0, 0x0, 0, 0, 0, 0, 8, 4, 8, 8 },
    { IPIX_FMT_A4, 4, 1, 4, 0, "IPIX_FMT_A4",
      0x0, 0x0, 0x0, 0xf, 0, 0, 0, 0, 8, 8, 8, 4 },
    { IPIX_FMT_R1G2B1, 4, 0, 1, 0, "IPIX_FMT_R1G2B1",
      0x8, 0x6, 0x1, 0x0, 3, 1, 0, 0, 7, 6, 7, 8 },
    { IPIX_FMT_B1G2R1, 4, 0, 1, 0, "IPIX_FMT_B1G2R1",
      0x1, 0x6, 0x8, 0x0, 0, 1, 3, 0, 7, 6, 7, 8 },
    { IPIX_FMT_A1R1G1B1, 4, 1, 0, 0, "IPIX_FMT_A1R1G1B1",
      0x4, 0x2, 0x1, 0x8, 2, 1, 0, 3, 7, 7, 7, 7 },
    { IPIX_FMT_A1B1G1R1, 4, 1, 0, 0, "IPIX_FMT_A1B1G1R1",
      0x1, 0x2, 0x4, 0x8, 0, 1, 2, 3, 7, 7, 7, 7 },
    { IPIX_FMT_R1G1B1A1, 4, 1, 0, 0, "IPIX_FMT_R1G1B1A1",
      0x8, 0x4, 0x2, 0x1, 3, 2, 1, 0, 7, 7, 7, 7 },
    { IPIX_FMT_B1G1R1A1, 4, 1, 0, 0, "IPIX_FMT_B1G1R1A1",
      0x2, 0x4, 0x8, 0x1, 1, 2, 3, 0, 7, 7, 7, 7 },
    { IPIX_FMT_X1R1G1B1, 4, 0, 1, 0, "IPIX_FMT_X1R1G1B1",
      0x4, 0x2, 0x1, 0x0, 2, 1, 0, 0, 7, 7, 7, 8 },
    { IPIX_FMT_X1B1G1R1, 4, 0, 1, 0, "IPIX_FMT_X1B1G1R1",
      0x1, 0x2, 0x4, 0x0, 0, 1, 2, 0, 7, 7, 7, 8 },
    { IPIX_FMT_R1G1B1X1, 4, 0, 1, 0, "IPIX_FMT_R1G1B1X1",
      0x8, 0x4, 0x2, 0x0, 3, 2, 1, 0, 7, 7, 7, 8 },
    { IPIX_FMT_B1G1R1X1, 4, 0, 1, 0, "IPIX_FMT_B1G1R1X1",
      0x2, 0x4, 0x8, 0x0, 1, 2, 3, 0, 7, 7, 7, 8 },
    { IPIX_FMT_C1, 1, 0, 2, 0, "IPIX_FMT_C1",
      0x0, 0x0, 0x0, 0x0, 0, 0, 0, 0, 8, 8, 8, 8 },
    { IPIX_FMT_G1, 1, 0, 3, 0, "IPIX_FMT_G1",
      0x0, 0x1, 0x0, 0x0, 0, 0, 0, 0, 8, 7, 8, 8 },
    { IPIX_FMT_A1, 1, 1, 4, 0, "IPIX_FMT_A1",
      0x0, 0x0, 0x0, 0x1, 0, 0, 0, 0, 8, 8, 8, 7 },
};


/* lookup table for scaling 1 bit colors up to 8 bits */
const uint _ipixel_scale_1[2] = { 0, 255 };

/* lookup table for scaling 2 bit colors up to 8 bits */
const uint _ipixel_scale_2[4] = { 0, 85, 170, 255 };

/* lookup table for scaling 3 bit colors up to 8 bits */
const uint _ipixel_scale_3[8] = { 0, 36, 72, 109, 145, 182, 218, 255 };

/* lookup table for scaling 4 bit colors up to 8 bits */
const uint _ipixel_scale_4[16] = 
{
    0, 16, 32, 49, 65, 82, 98, 115, 
    139, 156, 172, 189, 205, 222, 238, 255
};

/* lookup table for scaling 5 bit colors up to 8 bits */
const uint _ipixel_scale_5[32] =
{
   0,   8,   16,  24,  32,  41,  49,  57,
   65,  74,  82,  90,  98,  106, 115, 123,
   131, 139, 148, 156, 164, 172, 180, 189,
   197, 205, 213, 222, 230, 238, 246, 255
};

/* lookup table for scaling 6 bit colors up to 8 bits */
const uint _ipixel_scale_6[64] =
{
   0,   4,   8,   12,  16,  20,  24,  28,
   32,  36,  40,  44,  48,  52,  56,  60,
   64,  68,  72,  76,  80,  85,  89,  93,
   97,  101, 105, 109, 113, 117, 121, 125,
   129, 133, 137, 141, 145, 149, 153, 157,
   161, 165, 170, 174, 178, 182, 186, 190,
   194, 198, 202, 206, 210, 214, 218, 222,
   226, 230, 234, 238, 242, 246, 250, 255
};

/* default color index */
public static iColorIndex _ipixel_static_index[2];

iColorIndex *_ipixel_src_index = &_ipixel_static_index[0];
iColorIndex *_ipixel_dst_index = &_ipixel_static_index[1];

/* default palette */
IRGB _ipaletted[256];

byte ipixel_blend_lut[2048 * 2];

byte _ipixel_mullut[256][256];   /* [x][y] = x * y / 255 */
byte _ipixel_divlut[256][256];   /* [x][y] = y * 255 / x */

byte _ipixel_bit_scale[9][256];   /* component scale for 0-8 bits */

/* memcpy hook */
void *(*ipixel_memcpy_hook)(void *dst, void* src, size_t size) = null;

#if defined(__BORLANDC__) && !defined(__MSDOS__)
	#pragma warn -8004  
	#pragma warn -8057
#endif


/**********************************************************************
 * 256 PALETTE INTERFACE
 **********************************************************************/

/* find best fit color */
int ipixel_palette_fit(const IRGB *pal, int r, int g, int b, int palsize)
{ 
	public static uint diff_lookup[512 * 3] = { 0 };
	int lowest = 0x7FFFFFFF, bestfit = 0;
	int coldiff, i, k;
	IRGB *rgb;

	/* calculate color difference lookup table:
	 * COLOR DIFF TABLE
	 * table1: diff_lookup[i | i = 256.511, n=0.(+255)] = (n * 30) ^ 2
	 * table2: diff_lookup[i | i = 256.1,   n=0.(-255)] = (n * 30) ^ 2
	 * result: f(n) = (n * 30) ^ 2 = diff_lookup[256 + n]
	 */
	if (diff_lookup[0] == 0) {
		for (i = 0; i < 256; i++) {
			k = i * i;
			diff_lookup[ 256 + i] = diff_lookup[ 256 - i] = k * 30 * 30;
			diff_lookup[ 768 + i] = diff_lookup[ 768 - i] = k * 59 * 59;
			diff_lookup[1280 + i] = diff_lookup[1280 - i] = k * 11 * 11;
		}
		diff_lookup[0] = 1;
	}

	/* range correction */
	r = r & 255;
	g = g & 255;
	b = b & 255;

	/*
	 * vector:   c1 = [r1, g1, b1], c2 = [r2, g2, b2]
	 * distance: dc = length(c1 - c2)
	 * coldiff:  dc^2 = (r1 - r2)^2 + (g1 - g2)^2 + (b1 - b2)^2
	 */
	for (i = palsize, rgb = (IRGB*)pal; i > 0; rgb++, i--) {
		coldiff  = diff_lookup[ 768 + rgb.g - g];
		if (coldiff >= lowest) continue;

		coldiff += diff_lookup[ 256 + rgb.r - r];
		if (coldiff >= lowest) continue;

		coldiff += diff_lookup[1280 + rgb.b - b];
		if (coldiff >= lowest) continue;

		bestfit = (int)(rgb - (IRGB*)pal); /* faster than `bestfit = i;' */
		if (coldiff == 0) return bestfit;
		lowest = coldiff;
	}

	return bestfit;
} 

/* convert palette to index */
int ipalette_to_index(iColorIndex *index, IRGB *pal, int palsize)
{
	uint r, g, b, a;
	int i;
	index.color = palsize;
	for (i = 0; i < palsize; i++) {
		r = pal[i].r;
		g = pal[i].g;
		b = pal[i].b;
		a = 255;
		index.rgba[i] = IRGBA_TO_PIXEL(A8R8G8B8, r, g, b, a);
	}
	for (i = 0; i < 0x8000; i++) {
		IRGBA_FROM_PIXEL(X1R5G5B5, i, r, g, b, a);
		index.ent[i] = ipixel_palette_fit(pal, r, g, b, palsize);
	}
	return 0;
}


/* get raw color */
uint ipixel_assemble(int pixfmt, int r, int g, int b, int a)
{
	uint c;
	IRGBA_ASSEMBLE(pixfmt, c, r, g, b, a);
	return c;
}

/* get r, g, b, a from color */
void ipixel_desemble(int pixfmt, uint c, int *r, int *g, 
	int *b, int *a)
{
	int R, G, B, A;
	IRGBA_DISEMBLE(pixfmt, c, R, G, B, A);
	*r = R;
	*g = G;
	*b = B;
	*a = A;
}


/* stand aint memcpy */
void *ipixel_memcpy(void *dst, void* src, size_t size)
{
	if (ipixel_memcpy_hook) {
		return ipixel_memcpy_hook(dst, src, size);
	}

#ifndef IPIXEL_NO_MEMCPY
	return memcpy(dst, src, size);
#else
	byte *dd = (byte*)dst;
	byte*ss = (byte*)src;
	for (; size >= 8; size -= 8) {
		*(uint*)(dd + 0) = *(const uint*)(ss + 0);
		*(uint*)(dd + 4) = *(const uint*)(ss + 4);
		dd += 8;
		ss += 8;
	}
	for (; size > 0; size--) *dd++ = *ss++;
	return dst;
#endif
}


/**********************************************************************
 * Fast 1/2 byte . 4 byte
 **********************************************************************/
void ipixel_lut_2_to_4(int sfmt, int dfmt, uint *table)
{
	uint c;

	if (ipixelfmt[sfmt].pixelbyte != 2 || ipixelfmt[dfmt].pixelbyte != 4) {
		assert(0);
		return;
	}

	for (c = 0; c < 256; c++) {
		uint r1, g1, b1, a1;
		uint r2, g2, b2, a2;
		uint c1, c2;
#if IPIXEL_BIG_ENDIAN
		c1 = c << 8;
		c2 = c;
#else
		c1 = c;
		c2 = c << 8;
#endif
		IRGBA_DISEMBLE(sfmt, c1, r1, g1, b1, a1);
		IRGBA_DISEMBLE(sfmt, c2, r2, g2, b2, a2);
		IRGBA_ASSEMBLE(dfmt, c1, r1, g1, b1, a1);
		IRGBA_ASSEMBLE(dfmt, c2, r2, g2, b2, a2);
		table[c +   0] = c1;
		table[c + 256] = c2;
	}
}

void ipixel_lut_1_to_4(int sfmt, int dfmt, uint *table)
{
	uint c;

	if (ipixelfmt[sfmt].pixelbyte != 1 || ipixelfmt[dfmt].pixelbyte != 4) {
		assert(0);
		return;
	}

	for (c = 0; c < 256; c++) {
		uint c1, r1, g1, b1, a1;
		c1 = c;
		IRGBA_DISEMBLE(sfmt, c1, r1, g1, b1, a1);
		IRGBA_ASSEMBLE(dfmt, c1, r1, g1, b1, a1);
		table[c] = c1;
	}
}

void ipixel_lut_conv_2(uint *dst, ushort *src, int w, 
	const uint *lut)
{
	const byte *input = ( byte*)src;
	uint c1, c2;
	for (; w > 0; w--) {
		c1 = lut[*input++ +   0];
		c2 = lut[*input++ + 256];
		*dst++ = c1 | c2;
	}
}

void ipixel_lut_conv_1(uint *dst, ushort *src, int w, 
	const uint *lut)
{
	const byte *input = ( byte*)src;
	for (; w > 0; w--) {
		*dst++ = lut[*input++];
	}
}


/**********************************************************************
 * DEFAULT FETCH PROC
 **********************************************************************/
#define IFETCH_PROC(fmt, bpp) \
public static void _ifetch_proc_##fmt(const void *bits, int x, \
    int w, uint *buffer, iColorIndex *_ipixel_src_index) \
{ \
    uint c1, r1, g1, b1, a1; \
	for (; w > 0; w--) { \
		c1 = _ipixel_fetch(bpp, bits, x); \
		x++; \
		IRGBA_FROM_PIXEL(fmt, c1, r1, g1, b1, a1); \
		c1 = IRGBA_TO_PIXEL(A8R8G8B8, r1, g1, b1, a1); \
		*buffer++ = c1; \
	} \
    _ipixel_src_index = _ipixel_src_index; \
}


/**********************************************************************
 * DEFAULT STORE PROC
 **********************************************************************/
#define ISTORE_PROC(fmt, bpp) \
public static void _istore_proc_##fmt(void *bits, uint *values, \
    int x, int w, iColorIndex *_ipixel_dst_index) \
{ \
    uint c1, r1, g1, b1, a1; \
	for (; w > 0; w--) { \
		c1 = *values++; \
		IRGBA_FROM_PIXEL(A8R8G8B8, c1, r1, g1, b1, a1); \
		c1 = IRGBA_TO_PIXEL(fmt, r1, g1, b1, a1); \
		_ipixel_store(bpp, bits, x, c1); \
		x++; \
	} \
    _ipixel_dst_index = _ipixel_dst_index; \
	c1 = a1 + r1 + g1 + b1; \
}


/**********************************************************************
 * DEFAULT PIXEL FETCH PROC
 **********************************************************************/
#define IFETCH_PIXEL(fmt, bpp) \
public static uint _ifetch_pixel_##fmt(const void *bits, \
    int offset, iColorIndex *_ipixel_src_index) \
{ \
    uint c, r, g, b, a; \
    c = _ipixel_fetch(bpp, bits, offset); \
    IRGBA_FROM_PIXEL(fmt, c, r, g, b, a); \
    return IRGBA_TO_PIXEL(A8R8G8B8, r, g, b, a); \
}



/**********************************************************************
 * FETCHING PROCEDURES
 **********************************************************************/
public static void _ifetch_proc_A8R8G8B8(const void *bits, int x, 
	int w, uint *buffer, iColorIndex *idx)
{
	ipixel_memcpy(buffer, (const uint*)bits + x, w * sizeof(uint));
}

public static void _ifetch_proc_A8B8G8R8(const void *bits, int x,
	int w, uint *buffer, iColorIndex *idx)
{
	const uint *pixel = (const uint*)bits + x;
	for (; w > 0; w--) {
		*buffer++ = ((*pixel & 0xff00ff00) |
			((*pixel & 0xff0000) >> 16) | ((*pixel & 0xff) << 16));
		pixel++;
	}
}

public static void _ifetch_proc_R8G8B8A8(const void *bits, int x, 
	int w, uint *buffer, iColorIndex *idx)
{
	const uint *pixel = (const uint*)bits + x;
	for (; w > 0; w--) {
		*buffer++ = ((*pixel & 0xff) << 24) |
			((*pixel & 0xffffff00) >> 8);
		pixel++;
	}
}

IFETCH_PROC(B8G8R8A8, 32)

public static void _ifetch_proc_X8R8G8B8(const void *bits, int x,
	int w, uint *buffer, iColorIndex *idx)
{
	const uint *pixel = (const uint*)bits + x;
	for (; w > 0; w--) {
		*buffer++ = *pixel++ | 0xff000000;
	}
}

IFETCH_PROC(X8B8G8R8, 32)

public static void _ifetch_proc_R8G8B8X8(const void *bits, int x, 
	int w, uint *buffer, iColorIndex *idx)
{
	const uint *pixel = (const uint*)bits + x;
	for (; w > 0; w--) {
		*buffer++ = (0xff000000) |
			((*pixel & 0xffffff00) >> 8);
		pixel++;
	}
}

IFETCH_PROC(B8G8R8X8, 32)
IFETCH_PROC(P8R8G8B8, 32)

public static void _ifetch_proc_R8G8B8(const void *bits, int x,
	int w, uint *buffer, iColorIndex *idx)
{
	const byte *pixel = ( byte*)bits + x * 3;
	for (; w > 0; w--) {
		*buffer++ = IFETCH24(pixel) | 0xff000000;
		pixel += 3;
	}
}

/**********************************************************************
 * STORING PROCEDURES
 **********************************************************************/
public static void _istore_proc_A8R8G8B8(void *bits, 
	const uint *values, int x, int w, iColorIndex *idx)
{
	ipixel_memcpy(((uint*)bits) + x, values, w * sizeof(uint));
}

public static void _istore_proc_A8B8G8R8(void *bits,
	const uint *values, int x, int w, iColorIndex *idx)
{
	uint *pixel = (uint*)bits + x;
	for (; w > 0; w--) {
		*pixel++ = (values[0] & 0xff00ff00) |
			((values[0] & 0xff0000) >> 16) | ((values[0] & 0xff) << 16);
		values++;
	}
}

public static void _istore_proc_R8G8B8A8(void *bits,
	const uint *values, int x, int w, iColorIndex *idx)
{
	uint *pixel = (uint*)bits + x;
	for (; w > 0; w--) {
		*pixel++ = ((values[0] & 0xffffff) << 8) |
			((values[0] & 0xff000000) >> 24);
		values++;
	}
}

public static void _istore_proc_X8R8G8B8(void *bits, 
	const uint *values, int x, int w, iColorIndex *idx)
{
	uint *pixel = (uint*)bits + x;
	for (; w > 0; w--) {
		*pixel++ = values[0] & 0xffffff;
		values++;
	}
}


public static void _istore_proc_R8G8B8X8(void *bits,
	const uint *values, int x, int w, iColorIndex *idx)
{
	uint *pixel = (uint*)bits + x;
	for (; w > 0; w--) {
		*pixel++ = ((values[0] & 0xffffff) << 8);
		values++;
	}
}


public static void _istore_proc_R8G8B8(void *bits,
	const uint *values, int x, int w, iColorIndex *idx)
{
	byte *pixel = (byte*)bits + x * 3;
	uint c;
	int i;
	for (i = w; i > 0; i--) {
		c = *values++;
		ISTORE24(pixel, c);
		pixel += 3;
	}
}

public static void _istore_proc_B8G8R8(void *bits,
	const uint *values, int x, int w, iColorIndex *idx)
{
	byte *pixel = (byte*)bits + x * 3;
	uint c;
	int i;
	for (i = w; i > 0; i--) {
		c = *values++;
		c = (c & 0x00ff00) | ((c & 0xff0000) >> 16) | ((c & 0xff) << 16);
		ISTORE24(pixel, c);
		pixel += 3;
	}
}

public static void _istore_proc_R5G6B5(void *bits,
	const uint *values, int x, int w, iColorIndex *idx)
{
	ushort *pixel = (ushort*)bits + x;
	uint c1, r1, g1, b1;
	for (; w > 0; w--) {
		c1 = *values++;
		ISPLIT_RGB(c1, r1, g1, b1);
		*pixel++ = (ushort) (((ushort)(r1 & 0xf8) << 8) |
							  ((ushort)(g1 & 0xfc) << 3) |
							  ((ushort)(b1 & 0xf8) >> 3));
	}
}

public static void _istore_proc_B5G6R5(void *bits,
	const uint *values, int x, int w, iColorIndex *idx)
{
	ushort *pixel = (ushort*)bits + x;
	uint c1, r1, g1, b1;
	for (; w > 0; w--) {
		c1 = *values++;
		ISPLIT_RGB(c1, r1, g1, b1);
		*pixel++ = (ushort) (((ushort)(b1 & 0xf8) << 8) |
							  ((ushort)(g1 & 0xfc) << 3) |
							  ((ushort)(r1 & 0xf8) >> 3));
	}
}

public static void _istore_proc_X1R5G5B5(void *bits, 
	const uint *values, int x, int w, iColorIndex *idx)
{
	ushort *pixel = (ushort*)bits + x;
	uint c1, r1, g1, b1;
	for (; w > 0; w--) {
		c1 = *values++;
		ISPLIT_RGB(c1, r1, g1, b1);
		*pixel++ = (ushort) (((ushort)(r1 & 0xf8) << 7) |
							  ((ushort)(g1 & 0xf8) << 2) |
							  ((ushort)(b1 & 0xf8) >> 3));
	}
}

public static void _istore_proc_R4G4B4A4(void *bits,
	const uint *values, int x, int w, iColorIndex *idx)
{
	ushort *pixel = (ushort*)bits + x;
	uint c, a, r, g, b;
	int i;
	for (i = w; i > 0; i--) {
		c = *values++;
		ISPLIT_ARGB(c, a, r, g, b);
		*pixel++ = ((ushort)( 
			((ushort)((r) & 0xf0) << 8) | 
			((ushort)((g) & 0xf0) << 4) | 
			((ushort)((b) & 0xf0) >> 0) | 
			((ushort)((a) & 0xf0) >> 4)));
	}
}

/**********************************************************************
 * PIXEL FETCHING PROCEDURES
 **********************************************************************/

public static uint _ifetch_pixel_A8R8G8B8(const void *bits, 
    int offset, iColorIndex *_ipixel_src_index) 
{ 
    return _ipixel_fetch(32, bits, offset); 
}

public static uint _ifetch_pixel_A8B8G8R8(const void *bits,
	int offset, iColorIndex *idx)
{
	uint pixel = ((const uint*)(bits))[offset];
	return ((pixel & 0xff00ff00) | ((pixel & 0xff0000) >> 16) | 
		((pixel & 0xff) << 16));
}

public static uint _ifetch_pixel_R8G8B8A8(const void *bits,
	int offset, iColorIndex *idx)
{
	uint pixel = ((const uint*)(bits))[offset];
	return ((pixel & 0xff) << 24) | ((pixel & 0xffffff00) >> 8);
}

public static uint _ifetch_pixel_B8G8R8A8(const void *bits,
	int offset, iColorIndex *idx)
{
	uint pixel = ((const uint*)(bits))[offset];
	return		((pixel & 0x000000ff) << 24) |
				((pixel & 0x0000ff00) <<  8) |
				((pixel & 0x00ff0000) >>  8) |
				((pixel & 0xff000000) >> 24);
}


public static uint _ifetch_pixel_X8R8G8B8(const void *bits,
	int offset, iColorIndex *idx)
{
	return ((const uint*)(bits))[offset] | 0xff000000;
}

public static uint _ifetch_pixel_X8B8G8R8(const void *bits,
	int offset, iColorIndex *idx)
{
	uint pixel = ((const uint*)(bits))[offset];
	return ((pixel & 0x0000ff00) | ((pixel & 0xff0000) >> 16) | 
		((pixel & 0xff) << 16)) | 0xff000000;
}



/**********************************************************************
 * FETCHING STORING LOOK UP TABLE
 **********************************************************************/
public struct iPixelAccessProc
{
    iFetchProc fetch, fetch_builtin;
    iStoreProc store, store_builtin;
    iFetchPixelProc fetchpixel, fetchpixel_builtin;
};

/* builtin look table */
public static   iPixelAccessProc ipixel_access_proc[IPIX_FMT_COUNT] =
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
	ITABLE_ITEM(C4),
	ITABLE_ITEM(G4),
	ITABLE_ITEM(A4),
	ITABLE_ITEM(R1G2B1),
	ITABLE_ITEM(B1G2R1),
	ITABLE_ITEM(A1R1G1B1),
	ITABLE_ITEM(A1B1G1R1),
	ITABLE_ITEM(R1G1B1A1),
	ITABLE_ITEM(B1G1R1A1),
	ITABLE_ITEM(X1R1G1B1),
	ITABLE_ITEM(X1B1G1R1),
	ITABLE_ITEM(R1G1B1X1),
	ITABLE_ITEM(B1G1R1X1),
	ITABLE_ITEM(C1),
	ITABLE_ITEM(G1),
	ITABLE_ITEM(A1),
};

/* set procedure */
void ipixel_set_proc(int pixfmt, int type, void *proc)
{
	assert(pixfmt >= 0 && pixfmt < IPIX_FMT_COUNT);
	if (pixfmt < 0 || pixfmt >= IPIX_FMT_COUNT) return;
	if (type == IPIXEL_PROC_TYPE_FETCH) {
		if (proc != null) {
			ipixel_access_proc[pixfmt].fetch = (iFetchProc)proc;
		}	else {
			ipixel_access_proc[pixfmt].fetch = 
				ipixel_access_proc[pixfmt].fetch_builtin;
		}
	}
	else if (type == IPIXEL_PROC_TYPE_STORE) {
		if (proc != null) {
			ipixel_access_proc[pixfmt].store = (iStoreProc)proc;
		}	else {
			ipixel_access_proc[pixfmt].store = 
				ipixel_access_proc[pixfmt].store_builtin;
		}
	}
	else if (type == IPIXEL_PROC_TYPE_FETCHPIXEL) {
		if (proc != null) {
			ipixel_access_proc[pixfmt].fetchpixel = (iFetchPixelProc)proc;
		}	else {
			ipixel_access_proc[pixfmt].fetchpixel = 
				ipixel_access_proc[pixfmt].fetchpixel_builtin;
		}
	}
}



public struct iPixelAccessLutTable
{
	int fmt;
	iFetchProc fetch;
	iFetchPixelProc pixel;
};


public static iPixelAccessLutTable ipixel_access_lut[28] =
{
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
};

public static int ipixel_access_lut_fmt[IPIX_FMT_COUNT] = {
	-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 
	9, 10, 11, 12, 13, 14, 15, 16, 17, -1, -1, -1, 18, 19, 20, 21, 22, 23,
	24, 25, 26, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
	-1, -1, -1, -1, -1, -1, -1, -1 
};



public static int ipixel_lut_inited = 0;

void ipixel_init_lut(void)
{
	int i, j, k;

	if (ipixel_lut_inited != 0) return;

	/* init bit scaling table */
	for (i = 0; i < 9; i++) {
		byte *table = _ipixel_bit_scale[i];
		uint maxvalue = (1 << (8 - i)) - 1;
		int k = 0;
		if (maxvalue > 0) {
			for (k = 0; k <= maxvalue; k++) {
				table[k] = k * 255 / maxvalue;
			}
		}
		for (; k < 256; k++) {
			table[k] = 255;
		}
	}

	
	for (i = 0; i < 2048; i++) {
		uint da = _ipixel_scale_5[i >> 6];
		uint sa = _ipixel_scale_6[i & 63];
		uint FA = da + ((255 - da) * sa) / 255;
		uint SA = (FA != 0)? ((sa * 255) / FA) : 0;
		ipixel_blend_lut[i * 2 + 0] = (byte)SA;
		ipixel_blend_lut[i * 2 + 1] = (byte)FA;
	}

	for (i = 0; i < 256; i++) {
		for (j = 0; j < 256; j++) {
			k = i * j;
			_ipixel_mullut[i][j] = (byte)_ipixel_fast_div_255(k);
		}
	}
	for (i = 0; i < 256; i++) {
		_ipixel_divlut[0][i] = 0;
		for (j = 1; j < i; j++) {
			_ipixel_divlut[i][j] = (byte)((j * 255) / i);
		}
		for (j = i; j < 256; j++) {
			_ipixel_divlut[i][j] = 255;
		}
	}
	ipixel_lut_inited = 1;
}

/* get color fetching procedure */
iFetchProc ipixel_get_fetch(int pixfmt, int access_mode)
{
	assert(pixfmt >= 0 && pixfmt < IPIX_FMT_COUNT);
	if (pixfmt < 0 || pixfmt >= IPIX_FMT_COUNT) return null;
	if (access_mode == IPIXEL_ACCESS_MODE_NORMAL) {
		int id = ipixel_access_lut_fmt[pixfmt];
		if (ipixel_lut_inited == 0) ipixel_init_lut();
		if (id >= 0) return ipixel_access_lut[id].fetch;
		return ipixel_access_proc[pixfmt].fetch;
	}
	if (access_mode == IPIXEL_ACCESS_MODE_ACCURATE) {
		return ipixel_access_proc[pixfmt].fetch;
	}
	return ipixel_access_proc[pixfmt].fetch_builtin;
}

/* get color storing procedure */
iStoreProc ipixel_get_store(int pixfmt, int access_mode)
{
	assert(pixfmt >= 0 && pixfmt < IPIX_FMT_COUNT);
	if (pixfmt < 0 || pixfmt >= IPIX_FMT_COUNT) return null;
	if (access_mode == IPIXEL_ACCESS_MODE_NORMAL) {
		if (ipixel_lut_inited == 0) ipixel_init_lut();
		return ipixel_access_proc[pixfmt].store;
	}
	if (access_mode == IPIXEL_ACCESS_MODE_ACCURATE) {
		return ipixel_access_proc[pixfmt].store;
	}
	return ipixel_access_proc[pixfmt].store_builtin;
}

/* get color pixel fetching procedure */
iFetchPixelProc ipixel_get_fetchpixel(int pixfmt, int access_mode)
{
	assert(pixfmt >= 0 && pixfmt < IPIX_FMT_COUNT);
	if (pixfmt < 0 || pixfmt >= IPIX_FMT_COUNT) return null;
	if (access_mode == IPIXEL_ACCESS_MODE_NORMAL) {
		int id = ipixel_access_lut_fmt[pixfmt];
		if (ipixel_lut_inited == 0) ipixel_init_lut();
		if (id >= 0) return ipixel_access_lut[id].pixel;
		return ipixel_access_proc[pixfmt].fetchpixel;
	}
	if (access_mode == IPIXEL_ACCESS_MODE_ACCURATE) {
		return ipixel_access_proc[pixfmt].fetchpixel;
	}
	return ipixel_access_proc[pixfmt].fetchpixel_builtin;
}


/* returns pixel format names */
byte*ipixelfmt_name(int fmt)
{
	return ipixelfmt[fmt].name;
}



/* draw span over in A8R8G8B8 or X8R8G8B8 */
public static void ipixel_span_draw_proc_over_32(void *bits,
	int offset, int w, uint *card, byte *cover,
	const iColorIndex *_ipixel_src_index)
{
	uint *dst = ((uint*)bits) + offset;
	uint alpha, cc;
	int inc;
	if (cover == null) {
		for (inc = w; inc > 0; inc--) {
			alpha = card[0] >> 24;
			if (alpha == 255) {
				dst[0] = card[0];
			}
			else if (alpha > 0) {
				IBLEND_PARGB(dst[0], card[0]);
			}
			card++;
			dst++;
		}
	}	else {
		for (inc = w; inc > 0; inc--) {
			alpha = card[0] >> 24;
			cc = cover[0];
			if (cc + alpha == 510) {
				dst[0] = card[0];
			}
			else if (cc && alpha) {
				IBLEND_PARGB_COVER(dst[0], card[0], cc);
			}
			card++;
			dst++;
			cover++;
		}
	}
}



public struct iPixelSpanDrawProc
{
	iSpanDrawProc blend, srcover, additive;
	iSpanDrawProc blend_builtin, srcover_builtin, additive_builtin;
};


public static public struct iPixelSpanDrawProc ipixel_span_proc_list[IPIX_FMT_COUNT] =
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
	ITABLE_ITEM(C4),
	ITABLE_ITEM(G4),
	ITABLE_ITEM(A4),
	ITABLE_ITEM(R1G2B1),
	ITABLE_ITEM(B1G2R1),
	ITABLE_ITEM(A1R1G1B1),
	ITABLE_ITEM(A1B1G1R1),
	ITABLE_ITEM(R1G1B1A1),
	ITABLE_ITEM(B1G1R1A1),
	ITABLE_ITEM(X1R1G1B1),
	ITABLE_ITEM(X1B1G1R1),
	ITABLE_ITEM(R1G1B1X1),
	ITABLE_ITEM(B1G1R1X1),
	ITABLE_ITEM(C1),
	ITABLE_ITEM(G1),
	ITABLE_ITEM(A1),
};
 

public static iSpanDrawProc ipixel_span_draw_over = ipixel_span_draw_proc_over_32;

/* get a span drawing function with given pixel format */
iSpanDrawProc ipixel_get_span_proc(int fmt, int op, int builtin)
{
	assert(fmt >= 0 && fmt < IPIX_FMT_COUNT);
	if (fmt < -1 || fmt >= IPIX_FMT_COUNT) {
		abort();
		return null;
	}
	if (ipixel_lut_inited == 0) ipixel_init_lut();
	if (builtin) {
		if (fmt < 0) return ipixel_span_draw_proc_over_32;
		if (op == 0) return ipixel_span_proc_list[fmt].blend_builtin;
		else if (op == 1) return ipixel_span_proc_list[fmt].srcover_builtin;
		else return ipixel_span_proc_list[fmt].additive_builtin;
	}	else {
		if (fmt < 0) return ipixel_span_draw_over;
		if (op == 0) return ipixel_span_proc_list[fmt].blend;
		else if (op == 1) return ipixel_span_proc_list[fmt].srcover;
		else return ipixel_span_proc_list[fmt].additive;
	}
}

/* set a span drawing function */
void ipixel_set_span_proc(int fmt, int op, iSpanDrawProc proc)
{
	assert(fmt >= 0 && fmt < IPIX_FMT_COUNT);
	if (fmt < -1 || fmt >= IPIX_FMT_COUNT) {
		abort();
		return;
	}
	if (ipixel_lut_inited == 0) ipixel_init_lut();
	if (fmt < 0) {
		if (proc != null) {
			ipixel_span_draw_over = proc;
		}	else {
			ipixel_span_draw_over = ipixel_span_draw_proc_over_32;
		}
	}	
	else {
		if (op == 0) {
			if (proc != null) {
				ipixel_span_proc_list[fmt].blend = proc;
			}	else {
				ipixel_span_proc_list[fmt].blend = 
					ipixel_span_proc_list[fmt].blend_builtin;
			}
		}	
		else if (op == 1) {
			if (proc != null) {
				ipixel_span_proc_list[fmt].srcover = proc;
			}	else {
				ipixel_span_proc_list[fmt].srcover = 
					ipixel_span_proc_list[fmt].srcover_builtin;
			}
		}
		else {
			if (proc != null) {
				ipixel_span_proc_list[fmt].additive = proc;
			}	else {
				ipixel_span_proc_list[fmt].additive =
					ipixel_span_proc_list[fmt].additive_builtin;
			}
		}
	}
}



/**********************************************************************
 * CARD operations
 **********************************************************************/

/* reverse card */
void ipixel_card_reverse(uint *card, int size)
{
	uint *p1, *p2;
	uint value;
	for (p1 = card, p2 = card + size - 1; p1 < p2; p1++, p2--) {
		value = *p1;
		*p1 = *p2;
		*p2 = value;
	}
}


/* multi card */
void ipixel_card_multi_default(uint *card, int size, uint color)
{
	uint r1, g1, b1, a1, r2, g2, b2, a2, f;
	IRGBA_FROM_A8R8G8B8(color, r1, g1, b1, a1);
	if ((color & 0xffffff) == 0xffffff) f = 1;
	else f = 0;
	if (color == 0xffffffff) {
		return;
	}
	else if (color == 0) {
		memset(card, 0, sizeof(uint) * size);
	}
	else if (f) {
		byte *src = (byte*)card;
		if (a1 == 0) {
			for (; size > 0; size--) {
			#if IPIXEL_BIG_ENDIAN
				src[0] = 0;
			#else
				src[3] = 0;
			#endif
				src += sizeof(uint);
			}
			return;
		}
		a1 = _ipixel_norm(a1);
		for (; size > 0; size--) {
		#if IPIXEL_BIG_ENDIAN
			a2 = src[0];
			src[0] = (byte)((a2 * a1) >> 8);
		#else
			a2 = src[3];
			src[3] = (byte)((a2 * a1) >> 8);
		#endif
			src += sizeof(uint);
		}
	}
	else {
		byte *src = (byte*)card;
		a1 = _ipixel_norm(a1);
		r1 = _ipixel_norm(r1);
		g1 = _ipixel_norm(g1);
		b1 = _ipixel_norm(b1);
		for (; size > 0; src += sizeof(uint), size--) {
			_ipixel_load_card(src, r2, g2, b2, a2);
			r2 = (r1 * r2) >> 8;
			g2 = (g1 * g2) >> 8;
			b2 = (b1 * b2) >> 8;
			a2 = (a1 * a2) >> 8;
			*((uint*)src) = IRGBA_TO_A8R8G8B8(r2, g2, b2, a2);
		}
	}
}


void (*ipixel_card_multi_proc)(uint *card, int size, uint color) =
	ipixel_card_multi_default;

/* multi card */
void ipixel_card_multi(uint *card, int size, uint color)
{
	ipixel_card_multi_proc(card, size, color);
}


/* mask card */
void ipixel_card_mask_default(uint *card, int size, uint *mask)
{
	uint r1, g1, b1, a1, r2, g2, b2, a2;
	for (; size > 0; card++, mask++, size--) {
		_ipixel_load_card(mask, r1, g1, b1, a1);
		_ipixel_load_card(card, r2, g2, b2, a2);
		r2 = _imul_y_div_255(r2, r1);
		g2 = _imul_y_div_255(g2, g1);
		b2 = _imul_y_div_255(b2, b1);
		a2 = _imul_y_div_255(a2, a1);
		*card = IRGBA_TO_A8R8G8B8(r2, g2, b2, a2);
	}
}

void (*ipixel_card_mask_proc)(uint *card, int size, uint *mask) =
	ipixel_card_mask_default;

/* mask card */
void ipixel_card_mask(uint *card, int size, uint *mask)
{
	ipixel_card_mask_proc(card, size, mask);
}

/* cover multi */
void ipixel_card_cover_default(uint *card, int size, byte *cover)
{
	int cc, aa;
	for (; size > 0; card++, size--) {
		cc = *cover++;
		if (cc == 0) {
			((byte*)card)[_ipixel_card_alpha] = 0;
		}
		else {
			aa = ((byte*)card)[_ipixel_card_alpha];
			if (aa == 0) continue;
			aa *= cc;
			((byte*)card)[_ipixel_card_alpha] = 
				(byte)_ipixel_fast_div_255(aa);
		}
	}
}

void (*ipixel_card_cover_proc)(uint *card, int size, byte *cover) 
	= ipixel_card_cover_default;

/* mask cover */
void ipixel_card_cover(uint *card, int size, byte *cover)
{
	ipixel_card_cover_proc(card, size, cover);
}

void ipixel_card_over_default(uint *dst, int size, uint *card,
	const byte *cover)
{
	uint *endup = dst + size;
	if (cover == null) {
		for (; dst < endup; card++, dst++) {
			IBLEND_PARGB(dst[0], card[0]);
		}
	}	else {
		for (; dst < endup; cover++, card++, dst++) {
			IBLEND_PARGB_COVER(dst[0], card[0], cover[0]);
		}
	}
}

void (*ipixel_card_over_proc)(uint*, int, uint*, byte*) =
	ipixel_card_over_default;

/* card composite: src over */
void ipixel_card_over(uint *dst, int size, uint *card, 
	const byte *cover)
{
	ipixel_card_over_proc(dst, size, card, cover);
}

/* default card permute */
public static void ipixel_card_permute_default(uint *card, int w, 
	int b0, int b1, int b2, int b3)
{
	byte *src = (byte*)card;
	union { byte quad[4]; uint color; } cc;
	for (; w > 0; src += 4, w--) {
		cc.color = *((uint*)src);
		src[0] = cc.quad[b0];
		src[1] = cc.quad[b1];
		src[2] = cc.quad[b2];
		src[3] = cc.quad[b3];
	}
}

public static void (*ipixel_card_permute_proc)(uint*, int, int, int, int, int) = 
	ipixel_card_permute_default;

/* card permute */
void ipixel_card_permute(uint *card, int w, int a, int b, int c, int d)
{
	ipixel_card_permute_proc(card, w, a, b, c, d);
}


/* card proc set */
void ipixel_card_set_proc(int id, void *proc)
{
	if (id == 0) {
		if (proc == null) ipixel_card_multi_proc = ipixel_card_multi_default;
		else {
			ipixel_card_multi_proc = 
				(void (*)(uint *, int, uint))proc;
		}
	}
	else if (id == 1) {
		if (proc == null) ipixel_card_mask_proc = ipixel_card_mask_default;
		else {
			ipixel_card_mask_proc = 
				(void (*)(uint *, int, uint *))proc;
		}
	}
	else if (id == 2) {
		if (proc == null) ipixel_card_cover_proc = ipixel_card_cover_default;
		else {
			ipixel_card_cover_proc = 
				(void (*)(uint *, int, byte *))proc;
		}
	}
	else if (id == 3) {
		if (proc == null) ipixel_card_over_proc = ipixel_card_over_default;
		else {
			ipixel_card_over_proc = 
				(void (*)(uint*, int, uint*, byte*))proc;
		}
	}
	else if (id == 4) {
		if (proc == null) 
			ipixel_card_permute_proc = ipixel_card_permute_default;
		else
			ipixel_card_permute_proc = 
				(void (*)(uint*, int, int, int, int, int))proc;
	}
}


public struct iPixelHLineDrawProc
{
	iHLineDrawProc blend, srcover, additive;
	iHLineDrawProc blend_builtin, srcover_builtin, additive_builtin;
};


public static public struct iPixelHLineDrawProc ipixel_hline_proc_list[IPIX_FMT_COUNT] =
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
	ITABLE_ITEM(C4),
	ITABLE_ITEM(G4),
	ITABLE_ITEM(A4),
	ITABLE_ITEM(R1G2B1),
	ITABLE_ITEM(B1G2R1),
	ITABLE_ITEM(A1R1G1B1),
	ITABLE_ITEM(A1B1G1R1),
	ITABLE_ITEM(R1G1B1A1),
	ITABLE_ITEM(B1G1R1A1),
	ITABLE_ITEM(X1R1G1B1),
	ITABLE_ITEM(X1B1G1R1),
	ITABLE_ITEM(R1G1B1X1),
	ITABLE_ITEM(B1G1R1X1),
	ITABLE_ITEM(C1),
	ITABLE_ITEM(G1),
	ITABLE_ITEM(A1),
};




/* get a hline drawing function with given pixel format */
iHLineDrawProc ipixel_get_hline_proc(int fmt, int op, int builtin)
{
	assert(fmt >= 0 && fmt < IPIX_FMT_COUNT);
	if (fmt < 0 || fmt >= IPIX_FMT_COUNT) {
		abort();
		return null;
	}
	if (ipixel_lut_inited == 0) ipixel_init_lut();
	if (builtin) {
		if (op == 0) 
			return ipixel_hline_proc_list[fmt].blend_builtin;
		else if (op == 1)
			return ipixel_hline_proc_list[fmt].srcover_builtin;
		else
			return ipixel_hline_proc_list[fmt].additive_builtin;
	}	else {
		if (op == 0) 
			return ipixel_hline_proc_list[fmt].blend;
		else if (op == 1)
			return ipixel_hline_proc_list[fmt].srcover;
		else 
			return ipixel_hline_proc_list[fmt].additive;
	}
}

/* set a hline drawing function */
void ipixel_set_hline_proc(int fmt, int op, iHLineDrawProc proc)
{
	assert(fmt >= 0 && fmt < IPIX_FMT_COUNT);
	if (fmt < 0 || fmt >= IPIX_FMT_COUNT) {
		abort();
		return;
	}
	if (ipixel_lut_inited == 0) ipixel_init_lut();
	if (op == 0) {
		if (proc != null) {
			ipixel_hline_proc_list[fmt].blend = proc;
		}	else {
			ipixel_hline_proc_list[fmt].blend = 
				ipixel_hline_proc_list[fmt].blend_builtin;
		}
	}	
	else if (op == 1) {
		if (proc != null) {
			ipixel_hline_proc_list[fmt].srcover = proc;
		}	else {
			ipixel_hline_proc_list[fmt].srcover = 
				ipixel_hline_proc_list[fmt].srcover_builtin;
		}
	}
	else {
		if (proc != null) {
			ipixel_hline_proc_list[fmt].additive = proc;
		}	else {
			ipixel_hline_proc_list[fmt].additive = 
				ipixel_hline_proc_list[fmt].additive_builtin;
		}
	}
}


/* ipixel_blend - blend between two formats 
 * you must provide a working memory pointer to workmem. if workmem eq null,
 * this function will do nothing but returns how many bytes needed in workmem
 * dfmt        - dest pixel format
 * dbits       - dest pixel buffer
 * dpitch      - dest row stride
 * dx          - dest x offset
 * sfmt        - source pixel format
 * sbits       - source pixel buffer
 * spitch      - source row stride
 * sx          - source x offset
 * w           - width
 * h           - height
 * color       - color
 * op          - blending operator (IPIXEL_BLEND_OP_BLEND, ADD, COPY)
 * flip        - flip (IPIXEL_FLIP_NONE, HFLIP, VFLIP)
 * dindex      - dest index
 * sindex      - source index
 * workmem     - working memory
 * this function need some memory to work with. to avoid allocating, 
 * you must provide a memory block whose size is (w * 4) to it.
 */
int ipixel_blend(int dfmt, void *dbits, int dpitch, int dx, int sfmt, 
	const void *sbits, int spitch, int sx, int w, int h, uint color,
	int op, int flip, iColorIndex *dindex, 
	const iColorIndex *sindex, void *workmem)
{
	uint *buffer = (uint*)workmem;
	byte *dline = (byte*)dbits;
	const byte *sline = ( byte*)sbits;
	iSpanDrawProc drawspan = null;
	iFetchProc fetch;
	iStoreProc store;
	int k;

	if (workmem == null) {
		return w * sizeof(uint);
	}

	fetch = ipixel_get_fetch(sfmt, IPIXEL_ACCESS_MODE_NORMAL);
	store = ipixel_get_store(dfmt, IPIXEL_ACCESS_MODE_NORMAL);

	if (op == IPIXEL_BLEND_OP_BLEND) {
		drawspan = ipixel_get_span_proc(dfmt, 0, 0);
	}
	else if (op == IPIXEL_BLEND_OP_SRCOVER) {
		drawspan = ipixel_get_span_proc(dfmt, 1, 0);
	}
	else if (op == IPIXEL_BLEND_OP_ADD) {
		drawspan = ipixel_get_span_proc(dfmt, 2, 0);
	}

	if ((flip & IPIXEL_FLIP_VFLIP) != 0) {
		sline = sline + spitch * (h - 1);
		spitch = -spitch;
	}

	if (sfmt == IPIX_FMT_P8R8G8B8 && dfmt == IPIX_FMT_P8R8G8B8) {
		if (op == IPIXEL_BLEND_OP_BLEND && color == 0xffffffff) {
			sfmt = IPIX_FMT_A8R8G8B8;
			dfmt = IPIX_FMT_A8R8G8B8;
			drawspan = ipixel_get_span_proc(-1, 0, 0);
		}
	}

	if (sfmt == IPIX_FMT_A8R8G8B8 && (flip & IPIXEL_FLIP_HFLIP) == 0 &&
		color == 0xffffffff) {
		if (drawspan != null) {
			for (k = 0; k < h; sline += spitch, dline += dpitch, k++) {
				const uint *src = ((const uint*)sline) + sx;
				uint *dst = ((uint*)dline);
				drawspan(dst, dx, w, src, null, dindex);
			}
		}	else {
			for (k = 0; k < h; sline += spitch, dline += dpitch, k++) {
				const uint *src = ((const uint*)sline) + sx;
				uint *dst = ((uint*)dline);
				store(dst, src, dx, w, dindex);
			}
		}
		return 0;
	}


	if ((flip & IPIXEL_FLIP_HFLIP) == 0) {
		if (drawspan != null) {
			if (color == 0xffffffff) {
				IPIXEL_BLEND_LOOP( {
					drawspan(dst, dx, w, buffer, null, dindex);
				});
			}	else {
				IPIXEL_BLEND_LOOP( {
					ipixel_card_multi_proc(buffer, w, color);
					drawspan(dst, dx, w, buffer, null, dindex);
				});
			}
		}	else {
			if (color == 0xffffffff) {
				IPIXEL_BLEND_LOOP( {
					store(dst, buffer, dx, w, dindex);
				});
			}	else {
				IPIXEL_BLEND_LOOP( {
					ipixel_card_multi_proc(buffer, w, color);
					store(dst, buffer, dx, w, dindex);
				});
			}
		}
	}	else {
		if (drawspan != null) {
			if (color == 0xffffffff) {
				IPIXEL_BLEND_LOOP( {
					ipixel_card_reverse(buffer, w);
					drawspan(dst, dx, w, buffer, null, dindex);
				});
			}	else {
				IPIXEL_BLEND_LOOP( {
					ipixel_card_reverse(buffer, w);
					ipixel_card_multi_proc(buffer, w, color);
					drawspan(dst, dx, w, buffer, null, dindex);
				});
			}
		}	else {
			if (color == 0xffffffff) {
				IPIXEL_BLEND_LOOP( {
					ipixel_card_reverse(buffer, w);
					store(dst, buffer, dx, w, dindex);
				});
			}	else {
				IPIXEL_BLEND_LOOP( {
					ipixel_card_reverse(buffer, w);
					ipixel_card_multi_proc(buffer, w, color);
					store(dst, buffer, dx, w, dindex);
				});
			}
		}
	}

	#undef IPIXEL_BLEND_LOOP

	return 0;
}




/* blit driver desc */
public struct iPixelBlitProc
{
	iBlitProc normal, normal_default;
	iBlitProc mask, mask_default;
};



/* blit procedure look up table */
public static public struct iPixelBlitProc ipixel_blit_proc_list[6] =
{
	ITABLE_ITEM(32),
	ITABLE_ITEM(24),
	ITABLE_ITEM(16),
	ITABLE_ITEM(8),
	ITABLE_ITEM(4),
	ITABLE_ITEM(1),
};

public static int ipixel_lookup_bpp[33] = {
	-1, 5, -1, -1, 4, -1, -1, -1, 3, -1, -1, -1, -1, -1, -1, 
	2, 2, -1, -1, -1, -1, -1, -1, -1, 1, -1, -1, -1, -1, -1,
	-1, -1, 0,
};


/* get normal blit procedure */
/* if ismask equals to zero, returns normal bliter */
/* if ismask doesn't equal to zero, returns transparent bliter */
iBlitProc ipixel_blit_get(int bpp, int istransparent, int isdefault)
{
	int index;
	if (bpp < 0 || bpp > 32) return null;
	index = ipixel_lookup_bpp[bpp];
	if (index < 0) return null;
	if (isdefault) {
		if (istransparent) return ipixel_blit_proc_list[index].mask_default;
		return ipixel_blit_proc_list[index].normal_default;
	}
	if (istransparent) return ipixel_blit_proc_list[index].mask;
	return ipixel_blit_proc_list[index].normal;
}

/* set normal blit procedure */
/* if ismask equals to zero, set normal bliter */
/* if ismask doesn't equal to zero, set transparent bliter */
void ipixel_set_blit_proc(int bpp, int istransparent, iBlitProc proc)
{
	int index;
	if (bpp < 0 || bpp > 32) return;
	index = ipixel_lookup_bpp[bpp];
	if (index < 0) return;
	if (istransparent == 0) {
		if (proc != null) {
			ipixel_blit_proc_list[index].normal = proc;
		}	else {
			ipixel_blit_proc_list[index].normal = 
				ipixel_blit_proc_list[index].normal_default;
		}
	}	else {
		if (proc != null) {
			ipixel_blit_proc_list[index].mask = proc;
		}	else {
			ipixel_blit_proc_list[index].mask = 
				ipixel_blit_proc_list[index].mask_default;
		}
	}
}



/* ipixel_blit - bliting (copy pixel from one rectangle to another)
 * it will only copy pixels in the same depth (1/4/8/16/24/32).
 * no color format will be convert (using ipixel_convert to do it)
 * bpp    - color depth of the two bitmap
 * dst    - dest bits
 * dpitch - dest pitch (row stride)
 * dx     - dest x offset
 * src    - source bits
 * spitch - source pitch (row stride)
 * sx     - source x offset
 * w      - width
 * h      - height
 * mask   - mask color (colorkey), no effect without IPIXEL_BLIT_MASK
 * mode   - IPIXEL_FLIP_HFLIP | IPIXEL_FLIP_VFLIP | IPIXEL_BLIT_MASK ..
 * for transparent bliting, set mode with IPIXEL_BLIT_MASK, bliter will 
 * skip the colors equal to 'mask' parameter.
 */
void ipixel_blit(int bpp, void *dst, int dpitch, int dx, void* src, 
	int spitch, int sx, int w, int h, uint mask, int mode)
{
	int transparent, flip, index, retval;
	iBlitProc bliter;

	transparent = (mask & IPIXEL_BLIT_MASK)? 1 : 0;
	flip = mode & (IPIXEL_FLIP_HFLIP | IPIXEL_FLIP_VFLIP);
	
	assert(bpp >= 0 && bpp <= 32);

	index = ipixel_lookup_bpp[bpp];

	if (transparent) bliter = ipixel_blit_proc_list[index].mask;
	else bliter = ipixel_blit_proc_list[index].normal;

	/* using current bliter */
	bliter = ipixel_blit_get(bpp, transparent, 0);

	if (bliter) {
		retval = bliter(dst, dpitch, dx, src, spitch, sx, w, h, mask, flip);
		if (retval == 0) return; /* return for success */
	}

	/* using default bliter */
	if (transparent) bliter = ipixel_blit_proc_list[index].mask_default;
	else bliter = ipixel_blit_proc_list[index].normal_default;
	
	bliter(dst, dpitch, dx, src, spitch, sx, w, h, mask, flip);
}


/**********************************************************************
 * CONVERTING
 **********************************************************************/
public static iPixelCvt ipixel_cvt_table[IPIX_FMT_COUNT][IPIX_FMT_COUNT][8];
public static int ipixel_cvt_inited = 0;

/* initialize converting procedure table */
public static void ipixel_cvt_init(void)
{
	int dfmt, sfmt, i;
	if (ipixel_cvt_inited) return;
	for (dfmt = 0; dfmt < IPIX_FMT_COUNT; dfmt++) {
		for (sfmt = 0; sfmt < IPIX_FMT_COUNT; sfmt++) {
			for (i = 0; i < 8; i++) ipixel_cvt_table[dfmt][sfmt][i] = null;
		}
	}
	ipixel_cvt_inited = 1;
}

/* get converting procedure */
iPixelCvt ipixel_cvt_get(int dfmt, int sfmt, int index)
{
	if (ipixel_cvt_inited == 0) ipixel_cvt_init();
	if (dfmt < 0 || dfmt >= IPIX_FMT_COUNT) return null;
	if (sfmt < 0 || sfmt >= IPIX_FMT_COUNT) return null;
	if (index < 0 || index >= 8) return null;
	return ipixel_cvt_table[dfmt][sfmt][index];
}

/* set converting procedure */
void ipixel_cvt_set(int dfmt, int sfmt, int index, iPixelCvt proc)
{
	if (ipixel_cvt_inited == 0) ipixel_cvt_init();
	if (dfmt < 0 || dfmt >= IPIX_FMT_COUNT) return;
	if (sfmt < 0 || sfmt >= IPIX_FMT_COUNT) return;
	if (index < 0 || index >= 8) return;
	ipixel_cvt_table[dfmt][sfmt][index] = proc;
}

/* ipixel_slow: default slow converter */
int ipixel_cvt_slow(int dfmt, void *dbits, int dpitch, int dx, int sfmt, 
	const void *sbits, int spitch, int sx, int w, int h, uint mask, 
	int mode, iColorIndex *dindex, iColorIndex *sindex)
{
	const iColorIndex *_ipixel_dst_index = dindex;
	const iColorIndex *_ipixel_src_index = sindex;
	int flip, sbpp, dbpp, i, j;
	int transparent;

	flip = mode & (IPIXEL_FLIP_HFLIP | IPIXEL_FLIP_VFLIP);
	transparent = (mode & IPIXEL_BLIT_MASK)? 1 : 0;

	sbpp = ipixelfmt[sfmt].bpp;
	dbpp = ipixelfmt[dfmt].bpp;

	if (flip & IPIXEL_FLIP_VFLIP) { 
		sbits = ( byte*)sbits + spitch * (h - 1); 
		spitch = -spitch; 
	} 

	for (j = 0; j < h; j++) {
		uint cc = 0, r, g, b, a;
		int incx, x1, x2 = dx;

		if ((flip & IPIXEL_FLIP_HFLIP) == 0) x1 = sx, incx = 1;
		else x1 = sx + w - 1, incx = -1;

		for (i = w; i > 0; x1 += incx, x2++, i--) {
			switch (sbpp) {
				case  1: cc = _ipixel_fetch(1, sbits, x1); break;
				case  4: cc = _ipixel_fetch(4, sbits, x1); break;
				case  8: cc = _ipixel_fetch(8, sbits, x1); break;
				case 16: cc = _ipixel_fetch(16, sbits, x1); break;
				case 24: cc = _ipixel_fetch(24, sbits, x1); break;
				case 32: cc = _ipixel_fetch(32, sbits, x1); break;
			}

			if (transparent && cc == mask) 
				continue;

			IRGBA_DISEMBLE(sfmt, cc, r, g, b, a);
			IRGBA_ASSEMBLE(dfmt, cc, r, g, b, a);

			switch (dbpp) {
				case  1: _ipixel_store(1, dbits, x2, cc); break;
				case  4: _ipixel_store(4, dbits, x2, cc); break;
				case  8: _ipixel_store(8, dbits, x2, cc); break;
				case 16: _ipixel_store(16, dbits, x2, cc); break;
				case 24: _ipixel_store(24, dbits, x2, cc); break;
				case 32: _ipixel_store(32, dbits, x2, cc); break;
			}
		}

		sbits = ( byte*)sbits + spitch;
		dbits = (byte*)dbits + dpitch;
	}

	return 0;
}


/* ipixel_convert: convert pixel format 
 * you must provide a working memory pointer to mem. if mem eq null,
 * this function will do nothing but returns how many bytes needed in mem
 * dfmt   - dest color format
 * dbits  - dest bits
 * dpitch - dest pitch (row stride)
 * dx     - dest x offset
 * sfmt   - source color format
 * sbits  - source bits
 * spitch - source pitch (row stride)
 * sx     - source x offset
 * w      - width
 * h      - height
 * mask   - mask color (colorkey), no effect without IPIXEL_BLIT_MASK
 * mode   - IPIXEL_FLIP_HFLIP | IPIXEL_FLIP_VFLIP | IPIXEL_BLIT_MASK ..
 * didx   - dest color index
 * sidx   - source color index
 * mem    - work memory
 * for transparent converting, set mode with IPIXEL_BLIT_MASK, it will 
 * skip the colors equal to 'mask' parameter.
 */
int ipixel_convert(int dfmt, void *dbits, int dpitch, int dx, int sfmt, 
	const void *sbits, int spitch, int sx, int w, int h, uint mask, 
	int mode, iColorIndex *didx, iColorIndex *sidx, void *mem)
{
	iPixelCvt cvt = null;
	int flip, index;

	if (mem == null) {
		return w * sizeof(uint);
	}

	if (ipixel_cvt_inited == 0) ipixel_cvt_init();

	assert(dfmt >= 0 && dfmt < IPIX_FMT_COUNT);
	assert(sfmt >= 0 && sfmt < IPIX_FMT_COUNT);

	flip = mode & (IPIXEL_FLIP_HFLIP | IPIXEL_FLIP_VFLIP);
	index = (mode & IPIXEL_BLIT_MASK)? 1 : 0;

	if (mode & IPIXEL_CVT_FLAG) 
		index = 2 + ((mode >> 8) & 3);

	if (didx == null) didx = _ipixel_dst_index;
	if (sidx == null) sidx = _ipixel_src_index;

	cvt = ipixel_cvt_table[dfmt][sfmt][index];

	/* using converting procedure */
	if (cvt != null) {
		int retval = cvt(dbits, dpitch, dx, sbits, spitch, sx, w, h, 
			mask, flip, didx, sidx);
		if (retval == 0) return 0;
	}

	/* using bliting procedure when no convertion needed */
	if (sfmt == dfmt && ipixelfmt[sfmt].type != IPIX_FMT_TYPE_INDEX) {
		ipixel_blit(ipixelfmt[sfmt].bpp, dbits, dpitch, dx, sbits, 
			spitch, sx, w, h, mask, mode);
		return 0;
	}

	/* without transparent color key using ipixel_blend */
	if ((mode & IPIXEL_BLIT_MASK) == 0) {
		int operator = IPIXEL_BLEND_OP_COPY;
		if (mode & IPIXEL_CVT_FLAG) operator = ((mode >> 8) & 3);
		return ipixel_blend(dfmt, dbits, dpitch, dx, sfmt, sbits, spitch, sx,
			w, h, 0xffffffff, operator, flip, didx, sidx, mem);
	}

	/* using ipixel_cvt_slow to proceed other convertion */
	ipixel_cvt_slow(dfmt, dbits, dpitch, dx, sfmt, sbits, spitch, sx,
		w, h, mask, mode, didx, sidx);

	return 0;
}


/**********************************************************************
 * FREE FORMAT CONVERT
 **********************************************************************/

public static iPixelFmtReader ipixel_fmt_reader[4] = { null, null, null, null };
public static iPixelFmtWriter ipixel_fmt_writer[4] = { null, null, null, null };
public static iPixelFmtPermute ipixel_fmt_permutor[2][2] = 
	{ {null, null}, {null, null} };

/* default free format reader */
public static int ipixel_fmt_reader_default(const iPixelFmt *fmt, 
	const void *bits, int x, int w, uint *card);

/* default free format writer */
public static int ipixel_fmt_writer_default(const iPixelFmt *fmt,
	void *bits, int x, int w, uint *card);

/* default permute proc */
public static int ipixel_fmt_permute_default(int dbpp, byte *dst, int w, int step,
	int sbpp, byte *src, int *pos, uint mask, int mode);


/* ipixel_fmt_init: init pixel format structure
 * depth: color bits, one of 8, 16, 24, 32
 * rmask: mask for red, eg:   0x00ff0000
 * gmask: mask for green, eg: 0x0000ff00
 * bmask: mask for blue, eg:  0x000000ff
 * amask: mask for alpha, eg: 0xff000000
 */
int ipixel_fmt_init(iPixelFmt *fmt, int depth, 
	uint rmask, uint gmask, uint bmask, uint amask)
{
	int pixelbyte = (depth + 7) / 8;
	int i;
	fmt.bpp = pixelbyte * 8;
	if (depth < 8 || depth > 32) {
		return -1;
	}
	if (ipixel_lut_inited == 0) 
		ipixel_init_lut();
	for (i = 0; i < IPIX_FMT_COUNT; i++) {
		if (ipixelfmt[i].amask == amask &&
			ipixelfmt[i].rmask == rmask &&
			ipixelfmt[i].gmask == gmask &&
			ipixelfmt[i].bmask == bmask) {
			if (fmt.bpp == ipixelfmt[i].bpp) {
				fmt[0] = ipixelfmt[i];
				return 0;
			}
		}
	}
	fmt.format = IPIX_FMT_PACKED;
	fmt.bpp = pixelbyte * 8;
	fmt.rmask = rmask;
	fmt.gmask = gmask;
	fmt.bmask = bmask;
	fmt.amask = amask;
	fmt.alpha = (amask == 0)? 0 : 1;
	fmt.type = (fmt.alpha)? IPIX_FMT_TYPE_ARGB : IPIX_FMT_TYPE_RGB;
	fmt.pixelbyte = pixelbyte;
	fmt.name = "IPIX_FMT_PACKED";
	for (i = 0; i < 4; i++) {
		uint mask = 0;
		int shift = 0;
		int loss = 8;
		switch (i) {
		case 0: mask = fmt.rmask; break;
		case 1: mask = fmt.gmask; break;
		case 2: mask = fmt.bmask; break;
		case 3: mask = fmt.amask; break;
		}
		if (mask != 0) {
			int zeros = 0;
			int ones = 0;
			for (zeros = 0; (mask & 1) == 0; zeros++, mask >>= 1);
			for (ones = 0; (mask & 1) == 1; ones++, mask >>= 1);
			shift = zeros;
			if (ones <= 8) {
				loss = 8 - ones;
			}	else {
				return -2;
			}
		}
		switch (i) {
		case 0: fmt.rloss = loss; fmt.rshift = shift; break;
		case 1: fmt.gloss = loss; fmt.gshift = shift; break;
		case 2: fmt.bloss = loss; fmt.bshift = shift; break;
		case 3: fmt.aloss = loss; fmt.ashift = shift; break;
		}
	}
	return 0;
}


/* set free format reader */
void ipixel_fmt_set_reader(int depth, iPixelFmtReader reader)
{
	int index = ((depth + 7) / 8) - 1;
	if (index >= 0 && index < 4) {
		ipixel_init_lut();
		ipixel_fmt_reader[index] = reader;
	}
}


/* set free format writer */
void ipixel_fmt_set_writer(int depth, iPixelFmtWriter writer)
{
	int index = ((depth + 7) / 8) - 1;
	if (index >= 0 && index < 4) {
		ipixel_fmt_writer[index] = writer;
	}
}


/* get free format reader */
iPixelFmtReader ipixel_fmt_get_reader(int depth, int isdefault)
{
	int index = ((depth + 7) / 8) - 1;
	int inited = 0;
	if (inited == 0) {
		ipixel_init_lut();
		inited = 1;
	}
	if (index < 0 || index >= 4) return null;
	if (ipixel_fmt_reader[index] == null || isdefault != 0) {
		return ipixel_fmt_reader_default;
	}
	return ipixel_fmt_reader[index];
}


/* get free format writer */
iPixelFmtWriter ipixel_fmt_get_writer(int depth, int isdefault)
{
	int index = ((depth + 7) / 8) - 1;
	if (index < 0 || index >= 4) return null;
	if (ipixel_fmt_writer[index] == null || isdefault != 0) {
		return ipixel_fmt_writer_default;
	}
	return ipixel_fmt_writer[index];
}

/* setup permute proc */
void ipixel_fmt_set_permute(int dbpp, int sbpp, iPixelFmtPermute permute)
{
	int dpos = (dbpp == 32)? 0 : 1;
	int spos = (sbpp == 32)? 0 : 1;
	ipixel_fmt_permutor[dpos][spos] = permute;
}

/* get permute proc */
iPixelFmtPermute ipixel_fmt_get_permute(int dbpp, int sbpp, int isdefault)
{
	if (isdefault) {
		return ipixel_fmt_permute_default;
	}
	else {
		int dpos = (dbpp == 32)? 0 : 1;
		int spos = (sbpp == 32)? 0 : 1;
		iPixelFmtPermute proc = ipixel_fmt_permutor[dpos][spos];
		return (proc)? proc : ipixel_fmt_permute_default;
	}
}

/* free format defaut writer */
public static int ipixel_fmt_writer_default(const iPixelFmt *fmt,
	void *bits, int x, int w, uint *card)
{
	int rshift = fmt.rshift;
	int gshift = fmt.gshift;
	int bshift = fmt.bshift;
	int ashift = fmt.ashift;
	int rloss = fmt.rloss;
	int gloss = fmt.gloss;
	int bloss = fmt.bloss;
	int aloss = fmt.aloss;
	uint r, g, b, a, cc;
	byte *dst = ((byte*)bits) + fmt.pixelbyte * x;
	switch (fmt.pixelbyte) {
	case 1: 
		for (; w > 0; x++, card++, w--) {
			_ipixel_load_card(card, r, g, b, a);
			r = (r >> rloss) << rshift;
			g = (g >> gloss) << gshift;
			b = (b >> bloss) << bshift;
			a = (a >> aloss) << ashift;
			cc = r | g | b | a;
			_ipixel_store(8, dst, 0, cc);
			dst += 1;
		}
		break;
	case 2:
		for (; w > 0; x++, card++, w--) {
			_ipixel_load_card(card, r, g, b, a);
			r = (r >> rloss) << rshift;
			g = (g >> gloss) << gshift;
			b = (b >> bloss) << bshift;
			a = (a >> aloss) << ashift;
			cc = r | g | b | a;
			_ipixel_store(16, dst, 0, cc);
			dst += 2;
		}
		break;
	case 3:
		for (; w > 0; x++, card++, w--) {
			_ipixel_load_card(card, r, g, b, a);
			r = (r >> rloss) << rshift;
			g = (g >> gloss) << gshift;
			b = (b >> bloss) << bshift;
			a = (a >> aloss) << ashift;
			cc = r | g | b | a;
			_ipixel_store(24, dst, 0, cc);
			dst += 3;
		}
		break;
	case 4:
		for (; w > 0; x++, card++, w--) {
			_ipixel_load_card(card, r, g, b, a);
			r = (r >> rloss) << rshift;
			g = (g >> gloss) << gshift;
			b = (b >> bloss) << bshift;
			a = (a >> aloss) << ashift;
			cc = r | g | b | a;
			_ipixel_store(32, dst, 0, cc);
			dst += 4;
		}
		break;
	}
	return 0;
}


/* free format default reader */
public static int ipixel_fmt_reader_default(const iPixelFmt *fmt, 
	const void *bits, int x, int w, uint *card)
{
	uint rmask = fmt.rmask;
	uint gmask = fmt.gmask;
	uint bmask = fmt.bmask;
	uint amask = fmt.amask;
	int rshift = fmt.rshift;
	int gshift = fmt.gshift;
	int bshift = fmt.bshift;
	int ashift = fmt.ashift;
	const byte *rscale = &_ipixel_bit_scale[fmt.rloss][0];
	const byte *gscale = &_ipixel_bit_scale[fmt.gloss][0];
	const byte *bscale = &_ipixel_bit_scale[fmt.bloss][0];
	const byte *ascale = &_ipixel_bit_scale[fmt.aloss][0];
	const byte *src = (( byte*)bits) + fmt.pixelbyte * x;
	uint r, g, b, a, cc;
	switch (fmt.pixelbyte) {
	case 1:
		for (; w > 0; x++, card++, w--) {
			cc = _ipixel_fetch(8, src, 0);
			r = rscale[(cc & rmask) >> rshift];
			g = gscale[(cc & gmask) >> gshift];
			b = bscale[(cc & bmask) >> bshift];
			a = ascale[(cc & amask) >> ashift];
			src += 1;
			card[0] = IRGBA_TO_A8R8G8B8(r, g, b, a);
		}
		break;
	case 2:
		for (; w > 0; x++, card++, w--) {
			cc = _ipixel_fetch(16, src, 0);
			r = rscale[(cc & rmask) >> rshift];
			g = gscale[(cc & gmask) >> gshift];
			b = bscale[(cc & bmask) >> bshift];
			a = ascale[(cc & amask) >> ashift];
			src += 2;
			card[0] = IRGBA_TO_A8R8G8B8(r, g, b, a);
		}
		break;
	case 3:
		for (; w > 0; x++, card++, w--) {
			cc = _ipixel_fetch(24, src, 0);
			r = rscale[(cc & rmask) >> rshift];
			g = gscale[(cc & gmask) >> gshift];
			b = bscale[(cc & bmask) >> bshift];
			a = ascale[(cc & amask) >> ashift];
			src += 3;
			card[0] = IRGBA_TO_A8R8G8B8(r, g, b, a);
		}
		break;
	case 4:
		for (; w > 0; x++, card++, w--) {
			cc = _ipixel_fetch(32, src, 0);
			r = rscale[(cc & rmask) >> rshift];
			g = gscale[(cc & gmask) >> gshift];
			b = bscale[(cc & bmask) >> bshift];
			a = ascale[(cc & amask) >> ashift];
			src += 4;
			card[0] = IRGBA_TO_A8R8G8B8(r, g, b, a);
		}
		break;
	}
	return 0;
}


/* ipixel_slow: default slow converter */
int ipixel_fmt_slow(const iPixelFmt *dfmt, void *dbits, int dpitch, 
	int dx, iPixelFmt *sfmt, void *sbits, int spitch, 
	int sx, int w, int h, uint mask, int mode, 
	const iColorIndex *dindex, iColorIndex *sindex)
{
	const iColorIndex *_ipixel_dst_index = dindex;
	const iColorIndex *_ipixel_src_index = sindex;
	int flip, sbpp, dbpp, i, j;
	int transparent;

	flip = mode & (IPIXEL_FLIP_HFLIP | IPIXEL_FLIP_VFLIP);
	transparent = (mode & IPIXEL_BLIT_MASK)? 1 : 0;

	sbpp = sfmt.bpp;
	dbpp = dfmt.bpp;

	if (flip & IPIXEL_FLIP_VFLIP) { 
		sbits = ( byte*)sbits + spitch * (h - 1); 
		spitch = -spitch; 
	} 

	for (j = 0; j < h; j++) {
		uint cc = 0, r, g, b, a;
		int incx, x1, x2 = dx;

		if ((flip & IPIXEL_FLIP_HFLIP) == 0) x1 = sx, incx = 1;
		else x1 = sx + w - 1, incx = -1;

		for (i = w; i > 0; x1 += incx, x2++, i--) {
			switch (sbpp) {
				case  1: cc = _ipixel_fetch(1, sbits, x1); break;
				case  4: cc = _ipixel_fetch(4, sbits, x1); break;
				case  8: cc = _ipixel_fetch(8, sbits, x1); break;
				case 16: cc = _ipixel_fetch(16, sbits, x1); break;
				case 24: cc = _ipixel_fetch(24, sbits, x1); break;
				case 32: cc = _ipixel_fetch(32, sbits, x1); break;
			}

			if (transparent && cc == mask) 
				continue;

			IRGBA_FMT_DISEMBLE(sfmt, cc, r, g, b, a);
			IRGBA_FMT_ASSEMBLE(dfmt, cc, r, g, b, a);

			switch (dbpp) {
				case  1: _ipixel_store(1, dbits, x2, cc); break;
				case  4: _ipixel_store(4, dbits, x2, cc); break;
				case  8: _ipixel_store(8, dbits, x2, cc); break;
				case 16: _ipixel_store(16, dbits, x2, cc); break;
				case 24: _ipixel_store(24, dbits, x2, cc); break;
				case 32: _ipixel_store(32, dbits, x2, cc); break;
			}
		}

		sbits = ( byte*)sbits + spitch;
		dbits = (byte*)dbits + dpitch;
	}

	return 0;
}


/* calculate position */
public static int ipixel_fmt_position(int shift) 
{
#if IPIXEL_BIG_ENDIAN
	int table[] = {3, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, 
		-1, -1, -1, -1, 1, -1, -1, -1, -1, -1, -1, -1, 0, -1, -1, 
		-1, -1, -1, -1, -1, -1};
#else
	int table[] = {0, -1, -1, -1, -1, -1, -1, -1, 1, -1, -1, -1, -1, 
		-1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, 3, -1, -1, -1, -1,
		-1, -1, -1, -1};
#endif
	return table[shift];
}

/* default permute proc */
public static int ipixel_fmt_permute_default(int dbpp, byte *dst, int w, int step,
	int sbpp, byte *src, int *pos, uint mask, int mode)
{
	uint cc = 0;
	int b0 = pos[0];
	int b1 = pos[1];
	int b2 = pos[2];
	int b3 = pos[3];
	int ba = pos[4];
	int ca = pos[5];
	if (dbpp == 32) {
		if (sbpp == 32) {
			if ((mode & IPIXEL_BLIT_MASK) == 0) {
				for (; w > 0; src += 4, dst += step, w--) {
					dst[0] = src[b0];
					dst[1] = src[b1];
					dst[2] = src[b2];
					dst[3] = src[b3];
				}
			}	else {
				for (; w > 0; src += 4, dst += step, w--) {
					cc = _ipixel_fetch(32, src, 0);
					if (cc != mask) {
						dst[0] = src[b0];
						dst[1] = src[b1];
						dst[2] = src[b2];
						dst[3] = src[b3];
					}
				}
			}
		}
		else {
			if ((mode & IPIXEL_BLIT_MASK) == 0) {
				for (; w > 0; src += 4, dst += step, w--) {
					dst[0] = src[b0];
					dst[1] = src[b1];
					dst[2] = src[b2];
					dst[3] = src[b3];
					dst[ba] = ca;
				}
			}	else {
				for (; w > 0; src += 4, dst += step, w--) {
					cc = _ipixel_fetch(32, src, 0);
					if (cc != mask) {
						dst[0] = src[b0];
						dst[1] = src[b1];
						dst[2] = src[b2];
						dst[3] = src[b3];
						dst[ba] = ca;
					}
				}
			}
		}
	}
	else {
		if (sbpp == 32) {
			if ((mode & IPIXEL_BLIT_MASK) == 0) {
				for (; w > 0; src += 4, dst += step, w--) {
					dst[0] = src[b0];
					dst[1] = src[b1];
					dst[2] = src[b2];
				}
			}	else {
				for (; w > 0; src += 4, dst += step, w--) {
					cc = _ipixel_fetch(32, src, 0);
					if (cc != mask) {
						dst[0] = src[b0];
						dst[1] = src[b1];
						dst[2] = src[b2];
					}
				}
			}
		}
		else {
			if ((mode & IPIXEL_BLIT_MASK) == 0) {
				for (; w > 0; src += 3, dst += step, w--) {
					dst[0] = src[b0];
					dst[1] = src[b1];
					dst[2] = src[b2];
				}
			}	else {
				for (; w > 0; src += 3, dst += step, w--) {
					cc = _ipixel_fetch(24, src, 0);
					if (cc != mask) {
						dst[0] = src[b0];
						dst[1] = src[b1];
						dst[2] = src[b2];
					}
				}
			}
		}
	}
	return 0;
}

/* permute format */
int ipixel_fmt_permute(const iPixelFmt *dfmt, void *dbits, int dpitch,
	int dx, iPixelFmt *sfmt, void *sbits, int spitch, 
	int sx, int w, int h, uint mask, int mode)
{
	int sr, sg, sb, sa, dr, dg, db, da;
	int pos[6] = { 0, 0, 0, 0, 0, 0 };
	int dbytes, sbytes, step, dbpp, sbpp;
	iPixelFmtPermute permutor = null;
	
	dbytes = (dfmt.bpp + 7) / 8;
	sbytes = (sfmt.bpp + 7) / 8;

	if (sfmt.bpp != 24 && sfmt.bpp != 32)
		return -1;
	if (dfmt.bpp != 24 && dfmt.bpp != 32)
		return -2;
	if (dfmt.aloss != sfmt.aloss)
		return -3;

	sr = ipixel_fmt_position(sfmt.rshift);
	sg = ipixel_fmt_position(sfmt.gshift);
	sb = ipixel_fmt_position(sfmt.bshift);
	sa = ipixel_fmt_position(sfmt.ashift);
	dr = ipixel_fmt_position(dfmt.rshift);
	dg = ipixel_fmt_position(dfmt.gshift);
	db = ipixel_fmt_position(dfmt.bshift);
	da = ipixel_fmt_position(dfmt.ashift);

	if ((sr | sg | sb | sa | dr | dg | db | da) < 0) 
		return -4;

	pos[dr] = sr;
	pos[dg] = sg;
	pos[db] = sb;
	pos[da] = sa;
	pos[4] = da;
	pos[5] = (dfmt.amask == 0)? 0 : 255;

	if (mode & IPIXEL_FLIP_VFLIP) { 
		sbits = ( byte*)sbits + spitch * (h - 1); 
		spitch = -spitch; 
	}

	dbpp = dfmt.bpp;
	sbpp = sfmt.bpp;

	permutor = ipixel_fmt_get_permute(dfmt.bpp, sfmt.bpp, 0);

	for (; h > 0; h--) {
		const byte *src = (( byte*)sbits) + sx * sbytes;
		byte *dst = ((byte*)dbits) + dx * dbytes;
		if ((mode & IPIXEL_FLIP_HFLIP) == 0) {
			step = dbytes;
		}	else {
			dst += (w - 1) * dbytes;
			step = -dbytes;
		}
		permutor(dbpp, dst, w, step, sbpp, src, pos, mask, mode);
		sbits = ( byte*)sbits + spitch;
		dbits = (byte*)dbits + dpitch;
	}

	return 0;
}


/* ipixel_fmt_cvt: free format convert
 * you must provide a working memory pointer to mem. if mem eq null,
 * this function will do nothing but returns how many bytes needed in mem
 * dfmt   - dest pixel format structure
 * dbits  - dest bits
 * dpitch - dest pitch (row stride)
 * dx     - dest x offset
 * sfmt   - source pixel format structure
 * sbits  - source bits
 * spitch - source pitch (row stride)
 * sx     - source x offset
 * w      - width
 * h      - height
 * mask   - mask color (colorkey), no effect without IPIXEL_BLIT_MASK
 * mode   - IPIXEL_FLIP_HFLIP | IPIXEL_FLIP_VFLIP | IPIXEL_BLIT_MASK ..
 * mem    - work memory
 * for transparent converting, set mode with IPIXEL_BLIT_MASK, it will 
 * skip the colors equal to 'mask' parameter.
 */
int ipixel_fmt_cvt(const iPixelFmt *dfmt, void *dbits, int dpitch, 
	int dx, iPixelFmt *sfmt, void *sbits, int spitch, int sx,
	int w, int h, uint mask, int mode, iColorIndex *sindex,
	const iColorIndex *dindex, void *mem)
{
	if (mem == null) {
		return (int)(w * sizeof(uint));
	}

	if (dfmt.format != IPIX_FMT_PACKED && 
		sfmt.format != IPIX_FMT_PACKED) {
		return ipixel_convert(dfmt.format, dbits, dpitch, dx,
				sfmt.format, sbits, spitch, sx, w, h, mask, mode, 
				null, null, mem);
	}

	if (dfmt.bpp == sfmt.bpp && dfmt.type == sfmt.type) {
		if (dfmt.rmask == sfmt.rmask && dfmt.gmask == sfmt.gmask &&
			dfmt.bmask == sfmt.bmask && dfmt.amask == sfmt.amask) {
			ipixel_blit(sfmt.bpp, dbits, dpitch, dx, sbits, spitch, 
					sx, w, h, mask, mode);
			return 0;
		}
	}

	if (dindex == null) dindex = _ipixel_dst_index;
	if (sindex == null) sindex = _ipixel_src_index;

	if (sfmt.rloss + sfmt.gloss + sfmt.bloss == 0) {
		if (dfmt.rloss + dfmt.gloss + dfmt.bloss == 0) {
			int hh = ipixel_fmt_permute(dfmt, dbits, dpitch, dx,
					sfmt, sbits, spitch, sx, w, h, mask, mode);
			if (hh == 0) {
				return 0;
			}
		}
	}

	if ((mode & IPIXEL_BLIT_MASK) == 0) {
		iPixelFmtReader reader = ipixel_fmt_get_reader(sfmt.bpp, 0);
		iPixelFmtWriter writer = ipixel_fmt_get_writer(dfmt.bpp, 0);
		iFetchProc fetch = null;
		iStoreProc store = null;
		if (sfmt.format >= 0 && sfmt.format < IPIX_FMT_COUNT) 
			fetch = ipixel_get_fetch(sfmt.format, 0);
		if (dfmt.format >= 0 && dfmt.format < IPIX_FMT_COUNT)
			store = ipixel_get_store(dfmt.format, 0);
		if (reader != null && writer != null) {
			uint *card = (uint*)mem;
			int j;
			if (mode & IPIXEL_FLIP_VFLIP) { 
				sbits = ( byte*)sbits + spitch * (h - 1); 
				spitch = -spitch; 
			} 
			for (j = 0; j < h; j++) {
				if (fetch) {
					fetch(sbits, sx, w, card, sindex);
				}	else {
					reader(sfmt, sbits, sx, w, card);
				}
				if (mode & IPIXEL_FLIP_HFLIP) {
					ipixel_card_reverse(card, w);
				}
				if (store) {
					store(dbits, card, dx, w, dindex);
				}	else {
					writer(dfmt, dbits, dx, w, card);
				}
				sbits = ( byte*)sbits + spitch;
				dbits = (byte*)dbits + dpitch;
			}
			return 0;
		}
	}
	return ipixel_fmt_slow(dfmt, dbits, dpitch, dx, sfmt, sbits, spitch,
			sx, w, h, mask, mode, dindex, sindex);
}


/**********************************************************************
 * CLIPPING
 **********************************************************************/

/*
 * ipixel_clip - clip the rectangle from the src clip and dst clip then
 * caculate a new rectangle which is shared between dst and src cliprect:
 * clipdst  - dest clip array (left, top, right, bottom)
 * clipsrc  - source clip array (left, top, right, bottom)
 * (x, y)   - dest position
 * rectsrc  - source rect
 * mode     - check IPIXEL_FLIP_HFLIP or IPIXEL_FLIP_VFLIP
 * return zero for successful, return non-zero if there is no shared part
 */
int ipixel_clip(const int *clipdst, int *clipsrc, int *x, int *y,
    int *rectsrc, int mode)
{
    int dcl = clipdst[0];       /* dest clip: left     */
    int dct = clipdst[1];       /* dest clip: top      */
    int dcr = clipdst[2];       /* dest clip: right    */
    int dcb = clipdst[3];       /* dest clip: bottom   */
    int scl = clipsrc[0];       /* source clip: left   */
    int sct = clipsrc[1];       /* source clip: top    */
    int scr = clipsrc[2];       /* source clip: right  */
    int scb = clipsrc[3];       /* source clip: bottom */
    int dx = *x;                /* dest x position     */
    int dy = *y;                /* dest y position     */
    int sl = rectsrc[0];        /* source rectangle: left   */
    int st = rectsrc[1];        /* source rectangle: top    */
    int sr = rectsrc[2];        /* source rectangle: right  */
    int sb = rectsrc[3];        /* source rectangle: bottom */
    int hflip, vflip;
    int w, h, d;
    
    hflip = (mode & IPIXEL_FLIP_HFLIP)? 1 : 0;
    vflip = (mode & IPIXEL_FLIP_VFLIP)? 1 : 0;

    if (dcr <= dcl || dcb <= dct || scr <= scl || scb <= sct) 
        return -1;

    if (sr <= scl || sb <= sct || sl >= scr || st >= scb) 
        return -2;

    /* check dest clip: left */
    if (dx < dcl) {
        d = dcl - dx;
        dx = dcl;
        if (!hflip) sl += d;
        else sr -= d;
    }

    /* check dest clip: top */
    if (dy < dct) {
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
    if (dx + w > dcr) {
        d = dx + w - dcr;
        if (!hflip) sr -= d;
        else sl += d;
    }

    /* check dest clip: bottom */
    if (dy + h > dcb) {
        d = dy + h - dcb;
        if (!vflip) sb -= d;
        else st += d;
    }

    if (sl >= sr || st >= sb) 
        return -4;

    /* check source clip: left */
    if (sl < scl) {
        d = scl - sl;
        sl = scl;
        if (!hflip) dx += d;
    }

    /* check source clip: top */
    if (st < sct) {
        d = sct - st;
        st = sct;
        if (!vflip) dy += d;
    }

    if (sl >= sr || st >= sb) 
        return -5;

    /* check source clip: right */
    if (sr > scr) {
        d = sr - scr;
        sr = scr;
        if (hflip) dx += d;
    }

    /* check source clip: bottom */
    if (sb > scb) {
        d = sb - scb;
        sb = scb;
        if (vflip) dy += d;
    }

    if (sl >= sr || st >= sb) 
        return -6;

    *x = dx;
    *y = dy;

    rectsrc[0] = sl;
    rectsrc[1] = st;
    rectsrc[2] = sr;
    rectsrc[3] = sb;

    return 0;
}




/**********************************************************************
 * COMPOSITE
 **********************************************************************/
public static iPixelComposite ipixel_composite_table[40][2];
public static int ipixel_composite_inited = 0;

public static void ipixel_comp_src(uint *dst, uint *src, int w)
{
	ipixel_memcpy(dst, src, w * sizeof(uint));
}

public static void ipixel_comp_dst(uint *dst, uint *src, int w)
{
}

public static void ipixel_comp_clear(uint *dst, uint *src, int w)
{
	memset(dst, 0, sizeof(uint) * w);
}

public static void ipixel_comp_blend(uint *dst, uint *src, int w)
{
	uint r1, g1, b1, a1, r2, g2, b2, a2;
	for (; w > 0; dst++, src++, w--) {
		a1 = (( byte*)src)[_ipixel_card_alpha];
		if (a1 == 0) continue;
		else if (a1 == 255) dst[0] = src[0];
		else {
			_ipixel_load_card(src, r1, g1, b1, a1);
			_ipixel_load_card(dst, r2, g2, b2, a2);
			IBLEND_NORMAL_FAST(r1, g1, b1, a1, r2, g2, b2, a2);
			dst[0] = IRGBA_TO_A8R8G8B8(r2, g2, b2, a2);
		}
	}
}

public static void ipixel_comp_add(uint *dst, uint *src, int w)
{
	uint r1, g1, b1, a1, r2, g2, b2, a2;
	for (; w > 0; dst++, src++, w--) {
		_ipixel_load_card(src, r1, g1, b1, a1);
		_ipixel_load_card(dst, r2, g2, b2, a2);
		r2 += r1, g2 += g1, b2 += b1, a2 += a1; \
		r2 = ICLIP_256(r2); \
		g2 = ICLIP_256(g2); \
		b2 = ICLIP_256(b2); \
		a2 = ICLIP_256(a2); \
		dst[0] = IRGBA_TO_A8R8G8B8(r2, g2, b2, a2);
	}
}

public static void ipixel_comp_sub(uint *dst, uint *src, int w)
{
	int r1, g1, b1, a1, r2, g2, b2, a2;
	for (; w > 0; dst++, src++, w--) {
		_ipixel_load_card(src, r1, g1, b1, a1);
		_ipixel_load_card(dst, r2, g2, b2, a2);
		r2 -= r1, g2 -= g1, b2 -= b1, a2 -= a1;
		r2 = ICLIP_ZERO(r2);
		g2 = ICLIP_ZERO(g2);
		b2 = ICLIP_ZERO(b2);
		a2 = ICLIP_ZERO(a2);
		dst[0] = IRGBA_TO_A8R8G8B8(r2, g2, b2, a2);
	}
}

public static void ipixel_comp_sub_inv(uint *dst, uint *src, int w)
{
	int r1, g1, b1, a1, r2, g2, b2, a2;
	for (; w > 0; dst++, src++, w--) {
		_ipixel_load_card(src, r1, g1, b1, a1);
		_ipixel_load_card(dst, r2, g2, b2, a2);
		r2 = r1 - r2;
		g2 = g1 - g2;
		b2 = b1 - b2;
		a2 = a1 - a2;
		r2 = ICLIP_ZERO(r2);
		g2 = ICLIP_ZERO(g2);
		b2 = ICLIP_ZERO(b2);
		a2 = ICLIP_ZERO(a2);
		dst[0] = IRGBA_TO_A8R8G8B8(r2, g2, b2, a2);
	}
}

public static void ipixel_comp_preblend(uint *dst, uint *src, int w)
{
	uint c1, c2, a;
	for (; w > 0; dst++, src++, w--) {
		c1 = src[0];
		a = src[0] >> 24;
		if (a == 255) {
			dst[0] = src[0];
		}
		else if (a > 0) {
			c2 = dst[0];
			IBLEND_PARGB(c2, c1);
			dst[0] = c2;
		}
	}
}

public static void ipixel_comp_allanon(uint *dst, uint *src, int w)
{
	uint c1, c2, c3, c4;
	for (; w > 0; dst++, src++, w--) {
		if ((src[0] >> 24) != 0) {
			c1 = src[0] & 0x00ff00ff;
			c2 = (src[0] >> 8) & 0x00ff00ff;
			c3 = dst[0] & 0x00ff00ff;
			c4 = (dst[0] >> 8) & 0x00ff00ff;
			c1 = (c1 + c3) >> 1;
			c2 = (c2 + c4) >> 1;
			dst[0] = (c1 & 0x00ff00ff) | ((c2 & 0x00ff00ff) << 8);
		}
	}
}

public static void ipixel_comp_tint(uint *dst, uint *src, int w)
{
	uint r1, g1, b1, a1, r2, g2, b2, a2;
	for (; w > 0; dst++, src++, w--) {
		if ((src[0] >> 24) != 0) {
			_ipixel_load_card(src, r1, g1, b1, a1);
			_ipixel_load_card(dst, r2, g2, b2, a2);
			r1 = r1 * r2;
			g1 = g1 * g2;
			b1 = b1 * b2;
			r1 = _ipixel_fast_div_255(r1);
			g1 = _ipixel_fast_div_255(g1);
			b1 = _ipixel_fast_div_255(b1);
			dst[0] = IRGBA_TO_A8R8G8B8(r1, g1, b1, a2);
		}
	}
	r1 = a2 + a1;
}

public static void ipixel_comp_diff(uint *dst, uint *src, int w)
{
	int r1, g1, b1, a1, r2, g2, b2, a2;
	for (; w > 0; dst++, src++, w--) {
		if ((src[0] >> 24) != 0) {
			_ipixel_load_card(src, r1, g1, b1, a1);
			_ipixel_load_card(dst, r2, g2, b2, a2);
			r1 -= r2;
			g1 -= g2;
			b1 -= b2;
			r2 = (r1 < 0)? -r1 : r1;
			g2 = (g1 < 0)? -g1 : g1;
			b2 = (b1 < 0)? -b1 : b1;
			if (a1 > a2) a2 = a1;
			dst[0] = IRGBA_TO_A8R8G8B8(r2, g2, b2, a2);
		}
	}
}

public static void ipixel_comp_darken(uint *dst, uint *src, int w)
{
	uint r1, g1, b1, a1, r2, g2, b2, a2;
	for (; w > 0; dst++, src++, w--) {
		if ((src[0] >> 24) != 0) {
			_ipixel_load_card(src, r1, g1, b1, a1);
			_ipixel_load_card(dst, r2, g2, b2, a2);
			if (a1 < a2) a2 = a1;
			if (r1 < r2) r2 = r1;
			if (g1 < g2) g2 = g1;
			if (b1 < b2) b2 = b1;
			dst[0] = IRGBA_TO_A8R8G8B8(r2, g2, b2, a2);
		}
	}
}

public static void ipixel_comp_lighten(uint *dst, uint *src, int w)
{
	uint r1, g1, b1, a1, r2, g2, b2, a2;
	for (; w > 0; dst++, src++, w--) {
		if ((src[0] >> 24) != 0) {
			_ipixel_load_card(src, r1, g1, b1, a1);
			_ipixel_load_card(dst, r2, g2, b2, a2);
			if (a1 > a2) a2 = a1;
			if (r1 > r2) r2 = r1;
			if (g1 > g2) g2 = g1;
			if (b1 > b2) b2 = b1;
			dst[0] = IRGBA_TO_A8R8G8B8(r2, g2, b2, a2);
		}
	}
}

public static void ipixel_comp_screen(uint *dst, uint *src, int w)
{
	uint r1, g1, b1, a1, r2, g2, b2, a2, res1, res2;
	for (; w > 0; dst++, src++, w--) {
		if ((src[0] >> 24) != 0) {
			_ipixel_load_card(src, r1, g1, b1, a1);
			_ipixel_load_card(dst, r2, g2, b2, a2);

			#define IPIXEL_SCREEN_VALUE(b, t) do { \
				res1 = 0xFF - b; res2 = 0xff - t; \
				res1 = 0xff - ((res1 * res2) >> 8); \
				b = res1; } while (0)

			IPIXEL_SCREEN_VALUE(r2, r1);
			IPIXEL_SCREEN_VALUE(g2, g1);
			IPIXEL_SCREEN_VALUE(b2, b1);

			#undef IPIXEL_SCREEN_VALUE

			if (a1 > a2) a2 = a1;
			dst[0] = IRGBA_TO_A8R8G8B8(r2, g2, b2, a2);
		}
	}
}

public static void ipixel_comp_overlay(uint *dst, uint *src, int w)
{
	uint r1, g1, b1, a1, r2, g2, b2, a2, tmp_screen, tmp_mult, res;
	for (; w > 0; dst++, src++, w--) {
		if ((src[0] >> 24) != 0) {
			_ipixel_load_card(src, r1, g1, b1, a1);
			_ipixel_load_card(dst, r2, g2, b2, a2);

		#define IPIXEL_OVERLAY_VALUE(b, t) do { \
				tmp_screen = 0xff - (((0xff - (int)b) * (0xff - t)) >> 8); \
				tmp_mult   = (b * t) >> 8; \
				res = (b * tmp_screen + (0xff - b) * tmp_mult) >> 8; \
				b = res; \
			}	while (0)

			IPIXEL_OVERLAY_VALUE(r2, r1);
			IPIXEL_OVERLAY_VALUE(g2, g1);
			IPIXEL_OVERLAY_VALUE(b2, b1);

		#undef IPIXEL_OVERLAY_VALUE

			if (a1 > a2) a2 = a1;
			dst[0] = IRGBA_TO_A8R8G8B8(r2, g2, b2, a2);
		}
	}
}

/* initialize compositors */
public static void ipixel_composite_init(void)
{
	if (ipixel_composite_inited) return;
	#define ipixel_composite_install(opname, name) do { \
		ipixel_composite_table[IPIXEL_OP_##opname][0] = ipixel_comp_##name; \
		ipixel_composite_table[IPIXEL_OP_##opname][1] = ipixel_comp_##name; \
	}	while (0) 

	ipixel_composite_install(SRC, src);
	ipixel_composite_install(DST, dst);
	ipixel_composite_install(CLEAR, clear);
	ipixel_composite_install(BLEND, blend);
	ipixel_composite_install(ADD, add);
	ipixel_composite_install(SUB, sub);
	ipixel_composite_install(SUB_INV, sub_inv);

	ipixel_composite_install(XOR, xor);
	ipixel_composite_install(PLUS, plus);
	ipixel_composite_install(SRC_ATOP, src_atop);
	ipixel_composite_install(SRC_IN, src_in);
	ipixel_composite_install(SRC_OUT, src_out);
	ipixel_composite_install(SRC_OVER, src_over);
	ipixel_composite_install(DST_ATOP, dst_atop);
	ipixel_composite_install(DST_IN, dst_in);
	ipixel_composite_install(DST_OUT, dst_out);
	ipixel_composite_install(DST_OVER, dst_over);

	ipixel_composite_install(PREMUL_XOR, pre_xor);
	ipixel_composite_install(PREMUL_PLUS, pre_plus);
	ipixel_composite_install(PREMUL_SRC_ATOP, pre_src_atop);
	ipixel_composite_install(PREMUL_SRC_IN, pre_src_in);
	ipixel_composite_install(PREMUL_SRC_OUT, pre_src_out);
	ipixel_composite_install(PREMUL_SRC_OVER, pre_src_over);
	ipixel_composite_install(PREMUL_DST_ATOP, pre_dst_atop);
	ipixel_composite_install(PREMUL_DST_IN, pre_dst_in);
	ipixel_composite_install(PREMUL_DST_OUT, pre_dst_out);
	ipixel_composite_install(PREMUL_DST_OVER, pre_dst_over);

	ipixel_composite_install(PREMUL_BLEND, preblend);
	ipixel_composite_install(ALLANON, allanon);
	ipixel_composite_install(TINT, tint);
	ipixel_composite_install(DIFF, diff);
	ipixel_composite_install(DARKEN, darken);
	ipixel_composite_install(LIGHTEN, lighten);
	ipixel_composite_install(SCREEN, screen);
	ipixel_composite_install(OVERLAY, overlay);

	#undef ipixel_composite_install

	ipixel_init_lut();
	ipixel_composite_inited = 1;
}


/* get compositor */
iPixelComposite ipixel_composite_get(int op, int isdefault)
{
	if (ipixel_composite_inited == 0) ipixel_composite_init();
	if (op < 0 || op > IPIXEL_OP_OVERLAY) return null;
	return ipixel_composite_table[op][isdefault ? 1 : 0];
}

/* set compositor */
void ipixel_composite_set(int op, iPixelComposite composite)
{
	if (ipixel_composite_inited == 0) ipixel_composite_init();
	if (op < 0 || op > IPIXEL_OP_OVERLAY) return;
	if (composite == null) composite = ipixel_composite_table[op][1];
	ipixel_composite_table[op][0] = composite;
}

/* composite operator names */
byte* ipixel_composite_opnames[] = {
	"IPIXEL_OP_SRC",
	"IPIXEL_OP_DST",
	"IPIXEL_OP_CLEAR",
	"IPIXEL_OP_BLEND",
	"IPIXEL_OP_ADD",
	"IPIXEL_OP_SUB",
	"IPIXEL_OP_SUB_INV",
	"IPIXEL_OP_XOR",
	"IPIXEL_OP_PLUS",
	"IPIXEL_OP_SRC_ATOP",
	"IPIXEL_OP_SRC_IN",
	"IPIXEL_OP_SRC_OUT",
	"IPIXEL_OP_SRC_OVER",
	"IPIXEL_OP_DST_ATOP",
	"IPIXEL_OP_DST_IN",
	"IPIXEL_OP_DST_OUT",
	"IPIXEL_OP_DST_OVER",
	"IPIXEL_OP_PREMUL_XOR",
	"IPIXEL_OP_PREMUL_PLUS",
	"IPIXEL_OP_PREMUL_SRC_OVER",
	"IPIXEL_OP_PREMUL_SRC_IN",
	"IPIXEL_OP_PREMUL_SRC_OUT",
	"IPIXEL_OP_PREMUL_SRC_ATOP",
	"IPIXEL_OP_PREMUL_DST_OVER",
	"IPIXEL_OP_PREMUL_DST_IN",
	"IPIXEL_OP_PREMUL_DST_OUT",
	"IPIXEL_OP_PREMUL_DST_ATOP",
	"IPIXEL_OP_PREMUL_BLEND",
	"IPIXEL_OP_ALLANON",
	"IPIXEL_OP_TINT",
	"IPIXEL_OP_DIFF",
	"IPIXEL_OP_DARKEN",
	"IPIXEL_OP_LIGHTEN",
	"IPIXEL_OP_SCREEN",
	"IPIXEL_OP_OVERLAY",
};

/* get composite operator names */
byte*ipixel_composite_opname(int op)
{
	if (op < 0 || op > IPIXEL_OP_OVERLAY) return "UNKNOW";
	return ipixel_composite_opnames[op];
}


/**********************************************************************
 * Palette & Others
 **********************************************************************/

/* fetch card from IPIX_FMT_C8 */
void ipixel_palette_fetch(byte*src, int w, uint *card,
	const IRGB *palette)
{
	uint r, g, b;
	for (; w > 0; src++, card++, w--) {
		r = palette[*src].r;
		g = palette[*src].g;
		b = palette[*src].b;
		card[0] = IRGBA_TO_A8R8G8B8(r, g, b, 255);
	}
}

/* store card into IPIX_FMT_C8 */
void ipixel_palette_store(byte *dst, int w, uint *card,
	const IRGB *palette, int palsize)
{
	uint r, g, b;
	for (; w > 0; dst++, card++, w--) {
		ISPLIT_RGB(card[0], r, g, b);
		dst[0] = ipixel_palette_fit(palette, r, g, b, palsize);
	}
}

/* batch draw dots */
int ipixel_set_dots(int bpp, void *bits, int pitch, int w, int h,
	uint color, short *xylist, int count, int *clip)
{
	byte *image = (byte*)bits;
	int cl = 0, cr = w, ct = 0, cb = h, i;
	int pixelbyte = (bpp + 7) / 8;
	if (clip) {
		cl = clip[0];
		ct = clip[1];
		cr = clip[2];
		cb = clip[3];
		if (cl < 0) cl = 0;
		if (ct < 0) ct = 0;
		if (cr > w) cr = w;
		if (cb > h) cb = h;
	}
	cr--;    /* use range [cl, cr] */
	cb--;
	switch (pixelbyte) {
	case 1:
		for (i = count; i > 0; xylist += 2, i--) {
			int x = xylist[0];
			int y = xylist[1];
			if (((x - cl) | (y - ct) | (cr - x) | (cb - y)) >= 0) {
				byte *line = image + pitch * y;
				_ipixel_store(8, line, x, color);
			}
		}
		break;
	case 2:
		for (i = count; i > 0; xylist += 2, i--) {
			int x = xylist[0];
			int y = xylist[1];
			if (((x - cl) | (y - ct) | (cr - x) | (cb - y)) >= 0) {
				byte *line = image + pitch * y;
				_ipixel_store(16, line, x, color);
			}
		}
		break;
	case 3:
		for (i = count; i > 0; xylist += 2, i--) {
			int x = xylist[0];
			int y = xylist[1];
			if (((x - cl) | (y - ct) | (cr - x) | (cb - y)) >= 0) {
				byte *line = image + pitch * y;
				_ipixel_store(24, line, x, color);
			}
		}
		break;
	case 4:
		for (i = count; i > 0; xylist += 2, i--) {
			int x = xylist[0];
			int y = xylist[1];
			if (((x - cl) | (y - ct) | (cr - x) | (cb - y)) >= 0) {
				byte *line = image + pitch * y;
				_ipixel_store(24, line, x, color);
			}
		}
		break;
	}
	return 0;
}


/* vim: set ts=4 sw=4 tw=0 noet :*/

