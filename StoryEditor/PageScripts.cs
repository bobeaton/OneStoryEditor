using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// the JS files embedded with LogicalName "OneStoryProjectEditor.js.&lt;file&gt;" (see StoryEditor.csproj)
    /// </summary>
    internal static class PageScripts
    {
        private const string CstrResourcePrefix = "OneStoryProjectEditor.js.";
        private static readonly ConcurrentDictionary<string, string> Cache = new ConcurrentDictionary<string, string>();

        public static string Bridge => Get("bridge.js");

        public static string Get(string strFileName)
        {
            return Cache.GetOrAdd(strFileName, name =>
            {
                using (var stream = typeof(PageScripts).Assembly.GetManifestResourceStream(CstrResourcePrefix + name))
                {
                    if (stream == null)
                        throw new InvalidOperationException("missing embedded script " + name);
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                        return reader.ReadToEnd();
                }
            });
        }

        public static string ScriptBlock(params string[] astrScripts)
        {
            return "<script type=\"text/javascript\">" + Environment.NewLine +
                   String.Join(Environment.NewLine, astrScripts) + Environment.NewLine +
                   "</script>";
        }
    }
}
