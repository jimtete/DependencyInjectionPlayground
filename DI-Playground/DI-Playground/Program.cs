using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Features.OwnedInstances;
using Module = Autofac.Module;

namespace DI_Playground
{
    public interface ILog : IDisposable
    {
        void Write(string message);
    }

    public class ConsoleLog : ILog 
    {
        public ConsoleLog()
        {
            Console.WriteLine($"Console log created at {DateTime.Now.Ticks}");
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
    
    public class SMSLog : ILog
    {
        private string _phoneNumber;

        public SMSLog(string phoneNumber)
        {
            _phoneNumber = phoneNumber;
        }

        public void Dispose()
        {
            
        }
        
        public void Write(string message)
        {
            Console.WriteLine($"SMS to {_phoneNumber} : {message}");
        }
    }

    public class Reporting
    {
        private Owned<ConsoleLog> _log;

        public Reporting(Owned<ConsoleLog> log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(paramName: nameof(log));
            }
            _log = log;
            Console.WriteLine("Reporting component created");
        }

        public void ReportOnce()
        {
            _log.Value.Write("Log started");
            _log.Dispose();
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            new Lazy<ConsoleLog>(() => new ConsoleLog());
            
            var builder = new ContainerBuilder();
            builder.RegisterType<ConsoleLog>();
            builder.RegisterType<Reporting>();
            using (var c = builder.Build())
            {
                c.Resolve<Reporting>().ReportOnce();
                Console.WriteLine("Done reporting");
            }
        }
    }
}