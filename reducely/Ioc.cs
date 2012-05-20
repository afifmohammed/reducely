using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Activation;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.Conventions.BindingGenerators;
using Ninject.Extensions.Conventions.Syntax;
using Ninject.Modules;
using Ninject.Syntax;

namespace Reducely
{
    internal class Reducely : NinjectModule
    {
        private readonly Func<IFromSyntax, IIncludingNonePublicTypesSelectSyntax> _fromAction;

        public Reducely(Func<IFromSyntax, IIncludingNonePublicTypesSelectSyntax> fromAction)
        {
            _fromAction = fromAction;
        }

        public override void Load()
        {
            Kernel.Bind(typeof(IReduceFor<,>)).To(typeof(ByComposition<,>)).When(r => !TargetIsCompositionRoot(r));
            Kernel.Bind(c => _fromAction(c)
                                 .SelectAllClasses()
                                 .InheritedFrom(typeof(IReduceFor<,>))
                                 .BindWith<BindByComposition>()
                                 .Configure(x => x.When(TargetIsCompositionRoot)));
        }

        private bool TargetIsCompositionRoot(IRequest r)
        {
            if (r.Target == null) 
                return false;

            var satisfies = r.Target.Name == "refines";

            return satisfies;
        }
    }

    internal class BindByComposition : IBindingGenerator
    {
        public IEnumerable<IBindingWhenInNamedWithOrOnSyntax<object>> CreateBindings(Type type, IBindingRoot bindingRoot)
        {
            var selector = new BindableTypeSelector();
            var @interface = selector.GetBindableInterfaces(type).SingleOrDefault(IsAnIReduceInterface);

            if (@interface == null)
                return Enumerable.Empty<IBindingWhenInNamedWithOrOnSyntax<object>>();

            if (typeof(ByComposition<,>).IsAssignableFrom(type))
                return Enumerable.Empty<IBindingWhenInNamedWithOrOnSyntax<object>>();

            return new[] { bindingRoot.Bind(@interface).To(type) };
        }

        private bool IsAnIReduceInterface(Type @interface)
        {
            return @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IReduceFor<,>);
        }
    }
}