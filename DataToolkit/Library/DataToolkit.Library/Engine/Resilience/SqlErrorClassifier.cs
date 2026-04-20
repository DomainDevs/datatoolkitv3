using Microsoft.Data.SqlClient;

namespace DataToolkit.Library.Engine.Resilience;

public static class SqlErrorClassifier
{
    public static SqlErrorType Classify(Exception ex)
    {
        if (ex is not SqlException sqlEx)
            return SqlErrorType.Unknown;

        return sqlEx.Number switch
        {
            // TRANSIENT (SAFE TO RETRY)
            1205 => SqlErrorType.Transient, // Deadlock
            -2 => SqlErrorType.Transient, // Timeout
            40197 => SqlErrorType.Transient,
            40501 => SqlErrorType.Transient,
            40613 => SqlErrorType.Transient,
            10928 => SqlErrorType.Transient,
            10929 => SqlErrorType.Transient,

            // PERMANENT (NEVER RETRY)
            18456 => SqlErrorType.Permanent, // Login failed
            2627 => SqlErrorType.Permanent, // PK violation
            547 => SqlErrorType.Permanent, // Constraint
            208 => SqlErrorType.Permanent, // Invalid object
            207 => SqlErrorType.Permanent, // Invalid column

            _ => SqlErrorType.Unknown
        };
    }
}