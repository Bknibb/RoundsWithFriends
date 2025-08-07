param (
    [Parameter(Mandatory = $true)]
    [string]$Version,          # e.g. "v4.0.1"

    [Parameter(Mandatory = $true)]
    [string]$Description,      # e.g. "Improved performance, bug fixes"

    [string]$ProjectPath = "./RoundsWithFriends/RoundsWithFriends.csproj",   # Change this to your actual .csproj
	[string]$ProjectDir = "./RoundsWithFriends",
    [string]$DllName = "RoundsWithFriends.dll",             # Name of the built DLL to attach
	[string]$Repo = "Bknibb/RoundsWithFriends",
	[string]$BinDir = "./RoundsWithFriends/bin/Debug/net472"
)

# Ensure we're using a clean version string
if ($Version -notmatch '^v\d+\.\d+\.\d+$') {
    Write-Error "Version must follow the format vX.Y.Z (e.g. v1.2.3)"
    exit 1
}

# Check version in RoundsWithFriends.cs
$roundsWithFriendsCsPath = "./RoundsWithFriends/RoundsWithFriends.cs"
if (-not (Test-Path $roundsWithFriendsCsPath)) {
    Write-Error "❌ RoundsWithFriends.cs file not found at path $roundsWithFriendsCsPath"
    exit 1
}

$csContent = Get-Content $roundsWithFriendsCsPath -Raw
if ($csContent -match 'public\s+const\s+string\s+Version\s*=\s*"(?<csVersion>\d+\.\d+\.\d+)"') {
    $csVersion = $matches['csVersion']
    $cleanInputVersion = $Version.TrimStart("v")

    if ($csVersion -ne $cleanInputVersion) {
        Write-Error "❌ Version mismatch: script version is '$cleanInputVersion' but RoundsWithFriends.cs has '$csVersion'"
        exit 1
    }

    Write-Host "✅ Version match confirmed: $csVersion"
} else {
    Write-Error "❌ Could not find a version declaration in RoundsWithFriends.cs"
    exit 1
}

& '.\github release.ps1' $Version $Description $ProjectPath $DllName $Repo
& '.\publish.ps1' $Version.TrimStart("v") 'Release' $BinDir $DllName <# Set to any valid directory as not needed #> ".\" $ProjectDir

Write-Host "Combined Release Complete"
