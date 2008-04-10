rmdir .\exe /S /Q
mkdir .\exe

cd ..\..
.\libs\nant-0.85\nant.exe

copy .bin\*.* .\Samples\PublishSubscribe\exe /Y

cd Samples\PublishSubscribe
msbuild PublishSubscribe.sln
copy .\bin\*.* .\exe /Y