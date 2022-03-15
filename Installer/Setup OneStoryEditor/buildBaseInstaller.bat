@echo off

REM Command line arguments and defined properties.
SET MsiFileName=%1
SHIFT

call signingProxy %MsiFileName%

@REM Cleanup debris from this build
DEL *.wixobj
DEL *.wixpdb