using SimpleInjector;

namespace Poc.Test
{
    internal class SimpleInjectorViewComponentActivator
    {
        private Container container;

        public SimpleInjectorViewComponentActivator(Container container)
        {
            this.container = container;
        }
    }
}