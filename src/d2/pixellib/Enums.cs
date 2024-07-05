using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d2;



public enum IBLIT : int
{
    /// <summary> blit mode: enable clip </summary>
    CLIP = 1,
    /// <summary> blit mode: enable transparent blit </summary>
    MASK = 2,
    /// <summary> flip horizon </summary>
    HFLIP = 4,
    /// <summary> flip vertical </summary>
    VFLIP = 8,
}
