@echo off
color f0
title ApexAdaptive100+
echo Checking Environment...
if exist nw.exe goto afternw
echo Lack NW.js Detected
echo Start Install NW.js...
echo Download From Taobao Source...
curl https://npm.taobao.org/mirrors/nwjs/v0.55.0/nwjs-v0.55.0-win-x64.zip -o nwjs.zip -L
echo Unzip NW.js...
7za x nwjs.zip
del /f /s /q nwjs.zip
echo Finishing Install...
for /f "delims=" %%a in ('dir nwjs* /B') do set nwjspath=%%a
xcopy %nwjspath% %cd% /s /i /q
rd /s /q %nwjspath%

:afternw
echo NW.js Installed
echo Detecting Config...
:det
ping 127.1 >nul -n 1
if not exist "C:\Users\%username%\Saved Games\Respawn\Apex\local\videoconfig.txt" echo Run Game First To Generate Config & goto det
echo Detecting Game...
:pre
ping 127.1 >nul -n 1
tasklist|find /i "r5apex.exe" ||goto check1
echo Close Game To Continue... & goto pre
:check1
echo Launch Main Program...
start  nw.exe -username %username%
ping 127.1 >nul -n 5
