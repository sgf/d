using ColorHelper;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d2;

public class DefaultCanvas : ICanvas
{
    PixelFarm.Drawing.IPainter p;
    public ICanvas Clear(uint color)
    {
        throw new NotImplementedException();
    }

    public ICanvas Clip(RtF rect)
    {
        p.SetClipBox((int)rect.X, (int)rect.Y, (int)(rect.X + rect.Width), (int)(rect.Y + rect.Height));
        return this;
    }

    public ICanvas DrawBG(RtF bounds, BGStyle style, BorderBoxStyle border)
    {
        if (border.HasRadius)
        {
            p.Clear(Color.Transparent.UIntValue);
            //this.FillRoundedRect(border.left, 0, (int)(bounds.Width - border.width), (int)(bounds.Height - border.width), style.color, radius, radius, border.width);
            return this;
        }
        p.Clear(style.color);
        return this;
    }

    public ICanvas DrawBitmap(IImg  img, RtF rect, bool disabled = false)
    {
        //p.DrawImage(img, rect.X, rect.Y);// disabled
        return this;
    }

    public ICanvas DrawBitmap(IImg img, float x, float y, bool disabled = false)
    {
        //p.DrawImage(img, x, y);// disabled
        return this;
    }

    public ICanvas DrawBorder(RtF bounds, BorderBoxStyle border)
    {
        //if (border.radius > 0)//尝试直接判断所有子radius数组,任意一个有大的就绘制边框? 但是绘制逻辑也要被拆分成4份.
        //{
        //    this.DrawRoundedRect(0, 0, bounds.Width - border.width, bounds.Height - border.width,
        //        border.color, border.radius, border.radius, border.width);
        //    return this;
        //}

        // Left Border
        var left = border.left;
        var right = border.right;
        var top = border.top;
        var bottom = border.bottom;
        if (left.width > 0)
        {
            var left_offset = left.width / 2f;
            this.DrawLine(left_offset, 0, left_offset, bounds.Height, left.color, (int)left.width);
        }

        // Right Border
        if (right.width > 0)
        {
            var right_offset = right.width / 2f;
            this.DrawLine(bounds.Width - right_offset, 0, bounds.Width - right_offset, bounds.Height, right.color, (int)right.width);
        }

        // Top Border
        if (top.width > 0)
        {
            var top_offset = top.width / 2f;
            this.DrawLine(0, top_offset, bounds.Width, top_offset, top.color, (int)top.width);
        }

        // Bottom Border
        if (bottom.width > 0)
        {
            var bottom_offset = bottom.width / 2f;
            this.DrawLine(0, bounds.Height - bottom_offset, bounds.Width, bounds.Height - bottom_offset, bottom.color, (int)bottom.width);
        }

        return this;
    }

    public ICanvas DrawCircle(int x, int y, int radius, uint color, int strokeWidth = 1)
    {
        p.StrokeColor = color;
        p.StrokeWidth = strokeWidth;
        p.DrawCircle(x, y, radius);
        return this;
    }

    public ICanvas DrawFocusRect(int x, int y, int width, int height, int inset = 0)
    {
        p.DrawRect(x, y, width, height);
        return this;
    }

    public ICanvas DrawFocusRect(RtF r, int inset = 0)
    {
        p.DrawRect(r.X, r.Y, r.Width, r.Height);
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="color"></param>
    /// <param name="thickness">粗细</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public ICanvas DrawLine(float x1, float y1, float x2, float y2, uint color, int thickness = 1)
    {
        p.StrokeColor = color;
        p.StrokeWidth = thickness;
        p.DrawLine(x1, y1, x2, y2);
        return this;
    }

    public ICanvas DrawRect(int x, int y, int width, int height, uint color, int strokeWidth = 1)
    {
        p.StrokeColor = color;
        p.StrokeWidth = strokeWidth;
        p.DrawRect(x, y, width, height);
        return this;
    }

    public ICanvas DrawRect(RtF r, uint color, int strokeWidth = 1)
    {
        p.StrokeColor = color;
        p.StrokeWidth = strokeWidth;
        p.DrawRect(r.X, r.Y, r.Width, r.Height);
        return this;
    }

    public ICanvas DrawRoundedRect(int x, int y, int width, int height, uint color, int rx = 3, int ry = 3, int strokeWidth = 1)
    {
        //p.StrokeColor = color;
        p.StrokeWidth = strokeWidth;
        var fill = new RoundedRect(x, y + height, x + width, y, rx);// BackgroundRadius.NW
        fill.SetRadius(rx, ry);
        //using var ctx = Tools.BorrowRoundedRect(out var roundRect);
        using var vtx1 = Tools.BorrowVxs(out var v1);
        var rectOutline = fill.MakeVxs(v1).CreateTrim();
        p.Draw(rectOutline, color);
        //p.Render(rectOutline, color);
        return this;
    }

    public ICanvas FillCircle(int x, int y, int radius, uint color)
    {
        //p.StrokeWidth = thickness;
        p.StrokeColor = color;
        p.RenderQuality = RenderQuality.HighQuality;
        p.FillCircle(x, y, radius, color);
        return this;
    }

    public ICanvas FillRect(RtF Rect, uint color)
    {
        p.FillRect(Rect.Left, Rect.Top, Rect.Width, Rect.Height, color);
        return this;
    }

    public ICanvas FillRect(int x, int y, int width, int height, uint color)
    {
        p.FillRect(x, y, x + width, y + height, color);
        return this;
    }

    public ICanvas FillRoundedRect(int x, int y, int width, int height, uint color, int rx = 3, int ry = 3, int strokeWidth = 1)
    {
        //using var paint = new SKPaint
        //{
        //    Color = color,
        //    IsStroke = false,
        //    IsAntialias = true,
        //    StrokeWidth = strokeWidth
        //};
        //var r = new SKRoundRect();
        //canvas.DrawRoundRect(x + .5f, y + .5f, width, height, rx, ry, paint);
        return this;
    }

    public Bitmap ToBitmap( IImg skiaImage)
    {
        throw new NotImplementedException();
    }

    //public Bitmap ToBitmap(this SKBitmap skiaBitmap)
    //{
    //    throw new NotImplementedException();
    //}
}



/// <summary>
/// 
/// </summary>
/// <param name="Color">颜色</param>
/// <param name="IsStroke">true=轮廓描边,false=fill填充</param>
/// <param name="StrokeWidth">轮廓描边粗细</param>
/// <param name="IsAntialias">true=抗锯齿,false=快速模式</param>
public readonly struct DrawCfg(Color Color, bool IsStroke = true, float StrokeWidth = 1f, bool IsAntialias = false)
{
    //float StrokeMiter
    //SKStrokeCap StrokeCap
    //    SKStrokeJoin StrokeJoin
    //      SKShader Shader
    //     SKMaskFilter MaskFilter

    // float TextSize
    //SKTextAlign TextAlign
    //SKTextEncoding TextEncoding
    //float TextScaleX
    //float TextSkewX

    //float FontSpacing
    //SKFontMetrics FontMetrics

    //SKColorFilter ColorFilter
    //    SKFilterQuality FilterQuality
    //    SKTypeface Typeface
    //    SKPathEffect PathEffect
    //    MeasureText

}
