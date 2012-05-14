using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace Reducely
{
    public interface IReducelyBuilder : IDisposable, IFluentSyntax
    {
        IReduceFor<TSource, TCriteria> Build<TSource, TCriteria>();
    }

    public class ReducelyContainer : IDisposable, IFluentSyntax
    {
        private readonly IKernel _kernel;
        private readonly bool _iOwnTheKernel;

        public ReducelyContainer() : this(new StandardKernel())
        {
            _iOwnTheKernel = true;
        }

        public ReducelyContainer(IKernel kernel)
        {
            _kernel = kernel;
            _kernel.Settings.InjectNonPublic = true;
        }

        public IReducelyBuilder For(IEnumerable<Assembly> assemblies)
        {
            _kernel.Load(new Reducely(x => x.From(assemblies)));
            return new Builder(_kernel);
        }

        private class Builder : IReducelyBuilder, IFluentSyntax
        {
            private readonly IKernel _kernel;

            public Builder(IKernel kernel)
            {
                _kernel = kernel;
            }

            public IReduceFor<TSource, TCriteria> Build<TSource, TCriteria>()
            {
                return _kernel.Get<IReduceFor<TSource, TCriteria>>();
            }

            public void Dispose()
            {
                _kernel.Dispose();
            }
        }

        public void Dispose()
        {
            if(_iOwnTheKernel) _kernel.Dispose();
        }
    }

    internal class Reducely : NinjectModule
    {
        private readonly Action<AssemblyScanner>[] _actions;

        public Reducely(params Action<AssemblyScanner>[] actions)
        {
            _actions = actions;
        }

        public override void Load()
        {
            Kernel.Bind(typeof(IReduceFor<,>)).To(typeof(ByComposition<,>)).InRequestScope();
            Kernel.Scan(scanner =>
                            {
                                _actions.ForEach(a => a(scanner));
                                scanner.Where(target => target.IsClass && !target.IsAbstract);
                                scanner.Where(target => target.GetInterfaces().Any(IsAnIReduceInterface));
                                scanner.BindWith<BindByComposition>();
                            });
        }

        private bool IsAnIReduceInterface(Type @interface)
        {
            if (!@interface.IsInterface) return false;

            if (!@interface.IsGenericType) return false;

            return @interface.GetGenericTypeDefinition() == typeof (IReduceFor<,>);
        }
    }

    
}