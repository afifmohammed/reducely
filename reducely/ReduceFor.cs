namespace Reducely
{
    public abstract class ReduceFor<TSource, TCriteria> : IReduceFor<TSource, TCriteria>, IReduceBy<TCriteria, TSource>
    {
        protected TSource Source;

        public IReduceBy<TCriteria, TSource> For(TSource source)
        {
            Source = source;
            return this;
        }

        public abstract TSource By(TCriteria criteria);
    }
}