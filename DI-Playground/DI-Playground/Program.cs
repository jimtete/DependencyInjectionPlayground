﻿using System;
using System.Collections.Generic;
using Autofac;

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
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ConsoleLog>().As<ILog>();

            builder.Register(c =>
                new Engine(c.Resolve<ILog>(), 123));
            
            //builder.RegisterType<Engine>();
            builder.RegisterType<Car>();

            var container = builder.Build();

            var car = container.Resolve<Car>();
            car.Go();
        }
    }
}