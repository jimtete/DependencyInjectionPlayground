using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;

namespace DI_Playground
{
    public class Service
    {
        public string DoSomething(int value)
        {
            return $"I have {value}";
        }
    }

    public class DomainObject
    {
        private Service _service;
        private int _value;
        
        public delegate DomainObject Factory(int value);

        public DomainObject(Service service, int value)
        {
            _service = service;
            _value = value;
        }

        public override string ToString()
        {
            return _service.DoSomething(_value);
        }
    }

    public class Demo
    {
        public static void Main(string[] args)
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<Service>();
            cb.RegisterType<DomainObject>();

            var container = cb.Build();
            var dobj = container.Resolve<DomainObject>(
                new PositionalParameter(1, 42));
            Console.WriteLine(dobj);
            
            /**/
            
            var factory = container.Resolve<DomainObject.Factory>();
            var dobj2 = factory(42);
            Console.WriteLine(dobj2);
        }
    }
}