@echo off

REM Command line arguments and defined properties.
SET MsiFileName=%1
SHIFT

call signingProxy %MsiFileName%
