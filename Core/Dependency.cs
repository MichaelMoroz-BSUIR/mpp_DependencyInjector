using System;
using System.Linq;

namespace Core
{
    internal class Dependency
    {
        public DependencyInjector Injector { get; }
        public Type Type { get; }

        public Dependency(DependencyInjector injector, Type type)
        {
            Injector = injector ?? throw new ArgumentNullException(nameof(injector));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public virtual object GetInstance()
        {
            var constructors = Type.GetConstructors();
            if (constructors.Length == 0)
                throw new InvalidOperationException("No constructors present");
            var constructor = constructors[0];
            var cParams = constructor.GetParameters()
                .Select(p =>
                {
                    if (Injector.Dependencies.TryGetValue(p.ParameterType, out var dependencies) && dependencies.Count != 0)
                    {
                        return dependencies[0].GetInstance();
                    }
                    throw new InvalidOperationException("No dependency registered for parameter");
                })
                .ToArray();
            return constructor.Invoke(cParams);
        }
    }
}