using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static d2.IImg;

namespace d2;

public enum ResizeMode
{
    Fit,
    Bleed,
    Stretch
}

public partial interface IImg<TColor> : IEnumerable<TColor >, IDrawable, IDisposable where TColor : unmanaged, IColor
{
    /// <summary> Gets the color format used in this image. </summary>
    CorFmt ColorFormat { get; }

    /// <summary> Gets the width of this image. </summary>
    int Width { get; }

    /// <summary> Gets the height of this image. </summary>
    int Height { get; }

    /// <summary> Gets the size in bytes of this image. </summary>
    int SizeInBytes { get; }

    /// <summary>
    /// Gets the color at the specified location.
    /// </summary>
    /// <param name="x">The X-location to read.</param>
    /// <param name="y">The Y-location to read.</param>
    /// <returns>The color at the specified location.</returns>
    IColor this[int x, int y] { get; }

    /// <summary>
    /// Gets an image representing the specified location.
    /// </summary>
    /// <param name="x">The X-location of the image.</param>
    /// <param name="y">The Y-location of the image.</param>
    /// <param name="width">The width of the sub-image.</param>
    /// <param name="height"></param>
    /// <returns></returns>
    IImg this[int x, int y, int width, int height] { get; }

    /// <summary> Gets a list of all rows of this image. </summary>
    IImageRows Rows { get; }

    /// <summary> Gets a list of all columns of this image. </summary>
    IImageColumns Columns { get; }


    ///// <summary>
    ///// Gets an <see cref="RefImage{TColor}"/> representing this <see cref="IImg"/>.
    ///// </summary>
    ///// <typeparam name="TColor">The color-type of the iamge.</typeparam>
    ///// <returns>The <inheritdoc cref="RefImage{TColor}"/>.</returns>
    //RefImage<TColor> AsRefImage<TColor> () where TColor : struct, IColor;

    /// <summary>
    /// Copies the contents of this <see cref="IImg"/> into a destination <see cref="Span{T}"/> instance.
    /// </summary>
    /// <param name="destination">The destination <see cref="Span{T}"/> instance.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="destination"/> is shorter than the source <see cref="IImg"/> instance.
    /// </exception>
    void CopyTo (in Span<byte> destination);

    /// <summary> Allocates a new array and copies this <see cref="IImg"/> into it. </summary>
    /// <returns>The new array containing the data of this <see cref="IImg"/>.</returns>
    byte[] ToArray ();


    IImg Downsize (float maxWidthOrHeight, bool disposeOriginal = false);
    IImg Downsize (float maxWidth, float maxHeight, bool disposeOriginal = false);
    IImg Resize (float width, float height, ResizeMode resizeMode = ResizeMode.Fit, bool disposeOriginal = false);
    void Save (Stream stream, ImgFmt format = ImgFmt.Png, float quality = 1);
    Task SaveAsync (Stream stream, ImgFmt format = ImgFmt.Png, float quality = 1);
    IImg ToPlatformImage ();

    public SzF Size => new(this.Width, this.Height);
}

public enum CorFmt : int
{
    /// <summary> BGRA8888 </summary>
    BGRA = 4 << 8 | 1,
    /// <summary> ABGR8888 </summary>
    ABGR = 4 << 8 | 2,
    /// <summary> RGBA8888 </summary>
    RGBA = 4 << 8 | 3,
    /// <summary> ARGB8888 </summary>
    ARGB = 4 << 8 | 4,
    /// <summary> BGR888 </summary>
    BGR = 3 << 8 | 5,
    /// <summary> RGB888 </summary>
    RGB = 3 << 8 | 6,
    /// <summary> RGB565 </summary>
    RGB565 = 2 << 8 | 7,
    /// <summary> RGBA0008 灰度图? </summary>
    RGBA0008 = 1 << 8 | 7,
}

public static class CorFmtExtension
{

    /// <summary> 每像字节大小 </summary>
    /// <param name="_this"></param>
    /// <returns></returns>
    public static byte ByteSize (this CorFmt _this)
    {
        return (byte)((int)_this >> 8);
    }

    /// <summary> 每像素位大小 </summary>
    /// <param name="_this"></param>
    /// <returns></returns>
    public unsafe static byte BBP (this CorFmt _this)
    {
        return (byte)(((int)_this >> 8) * sizeof (byte));
    }

    //public static byte Id (this CorFmt _this)
    //{
    //    return (byte)((int)_this & 0xf);
    //}

    //public static implicit extension CorFmtExtension for CorFmt
    //{
    //    public int Age => DateTime.Now.Year - this.Birthday.Year;
    //}

}

