using System;
using System.Collections.Generic;
using System.Reflection;
using Ninject;

namespace Reducely
{
    public interface IReducelyBuilder : IDisposable, IFluentSyntax
    {
        IReduceFor<TSource, TCriteria> Build<TSource, TCriteria>();
    }

    public class ReducelyBuilder : IDisposable, IFluentSyntax
    {
        private readonly IKernel _kernel;
        private readonly bool _iOwnTheKernel;

        public ReducelyBuilder()
            : this(new StandardKernel())
        {
            _iOwnTheKernel = true;
        }

        public ReducelyBuilder(IKernel kernel)
        {
            _kernel = kernel;
            _kernel.Settings.InjectNonPublic = true;
        }

        public IReducelyBuilder For(IEnumerable<Assembly> assemblies)
        {
            _kernel.Load(new Reducely(x => x.From(assemblies)));
            return new Builder(_kernel, _iOwnTheKernel);
        }

        private class Builder : IReducelyBuilder, IFluentSyntax
        {
            private readonly IKernel _kernel;
            private readonly bool _iOwnTheKernel;
            public Builder(IKernel kernel, bool iOwnTheKernel)
            {
                _kernel = kernel;
                _iOwnTheKernel = iOwnTheKernel;
            }

            public IReduceFor<TSource, TCriteria> Build<TSource, TCriteria>()
            {
                return _kernel.Get<IReduceFor<TSource, TCriteria>>();
            }

            public void Dispose()
            {
                if (_iOwnTheKernel) _kernel.Dispose();
            }
        }

        public void Dispose()
        {
            if (_iOwnTheKernel) _kernel.Dispose();
        }
    }
}