namespace WaveFunctionCollapseLib
{
    public class WaveFunctionCollapseItemBuilder<T> where T : class
    {
        private T? value;

        private Dictionary<Orientations, int[]> types;

        public WaveFunctionCollapseItemBuilder()
        {
            types = new();
        }

        public WaveFunctionCollapseItem<T> Build()
        {
            var result = (WaveFunctionCollapseItem<T>)Activator.CreateInstance(typeof(WaveFunctionCollapseItem<T>), value, types);
            value = null;
            types = new();
            return result;
        }

        public WaveFunctionCollapseItemBuilder<T> WithItem(T item)
        {
            value = item;
            return this;
        }
        public WaveFunctionCollapseItemBuilder<T> WithTopNeighbourType(int[] type)
        {
            types[Orientations.TOP] = type;
            return this;
        }

        public WaveFunctionCollapseItemBuilder<T> WithRightNeighbourType(int[] type)
        {
            types[Orientations.RIGHT] = type;
            return this;
        }
      
        public WaveFunctionCollapseItemBuilder<T> WithBottomNeighbourType(int[] type)
        {
            types[Orientations.BOTTOM] = type;
            return this;
        }
        public WaveFunctionCollapseItemBuilder<T> WithLeftNeighbourType(int[] type)
        {
            types[Orientations.LEFT] = type;
            return this;
        }
    }


}
