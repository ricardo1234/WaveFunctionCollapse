using System;
using System.Linq;

namespace WaveFunctionCollapseLib
{
    /// <summary>
    /// Item to be extended
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WaveFunctionCollapseItem<T> where T : class
    {
        /// <summary>
        /// Value of the item
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Value of each side
        /// </summary>
        public Dictionary<Orientations, int[]> MyTypes { get; }

        /// <summary>
        /// Consctructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="myTypes"></param>
        public WaveFunctionCollapseItem(T value, Dictionary<Orientations, int[]> myTypes)
        {
            Value = value;
            MyTypes = myTypes;
        }

        /// <summary>
        /// Verify is side is valid for the requirement
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="requiredType"></param>
        /// <returns></returns>
        public bool ValidSide(Orientations orientation, int[] requiredType)
        {
            var equals = true;
            for (int i = 0; i < MyTypes[orientation].Length; i++)
                equals = equals && MyTypes[orientation][i] == requiredType[i];
            return equals;
        }
        
        /// <summary>
        /// Hash Code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Value, MyTypes);
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            return obj is WaveFunctionCollapseItem<T> item &&
                   EqualityComparer<T>.Default.Equals(Value, item.Value) &&
                   EqualityComparer<Dictionary<Orientations, int[]>>.Default.Equals(MyTypes, item.MyTypes);
        }
        /// <summary>
        /// To String
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Value} | TOP: [{string.Join(",", MyTypes[Orientations.TOP])}] | RIGHT: [{string.Join(",", MyTypes[Orientations.RIGHT])}] | BOTTOM: [{string.Join(",", MyTypes[Orientations.BOTTOM])}] | LEFT: [{string.Join(",", MyTypes[Orientations.LEFT])}] ";
    }


}
