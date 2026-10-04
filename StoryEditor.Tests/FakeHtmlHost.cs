using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OneStoryProjectEditor.Tests
{
    /// <summary>
    /// records what C# posts and requests; answers requests with OnRequest; lets a test raise page messages
    /// </summary>
    public class FakeHtmlHost : IHtmlHost
    {
        public readonly List<HtmlMessage> Posts = new List<HtmlMessage>();
        public readonly List<HtmlMessage> Requests = new List<HtmlMessage>();
        public Func<HtmlMessage, HtmlMessage> OnRequest { get; set; }

        public Control Control { get; } = new Panel();
        public bool IsReady { get; set; } = true;
        public string LoadedHtml { get; private set; }
        public int PrintPreviewCount { get; private set; }
        public event EventHandler<HtmlMessage> MessageReceived;
        public event EventHandler DocumentReady;

        public void LoadHtml(string strHtml)
        {
            LoadedHtml = strHtml;
        }

        public void Post(string strType, object payload = null)
        {
            Posts.Add(HtmlMessage.Create(strType, payload));
        }

        public HtmlMessage Request(string strType, object payload, TimeSpan timeout)
        {
            var msg = HtmlMessage.Create(strType, payload);
            Requests.Add(msg);
            return IsReady ? OnRequest?.Invoke(msg) : null;
        }

        public void ShowPrintPreview()
        {
            PrintPreviewCount++;
        }

        public void Raise(string strType, object payload = null)
        {
            MessageReceived?.Invoke(this, HtmlMessage.Create(strType, payload));
        }

        public void RaiseReady()
        {
            IsReady = true;
            DocumentReady?.Invoke(this, EventArgs.Empty);
        }

        public static HtmlMessage Reply(object payload = null)
        {
            return HtmlMessage.Create(HtmlMessage.CstrTypeReply, payload);
        }

        public int DisposeCount { get; private set; }

        public void Dispose()
        {
            DisposeCount++;
            Control.Dispose();
        }
    }
}
