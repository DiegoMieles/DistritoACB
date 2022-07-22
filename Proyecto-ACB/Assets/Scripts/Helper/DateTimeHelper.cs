using System;

/// <summary>
/// Clase con utilidades relacionadas a componentes DateTime y fechas en general
/// </summary>
public static class DateTimeHelper
{
    /// <summary>
    /// Convierte una fecha específica al formato Unix
    /// </summary>
    /// <param name="value">Fecha a convertir</param>
    /// <returns>Retorna la fecha en formato Unix</returns>
    public static long ToUnixTimestamp(this DateTime value)
    {
        return (long)(value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }

    /// <summary>
    /// Obtiene la marca de tiempo Unix representando el tiempo actual
    /// </summary>
    /// <param name="ignored">Parametro ignorado</param>
    /// <returns>Marca de tiempo Unix representando el tiempo actual</returns>
    public static long UnixTimestamp(this DateTime ignored)
    {
        return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }

    /// <summary>
    /// Devuelve la fecha actual en formato local
    /// </summary>
    /// <param name="timestamp">Marca de tiempo Unix/posix</param>
    /// <returns>Fecha locale</returns>
    public static DateTime ParseUnixTimestamp(long timestamp)
    {
        return (new DateTime(1970, 1, 1)).AddSeconds(timestamp);
    }

}