# Raises the File-table version of specific files in a built MSI so they still get installed when upgrading over
# an older OSE that shipped a *higher*-versioned copy of the same file.
#
# Why: MajorUpgrade is scheduled afterInstallInitialize, so file costing runs while the old version's files are
# still on disk. Under the default file-versioning rules (and the REINSTALLMODE="muso" Burn always passes), a file
# whose version went down is costed "won't overwrite" - and then RemoveExistingProducts deletes the old copy,
# leaving the file missing. OSE 5.0 shipped LibChorus.dll/Chorus.exe/ChorusMerge.exe 5.0.0.653 (the current Chorus
# 5.0.0 NuGet/merge module build is 5.0.0.0) and Enchant.Net.dll 1.4.3.0 (current NuGet is 1.4.2); upgrading 5.0 ->
# 5.6 left all four missing and StoryEditor died before Main with a FileNotFoundException (2026-09-30 VM test).
# 5.1-5.5 all shipped the same versions as now, so only upgrades from 5.0 were affected.
#
# Forcing REINSTALLMODE=amus instead is NOT safe here: several files are installed twice to the same path by
# different components (e.g. Autofac.dll 9.3.2.0 from us and 2.6.3.862 from ChorusMergeModule.msm), and with 'a'
# whichever has the higher File sequence wins - which would downgrade Autofac.
#
# WiX can't express this itself: File/@DefaultVersion is overwritten by the binder with the real file version, and
# the Chorus files come from a prebuilt merge module. Only raises a version (never lowers it), so it's idempotent
# and becomes a no-op once the real file versions catch up. A later Repair will just re-copy these files (their
# on-disk version is lower than the File table claims), which is harmless.
param(
    [Parameter(Mandatory = $true)][string]$MsiPath
)

$ErrorActionPreference = 'Stop'

# FileName (long name, case-insensitive) -> minimum version to claim in the File table
$minimumVersions = @{
    'libchorus.dll'   = '5.0.0.654'
    'chorus.exe'      = '5.0.0.654'
    'chorusmerge.exe' = '5.0.0.654'
    'enchant.net.dll' = '1.4.3.1'
}

function Invoke-Com($obj, [string]$method, [object[]]$params = $null) {
    $obj.GetType().InvokeMember($method, 'InvokeMethod', $null, $obj, $params)
}
function Get-ComProp($obj, [string]$prop, [object[]]$params = $null) {
    $obj.GetType().InvokeMember($prop, 'GetProperty', $null, $obj, $params)
}
function Set-ComProp($obj, [string]$prop, [object[]]$params) {
    [void]$obj.GetType().InvokeMember($prop, 'SetProperty', $null, $obj, $params)
}

$installer = New-Object -ComObject WindowsInstaller.Installer
$db = Invoke-Com $installer 'OpenDatabase' @((Resolve-Path $MsiPath).Path, 1)  # msiOpenDatabaseModeTransact
$view = Invoke-Com $db 'OpenView' @('SELECT `File`, `FileName`, `Version` FROM `File`')
[void](Invoke-Com $view 'Execute')

$changed = 0
while ($null -ne ($record = Invoke-Com $view 'Fetch')) {
    $longName = ((Get-ComProp $record 'StringData' @(2)) -split '\|')[-1].ToLowerInvariant()
    if (-not $minimumVersions.ContainsKey($longName)) { continue }

    $fileKey = Get-ComProp $record 'StringData' @(1)
    $current = Get-ComProp $record 'StringData' @(3)
    $minimum = $minimumVersions[$longName]
    if ([version]$current -ge [version]$minimum) {
        Write-Host "RaiseFileVersions: $fileKey already $current (>= $minimum)"
        continue
    }

    Set-ComProp $record 'StringData' @(3, $minimum)
    [void](Invoke-Com $view 'Modify' @(4, $record))  # msiViewModifyReplace
    Write-Host "RaiseFileVersions: $fileKey $current -> $minimum"
    $changed++
}

[void](Invoke-Com $view 'Close')
[void](Invoke-Com $db 'Commit')
Write-Host "RaiseFileVersions: $changed File row(s) updated in $MsiPath"
