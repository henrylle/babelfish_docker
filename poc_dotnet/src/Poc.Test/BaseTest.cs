using NUnit.Framework;
using Poc.Web.IOC;
using Poc.Web.Repository;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;
using System.Data;

namespace Poc.Test
{
  public abstract class BaseTest
  {
    protected static Container _container;
    protected IUnitOfWork _unitOfWork;
    protected Scope _containerScope;

    [SetUp]
    public virtual void TestInitialize()
    {
      _container = PocWebModule.StartTest();
      _container.Options.AllowOverridingRegistrations = true;
      _container.Options.ResolveUnregisteredConcreteTypes = true;
      _containerScope = AsyncScopedLifestyle.BeginScope(_container);
      _container.Register(() => new SimpleInjectorControllerActivator(_container), Lifestyle.Singleton);
      _container.Register(() => new SimpleInjectorViewComponentActivator(_container), Lifestyle.Singleton);

      _unitOfWork = _container.GetInstance<IUnitOfWork>();
      _unitOfWork.Database.EnsureCreated();
      //_unitOfWork.StartTransaction(IsolationLevel.ReadUncommitted);
    }

    [TearDown]
    public virtual void TestCleanup()
    {
      //_unitOfWork?.Commit();
      _containerScope.Dispose();
      _container.Dispose();
    }
  }
}