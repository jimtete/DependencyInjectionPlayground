using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Features.Indexed;
using Autofac.Features.Metadata;
using Autofac.Features.OwnedInstances;
using Module = Autofac.Module;

namespace DI_Playground
{
    public interface ILog
    {
        void Write(string message);
    }

    public interface IConsole
    {
        
    }

    public class ConsoleLog : ILog, IConsole, IDisposable
    {
        public ConsoleLog()
        {
            Console.WriteLine("Creating a console log!");
        }
        public void Write(string message)
        {
            Console.WriteLine(message);
        }

        public void Dispose()
        {
            Console.WriteLine("Console logger no longer required");
        }
    }

    public class EmailLog : ILog
    {
        private const string _adminEmail = "admin@foo.com";

        public void Write(string message)
        {
            Console.WriteLine($"Email sent to {_adminEmail} : {message}");
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

    public class Car
    {
        private Engine _engine;
        private ILog _log;

        public Car(Engine engine)
        {
            _engine = engine;
            _log = new EmailLog();
        }

        public Car(Engine engine, ILog log)
        {
            _engine = engine;
            _log = log;
        }

        public void Go()
        {
            _engine.Ahead(100);
            _log.Write("car going forward...");
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

        public Child()
        {
            Console.WriteLine("Child being created");
        }
        
        public void SetParent(Parent parent)
        {
            Parent = parent;
        }

        public override string ToString()
        {
            return "Hi there";
        }
    }

    class BadChild : Child
    {
        public override string ToString()
        {
            return "I Hate you";
        }
    }

    public class ParentChildModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Parent>();
            builder.Register(c => new Child() { Parent = c.Resolve<Parent>() });
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Parent>();
            builder.RegisterType<Child>()
                .OnActivating(a =>
                {
                    Console.WriteLine("Child activating");
                    //a.Instance.Parent = a.Context.Resolve<Parent>();
                    
                    a.ReplaceInstance(new BadChild());
                })
                .OnActivated(a =>
                {
                    Console.WriteLine("Child activated");
                })
                .OnRelease(a =>
                {
                    Console.WriteLine("Child is about to be removed");
                });

            /*builder.RegisterType<ConsoleLog>().As<ILog>()
                .OnActivating(a =>
                {
                    a.ReplaceInstance(new SMSLog("+123456"));
                });*/

            builder.RegisterType<ConsoleLog>().AsSelf();
            builder.Register<ILog>(c => c.Resolve<ConsoleLog>())
                .OnActivating(a => a.ReplaceInstance(new SMSLog("+123456")));
            using (var scope = builder.Build().BeginLifetimeScope())
            {
                var child = scope.Resolve<Child>();
                var parent = child.Parent;
                Console.WriteLine(child);
                Console.WriteLine(parent);

                var log = scope.Resolve<ILog>();
                log.Write("Testing");
            }
        }
    }
}