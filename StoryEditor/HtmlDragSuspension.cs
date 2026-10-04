namespace OneStoryProjectEditor
{
    /// <summary>
    /// Holds StoryEditor.SuspendSaveDialog up for the length of one HTML5 drag: Begin is idempotent and End releases
    /// at most once, so a repeated start or a missing end (page rebuilt mid-drag) can't leak the static counter.
    /// </summary>
    internal class HtmlDragSuspension
    {
        public bool InProgress { get; private set; }

        public void Begin()
        {
            if (InProgress)
                return;
            InProgress = true;
            StoryEditor.SuspendSaveDialog++;
        }

        public void End()
        {
            if (!InProgress)
                return;
            InProgress = false;
            StoryEditor.SuspendSaveDialog--;
        }
    }
}
