@echo on
call "C:\ProgramData\Anaconda3\Scripts\activate.bat" && cd "C:\CodeProjects\SensorAI\Deploy" && "C:\ProgramData\Anaconda3\Scripts\pyinstaller.exe" "--noconsole" "--onefile" "SensorAI.py"
pause