using log4net;
using log4net.Repository.Hierarchy;
using log4net.Core;
using log4net.Appender;
using log4net.Layout;

namespace JanusApi
{
  public class Logger
  {
    private PatternLayout _layout = new PatternLayout();
    private const string LOG_PATTERN = "%date [%thread] %-5level %logger %message%newline";

    public string DefaultPattern
    {
      get { return LOG_PATTERN; }
    }

    public Logger()
    {
      _layout.ConversionPattern = DefaultPattern;
      _layout.ActivateOptions();
    }

    public PatternLayout DefaultLayout
    {
      get { return _layout; }
    }

    public void AddAppender(IAppender appender)
    {
      Hierarchy hierarchy =
        (Hierarchy)LogManager.GetRepository();

      hierarchy.Root.AddAppender(appender);
    }

    static Logger()
    {
      Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
      TraceAppender tracer = new TraceAppender();
      PatternLayout patternLayout = new PatternLayout();

      patternLayout.ConversionPattern = LOG_PATTERN;
      patternLayout.ActivateOptions();

      tracer.Layout = patternLayout;
      tracer.ActivateOptions();
      hierarchy.Root.AddAppender(tracer);

      RollingFileAppender roller = new RollingFileAppender();
      roller.Layout = patternLayout;
      roller.AppendToFile = true;
      roller.RollingStyle = RollingFileAppender.RollingMode.Size;
      roller.MaxSizeRollBackups = 4;
      roller.MaximumFileSize = "100KB";
      roller.StaticLogFileName = true;
      roller.File = "C:\\ProgramData\\Lattice Inc\\JanusApi\\JanusApi.txt";
      roller.ActivateOptions();
      hierarchy.Root.AddAppender(roller);

      hierarchy.Root.Level = Level.Debug;
      hierarchy.Configured = true;
    }

    public static ILog Create()
    {
      return LogManager.GetLogger("JanusApi");
    }
  }
}
