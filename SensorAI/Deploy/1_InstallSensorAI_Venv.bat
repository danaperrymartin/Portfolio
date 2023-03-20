@echo on
call workon venv
cd "C:\Code Projects\SensorAI\Deploy" && "C:\Users\Dana\anaconda3\Scripts\activate.bat" && "pip" "install" "--user" "-r" "requirements.txt"
pause