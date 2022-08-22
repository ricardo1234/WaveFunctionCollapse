namespace WaveFunctionCollapseLib
{
    /// <summary>
    /// Visitor pattern
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IVisitor<T> where T : class
    {
        /// <summary>
        /// Visit handler
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="possibilities"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="fastWay"></param>
        void Visit(T? visitor, int? possibilities, long x, long y, bool fastWay);
      
        /// <summary>
        /// Before visit handler
        /// </summary>
        void BeforeVisit();
    }
}
