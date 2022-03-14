@echo off

REM Command line arguments and defined properties.
SET AppName=%1
SHIFT
SET SafeAppName=%1
SHIFT
SET Version=%1
SHIFT

REM Parse version numbers into pieces
for /f "tokens=1,2,3,4 delims=/." %%a in ("%Version%") do set Major=%%a&set Minor=%%b&set Build=%%c&set Revision=%%d
REM Truncated version is the first three parts of the version number
SET TRUNCATEDVERSION=%Major%.%Minor%.%Build%

REM The Product Id Guid should be changed to a new guid whenever the third (build) number of the version changes.  Also, when the product number changes you should create a new base installer.
SET PRODUCTIDGUID=%1
SHIFT
SET UPGRADECODEGUID=%1
SHIFT

SET APPBUILDDIR=%1
SHIFT
SET APPDATADIR=%1
SHIFT

REM To protect the password for the certfile, you can configure your build process to read the password from an unversioned file and pass it into the CERTPASS variable.
SET CopyrightYear=%1
SHIFT
SET Manufacturer=%1
SHIFT
SET Arch=%1
if "%Arch%" == "" set Arch=x86

REM ICE validation must be run with admin privileges. The jenkins user is not an admin. Suppress ICE validation so it doesn't fail.
REM For some reason, ICE08 works without admin, and the quickest way to suppress everything else on the command line is to specify one ICE to run.
C:\Windows\System32\whoami /groups | find "BUILTIN\Administrators" > nul 2> nul
if errorlevel 1 set SuppressICE=-ice:ICE08

REM Ensure WiX tools are on the PATH
where heat >nul 2>nul
if not %errorlevel% == 0 set PATH=%WIX%/bin;%PATH%

@echo on
@REM Harvest (heat) the application and data files.
@BE: it's already been built heat.exe dir %APPBUILDDIR% -cg HarvestedAppFiles -gg -scom -sreg -sfrag -srd -sw5150 -sw5151 -dr APPFOLDER -var var.APPBUILDDIR -t KeyPathFix.xsl -out AppHarvest.wxs
@BE: it's already been built heat.exe dir %APPDATADIR% -cg HarvestedDataFiles -gg -scom -sreg -sfrag -srd -sw5150 -sw5151 -dr HARVESTDATAFOLDER -var var.APPDATADIR -t KeyPathFix.xsl -out DataHarvest.wxs

@REM Compile (candle) and Link (light) the MSI file.
@BE: it's already been built candle.exe -arch %Arch% -dApplicationName=%AppName% -dSafeApplicationName=%SafeAppName% -dManufacturer=%Manufacturer% -dVersionNumber=%Version% -dMajorVersion=%Major% -dMinorVersion=%Minor% -dAPPBUILDDIR=%APPBUILDDIR% -dAPPDATADIR=%APPDATADIR% -dUpgradeCode=%UPGRADECODEGUID% -dProductCode=%PRODUCTIDGUID% Framework.wxs AppHarvest.wxs DataHarvest.wxs WixUI_DialogFlow.wxs GIInstallDirDlg.wxs GIProgressDlg.wxs GIWelcomeDlg.wxs GICustomizeDlg.wxs GISetupTypeDlg.wxs

@BE: it's already been built light.exe Framework.wixobj AppHarvest.wixobj DataHarvest.wixobj WixUI_DialogFlow.wixobj GIInstallDirDlg.wixobj GIProgressDlg.wixobj GIWelcomeDlg.wixobj GICustomizeDlg.wixobj GISetupTypeDlg.wixobj -ext WixUIExtension -ext WixUtilExtension.dll -cultures:en-us -loc WixUI_en-us.wxl %SuppressICE% -sw1076 -out %SafeAppName%_%Version%.msi

call signingProxy %SafeAppName%_%Version%.msi

@REM build the ONLINE EXE bundle.
@BE: it's already been built candle.exe -arch %Arch% -dApplicationName=%AppName% -dSafeApplicationName=%SafeAppName% -dYear=%CopyrightYear% -dManufacturer=%Manufacturer% -dVersionNumber=%Version% -dUpgradeCode=%UPGRADECODEGUID% -dTruncatedVersion=%TRUNCATEDVERSION% -ext WixUtilExtension -ext WixBalExtension -ext WixUIExtension -ext WixNetFxExtension -ext WixDependencyExtension Bundle.wxs

@BE: it's already been built light.exe Bundle.wixobj -ext WixUIExtension -ext WixBalExtension -ext WixUtilExtension -ext WixNetFxExtension -ext WixDependencyExtension %SuppressICE% -out %SafeAppName%_%Version%_Online.exe

@REM build the OFFLINE EXE bundle.
@BE: it's already been built candle.exe -arch %Arch% -dApplicationName=%AppName% -dSafeApplicationName=%SafeAppName% -dYear=%CopyrightYear% -dManufacturer=%Manufacturer% -dVersionNumber=%Version% -dUpgradeCode=%UPGRADECODEGUID% -dTruncatedVersion=%TRUNCATEDVERSION% -ext WixUtilExtension -ext WixBalExtension -ext WixUIExtension -ext WixNetFxExtension -ext WixDependencyExtension OfflineBundle.wxs

@BE: it's already been built light.exe OfflineBundle.wixobj -ext WixUIExtension -ext WixBalExtension -ext WixUtilExtension -ext WixNetFxExtension -ext WixDependencyExtension %SuppressICE% -out %SafeAppName%_%Version%_Offline.exe

@REM Sign the standard installer.
@BE: it's built by a separate bat file in its own project insignia -ib %SafeAppName%_%Version%_Online.exe -o engine.exe
@BE: it's built by a separate bat file in its own project call signingProxy engine.exe
@BE: it's built by a separate bat file in its own project insignia -ab engine.exe %SafeAppName%_%Version%_Online.exe -o %SafeAppName%_%Version%_Online.exe
@BE: it's built by a separate bat file in its own project call signingProxy %SafeAppName%_%Version%_Online.exe

@REM Sign the offline installer.
@BE: it's built by a separate bat file in its own project insignia -ib %SafeAppName%_%Version%_Offline.exe -o engine.exe
@BE: it's built by a separate bat file in its own project call signingProxy engine.exe
@BE: it's built by a separate bat file in its own project insignia -ab engine.exe %SafeAppName%_%Version%_Offline.exe -o %SafeAppName%_%Version%_Offline.exe
@BE: it's built by a separate bat file in its own project call signingProxy %SafeAppName%_%Version%_Offline.exe

@REM Cleanup debris from this build
DEL *.wixobj
DEL *.wixpdb
DEL engine.exe
DEL AppHarvest.wxs
DEL DataHarvest.wxs