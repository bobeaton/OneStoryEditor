using System;
using System.IO;
using System.Reflection;                // to access Icu4Net via reflection (so it doesn't have to be present)

namespace OneStoryProjectEditor
{
    class BreakIterator
    {
        private object _icuBreakIteratorEncConverter = null;
        private MethodInfo _fnConvert = null;

        public BreakIterator()
        {
            var aIcu4Net = Assembly.LoadFile(Path.Combine(StoryProjectData.GetRunningFolder,
                                                          "IcuEC.dll"));    // ICU57 should be here now
            if (aIcu4Net == null)
                return;

            var typeIcuBreakIteratorEncConverter = aIcu4Net.GetType("SilEncConverters40.IcuBreakIteratorEncConverter");
            if (typeIcuBreakIteratorEncConverter == null)
                return;

            var aTypeParams = new[] { typeof(string) };
            _fnConvert = typeIcuBreakIteratorEncConverter.GetMethod("Convert", aTypeParams);

            // create the object
            _icuBreakIteratorEncConverter = Activator.CreateInstance(typeIcuBreakIteratorEncConverter);
        }

        public string AddWordBreaks(string str)
        {
            if ((_fnConvert == null) || (_icuBreakIteratorEncConverter == null))
                return str; // can't do it

            var oParams = new object[] { str };
            var ret = _fnConvert.Invoke(_icuBreakIteratorEncConverter, oParams);
            return ret.ToString();
        }

        public static bool IsAvailable
        { 
            get
            {
                var pathToIcu4Net = Path.Combine(StoryProjectData.GetRunningFolder,
                                                 "IcuEC.dll");
                return File.Exists(pathToIcu4Net);
            }
        }
    }
}
