using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d2;

public interface IDrawable
{
    void Draw (ICanvas canvas, RtF dirtyRect);
}
