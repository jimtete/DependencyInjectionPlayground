using Autofac;

namespace DI_Playground
{
    public interface IReportingService
    {
        void Report();
    }

    public class ReportingService : IReportingService
    {
        public void Report()
        {
            Console.WriteLine("Here's your report");
        }
    }

    public class ReportingServiceWithLogging : IReportingService
    {
        private IReportingService _decorated;

        public ReportingServiceWithLogging(IReportingService decorated)
        {
            _decorated = decorated;
        }
        public void Report()
        {
            Console.WriteLine("Commencing log...");
            _decorated.Report();
            Console.WriteLine("... ending log.");
        }
    }
    public class Program
    {
        static void Main(string[] args)
        {
            var b = new ContainerBuilder();
            b.RegisterType<ReportingService>().Named<IReportingService>("reporting");
            b.RegisterDecorator<IReportingService>(
                (context, service) => new ReportingServiceWithLogging(service), "reporting"
            );

            using (var c = b.Build())
            {
                var r = c.Resolve<IReportingService>();
                r.Report();
            }
        }
    }
}