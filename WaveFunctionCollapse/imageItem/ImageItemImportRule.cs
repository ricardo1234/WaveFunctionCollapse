using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse.imageItem
{
    /// <summary>
    /// Rule structure
    /// </summary>
    public class ImageItemImportRule
    {
        public string Img { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<RotationRules> Rotations { get; set; }

    }
}
