using Microsoft.Data.SqlClient;
using Serilog;

namespace DataToolkit.Library.Engine.Resilience;

public static class SqlErrorClassifier
{
    public static SqlErrorType Classify(Exception ex)
    {
        if (ex is not SqlException sqlEx)
            return SqlErrorType.Unknown;

        return sqlEx.Number switch
        {
            // RETRY SOLO SI ES 100% SEGURO (INFRA)
            1205 => SqlErrorType.Transient, // Deadlock
            -2 => SqlErrorType.Unknown,     // Timeout (NO seguro para write retry)

            // Azure infra issues
            40197 => SqlErrorType.Transient,
            40501 => SqlErrorType.Transient,
            40613 => SqlErrorType.Transient,
            10928 => SqlErrorType.Transient,
            10929 => SqlErrorType.Transient,

            // NO RETRY (DATA SAFETY)
            18456 => SqlErrorType.Permanent,    // Login failed
            2627 => SqlErrorType.Permanent,     //PK violation / UNIQUE constraint
            547 => SqlErrorType.Permanent,      //Foreign Key constraint violation
            208 => SqlErrorType.Permanent,      //Invalid object name
            207 => SqlErrorType.Permanent,      //Invalid column name

            _ => SqlErrorType.Unknown
        };
    }
}
