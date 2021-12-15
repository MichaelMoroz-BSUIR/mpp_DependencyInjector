using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class DependencyInjector
    {
        internal static bool IsBaseTypeOrInterface(Type type, Type parent)
        {
            return type.BaseType == parent ||
                   type.GetInterfaces()
                       .Any(t => t.Equals(parent));
        }
        internal static void Validate(Type serviceType, Type implementationType)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (implementationType is null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            if (!IsBaseTypeOrInterface(implementationType, serviceType))
            {
                throw new ArgumentException($"Implementation type: '{implementationType}" +
                    $"' not implements service type: '{serviceType}'");
            }
        }
        internal IDictionary<Type, IList<Dependency>> Dependencies { get; } = 
            new Dictionary<Type, IList<Dependency>>();

        public void Register<TDependency>(LifeTime lifeTime = LifeTime.Instance) where TDependency : class
        {
            Register(typeof(TDependency), lifeTime);
        }
        public void Register(Type serviceType, LifeTime lifeTime = LifeTime.Instance)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (!Dependencies.TryGetValue(serviceType, out var dependencies))
            {
                dependencies = new List<Dependency>();
                Dependencies.Add(serviceType, dependencies);
            }
            var dependency = (lifeTime) switch
            {
                LifeTime.Instance => new Dependency(this, serviceType),
                LifeTime.Singleton => new SingletonDependency(this, serviceType),
                _ => throw new ArgumentException(null, nameof(lifeTime)),
            };
            dependencies.Add(dependency);
        }

        public void Register<TDependency, TImplementation>(LifeTime lifeTime = LifeTime.Instance)
        {
            Register(typeof(TDependency), typeof(TImplementation), lifeTime);
        }
        public void Register(Type serviceType, Type implementationType, LifeTime lifeTime = LifeTime.Instance)
        {
            Validate(serviceType, implementationType);
            if (!Dependencies.TryGetValue(serviceType, out var dependencies))
            {
                dependencies = new List<Dependency>();
                Dependencies.Add(serviceType, dependencies);
            }
            var dependency = (lifeTime) switch
            {
                LifeTime.Instance => new Dependency(this, implementationType),
                LifeTime.Singleton => new SingletonDependency(this, implementationType),
                _ => throw new ArgumentException(null, nameof(lifeTime)),
            };
            dependencies.Add(dependency);
        }

        public void RegisterSingleton<TDependency>(TDependency implementationInstance) where TDependency : class
        {
            RegisterSingleton(typeof(TDependency), implementationInstance);
        }
        public void RegisterSingleton(Type serviceType, object implementationInstance)
        {
            Validate(serviceType, implementationInstance?.GetType());
            if (!Dependencies.TryGetValue(serviceType, out var dependencies))
            {
                dependencies = new List<Dependency>();
                Dependencies.Add(serviceType, dependencies);
            }
            dependencies.Add(new SingletonDependency(this, serviceType, implementationInstance));
        }

        public TDependency Resolve<TDependency>()
        {
            return (TDependency)Resolve(typeof(TDependency));
        }
        public object Resolve(Type serviceType)
        {
            if (Dependencies.TryGetValue(serviceType, out var dependencies))
            {
                return dependencies[0].GetInstance();
            }
            throw new InvalidOperationException("No such type is registered");
        }

        public IEnumerable<TDependency> ResolveAll<TDependency>()
        {
            return ResolveAll(typeof(TDependency)).Cast<TDependency>();
        }
        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            if (Dependencies.TryGetValue(serviceType, out var dependencies))
            {
                return dependencies.Select(d => d.GetInstance()).ToArray();
            }
            throw new InvalidOperationException("No such type is registered");
        }
    }
}