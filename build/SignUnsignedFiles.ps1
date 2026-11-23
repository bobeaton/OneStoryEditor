<#
.SYNOPSIS
  Signs every .dll/.exe an installer project packages that isn't already validly Authenticode-signed.

.DESCRIPTION
  The list of files comes straight from the Source="..." attributes in the installer project's own *.wxs
  files, so it always matches exactly what ends up in the MSI. Files that already carry a valid signature
  (SIL's own Encoding-Converters-Core files, Microsoft's, Google's, ...) are left alone; everything else is
  signed one file at a time via signingProxy.bat (the same path SignBuild uses for the MSI and bundle).

  Why: Windows Smart App Control / WDAC policies refuse to load unsigned binaries ("An Application Control
  policy has blocked this file", HRESULT 0x800711C7), so signing just the MSI and bundle isn't enough - the
  files they install have to be signed too, and before the MSI is compiled, since it embeds copies of them.

.PARAMETER WxsDir
  Folder holding the installer project's *.wxs files; relative Source paths are resolved from here.

.PARAMETER Verify
  After signing, fail (exit 1) if any packaged dll/exe is still not validly signed. Use where signing is
  expected to work (TeamCity); without it, files that can't be signed are just reported.
#>
param(
  [Parameter(Mandatory = $true)][string]$WxsDir,
  [switch]$Verify
)
$ErrorActionPreference = 'Stop'

# Load Get-AuthenticodeSignature's module from this PowerShell's own install explicitly: when powershell.exe
# (5.1) is launched from a process that inherited PowerShell 7's PSModulePath (e.g. a build started from pwsh),
# autoloading it fails with "CouldNotAutoloadMatchingModule".
Import-Module (Join-Path $PSHOME 'Modules\Microsoft.PowerShell.Security\Microsoft.PowerShell.Security.psd1')

$signingProxy = Join-Path $PSScriptRoot 'signingProxy.bat'
$wxsFiles = Get-ChildItem -Path $WxsDir -Filter *.wxs -File
if (-not $wxsFiles) { throw "No .wxs files found in '$WxsDir'" }

# Source="..." values ending in .dll/.exe; skip ones built from WiX preprocessor variables ($(var.X)), which
# point into other packages (e.g. Encoding-Converters-Core's merge modules) rather than at files we build.
$files = $wxsFiles |
  Select-String -Pattern 'Source="([^"$]+\.(?:dll|exe))"' -AllMatches |
  ForEach-Object { $_.Matches } |
  ForEach-Object { [IO.Path]::GetFullPath((Join-Path $WxsDir $_.Groups[1].Value)) } |
  Sort-Object -Unique

$missing = @($files | Where-Object { -not (Test-Path -LiteralPath $_) })
foreach ($m in $missing) { Write-Host "warning: packaged file not found, can't sign it: $m" }

$unsigned = @($files | Where-Object { (Test-Path -LiteralPath $_) -and (Get-AuthenticodeSignature -LiteralPath $_).Status -ne 'Valid' })
Write-Host "$($files.Count) packaged dll/exe files; $($unsigned.Count) not yet signed."

foreach ($f in $unsigned) {
  Write-Host "signing $f"
  & cmd.exe /c "`"$signingProxy`" `"$f`""
}

if ($Verify) {
  $bad = @($files | Where-Object { (Test-Path -LiteralPath $_) -and (Get-AuthenticodeSignature -LiteralPath $_).Status -ne 'Valid' })
  if ($bad.Count -gt 0) {
    foreach ($b in $bad) { Write-Host "error: still not validly signed: $b" }
    Write-Host "error: $($bad.Count) packaged file(s) are not validly signed"
    exit 1
  }
  Write-Host "All $($files.Count - $missing.Count) packaged dll/exe files are validly signed."
}
exit 0
