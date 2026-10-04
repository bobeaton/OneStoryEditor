using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// routes messages from a page to handlers by 'type'. One bad message must never take down the message loop,
    /// so unknown types are logged and handler exceptions are caught and reported
    /// </summary>
    public class HtmlMessageDispatcher
    {
        private readonly Dictionary<string, Action<HtmlMessage>> _handlers = new Dictionary<string, Action<HtmlMessage>>();

        // set by the owner (e.g. to put the error on the status bar); defaults to the debug log
        public Action<string> ReportError { get; set; }

        public IReadOnlyCollection<string> RegisteredTypes => _handlers.Keys;

        public void Register(string strType, Action<HtmlMessage> handler)
        {
            _handlers[strType] = handler;
        }

        public bool Dispatch(HtmlMessage msg)
        {
            if ((msg == null) || !_handlers.TryGetValue(msg.Type, out var handler))
            {
                Debug.WriteLine("HtmlMessageDispatcher: no handler for " + msg);
                return false;
            }

            try
            {
                handler(msg);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"HtmlMessageDispatcher: '{msg.Type}' threw {ex}");
                (ReportError ?? (s => Debug.WriteLine(s)))(ex.Message);
            }
            return true;
        }
    }
}
