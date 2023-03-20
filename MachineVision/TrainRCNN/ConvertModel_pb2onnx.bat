@echo on
call "C:\ProgramData\anaconda3\Scripts\activate.bat" && cd "E:\TrainRCNN_Server" && "python" "-m" "tf2onnx.convert" "--saved-model" "pix2pixmodel.pb" "--output" "pix2pixmodel.onnx"
pause