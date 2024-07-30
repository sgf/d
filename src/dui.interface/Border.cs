
namespace d2;

public struct BorderBoxStyle
{

    /// <summary> 边框盒子信息  </summary>
    /// <param name="left"></param>
    /// <param name="top"></param>
    /// <param name="right"></param>
    /// <param name="bottom"></param>
    public BorderBoxStyle(BorderInfo left, BorderInfo top, BorderInfo right, BorderInfo bottom)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }

    /// <summary> 边框盒子信息  </summary>
    /// <param name="radius">应用在所有4个圆角</param>
    /// <param name="width">应用在所有4个边框</param>
    /// <param name="color">应用在所有4个边框颜色</param>
    public BorderBoxStyle(int radius = default, BorderStyle style = default, float width = default, uint color = default)
    {
        if (width != default)
        {
            left = new(width, style, radius, color);
            top = new(width, style, radius, color);
            right = new(width, style, radius, color);
            bottom = new(width, style, radius, color);
        }
    }

    public BorderInfo left;
    public BorderInfo top;
    public BorderInfo right;
    public BorderInfo bottom;
    /// <summary> 4条细线边框 </summary>
    public static BorderBoxStyle thin = new(BorderInfo.thin, BorderInfo.thin, BorderInfo.thin, BorderInfo.thin);

    public static BorderBoxStyle @default = default;

    public bool HasRadius => left.radius > 0 || top.radius > 0 || right.radius > 0 || bottom.radius > 0;

}

/// <summary> 边框信息 </summary>
/// <param name="width">粗细</param>
/// <param name="style">外观样式</param>
/// <param name="radius">圆角半径</param>
/// <param name="color">颜色</param>
public record struct BorderInfo(float width = 0f, BorderStyle style = BorderStyle.none, int radius = 0, uint color = 0)
{
    /// <summary> 细线边框 </summary>
    public static BorderInfo thin = new(0.5f, BorderStyle.solid, 0, 0xccccccff);//灰色?
}

public enum BorderStyle : byte
{
    /// <summary> 默认无边框 </summary>
    none = 0,
    /// <summary> 定义一个点线边框 </summary>
    dotted,
    /// <summary> 定义一个虚线边框 </summary>
    dashed,
    /// <summary> 定义实线边框 </summary>
    solid,
    /// <summary> 定义两个边框。 两个边框的宽度和 border-width 的值相同 </summary>
    @double,
    ///// <summary> 定义3D沟槽边框。效果取决于边框的颜色值 </summary>
    //groove,
    ///// <summary> 定义3D脊边框。效果取决于边框的颜色值 </summary>
    //ridge,
    ///// <summary> 定义一个3D的嵌入边框。效果取决于边框的颜色值 </summary>
    //inset,
    ///// <summary> 定义一个3D突出边框。 效果取决于边框的颜色值 </summary>
    //outset,
    /// <summary> 定义隐藏边框 </summary>
    hidden,
}



public record struct BGStyle(IImg img, uint color = 0, layoutX layoutX = layoutX.center, layoutRepeat layoutY = layoutRepeat.none)
{

}

public enum layoutX : int
{
    center = 0,
    leftTop = 1,
    right = 2,
}


public enum layoutXYStart : int
{
    center_center,
    leftTop = layoutXStart.left | layoutYStart.top,
    rightTop = layoutXStart.right | layoutYStart.top,
    centerTop = layoutXStart.center | layoutYStart.top,
    leftCenter = layoutXStart.left | layoutYStart.center,
    rightCenter = layoutXStart.right | layoutYStart.center,
    centerCenter = layoutXStart.center | layoutYStart.center,
    leftBottom = layoutXStart.left | layoutYStart.bottom,
    rightBottom = layoutXStart.right | layoutYStart.bottom,
    centerBottom = layoutXStart.center | layoutYStart.bottom,
}

public enum layoutRepeat
{
    none = 0,
    /// <summary> 拉伸(无视变形至全面) </summary>
    zoomFull = 1,
    /// <summary> 拉伸(自动缩放至任意边相等为止，另一边居中) </summary>
    zoomAuto = 2,

    /// <summary>X轴平铺 </summary>
    tileX = 4,
    /// <summary>Y轴平铺 </summary>
    tileY = 8,
    /// <summary>平铺,直译：瓦片 </summary>
    tile = tileX | tileY,

}

public enum layoutXStart : int
{
    center = 0,
    left = 1,
    right = 2,
}

public enum layoutYStart : int
{
    center = 0 << 8,
    top = 1 << 8,
    bottom = 2 << 8,
}

//public enum layoutYStart : int
//{
//    /// <summary> 单张，自动 </summary>
//    @auto = 0 << 8,
//    /// <summary> 拉伸 </summary>
//    zoom = 1 << 8,
//    tile = 2 << 8,
//}



public enum layout
{
    center = layoutX.center | layoutYStart.center,
    /// <summary> 拉伸(无视变形至全面) </summary>
    zoomFull = 1,
    /// <summary> 拉伸(自动缩放至任意边相等为止，另一边居中) </summary>
    zoomAuto = 2,
    /// <summary>X轴平铺,直译：瓦片 </summary>
    tileX = 3,
    /// <summary>Y轴平铺,直译：瓦片 </summary>
    tileY = 4,
    /// <summary>平铺,直译：瓦片 </summary>
    tile = 5,

}