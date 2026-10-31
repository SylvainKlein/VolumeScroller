# Script de compilation et packaging pour Volume Scroller
$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "   Volume Scroller - Build & Packaging    " -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

$root = $PSScriptRoot
$portableDir = "$root\publish\VolumeScroller-Portable"
$installerDir = "$root\publish\Installer"

# Nettoyage des dossiers de publication
New-Item -ItemType Directory -Force -Path $portableDir | Out-Null
New-Item -ItemType Directory -Force -Path $installerDir | Out-Null

# 1. Publication en exécutable autonome .NET 9
Write-Host "`n[1/3] Compilation Release (Single-File)..." -ForegroundColor Yellow
dotnet publish "$root\src\VolumeScroller\VolumeScroller.csproj" `
    -c Release `
    -r win-x64 `
    --self-contained false `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o $portableDir

if ($LASTEXITCODE -ne 0) {
    Write-Error "Échec de la compilation."
    exit 1
}

# Nettoyage des fichiers temporaires dans Portable (garder uniquement .exe et .ico)
Remove-Item "$portableDir\*.pdb" -Force -ErrorAction SilentlyContinue
Copy-Item "$root\icon.ico" -Destination "$portableDir\icon.ico" -Force
Write-Host " -> Portable prêt : $portableDir\VolumeScroller.exe" -ForegroundColor Green

# 2. Création de l'archive ZIP Portable
Write-Host "`n[2/3] Création du ZIP Portable..." -ForegroundColor Yellow
$zipPath = "$root\publish\VolumeScroller-Portable.zip"
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
Compress-Archive -Path "$portableDir\*" -DestinationPath $zipPath -Force
Write-Host " -> Archive ZIP : $zipPath" -ForegroundColor Green

# 3. Compilation de l'installeur Inno Setup
Write-Host "`n[3/3] Recherche d'Inno Setup..." -ForegroundColor Yellow
$isccPaths = @(
    "ISCC.exe",
    "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
    "$env:ProgramFiles\Inno Setup 6\ISCC.exe",
    "${env:LOCALAPPDATA}\Programs\Inno Setup 6\ISCC.exe"
)

$iscc = $null
foreach ($path in $isccPaths) {
    if (Get-Command $path -ErrorAction SilentlyContinue) {
        $iscc = $path
        break
    }
    if (Test-Path $path) {
        $iscc = $path
        break
    }
}

if ($iscc) {
    Write-Host "Génération de l'installeur..." -ForegroundColor Yellow
    $issFile = "$root\installer\VolumeScroller.iss"
    & $iscc $issFile
    if ($LASTEXITCODE -eq 0) {
        Write-Host " -> Installeur créé : $installerDir\VolumeScroller-Setup.exe" -ForegroundColor Green
    } else {
        Write-Warning "Inno Setup a rencontré un problème."
    }
} else {
    Write-Host "Inno Setup non détecté. Seule la version portable a été générée." -ForegroundColor Yellow
}

Write-Host "`n==========================================" -ForegroundColor Cyan
Write-Host "  Compilation terminée avec succès !" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Cyan
