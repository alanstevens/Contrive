using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using log4net.Config;

namespace Contrive.Common
{
  public class Logger : IStartupTask
  {
    public const string CONFIG_FILE_NAME = "Log4Net.config";

    static bool _logInitialized;
    static readonly Dictionary<Type, ILog> _loggers = new Dictionary<Type, ILog>();

    public static Action<object, object> Debug = (source, message) => { };
    public static Action<object, object> Error = (source, message) => { };
    public static Action<object, object> Fatal = (source, message) => { };
    public static Action<object, object> Info = (source, message) => { };
    public static Action<object, object> Warn = (source, message) => { };
    public static Action<object> Trace = message => { };

    ILog _traceLogger;

    ILog TraceLogger { get { return _traceLogger ?? (_traceLogger = LogManager.GetLogger("Trace")); } }

    public void OnStartup()
    {
      Debug = (source, message) => GetLogger(GetSourceType(source)).Debug(message);
      Error = (source, message) => GetLogger(GetSourceType(source)).Error(message);
      Fatal = (source, message) => GetLogger(GetSourceType(source)).Fatal(message);
      Info = (source, message) => GetLogger(GetSourceType(source)).Info(message);
      Warn = (source, message) => GetLogger(GetSourceType(source)).Warn(message);
      Trace = message => TraceLogger.Info(message);
    }

    static Type GetSourceType(object source)
    {
      var sourceType = source.GetType();
      if (sourceType == typeof (Type)) return source as Type;
      return sourceType;
    }

    static void Initialize()
    {
      XmlConfigurator.ConfigureAndWatch(new FileInfo(GetConfigFilePath()));
    }

    static string GetConfigFilePath()
    {
      var basePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
      var configPath = Path.Combine(basePath, CONFIG_FILE_NAME);

      if (!File.Exists(configPath))
      {
        configPath = Path.Combine(basePath, "bin");
        configPath = Path.Combine(configPath, CONFIG_FILE_NAME);

        if (!File.Exists(configPath)) configPath = Path.Combine(basePath, @"..\" + CONFIG_FILE_NAME);
      }

      return configPath;
    }

    static void EnsureInitialized()
    {
      if (_logInitialized) return;

      Initialize();
      _logInitialized = true;
    }

    static ILog GetLogger(Type source)
    {
      EnsureInitialized();

      if (!_loggers.ContainsKey(source))
      {
        lock (_loggers)
        {
          if (!_loggers.ContainsKey(source)) _loggers.Add(source, LogManager.GetLogger(source));
        }
      }

      return _loggers[source];
    }
  }
} ;