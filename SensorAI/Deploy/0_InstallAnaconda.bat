powershell -noprofile -command "(New-Object Net.WebClient).DownloadFile('https://repo.anaconda.com/archive/Anaconda3-2021.05-Windows-x86_64.exe','Anaconda3-2021.05-Windows-x86_64.exe'); & .\Anaconda3-2021.05-Windows-x86_64.exe /quiet InstallAllUsers=1 PrependPath=1 Include_test=0 TargetDir="%UserProfile%\Anaconda3"; [Environment]::SetEnvironmentVariable('PATH', ${env:path} + ';%UserProfile%\Anaconda3'); & Remove-Item .\Anaconda3-2021.05-Windows-x86_64.exe"