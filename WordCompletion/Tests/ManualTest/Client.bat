timeout /t 10
..\..\bin\Debug\NetworkClient\NetworkClient.exe -S=%COMPUTERNAME% -P=11000 <"bigTest.in" >%1
exit