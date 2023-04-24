namespace James.Core;

/// <summary>
/// Marks a Service as Dependency that gets automatically registered
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class DependencyServiceAttribute : Attribute
{
    public DependencyLifetime Lifetime { get; }

    public DependencyServiceAttribute(DependencyLifetime lifetime)
    {
        Lifetime = lifetime;
    }
}

/// <summary>
/// Marks a Service as singleton dependency
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class SingletonDependency : DependencyServiceAttribute
{
    /// <inheritdoc />
    public SingletonDependency() : base(DependencyLifetime.Singleton) {}
}

/// <summary>
/// Marks a Service as scoped dependency
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ScopedDependency : DependencyServiceAttribute
{
    public ScopedDependency() : base(DependencyLifetime.Scoped) {}
}
/// <summary>
/// Marks a Service as transient dependency
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class TransientDependency : DependencyServiceAttribute
{
    public TransientDependency() : base(DependencyLifetime.Transient) {}
}
/// <summary>
/// Lifetime of a Service
/// </summary>
public enum DependencyLifetime
{
    /// <summary> Created once, reused every time </summary>
    Singleton,
    /// <summary> Create once per Request, reused while in Scope </summary>
    Scoped,
    /// <summary> Created every time the Service is requested </summary>
    Transient
}
