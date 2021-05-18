using NLog;
using NLog.Common;
using NLog.Targets;
using System;

namespace NLogWPFViewer
{
    [Target("NLogWPFViewerTarget")]
    public class NLogWPFViewerTarget : Target
    {
        public event Action<AsyncLogEventInfo> LogReceived;

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            base.Write(logEvent);

            if (LogReceived != null)
                LogReceived(logEvent);
        }
    }

    public class NLogEvenArgs : EventArgs
    {
        public LogEventInfo LogItem { get; private set; }
        public NLogEvenArgs(LogEventInfo item)
        {
            LogItem = item;
        }
    }
}
