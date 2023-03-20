@echo on
call "C:\ProgramData\anaconda3\Scripts\activate.bat" && cd "C:\CodeProjects\SensorAI\Deploy" && "C:\ProgramData\anaconda3\Scripts\pyinstaller.exe" "--noconsole" "SensorAI.py"
pause