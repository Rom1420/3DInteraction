# === CONFIG ===
# Chemin vers ton projet Unity (Unity Version Control local)
$UnityProjectPath = "C:\Users\abbon\Documents\Unity\3DInteraction\3DInteractionRepo"

# Branche git cible
$GitBranch = "master"
# ==============

Write-Host "=== Sync Unity Version Control -> Git repo (AUTO) ===" -ForegroundColor Cyan

if (-not (Test-Path $UnityProjectPath)) {
    Write-Host "ERREUR: Le chemin '$UnityProjectPath' n'existe pas." -ForegroundColor Red
    exit 1
}

# Vérif rapide projet Unity
$requiredFolders = @("Assets", "ProjectSettings", "Packages")
foreach ($f in $requiredFolders) {
    $p = Join-Path $UnityProjectPath $f
    if (-not (Test-Path $p)) {
        Write-Host "ATTENTION: Le dossier Unity ne contient pas '$f' (je continue quand même)..." -ForegroundColor Yellow
    }
}

# Dossier mirror dans le repo Git
$repoRoot = $PSScriptRoot
$destPath = Join-Path $repoRoot "unity_src"
if (-not (Test-Path $destPath)) {
    New-Item -ItemType Directory -Path $destPath | Out-Null
}

Write-Host "Copie depuis '$UnityProjectPath' vers '$destPath'..."

$excludeDirs = @(
    "Library",
    "Temp",
    "Logs",
    "obj",
    ".vs",
    ".git",
    ".idea",
    ".vscode",
    ".plastic"
)

$rcArgs = @(
    $UnityProjectPath,
    $destPath,
    "/MIR",
    "/R:2",
    "/W:2",
    "/NFL",
    "/NDL",
    "/NJH",
    "/NJS"
)

foreach ($ex in $excludeDirs) {
    $rcArgs += "/XD"
    $rcArgs += (Join-Path $UnityProjectPath $ex)
}

& robocopy.exe @rcArgs
$rcExit = $LASTEXITCODE
if ($rcExit -gt 3) {
    Write-Host "ERREUR robocopy (code $rcExit)" -ForegroundColor Red
    exit $rcExit
}

Write-Host "Copie OK (code $rcExit)" -ForegroundColor Green

# Git
Set-Location $repoRoot

# S'il n'y a aucun changement, on arrête proprement
$changes = git status --porcelain
if (-not $changes) {
    Write-Host "Aucun changement à committer 👍" -ForegroundColor Yellow
    exit 0
}

$commitMsg = "Sync from Unity VC - $(Get-Date -Format 'yyyy-MM-dd HH:mm')"

Write-Host "Commit: $commitMsg"
git add unity_src
git commit -m "$commitMsg"

Write-Host "Push vers origin/$GitBranch"
git push origin $GitBranch

Write-Host "Sync + commit + push terminés ✅" -ForegroundColor Green
