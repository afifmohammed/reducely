namespace Reducely
{
    public interface IReduceFor<TSource, in TCriteria>
    {
        IReduceBy<TCriteria, TSource> For(TSource source);
    }
}