using System;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// how C# talks to an HTML page: load it, post commands to it, ask it things (bounded), and hear from it.
    /// Nothing else may touch the browser (see spec: docs/superpowers/specs/2026-10-04-html-message-protocol-design.md)
    /// </summary>
    public interface IHtmlHost : IDisposable
    {
        Control Control { get; }                         // add this to the owning pane/form
        bool IsReady { get; }                            // the current document has sent 'ready'
        string LoadedHtml { get; }                       // the last string passed to LoadHtml
        void LoadHtml(string strHtml);                   // "" = blank page
        void Post(string strType, object payload = null); // C# -> JS, doesn't wait (queued until 'ready')
        HtmlMessage Request(string strType, object payload, TimeSpan timeout); // null if not ready or timed out
        event EventHandler<HtmlMessage> MessageReceived; // JS -> C#, raised on the UI thread
        event EventHandler DocumentReady;
        void ShowPrintPreview();
    }

    public class HtmlHostOptions
    {
        public bool AllowBrowserContextMenu { get; set; }   // IE's own right-click menu
        public bool AllowFileDrop { get; set; }             // dropping a file onto the browser navigates to it
    }

    public static class HtmlHostDefaults
    {
        // how long C# waits for a page to answer a Request
        public static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(2);
    }
}
