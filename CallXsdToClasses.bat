rem from: http://www.codeproject.com/Articles/15989/Creating-an-XML-file-based-on-XSD
rem %1 == the name of the xsd to process (e.g. "D:\Src\StoryEditor\StoryEditor\StoryProject.xsd")
rem %2 == namespace (e.g. "StoryEditor.Data")
"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\x64\xsd.exe" %1 /classes /language:CS /namespace:%2
