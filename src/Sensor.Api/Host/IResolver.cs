namespace Sensor.Api.Host
{
    public interface IResolver
    {
        T Resolve<T>();
    }
}