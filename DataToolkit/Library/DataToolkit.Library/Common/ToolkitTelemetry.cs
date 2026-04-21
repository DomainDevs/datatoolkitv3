using System.Diagnostics;
using Serilog;

namespace DataToolkit.Library.Common;

internal static class ToolkitTelemetry
{
    public static Stopwatch? Iniciar(DataToolkitOptions opt)
        => opt.Telemetry.Enabled ? Stopwatch.StartNew() : null;

    public static void Finalizar(
        ILogger logger,
        DataToolkitOptions opt,
        string accion,
        Stopwatch? sw)
    {
        Log(logger, opt, accion, null, sw);
    }

    public static void Error(
        ILogger logger,
        DataToolkitOptions opt,
        string accion,
        Exception ex,
        Stopwatch? sw)
    {
        Log(logger, opt, accion, ex, sw);
    }

    private static void Log(
        ILogger logger,
        DataToolkitOptions opt,
        string accion,
        Exception? ex,
        Stopwatch? sw)
    {
        if (sw == null) return;

        sw.Stop();
        var ms = sw.ElapsedMilliseconds;

        var prefix = opt.Logging.Prefix;

        if (ex != null)
        {
            logger.Error(
                ex,
                "[{Prefix}] ❌ ERROR: {Accion} ({ms}ms). {Msg}",
                prefix, accion, ms, ex.Message);
            return;
        }

        if (!opt.Logging.Enabled) return;

        if (ms >= opt.Telemetry.SlowMs)
        {
            logger.Warning(
                "[{Prefix}] ⚠️ RENDIMIENTO: {Accion} tardó {ms}ms",
                prefix, accion, ms);
        }
        else
        {
            logger.Information(
                "[{Prefix}] {Accion} finalizada ({ms}ms)",
                prefix, accion, ms);
        }
    }
}