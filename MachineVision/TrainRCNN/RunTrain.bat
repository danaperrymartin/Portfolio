@echo off
SET /A "index = 1"
SET /A "count = 200000"
:while
if %index% leq %count% (
   call "C:\Users\Dana\anaconda3\Scripts\activate.bat" && "python" "Train_FasterMaskRCNN.py"
   echo Completed epoch %index%
   SET /A "index = index + 1"
   goto :while
)