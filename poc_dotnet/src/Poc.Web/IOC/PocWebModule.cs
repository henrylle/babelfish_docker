using MediatR;
using Poc.Web.Repository;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Poc.Web.IOC
{
    public static class PocWebModule
    {
        private static Container _container;

        public static Container StartModule(IsolationLevel level = IsolationLevel.ReadUncommitted)
        {
            InitContainer();

            _container.Register<IUnitOfWork>(() =>
            {
                var fortesDocContext = new PocContext();
                fortesDocContext.StartTransaction(level);
                return fortesDocContext;
            }, Lifestyle.Scoped);
            return _container;
        }

        public static Container StartTest()
        {
            InitContainer();
            _container.Register<IUnitOfWork, PocContext>(Lifestyle.Scoped);
            return _container;
        }

        private static void InitContainer()
        {
            var assembliesToScan = GetAssemblies().Skip(1);
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            var registrations = from type in assembliesToScan.SelectMany(a => a.ExportedTypes).Select(t => t.GetTypeInfo())
                                let @interface = type.ImplementedInterfaces.FirstOrDefault(inter => inter.Name == $"I{type.Name}")
                                where @interface != null && type.IsClass && !type.IsGenericType
                                select (@interface, type.AsType());

            foreach (var reg in registrations)
                _container.Register(reg.Item1, reg.Item2, Lifestyle.Scoped);

            BuildMediator(_container);

            _container.Register(typeof(IRepository<>), typeof(Repository<>), Lifestyle.Scoped);
        }

        private static void BuildMediator(Container container)
        {
            var assemblies = GetAssemblies().ToArray();
            container.RegisterSingleton<IMediator, Mediator>();
            container.Register(typeof(IRequestHandler<,>), assemblies);
            container.Register(typeof(IRequestHandler<>), assemblies);
            container.Collection.Register(typeof(INotificationHandler<>), assemblies);
            container.Collection.Register(typeof(IPipelineBehavior<,>), assemblies);
            container.RegisterInstance(new ServiceFactory(container.GetInstance));
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(IMediator).GetTypeInfo().Assembly;
            yield return typeof(PocContext).GetTypeInfo().Assembly;
        }
    }
}
