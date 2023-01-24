using System.Collections.Concurrent;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;
using Autofac.Features.ResolveAnything;

namespace DI_Playground
{
    public abstract class BaseHandler
    {
        public virtual string Handle(string message)
        {
            return "Handled: " + message;
        }
    }

    public class HandlerA : BaseHandler
    {
        public override string Handle(string message)
        {
            return "Handled by A: " + message;
        }
    }
    public class HandlerB : BaseHandler
    {
        public override string Handle(string message)
        {
            return "Handled by B: " + message;
        }
    }

    public interface IHandlerFactory
    {
        T GetHandler<T>() where T : BaseHandler;
    }

    public class HandlerFactory : IHandlerFactory
    {
        public T GetHandler<T>() where T : BaseHandler
        {
            return Activator.CreateInstance<T>();
        }
    }

    public class ConsumerA
    {
        private HandlerA _handlerA;

        public ConsumerA(HandlerA handlerA)
        {
            _handlerA = handlerA;
        }

        public void DoWork()
        {
            Console.WriteLine(_handlerA.Handle("Consumer A"));
        }
    }
    
    public class ConsumerB
    {
        private HandlerB _handlerB;

        public ConsumerB(HandlerB handlerB)
        {
            _handlerB = handlerB;
        }

        public void DoWork()
        {
            Console.WriteLine(_handlerB.Handle("Consumer B"));
        }
    }

    public class HandlerRegistrationSource : IRegistrationSource
    {
        public IEnumerable<IComponentRegistration> RegistrationsFor(
            Service service,
            Func<Service,IEnumerable<ServiceRegistration>> registrationAccessor)
        {
            var swt = service as IServiceWithType;
            if (swt == null || swt.ServiceType == null || !swt.ServiceType.IsAssignableTo<BaseHandler>())
            {
                yield break;
            }

            yield return new ComponentRegistration(
                Guid.NewGuid(),
                new DelegateActivator(swt.ServiceType, (c, p) =>
                {
                    var provider = c.Resolve<IHandlerFactory>();
                    var method = provider.GetType().GetMethod("GetHandler").MakeGenericMethod(swt.ServiceType);
                    return method.Invoke(provider, null);
                }
            ),
                new CurrentScopeLifetime(),
                InstanceSharing.None,
                InstanceOwnership.OwnedByLifetimeScope,
                new [] {service},
                new ConcurrentDictionary<string, object?>());
        }

        public bool IsAdapterForIndividualComponents => false;
    }
    
    public class Program
    {
        static void Main(string[] args)
        {
            var b = new ContainerBuilder();
            b.RegisterType<HandlerFactory>().As<IHandlerFactory>();
            b.RegisterSource(new HandlerRegistrationSource());
            b.RegisterType<ConsumerA>();
            b.RegisterType<ConsumerB>();
            
            using (var c = b.Build())
            {
                c.Resolve<ConsumerA>().DoWork();
                c.Resolve<ConsumerB>().DoWork();
            }
        }
    }
}