using System.Reflection;
using Autofac;
using Autofac.Configuration;
using Microsoft.Extensions.Configuration;

namespace DI_Playground
{
    public interface IOperation
    {
        float Calculate(float a, float b);
    }

    public class Addition : IOperation
    {
        public float Calculate(float a, float b)
        {
            return a + b;
        }
    }

    public class Multiplication : IOperation
    {
        public float Calculate(float a, float b)
        {
            return a * b;
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "./");

            
            Console.WriteLine(Directory.GetCurrentDirectory());
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json");

            var configuration = configBuilder.Build();
            var containerBuilder = new ContainerBuilder();
            var configModule = new ConfigurationModule(configuration);
            containerBuilder.RegisterModule(configModule);

            using (var container = containerBuilder.Build())
            {
                float a = 3, b = 4;

                foreach (IOperation op in container.Resolve<IList<IOperation>>())
                {
                    Console.WriteLine($"{op.GetType().Name} of {a} and {b} = {op.Calculate(a,b)}");
                }
            }
        }
    }
}