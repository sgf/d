namespace d2;

public unsafe partial struct IBITMAP
{

    //public public static uint[] _cpu_feature = [0, 0, 0, 0];
    //public public static uint _cpu_cachesize;
    //public public static int _cpu_level = -1;
    //public public static int _cpu_device = -1;
    //public public static int _cpu_vendor = X86_VENDOR_UNKNOWN;
    //public public static byte _cpu_vendor_name[14] = "unknow";

    //public public static int _cpu_cache_l1i = 0;
    //public public static int _cpu_cache_l1d = 0;
    //public public static int _cpu_cache_l2 = 0;

    //---------------------------------------------------------------------
    // _x86_choose_blitter
    //---------------------------------------------------------------------
    public void _x86_choose_blitter()
    {
        if (Sse2.IsSupported)
        {
            IBITMAP.ibitmap_blitn = &iblit_mix_sse;//sse版本没实现
            IBITMAP.ibitmap_blitm = &iblit_mask_sse;
            //iblit_sse
        }

    } 

//---------------------------------------------------------------------
// iblit_sse - mmx normal blitter
// this routine is design to support the platform with sse feature,
// which has more than 256KB L2 cache
//---------------------------------------------------------------------
int iblit_sse(byte* dst, int pitch1, byte* src, int pitch2,
    int w, int h, int pixelbyte, int linesize)
{

        return 0;
    }

//---------------------------------------------------------------------
// iblit_mask_sse - sse mask blitter 
// this routine is designed to support sse mask blit
//---------------------------------------------------------------------
public static int iblit_mask_sse(byte* dst, int pitch1, byte* src, int pitch2,
    int w, int h, int pixelbyte, int linesize, uint ckey)
{
        return 0;
    
}

}