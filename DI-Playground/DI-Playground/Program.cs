using System;
using System.Collections.Generic;

namespace DI_Playground
{
    public interface ILog
    {
        void Write(string message);
    }

    public class ConsoleLog : ILog
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
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
            var log = new ConsoleLog();
            var engine = new Engine(log);
            var car = new Car(engine, log);
            car.Go();
        }
    }
}