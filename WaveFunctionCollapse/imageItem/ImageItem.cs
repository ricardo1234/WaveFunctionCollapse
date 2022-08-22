using System.Windows.Media;

namespace WaveFunctionCollapse.imageItem
{
     /// <summary>
     /// Item Image Definition
     /// </summary>
    public class ImageItem
    {
        public ImageSource? Image { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public int Rotation { get; init; }

        public override string ToString() => $"img: {Image} | rotation: {Rotation}";

    }
}
