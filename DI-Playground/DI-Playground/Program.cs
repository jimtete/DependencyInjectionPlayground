using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;

namespace DI_Playground
{
    public interface ILog
    {
        void Write(string message);
    }

    public interface IConsole
    {
        
    }

    public class ConsoleLog : ILog 
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class EmailLog : ILog, IConsole
    {
        private const string AdminEmail = "admin@foo.com";
        public void Write(string message)
        {
            Console.WriteLine($"Email sent to {AdminEmail} : {message}");
        }
    }

    public class Engine
    {
        private ILog _log;
        private int _id;

        public Engine(ILog log)
        {
            _log = log;
            _id = new Random().Next();
        }

        public Engine(ILog log, int id)
        {
            _log = log;
            _id = id;
        }

        public void Ahead(int power)
        {
            _log.Write($"Engine [{_id}] ahead {power}");
        }
    }

    public class Car
    {
        private Engine _engine;
        private ILog _log;

        public Car(Engine engine, ILog log)
        {
            _engine = engine;
            _log = log;
        }

        public Car(Engine engine)
        {
            _engine = engine;
            _log = new EmailLog();
        }

        public void Go()
        {
            _engine.Ahead(100);
            _log.Write("Car going forward...");
        }
    }

    public class SMSLog : ILog
    {
        private string _phoneNumber;

        public SMSLog(string phoneNumber)
        {
            _phoneNumber = phoneNumber;
        }
        
        public void Write(string message)
        {
            Console.WriteLine($"SMS to {_phoneNumber} : {message}");
        }
    }

    public class Parent
    {
        public override string ToString()
        {
            return "I am your father";
        }
    }

    public class Child
    {
        public string Name { get; set; }
        public Parent Parent { get; set; }

        public void SetParent(Parent parent)
        {
            Parent = parent;
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Parent>();
            
            //builder.RegisterType<Child>().PropertiesAutowired();

            /*builder.RegisterType<Child>()
                .WithProperty("Parent", new Parent());*/

            /*builder.Register(c =>
            {
                var child = new Child();
                child.SetParent(c.Resolve<Parent>());
                return child;
            });*/

            builder.RegisterType<Child>()
                .OnActivated(e =>
                {
                    var p = e.Context.Resolve<Parent>();
                    e.Instance.SetParent(new Parent());
                });
            
            var container = builder.Build();
            var parent = container.Resolve<Child>().Parent;
            Console.WriteLine(parent);
        }
    }
}