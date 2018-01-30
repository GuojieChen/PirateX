nuget pack %cd%\src\PirateX.Core\PirateX.Core.csproj -Properties Configuration=Release -IncludeReferencedProjects -OutputDirectory bin\ -version 1.0.0
nuget pack %cd%\src\PirateX.Net.NetMQ\PirateX.Net.NetMQ.csproj -Properties Configuration=Release  -IncludeReferencedProjects -OutputDirectory bin\ -version 1.0.0
nuget pack %cd%\src\PirateX.Net.SuperSocket\PirateX.Net.SuperSocket.csproj -Properties Configuration=Release -IncludeReferencedProjects  -OutputDirectory bin\ -version 1.0.0
nuget pack %cd%\src\PirateX.Protocol\PirateX.Protocol.csproj -Properties Configuration=Release -IncludeReferencedProjects  -OutputDirectory bin\ -version 1.0.0
nuget pack %cd%\src\PirateX.ServiceStackV3\PirateX.ServiceStackV3.csproj -Properties Configuration=Release -IncludeReferencedProjects  -OutputDirectory bin\ -version 1.0.0
nuget pack %cd%\src\PirateX.Middleware\PirateX.Middleware.csproj -Properties Configuration=Release -IncludeReferencedProjects  -OutputDirectory bin\ -version 1.0.0
pause