using System.ComponentModel.Composition;
using Autofac;
using Autofac.Extras.AggregateService;
using Autofac.Extras.AttributeMetadata;
using Autofac.Extras.DynamicProxy;
using Autofac.Features.AttributeFilters;
using Castle.DynamicProxy;

namespace DI_Playground
{
    public class CallLogger : IInterceptor
    {
        private TextWriter _output;

        public CallLogger(TextWriter output)
        {
            _output = output;
        }
        
        public void Intercept(IInvocation invocation)
        {
            var methodName = invocation.Method.Name;
            _output.WriteLine("Calling method {0} with args {1}",
                methodName,
                string.Join(",",
                    invocation.Arguments.Select(a => (a ?? "").ToString())
                )
            );
            invocation.Proceed();
            _output.WriteLine("Done calling {0}, result was {1}",
                methodName, invocation.ReturnValue
            );
        }
    }

    public interface IAudit
    {
        int Start(DateTime reportDate);
    }

    [Intercept(typeof(CallLogger))]
    public class Audit : IAudit
    {
        public int Start(DateTime reportDate)
        {
            Console.WriteLine($"Starting report on {reportDate}");
            return 42;
        }
    }
    
    public class Program
    {
        static void Main(string[] args)
        {
            var cb = new ContainerBuilder();
            cb.Register(c => new CallLogger(Console.Out))
                .As<IInterceptor>()
                .AsSelf();
            cb.RegisterType<Audit>()
                .EnableClassInterceptors();

            using (var container = cb.Build())
            {
                var audit = container.Resolve<Audit>();
                audit.Start(DateTime.Now);
            }
        }
    }
}