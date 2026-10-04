using System;
using System.Collections.Generic;

namespace OneStoryProjectEditor
{
    internal enum FlushChoice
    {
        Retry,
        SaveWithoutLatest,
        Cancel
    }

    internal enum FlushOutcome
    {
        AllEditsCollected,  // save as normal
        SomeEditsMissing,   // the user chose to save without the latest typing in a pane that didn't answer
        Cancelled           // don't save (the user cancelled, or it's an autosave, which tries again next time)
    }

    /// <summary>
    /// before any save, every HTML pane is asked to send the edits it hasn't sent yet (see spec, Section 3). This is
    /// the policy for when a pane doesn't answer; StoryEditor supplies the panes and the prompt
    /// </summary>
    internal static class PaneFlush
    {
        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(3);

        // the label of the first pane that didn't answer, or null when they all did
        public static string FlushAll(IEnumerable<KeyValuePair<string, Func<bool>>> panes)
        {
            foreach (var pane in panes)
                if (!pane.Value())
                    return pane.Key;
            return null;
        }

        public static FlushOutcome Run(Func<string> flushAll, Func<string, FlushChoice> askUser, bool bAutosave)
        {
            while (true)
            {
                var strPaneNotAnswering = flushAll();
                if (strPaneNotAnswering == null)
                    return FlushOutcome.AllEditsCollected;

                if (bAutosave)
                    return FlushOutcome.Cancelled;

                switch (askUser(strPaneNotAnswering))
                {
                    case FlushChoice.Retry:
                        continue;
                    case FlushChoice.SaveWithoutLatest:
                        return FlushOutcome.SomeEditsMissing;
                    default:
                        return FlushOutcome.Cancelled;
                }
            }
        }
    }
}
