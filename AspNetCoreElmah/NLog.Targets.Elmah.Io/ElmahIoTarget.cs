using System;
using System.Collections.Generic;
using System.Linq;
using Elmah.Io.Client;
using Elmah.Io.Client.Models;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Layouts;
using NLog.Targets;

namespace Elmah.Io.NLog
{
    [Target("elmah.io")]
    public class ElmahIoTarget : TargetWithLayout
    {
        private IElmahioAPI _client;

        [RequiredParameter]
        public string ApiKey { get; set; }

        [RequiredParameter]
        public string LogId { get; set; }

        public string Application { get; set; }

        public ElmahIoTarget()
        {
        }

        public ElmahIoTarget(IElmahioAPI client)
        {
            _client = client;
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if (_client == null)
            {
                _client = ElmahioAPI.Create(ApiKey);
            }

            var title = Layout != null && Layout.ToString() != "'${longdate}|${level:uppercase=true}|${logger}|${message}'"
                ? Layout.Render(logEvent)
                : logEvent.FormattedMessage;

            var message = new CreateMessage
            {
                Title = title,
                Severity = LevelToSeverity(logEvent.Level),
                DateTime = logEvent.TimeStamp.ToUniversalTime(),
                Detail = logEvent.Exception?.ToString(),
                Data = PropertiesToData(logEvent.Properties),
                Source = logEvent.LoggerName,
                User = Identity(logEvent),
                Hostname = MachineName(logEvent),
                Application = Application,
            };

            _client.Messages.CreateAndNotify(new Guid(LogId), message);
        }

        private string Identity(LogEventInfo logEvent)
        {
            // TODO
            return string.Empty;

            //var renderer = new IdentityLayoutRenderer
            //{
            //    Name = true,
            //    AuthType = false,
            //    IsAuthenticated = false
            //};
            //var user = renderer.Render(logEvent);
            //return string.IsNullOrWhiteSpace(user) ? null : user;
        }

        private string MachineName(LogEventInfo logEvent)
        {
            return new MachineNameLayoutRenderer().Render(logEvent);
        }

        private List<Item> PropertiesToData(IDictionary<object, object> properties)
        {
            var items = new List<Item>();
            foreach (var obj in properties)
            {
                if(obj.Value != null)
                {
                    items.Add(new Item { Key = obj.Key.ToString(), Value = obj.Value.ToString() });
                }
            }
            return items;
        }

        private string LevelToSeverity(LogLevel level)
        {
            if (level == LogLevel.Debug) return Severity.Debug.ToString();
            if (level == LogLevel.Error) return Severity.Error.ToString();
            if (level == LogLevel.Fatal) return Severity.Fatal.ToString();
            if (level == LogLevel.Trace) return Severity.Verbose.ToString();
            if (level == LogLevel.Warn) return Severity.Warning.ToString();
            return Severity.Information.ToString();
        }
    }
}
