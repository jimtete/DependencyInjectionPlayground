using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;

namespace DI_Playground
{
    public class Entity
    {
        public delegate Entity Factory();
        
        private static Random _random = new Random();
        private int _number;

        public Entity()
        {
            _number = _random.Next();
        }

        public override string ToString()
        {
            return "tester: " + _number;
        }
    }

    public class ViewModel
    {
        private readonly Entity.Factory _entityFactory;
        public ViewModel(Entity.Factory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public void Method()
        {
            var entity = _entityFactory();
            Console.WriteLine(entity);
        }
    }

    public class Demo
    {
        public static void Main(string[] args)
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<Entity>().InstancePerDependency();
            cb.RegisterType<ViewModel>();

            var container = cb.Build();
            var vm = container.Resolve<ViewModel>();
            vm.Method();
            vm.Method();
        }
    }
}