Readme for "Automatically upgrade your .NET applications on the fly" - devX October 2002
http://www.devx.com

Update History:  
8 October, 2002  Created Initial version    Anthony Glenwright  (anthony.glenwright@inventua.com)
----------------------------------------------------------------------------------------------------------------

The main projects for AutoUpgrade are:

AutoUpgrade.App:  AutoUpgrade stub executable, displays "status" UI and performs download.
AutoUpgrade.Lib:  AutoUpgrade class library.

You can load, modify and compile these using AutoUpgrade.sln.

AutoUpgrade.Sample:  Is a minimal windows forms application that utilizes the AutoUpgrade Library.  You 
	will probably have to remove and re-create the reference to AutoUpgrade.Lib.DLL when you load it
	in Visual Studio.

AutoUpgrade.Web:  Is a sample web service that creates a manifest file "on the fly".  You will need to
	create a virtual directory on localhost (or elsewhere) that points to the AutoUpgrade.Web directory,
	then select "Open... Project from web" from the file menu in Visual Studio and enter your virtual
	directory name.

	The sample contains an empty client subdirectory - you must copy *your* application client files
	into this directory.

Examples:  Example code for using the built-in "web deployment" features of .NET.  These are just the 
	code from the article text.

----------------------------------------------------------------------------------------------------------------
	
And most importantly:

Class specification.doc:  Is a description of the properties, methods and events in the Autoupgrade class.

----------------------------------------------------------------------------------------------------------------

Notes:  
The "full upgrade" feature was a work in progress at the time of publication.  The incremental
or "per-file" upgrade mechanism has been tested, but the "full upgrade" feature relies on:

1.  The "full upgrade" file must be a single file.  If you are using a Visual Studio deployment project,
you can use the .msi file generated at compile time and ignore setup.exe, setup.ini and the windows
installer files instMsiA.exe and instMsiW.exe.

2.  You must use your installer application (Visual studio, Installshield, WISE, etc) to create an 
"upgrade" installation package.  Refer to your documentation for details.  In most cases, you will 
also have to modify the AutoUpgrade.Lib source code to use the correct command-line switches to 
initiate an upgrade install.

Basically, I haven't ironed out all the problems with a full install, but I left the code in place so
that readers would have the basis of my work in that area on which to improve upon.

Regards,
Anthony