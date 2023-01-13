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
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            // named parameter.
            /*builder.RegisterType<SMSLog>()
                .As<ILog>()
                .WithParameter("phoneNumber", "6948336420");*/
            
            //typed parameter.
            /*builder.RegisterType<SMSLog>()
                .As<ILog>()
                .WithParameter(new TypedParameter(typeof(string), "6948336420"));*/
            
            //resolved parameter.
            /*builder.RegisterType<SMSLog>()
                .As<ILog>()
                .WithParameter(
                    new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "phoneNumber",
                        (pi, ctx) => "6948336420"
                    )
                );
            var container = builder.Build();
            var log = container.Resolve<ILog>();
            log.Write("test massage.");*/

            Random random = new Random();
            builder.Register((c, p)
                => new SMSLog(p.Named<string>("phoneNumber")))
                .As<ILog>();

            Console.WriteLine("About to build container");
            var container = builder.Build();

            var log = container.Resolve<ILog>(new NamedParameter("phoneNumber", random.Next().ToString()));
            log.Write("testing");
        }
    }
}