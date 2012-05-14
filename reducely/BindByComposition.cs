using System;
using System.Linq;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Conventions;

namespace Reducely
{
    internal class BindByComposition : IBindingGenerator
    {
        private readonly Type _interface = typeof(IReduceFor<,>);

        public void Process(Type target, Func<IContext, object> scopeCallback, IKernel kernel)
        {
            var @params = ReduceGenericParameters(target);
            if (!@params.Any())
                return;

            var compositionRoot = typeof(ByComposition<,>).ResolveClosingType(@params[0], @params[1]);
            var reduceInterface = target.ResolveClosingInterface(_interface);

            if (target != compositionRoot)
                kernel.Bind(reduceInterface).To(target).WhenInjectedInto(compositionRoot).InScope(scopeCallback);
        }

        private Type[] ReduceGenericParameters(Type target)
        {
            var @interface = target.GetInterfaces().SingleOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == _interface);

            if (@interface == null) return new Type[] { };

            var args = @interface.GetGenericArguments();
            var source = args[0];
            var criteria = args[1];

            return new[] { source, criteria };
        }
    }
}