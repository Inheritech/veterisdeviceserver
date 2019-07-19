using System;

namespace VeterisDeviceServer.Diagnostics
{

    /// <summary>
    /// Enumeración de niveles de verbosidad
    /// </summary>
    internal enum VerbosityLevel
    {
        Debug = 0,
        Default = 1,
        Info = 2,
        Warning = 3,
        Error = 4
    }

    /// <summary>
    /// Clase de apoyo para logging de eventos en base a verbosidad
    /// </summary>
    internal static class Debug
    {

        /// <summary>
        /// Escribir una linea de mensaje en consola y guardarlo en el log del programa con un nivel de verbosidad Default
        /// incluyendo el nombre del <see cref="Type"/> que lo emite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        public static void WriteLine(object sender, string message)
        {
            WriteLine(message, VerbosityLevel.Default, true, sourceName: sender.ToSourceName());
        }

        /// <summary>
        /// Escribir una linea de mensaje en consola y guardarlo en el log del programa con un nivel de verbosidad Default
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLine(string message)
        {
            WriteLine(message, VerbosityLevel.Default, true);
        }

        /// <summary>
        /// Escribir una linea de mensaje en consola y guardarlo en el log del programa sin suprimir el prefijo de importancia e incluyendo
        /// el nombre del <see cref="Type"/> que lo emite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <param name="verbosity"></param>
        public static void WriteLine(object sender, string message, VerbosityLevel verbosity)
        {
            WriteLine(message, verbosity, false, sourceName: sender.ToSourceName());
        }

        /// <summary>
        /// Escribir una linea de menesaje en consola y guardarlo en el log del programa sin suprimir el prefijo de importancia
        /// </summary>
        /// <param name="message"></param>
        /// <param name="verbosity"></param>
        public static void WriteLine(string message, VerbosityLevel verbosity)
        {
            WriteLine(message, verbosity, false);
        }

        /// <summary>
        /// Escribir una linea de mensaje en consola y guardarlo en el log del programa
        /// </summary>
        /// <param name="message">Mensaje a escribir</param>
        /// <param name="verbosity">Nivel de verbosidad del mensaje</param>
        /// <param name="supressPrefix">Especifica si se debe suprimir el prefijo de importancia</param>
        /// <param name="colorize">Especifica si se debe de colorear el output según la importancia</param>
        public static void WriteLine(string message, VerbosityLevel verbosity, bool supressPrefix, bool colorize = true, string sourceName = null)
        {
            Logging.WriteLine(message, verbosity);

            string prefix = string.Empty;

            if (sourceName != null)
                prefix = $"[{sourceName}] ";

            if (!supressPrefix && verbosity > 0) {
                prefix += $"{verbosity.ToString().ToUpper()}: ";
            }

            ConsoleColor foreground = Console.ForegroundColor;
            ConsoleColor background = Console.BackgroundColor;

            if (colorize) {
                switch (verbosity) {
                    case VerbosityLevel.Default:
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case VerbosityLevel.Info:
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case VerbosityLevel.Warning:
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case VerbosityLevel.Error:
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    default:
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                }

            }

            if (verbosity < VerbosityLevel.Error) {
                Console.WriteLine(prefix + message);
            } else {
                Console.Error.WriteLine(prefix + message);
            }

            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }

        /// <summary>
        /// Escribir mensaje en consola y guardarlo en el log del programa
        /// </summary>
        /// <param name="message">Mensaje a escribir</param>
        /// <param name="verbosity">Nivel de verbosidad del mensaje</param>
        public static void Write(string message, VerbosityLevel verbosity)
        {
            Logging.Write(message, verbosity);
            Console.Write(message);
        }

        /// <summary>
        /// Convertir un objeto en un nombre de fuente en base a su nombre de tipo o en caso que sea un string, devolver el valor
        /// </summary>
        /// <param name="sender">Objeto emisor de un mensaje</param>
        public static string ToSourceName(this object sender)
        {
            string sourceName = null;
            if (sender is string senderName) {
                sourceName = senderName;
            } else {
                sourceName = sender.GetType().Name;
            }
            return sourceName;
        }

    }
}
