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
        private IIndex<string, ILog> _logs;

        public Reporting(IIndex<string, ILog> logs)
        {
            _logs = logs;
        }

        public void Report()
        {
            _logs["sms"].Write("Starting report output");
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ConsoleLog>().Keyed<ILog>("cmd");
            builder.Register(c => new SMSLog("+123456")).Keyed<ILog>("sms");
            builder.RegisterType<Reporting>();
            using (var c = builder.Build())
            {
                c.Resolve<Reporting>().Report();
            }

        }
    }
}