using Autofac;

namespace Sensor.Api.Host
{
    public class AutofacResolver : IResolver
    {
        private readonly ILifetimeScope _scope;

        public AutofacResolver(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public T Resolve<T>()
        {
            return _scope.Resolve<T>();
        }
    }
}