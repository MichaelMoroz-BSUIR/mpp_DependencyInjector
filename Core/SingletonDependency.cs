using System;

namespace Core
{
    internal class SingletonDependency : Dependency
    {
        private object _instance;

        public SingletonDependency(DependencyInjector injector, Type type, object instance = null!)
            : base(injector, type)
        {
            _instance = instance;
        }
        public override object GetInstance()
        {
            lock (this)
            {
                return _instance ??= base.GetInstance();
            }
        }
    }
}