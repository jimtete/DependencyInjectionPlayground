using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Features.Metadata;
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

    public class Settings
    {
        public string LogMode { get; set; }
        //
    }

    public class Reporting
    {
        private Meta<ConsoleLog, Settings> _log;

        public Reporting(Meta<ConsoleLog, Settings> log)
        {
            _log = log;
        }

        public void Report()
        {
            _log.Value.Write("Starting report");

            //if (_log.Metadata["mode"] as string == "verbose")
            if (_log.Metadata.LogMode == "verbose")
            {
                _log.Value.Write($"VERBOSE MODE: Logger started on {DateTime.Now}");
            }
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            //builder.RegisterType<ConsoleLog>().WithMetadata("mode","verbose");
            builder.RegisterType<ConsoleLog>()
                .WithMetadata<Settings>(c => c.For(
                    x => x.LogMode, "verbose"));
            builder.RegisterType<Reporting>();
            using (var c = builder.Build())
            {
                c.Resolve<Reporting>().Report();
            }
        }
    }
}