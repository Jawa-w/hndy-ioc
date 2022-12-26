**Hndy.Ioc** is a lightweight IoC container based on C# source generators. *Hndy* is a variant of the word *Handy*. Let's just pronounce it as *Handy*. Hndy.Ioc has the following features:

- **No runtime reflection**. Instead Hndy.Ioc use the C# source generators to improve performance, and facilitate assembly trimming and obfuscation.
- **Auto-wiring with wieres**. Powerful partial auto-wiring are supported with Hndy.Ioc Wirers.

# Table of Contents <!-- omit in toc -->

- [Get Start in 1 minute](#get-start-in-1-minute)
- [Auto-Wiring](#auto-wiring)
  - [Wirers](#wirers)
  - [Build it yourself](#build-it-yourself)
- [Liftcycles](#liftcycles)
  - [Singleton](#singleton)
  - [Scoped](#scoped)
  - [Transient](#transient)
  - [IDisposable](#idisposable)
- [Named Instances \& Parameterized Instances](#named-instances--parameterized-instances)
- [Implicit Registrations](#implicit-registrations)
- [Other things you should know](#other-things-you-should-know)
  - [Registration Overrides](#registration-overrides)
  - [Child Container](#child-container)
  - [Null Instance](#null-instance)
  - [Null Name Instance](#null-name-instance)
  - [Circular Dependency](#circular-dependency)
  - [Late Injection](#late-injection)

# Get Start in 1 minute

All Services should be registered in a derived class from `IocRegistration`.

```c#
partial class MyRegistration : IocRegistration
{
}
```

You can register services in constructor method:

```c#
public MyRegistration()
{
    SingletonFor<IService>().Use<Service>();
}
```

Then create an `IocContainer`:

```c#
var container = new IocContainer(new MyRegistration());
```

Get services:

```c#
var service = container.Get<IService>();
// or
bool got = container.TryGet<IService>(out var service);
// or
IService? service = container.TryGet<IService>();
```

Parameters are supported too. Consider a *Logger* Service:

```c#
class Logger : ILogger
{
    public Logger(string name) { }
}
```

It can be registered as a parameterized instance:

```c#
public MyRegistration()
{
    TransientFor<ILogger, string>().Use<Logger>();
}
```

Get a logger:

```c#
var logger = container.Get<ILogger>("loggerName");
```

# Auto-Wiring
Auto-Wiring is the best way to build your service with IoC container. You don't have to specify the constructor and each parameters. IoC container will find the constructor and provide parameters automatically.

For example, there is a *Foobar* service:

```c#
public class Foobar : IFoobar
{
    public Foobar(Foo foo, Bar bar)
    {
    }
}
```

You can register it in auto-wiring way:

```c#
SingletonFor<IFoobar>().Use<Foobar>();
```

> Restriction: to use auto-wiring, service must have only one public constructor, or only one internal constructor if no public ones.

Or register it with a wirer to control more:

```c#
SingletonFor<IFoobar>().Use<Foobar>(new Wirers.Foobar()
{
    foo = new Foo()
});
```

## Wirers

Wirers are generated automatically by C# source generators while you writing your registeration code. It wraps contructor into a `IIocWirer` class, and converts each construct parameters into properties. You don't have to specify all properties, specify those you are going to, and leave others to IoC container.

You can not only specify parameter value directly, but also ask IoC container to build one if the parameter itself is a registered service.

```c#
SingletonFor<IFoobar>().Use<Foobar>(new Wirers.Foobar()
{
    // A named Foo instance will be provided as the 'foo' parameter of Foobar constructor.
    // C# 9.0 must be enabled. Otherwise: foo = new WirerParameter<Foo>("name")
    foo = new("name")
});
```

The 'bar' parameter you left to IoC container is equivalent to:

```c#
SingletonFor<IFoobar>().Use<Foobar>(new Wirers.Foobar()
{
    foo = new("name"),
    bar = new()
});
```

If you wanna specify a *null* value, just set *null*:

```c#
SingletonFor<IFoobar>().Use<Foobar>(new Wirers.Foobar()
{
    foo = null
});
```

## Build it yourself

Of course you are able to build services yourself.

Register a built service instance:

```c#
Singleton<IService>(new Service());
```

Register a lazy instance with a delegate:

```c#
Singleton<IService>(() => new Service());
Singleton<IService>(session => new Service(session.Get<IDependency>()));
```

# Liftcycles

There are 3 kind of service lifecycle in Hndy.Ioc. They are `Singleton`, `Scoped` and `Transient`.

## Singleton

If a service is registered as singleton. Only one instance will be created in an IoC container. The same instance will be returned when every time you get it. Singleton instances can be registered with the following method overrides:

```c#
Singleton<Service>();
Singleton<IService>(new Service());
Singleton<IService>(() => new Service());
Singleton<IService>(session => new Service(session.Get<IDependency>()));
SingletonFor<IService>().Use<Service>();
SingletonFor<IService>().Use<Service>(new Wirers.Service() { para = new() });
```

## Scoped

In Hndy.Ioc you can create an `IocScope` from an IoC container:

```c#
var scope = container.NewScope();
var service = scope.Get<Service>();
```

Every time you get a service instance from IoC container directly, not from scope, an implicit scope is also created, and service instance is gotten from the implicit scope in fact.

If a service is registered as scoped. Only one instance will be created in an IoC scope. The same instance will be returned when every time you get it from the scope. Scoped instances can be registered with the following method overrides:

```c#
Scoped<Service>();
Scoped<IService>(() => new Service());
Scoped<IService>(session => new Service(session.Get<IDependency>()));
ScopedFor<IService>().Use<Service>();
ScopedFor<IService>().Use<Service>(new Wirers.Service() { para = new() });
```

## Transient

If a service is registered as transient. A new instance will be created when every time you get it. Transient instances can be registered with the following method overrides:

```c#
Transient<Service>();
Transient<IService>(() => new Service());
Transient<IService>(session => new Service(session.Get<IDependency>()));
TransientFor<IService>().Use<Service>();
TransientFor<IService>().Use<Service>(new Wirers.Service() { para = new() });
```

## IDisposable

If a singleton service implements *IDisposable* interface, it will be disposed when you call `IocContainer.Dispose()`. If a scoped service implements *IDisposable* interface, it will be disposed when you call `IocScope.Dispose()`. Hndy.Ioc never track transient services.

# Named Instances & Parameterized Instances

Beside of default unnamed instance, you can register several named instances meanwhile.

```c#
// register named instances in auto-wiring way.
Singleton<Service, string>();
// register a named instance with the specified name.
Singleton<IService, string>("red", new RedService());
// register named instances with a delegate.
Singleton<IService, string>(s => new Service(s));
```

Named instances use name as the key of registerations. So the name has a type restriction. It must implements *IEquatable<T>* interface. Hndy.Ioc use this interface to check the equality of the names.

Parameterized instances are similar but without type restriction. However it has a lifecycle restriction. Parameterized instances are only supported in *Transient* lifecycle.

```c#
// register parameterized instances in auto-wiring way.
Transient<Service, ServiceOptions>();
// register parameterized instances with a delegate.
Transient<IService, ServiceOptions>(options => new Service(options));
```

# Implicit Registrations

In Hndy.Ioc, you have to explictly register every service before getting it, except:

- *IServiceLocator*，it returns the container itself.
- *IIocContainer*, it returns the container itself.
- *IIocScope*, it returns a *Transient* scope instance created by *IocContainer.NewScope()* method.
  
Actually, when you register a service in Hndy.Ioc, you can get it back later, and also a delegate for lazy creation:

```c#
// get a service
IService service = container.Get<IService>();
// get a delegate
Func<IService> serviceCreator = container.Get<Func<IService>>();
```

# Other things you should know

## Registration Overrides

When multiple instances are registered in one or multiple *IocRegistration* class to create *IocContainer*, the last one win.

```c#
// public MyRegistrations1()
// {
//     Singleton<IService>(new Service1());
// }
// public MyRegistrations2()
// {
//     Singleton<IService>(new Service2());
//     Singleton<IService>(new Service3());
// }

var container = new IocContainer(new MyRegistrations1(), new MyRegistrations2());
var service = container.Get<IService>(); // service is type of Service3.
```

## Child Container

A parent container can be specified when creating *IocContainer*. Services can be gotten from parent container if the child container missing the registration.

```c#
IocContainer parent;
IocContainer child = new IocContainer(parent, new MyRegistrations());
```

## Null Instance

When you register a null instance, nothing registered. The service has already been registerd before won't be overrided to *null*.

## Null Name Instance

When you register a named instance with a *null* name. It equals reistering the default unnamed instance.

```c#
Singleton<IService, string>(null, new Service());
// equals
Singleton<IService>(new Service());
```

## Circular Dependency

Circular dependencies will be detected when getting services in Hndy.Ioc, and then an *IocCircularDependenciesException* throws.

## Late Injection

In Hndy.Ioc all registerations must be done before creating container. After creation, registrations cannot be modified any more. However you can still inject *Singletion* services:

```c#
container.InjectSingleton<IService>(new Service());
```


