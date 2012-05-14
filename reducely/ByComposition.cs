using System.Collections.Generic;

namespace Reducely
{
    internal class ByComposition<TSource, TCriteria> : IReduceFor<TSource, TCriteria>, IReduceBy<TCriteria, TSource>
    {
        private readonly IEnumerable<IReduceFor<TSource, TCriteria>> _refines;
        private TSource _source;
        public ByComposition(IEnumerable<IReduceFor<TSource, TCriteria>> refines)
        {
            _refines = refines;
        }

        public IReduceBy<TCriteria, TSource> For(TSource source)
        {
            _source = source;
            return this;
        }

        public TSource By(TCriteria criteria)
        {
            foreach (var refine in _refines)
                _source = refine.For(_source).By(criteria);

            return _source;
        }
    }
}