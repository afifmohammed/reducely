namespace Reducely
{
    public interface IReduceBy<in TCriteria, out TSource>
    {
        TSource By(TCriteria criteria);
    }
}