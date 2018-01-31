@echo off
set version=1.0.2.1
set OutputDirectory=bin\packages\%version%

if not exist %OutputDirectory% mkdir %OutputDirectory%

echo "nuget pack"

nuget pack %cd%\src\PirateX.Core\PirateX.Core.csproj -Properties -IncludeReferencedProjects -OutputDirectory %OutputDirectory% -version %version%
nuget pack %cd%\src\PirateX.Net.NetMQ\PirateX.Net.NetMQ.csproj  -Properties -IncludeReferencedProjects -OutputDirectory %OutputDirectory% -version %version%
nuget pack %cd%\src\PirateX.Net.SuperSocket\PirateX.Net.SuperSocket.csproj  -Properties -IncludeReferencedProjects -OutputDirectory %OutputDirectory% -version %version%
nuget pack %cd%\src\PirateX.Protocol\PirateX.Protocol.csproj  -Properties -IncludeReferencedProjects -OutputDirectory %OutputDirectory% -version %version%
nuget pack %cd%\src\PirateX.ServiceStackV3\PirateX.ServiceStackV3.csproj  -Properties -IncludeReferencedProjects -OutputDirectory %OutputDirectory% -version %version%
nuget pack %cd%\src\PirateX.Middleware\PirateX.Middleware.csproj  -Properties -IncludeReferencedProjects -OutputDirectory %OutputDirectory% -version %version%
nuget pack %cd%\src\PirateX.ProtobufInitialize\PirateX.ProtobufInitialize.csproj  -Properties -IncludeReferencedProjects -OutputDirectory %OutputDirectory% -version %version%


echo "push to ProGet"

NuGet.exe push %OutputDirectory%\* Admin:Admin -Source http://192.168.1.158:8001/nuget/mrglee/

pause