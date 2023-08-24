@echo off
setlocal enabledelayedexpansion


:: 獲取當前腳本的完整路徑
set ThisScriptPath=%~dp0
set PrjPath= %ThisScriptPath%..\

::回到上一層
set PrjPath=%PrjPath:~0,-1%
echo %PrjPath%

:: 指定檔案路徑
set CONFIG=%~dp0config.txt


:: 讀取 txt 檔案到變數
:: tokens=1* : 分割後的欄位
:: delims== : 以 "="為分割符號
for /F "tokens=1* delims==" %%a in (%CONFIG%) do (
    if "%%a"=="UnityEditorPath" set EditorPath=%%b
    if "%%a"=="ExecuteClass" set ExecuteClass=%%b
    if "%%a"=="ExecuteMethod" set ExecuteMethod=%%b
    if "%%a"=="Params" set Params=%%b
)

echo !EditorPath!
echo !ExecuteClass!
echo !ExecuteMethod!
echo !Params!
!EditorPath! -projectPath %PrjPath% -executeMethod "CommandInvoker.Execute" -class !ExecuteClass! -method !ExecuteMethod! -params !Params!
pause