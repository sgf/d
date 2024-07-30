using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d2;

public interface ICanvas
{
    public ICanvas Clear(uint color);
    //private static readonly SKColorFilter disabled_matrix = SKColorFilter.CreateColorMatrix (new float[]
    //        {
    //                0.21f, 0.72f, 0.07f, 0, 0,
    //                0.21f, 0.72f, 0.07f, 0, 0,
    //                0.21f, 0.72f, 0.07f, 0, 0,
    //                0,     0,     0,     1, 0
    //        });

    public ICanvas DrawBorder(RtF bounds, BorderBoxStyle style);

    public ICanvas Clip(RtF Rect);

    /// <summary> border and background(Color/Image+layout) </summary>
    /// <param name="bounds"></param>
    /// <param name="style"></param>
    /// <param name="border"></param>
    /// <returns></returns>
    public ICanvas DrawBG(RtF bounds, BGStyle style, BorderBoxStyle border);
    public ICanvas DrawBitmap(IImg img, RtF rect, bool disabled = false);
    public ICanvas DrawBitmap(IImg img, float x, float y, bool disabled = false);
    public ICanvas DrawCircle(int x, int y, int radius, uint color, int strokeWidth = 1);
    public ICanvas DrawFocusRect(int x, int y, int width, int height, int inset = 0);
    public ICanvas DrawFocusRect(RtF Rect, int inset = 0);
    public ICanvas DrawLine(float x1, float y1, float x2, float y2, uint color, int thickness = 1);
    public ICanvas DrawRect(int x, int y, int width, int height, uint color, int strokeWidth = 1);
    public ICanvas DrawRect(RtF Rect, uint color, int strokeWidth = 1);
    public ICanvas DrawRoundedRect(int x, int y, int width, int height, uint color, int rx = 3, int ry = 3, int strokeWidth = 1);
    public ICanvas FillCircle(int x, int y, int radius, uint color);
    public ICanvas FillRect(RtF Rect, uint color);
    public ICanvas FillRect(int x, int y, int width, int height, uint color);
    public ICanvas FillRoundedRect(int x, int y, int width, int height, uint color, int rx = 3, int ry = 3, int strokeWidth = 1);

}


//public SzF GetSize<TColor> (this IImg<TColor> img) where  TColor:unmanaged, IColor
//    => new SzF (img.Width, img.Height);
//public Bitmap ToBitmap (this SKImage skiaImage);
//public Bitmap ToBitmap (this SKBitmap skiaBitmap);

//public Rect ToRect (this SKRect rect) => new Rect ((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height);
//public Size ToSize (this SKSize size) => new Size ((int)size.Width, (int)size.Height);

//public RtF ToSKRect(this RtF rect) => new RtF(rect.X, rect.Y, rect.Right, rect.Bottom);
//public SzF ToSKSize(this SzF size) => new SzF(size.Width, size.Height);