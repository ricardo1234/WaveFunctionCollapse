using System.Collections.Generic;

namespace WaveFunctionCollapse.imageItem
{
    /// <summary>
    /// Rotation rule structure
    /// </summary>
    public class RotationRules
    {
        public int RotationDegree { get; set; }
        public List<List<int>> SideRules { get; set; }
    }
}
