# Скрипт для сборки портативной версии
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "  Сборка портативной версии Bandwidth Calculator" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Проверяем, что мы в корневой папке проекта
if (-not (Test-Path "BandwidthCalculator.csproj")) {
    Write-Host "Ошибка: Запустите скрипт из папки с проектом!" -ForegroundColor Red
    pause
    exit 1
}

# Проверяем наличие .NET SDK
Write-Host "Проверка установки .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "Ошибка: .NET SDK не найден!" -ForegroundColor Red
    Write-Host "Установите .NET 8.0 SDK или выше" -ForegroundColor Yellow
    Write-Host "Ссылка: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Blue
    pause
    exit 1
}

Write-Host ".NET SDK $dotnetVersion обнаружен." -ForegroundColor Green
Write-Host ""

# Создаем папку для сборки
$publishDir = "Publish\Portable"
if (Test-Path $publishDir) {
    Write-Host "Очистка предыдущих сборок..." -ForegroundColor Yellow
    Remove-Item "$publishDir\*" -Recurse -Force
} else {
    New-Item -ItemType Directory -Path $publishDir -Force | Out-Null
}

Write-Host "Сборка портативной версии..." -ForegroundColor Green
Write-Host "Это может занять несколько минут..." -ForegroundColor Yellow
Write-Host ""

# Собираем портативную версию
dotnet publish -c Release -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:DebugType=None `
    -p:DebugSymbols=false `
    --output $publishDir

if ($LASTEXITCODE -ne 0) {
    Write-Host "Ошибка при сборке!" -ForegroundColor Red
    pause
    exit 1
}

Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host "  Сборка успешно завершена!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Green
Write-Host ""

# Показываем размер файла
$exePath = Join-Path $publishDir "BandwidthCalculator.exe"
if (Test-Path $exePath) {
    $fileSize = (Get-Item $exePath).Length / 1MB
    Write-Host "Размер исполняемого файла: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "Файлы находятся в папке: $publishDir" -ForegroundColor Yellow
Write-Host "Запустите файл: BandwidthCalculator.exe" -ForegroundColor Yellow
Write-Host ""

# Создаем ZIP архив
Write-Host "Создание ZIP архива..." -ForegroundColor Yellow
$zipPath = "Publish\BandwidthCalculator_Portable.zip"
Compress-Archive -Path "$publishDir\*" -DestinationPath $zipPath -Force -CompressionLevel Optimal

if (Test-Path $zipPath) {
    $zipSize = (Get-Item $zipPath).Length / 1MB
    Write-Host "ZIP архив создан: $zipPath ($([math]::Round($zipSize, 2)) MB)" -ForegroundColor Green
} else {
    Write-Host "Не удалось создать ZIP архив" -ForegroundColor Red
}

Write-Host ""
pause