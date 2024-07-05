using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d2;

public static class Ex
{

    //public static bool Has<TEnum>(this TEnum @tenum,TEnum @tSingleEnum) where TEnum:Enum
    //{
    //    return @tenum.HasFlag(@tSingleEnum);
    //}


}

public static class g
{

    public unsafe static void memcpy(void* dst, void* src, int len)=> Buffer.MemoryCopy(src, dst, len, len);

    public unsafe static void memset(void* dst, byte val, int len) => Unsafe.InitBlock(dst, val, (uint)len);

    //Buffer.SetByte()
    public unsafe static void memset(void* dst, byte val, uint len) => Unsafe.InitBlock(dst, val, len);

}