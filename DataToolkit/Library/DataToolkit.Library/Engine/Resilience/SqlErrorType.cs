namespace DataToolkit.Library.Engine.Resilience;

public enum SqlErrorType
{
    Transient,      // seguro para retry
    Permanent,      // nunca retry
    Unknown         // por defecto NO retry
}
