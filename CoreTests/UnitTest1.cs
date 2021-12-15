using Core;
using System;
using Xunit;

namespace CoreTests
{
    public class DependencyProviderFacts
    {
        private interface ISomeObj
        {
            int Value { get; }
        }

        interface IAgregateObj
        {
            int Value { get; }
            ISomeObj Obj { get; }
        }

        interface IGenericObj<T>
        {
            int Value { get; }
            T Obj { get; }
        }

        class GenericImpl1 : IGenericObj<ISomeObj>
        {
            public GenericImpl1(ISomeObj obj)
            {
                Obj = obj;
            }

            public int Value { get; } = new Random().Next();

            public ISomeObj Obj { get; }
        }

        class GenericImpl2 : IGenericObj<ISomeObj>
        {
            private int _value = new Random().Next();
            public int Value => _value;

            public ISomeObj Obj { get; }
        }

        private class ObjImpl : ISomeObj
        {
            private int _value = new Random().Next();
            public int Value => _value;
        }

        private class AgregateObj : IAgregateObj
        {
            public int Value { get; } = new Random().Next();


            public ISomeObj Obj { get; }

            public AgregateObj(ISomeObj obj)
            {
                Obj = obj;
            }
        }

        [Fact]
        public void Resolve_DifferentValues_Instance()
        {
            var provider = new DependencyInjector();
            provider.Register<ISomeObj, ObjImpl>(LifeTime.Instance);
            int inst1 = provider.Resolve<ISomeObj>().Value;
            for (int i = 0; i < 100; i++)
            {
                int inst2 = provider.Resolve<ISomeObj>().Value;
                Assert.NotEqual(inst1, inst2);
            }
        }

        [Fact]
        public void Resolve_SameValues_Singleton()
        {
            var provider = new DependencyInjector();
            provider.Register<ISomeObj, ObjImpl>(LifeTime.Singleton);
            int inst1 = provider.Resolve<ISomeObj>().Value;
            for (int i = 0; i < 100; i++)
            {
                int inst2 = provider.Resolve<ISomeObj>().Value;
                Assert.Equal(inst1, inst2);
            }
        }

        [Fact]
        public void Resolve_Instance_Instance()
        {
            var provider = new DependencyInjector();
            provider.Register<ISomeObj, ObjImpl>(LifeTime.Instance);
            provider.Register<IAgregateObj, AgregateObj>(LifeTime.Instance);

            IAgregateObj inst1 = provider.Resolve<IAgregateObj>();
            for (int i = 0; i < 100; i++)
            {
                IAgregateObj inst2 = provider.Resolve<IAgregateObj>();
                Assert.NotEqual(inst1.Value, inst2.Value);
                Assert.NotEqual(inst1.Obj.Value, inst2.Obj.Value);
            }
        }

        [Fact]
        public void Resolve_Instance_Singleton()
        {
            var provider = new DependencyInjector();
            provider.Register<ISomeObj, ObjImpl>(LifeTime.Instance);
            provider.Register<IAgregateObj, AgregateObj>(LifeTime.Singleton);
            int val1 = provider.Resolve<ISomeObj>().Value;
            IAgregateObj inst1 = provider.Resolve<IAgregateObj>();
            for (int i = 0; i < 100; i++)
            {
                int val2 = provider.Resolve<ISomeObj>().Value;
                IAgregateObj inst2 = provider.Resolve<IAgregateObj>();
                Assert.NotEqual(val1, val2);
                Assert.Equal(inst1.Value, inst2.Value);
                Assert.Equal(inst1.Obj.Value, inst2.Obj.Value);
            }
        }

        [Fact]
        public void Resolve_Singleton_Instance()
        {
            var provider = new DependencyInjector();
            provider.Register<ISomeObj, ObjImpl>(LifeTime.Singleton);
            provider.Register<IAgregateObj, AgregateObj>(LifeTime.Instance);
            int val1 = provider.Resolve<ISomeObj>().Value;
            IAgregateObj inst1 = provider.Resolve<IAgregateObj>();
            for (int i = 0; i < 100; i++)
            {
                int val2 = provider.Resolve<ISomeObj>().Value;
                IAgregateObj inst2 = provider.Resolve<IAgregateObj>();
                Assert.Equal(val1, val2);
                Assert.NotEqual(inst1.Value, inst2.Value);
                Assert.Equal(inst1.Obj.Value, inst2.Obj.Value);
            }
        }

        [Fact]
        public void Resolve_Singleton_Singleton()
        {
            var provider = new DependencyInjector();

            provider.Register<ISomeObj, ObjImpl>(LifeTime.Singleton);
            provider.Register<IAgregateObj, AgregateObj>(LifeTime.Singleton);
            int val1 = provider.Resolve<ISomeObj>().Value;
            IAgregateObj inst1 = provider.Resolve<IAgregateObj>();
            for (int i = 0; i < 100; i++)
            {
                int val2 = provider.Resolve<ISomeObj>().Value;
                IAgregateObj inst2 = provider.Resolve<IAgregateObj>();
                Assert.Equal(val1, val2);
                Assert.Equal(inst1.Value, inst2.Value);
                Assert.Equal(inst1.Obj.Value, inst2.Obj.Value);
            }
        }
    }
}