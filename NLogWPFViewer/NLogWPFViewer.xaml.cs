using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NLog;
using NLog.Common;

namespace NLogWPFViewer
{
    public partial class NLogWPFViewer : UserControl
    {
        public event EventHandler<NLogEvenArgs> ItemAdded;

        ObservableCollection<LogEventInfo> LogEntries { get; set; }

        public LogEventInfo[] GetLogEntries()
        {
            return LogEntries.ToArray();
        }

        public int MaxCount
        {
            get { return (int)GetValue(MaxCountProperty); }
            set { SetValue(MaxCountProperty, value); }
        }

        public static readonly DependencyProperty MaxCountProperty =
            DependencyProperty.Register("MaxCount", typeof(int), typeof(NLogWPFViewer), new PropertyMetadata(1000, OnMaxCountChanged));
        static void OnMaxCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as NLogWPFViewer;
            var value = (int)e.NewValue;
            c.MaxCount = Math.Min(50, value);
        }

        public bool AutoScroll
        {
            get { return (bool)GetValue(AutoScrollProperty); }
            set { SetValue(AutoScrollProperty, value); }
        }

        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.Register("AutoScroll", typeof(bool), typeof(NLogWPFViewer), new PropertyMetadata(true));

        public string TargetName
        {
            get { return (string)GetValue(TargetNameProperty); }
            set { SetValue(TargetNameProperty, value); }
        }

        public static readonly DependencyProperty TargetNameProperty =
            DependencyProperty.Register("TargetName", typeof(string), typeof(NLogWPFViewer), new PropertyMetadata(null, TargetNameChanged));

        // NB: this wont fire if TragetName is not set by the control user, regardles of the default value,
        //     so the cold run of SubscribeTarget in constructor is necessary to bind targets just in case the TargetName is never set
        static void TargetNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as NLogWPFViewer;
            var v = e.NewValue as string;
            //Debug.WriteLine($"NLogWPFViewer (${c.GetHashCode()}) Target name changed: {v}");
        
            c.SubscribeTarget(v);
        }

        public NLogWPFViewer()
        {
            LogEntries = new ObservableCollection<LogEventInfo>();

            InitializeComponent();

            // in case the dep prop is not set by its user
            SubscribeTarget(null);

            dg.ItemsSource = LogEntries;
        }

        void SubscribeTarget(string name)
        {
            foreach (var target in LogManager.Configuration.AllTargets.OfType<NLogWPFViewerTarget>())
            {
                target.LogReceived -= LogReceived;

                if (string.IsNullOrWhiteSpace(name) || target.Name.StartsWith(name))
                {
                    target.LogReceived += LogReceived;

                    //Debug.WriteLine($"NLogWPFViewer (${this.GetHashCode()}) Target bound {target.Name}");
                }
            }
        }

        void LogReceived(AsyncLogEventInfo entry)
        {
            Dispatcher.BeginInvoke(new Action(() => AddEntry(entry)));
        }

        private void AddEntry(AsyncLogEventInfo log)
        {
            if (LogEntries.Count >= MaxCount)
                LogEntries.RemoveAt(0);

            LogEntries.Add(log.LogEvent);

            if (AutoScroll)
                ScrollToItem(log.LogEvent);

            ItemAdded?.Invoke(this, new NLogEvenArgs(log.LogEvent));
        }

        public void Clear()
        {
            LogEntries.Clear();
        }

        private void ScrollToItem(LogEventInfo item)
        {
            dg.ScrollIntoView(item);
        }
    }
}
