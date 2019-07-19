using System;
using System.IO;

namespace VeterisDeviceServer.Diagnostics
{

    /// <summary>
    /// Clase de ayuda para manejo de logging en la aplicación
    /// </summary>
    internal class Logging
    {

        /// <summary>
        /// Directorio default para logs
        /// </summary>
        public const string LOG_FILE_DIR = "./logs/";

        /// <summary>
        /// Nombre default para el ultimo log
        /// </summary>
        public const string LOG_FILE_NAME = "latest.log";

        /// <summary>
        /// Propiedad de ayuda que retorna el tiempo actual en formato ISO-8601
        /// </summary>
        public static string CurrentISOTime => DateTime.Now.ToString("O");

        /// <summary>
        /// Archivo actual de Log
        /// </summary>
        public static StreamWriter LogFile
        {
            get
            {
                if (_logFileStreamWriter == null) {
                    _logFileStreamWriter = LoadLogFile();
                }

                return _logFileStreamWriter;
            }
        }

        protected static StreamWriter _logFileStreamWriter;

        public static string FormatMessage(string message, VerbosityLevel verbosity)
        {
            return $"<{CurrentISOTime}> {verbosity.ToString().ToUpper()}: {message}";
        }

        public static string FormatMessagePartial(string message, VerbosityLevel verbosity)
        {
            return $"<{CurrentISOTime}> Partial {verbosity.ToString().ToUpper()}: {message}";
        }

        /// <summary>
        /// Enviar al buffer del archivo de Log un mensaje con formato de acuerdo a la verbosidad y timestamp, luego escribir en disco
        /// </summary>
        /// <param name="message">Mensaje a escribir en el archivo</param>
        /// <param name="verbosity">Nivel de verbosidad para el mensaje ( Importancia )</param>
        public static void WriteLine(string message, VerbosityLevel verbosity)
        {
            LogFile.WriteLine(FormatMessage(message, verbosity));
            LogFile.Flush();
        }

        /// <summary>
        /// Enviar al buffer del archivo de Log un mensaje parcial con formato de acuerdo a la verbosidad y timestamp, luego escribir en disco
        /// </summary>
        /// <param name="message">Mensaje a escribir en el archivo</param>
        /// <param name="verbosity">Nivel de verbosidad para el mensaje ( Importancia )</param>
        public static void Write(string message, VerbosityLevel verbosity)
        {
            LogFile.WriteLine(FormatMessagePartial(message, verbosity));
            LogFile.Flush();
        }

        /// <summary>
        /// Liberar recursos de logging como el archivo de Log
        /// </summary>
        public static void FreeResources()
        {
            if (_logFileStreamWriter != null) {
                _logFileStreamWriter.Dispose();
                _logFileStreamWriter = null;
            }
        }

        /// <summary>
        /// Cargar archivo de log basado en el filepath por default
        /// </summary>
        /// <returns>Archivo de log cargado</returns>
        protected static StreamWriter LoadLogFile()
        {
            string latestLogPath = Path.Combine(LOG_FILE_DIR, LOG_FILE_NAME);

            if (!Directory.Exists(LOG_FILE_DIR)) {
                Directory.CreateDirectory(LOG_FILE_DIR);
            }

            if (File.Exists(latestLogPath)) {
                MoveLogFile(latestLogPath);
            }

            FileStream logFile = File.Create(latestLogPath);
            return new StreamWriter(logFile);
        }

        /// <summary>
        /// Mover ultimo archivo de Log a la carpeta por default con un timestamp como nombre
        /// </summary>
        /// <param name="latestLogPath">Ruta al ultimo archivo de Log</param>
        protected static void MoveLogFile(string latestLogPath)
        {
            DateTime writeTime = File.GetLastWriteTimeUtc(latestLogPath);
            string lastLogNewFilename = $"log-{writeTime:yyyy-dd-M--HH-mm-ss}.log";
            string lastLogNewFilepath = Path.Combine(LOG_FILE_DIR, lastLogNewFilename);
            File.Move(latestLogPath, lastLogNewFilepath);
        }

    }
}
