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

        public void SetParent(Parent parent)
        {
            Parent = parent;
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
            //builder.RegisterType<ConsoleLog>();
            builder.RegisterInstance(new ConsoleLog());
            var container = builder.Build();
            using (var scope = container.BeginLifetimeScope())
            {
                scope.Resolve<ConsoleLog>();
            }
        }
    }
}