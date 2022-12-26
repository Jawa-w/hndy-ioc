namespace Hndy.Ioc
{
    public interface IIocWirer
    {
        object Create(IIocSession session);
    }

    public interface IIocWirer<TService> : IIocWirer where TService : notnull
    {
    }
}