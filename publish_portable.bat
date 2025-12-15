@echo off
chcp 65001 >nul
echo ============================================
echo   Сборка портативной версии Bandwidth Calculator
echo ============================================
echo.

REM Проверяем, что мы в корневой папке проекта
if not exist "BandwidthCalculator.csproj" (
    echo Ошибка: Запустите скрипт из папки с проектом!
    pause
    exit /b 1
)

REM Проверяем наличие .NET SDK
echo Проверка установки .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo Ошибка: .NET SDK не найден!
    echo Установите .NET 8.0 SDK или выше
    echo Ссылка: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

echo .NET SDK обнаружен.
echo.

REM Создаем папку для сборки
if not exist "Publish" mkdir "Publish"
if not exist "Publish\Portable" mkdir "Publish\Portable"

echo Очистка предыдущих сборок...
if exist "Publish\Portable\*" del /Q "Publish\Portable\*"
echo.

echo Сборка портативной версии (может занять несколько минут)...
echo.

REM Собираем портативную версию
dotnet publish -c Release -r win-x64 ^
  --self-contained true ^
  -p:PublishSingleFile=true ^
  -p:IncludeNativeLibrariesForSelfExtract=true ^
  -p:EnableCompressionInSingleFile=true ^
  -p:DebugType=None ^
  -p:DebugSymbols=false ^
  --output "Publish\Portable"

if errorlevel 1 (
    echo Ошибка при сборке!
    pause
    exit /b 1
)

echo.
echo ============================================
echo   Сборка успешно завершена!
echo ============================================
echo.

REM Показываем размер файла
for %%F in ("Publish\Portable\BandwidthCalculator.exe") do (
    set /a size=%%~zF / 1048576
    echo Размер исполняемого файла: !size! MB
)

echo.
echo Файлы находятся в папке: Publish\Portable
echo Запустите файл: BandwidthCalculator.exe
echo.

REM Создаем ZIP архив
echo Создание ZIP архива...
powershell -Command "Compress-Archive -Path 'Publish\Portable\*' -DestinationPath 'Publish\BandwidthCalculator_Portable.zip' -Force" >nul

if exist "Publish\BandwidthCalculator_Portable.zip" (
    echo ZIP архив создан: Publish\BandwidthCalculator_Portable.zip
) else (
    echo Не удалось создать ZIP архив
)

echo.
pause