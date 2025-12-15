@echo off
chcp 65001 >nul
echo Очистка папок сборки...
echo.

if exist "Publish\Portable\*" (
    echo Удаляю файлы из Publish\Portable...
    del /Q "Publish\Portable\*"
)

if exist "Publish\Portable" (
    echo Удаляю папку Publish\Portable...
    rmdir "Publish\Portable"
)

if exist "Publish\Installer\*" (
    echo Удаляю файлы из Publish\Installer...
    del /Q "Publish\Installer\*"
)

if exist "Publish\Installer" (
    echo Удаляю папку Publish\Installer...
    rmdir "Publish\Installer"
)

if exist "Publish\*.zip" (
    echo Удаляю ZIP архивы...
    del /Q "Publish\*.zip"
)

if exist "Publish\*.msi" (
    echo Удаляю MSI установщики...
    del /Q "Publish\*.msi"
)

if exist "Publish\*.exe" (
    echo Удаляю EXE установщики...
    del /Q "Publish\*.exe"
)

if exist "Publish" (
    if not exist "Publish\*" (
        echo Удаляю пустую папку Publish...
        rmdir "Publish"
    )
)

echo.
echo bin и obj папки...
dotnet clean --verbosity quiet

echo.
echo Очистка завершена!
pause