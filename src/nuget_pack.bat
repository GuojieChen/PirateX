nuget pack %cd%\PirateX.Core\PirateX.Core.csproj -Prop Configuration=Release
nuget pack %cd%\PirateX\PirateX.csproj -Prop Configuration=Release
nuget pack %cd%\PirateX.Protocol\PirateX.Protocol.csproj -Prop Configuration=Release
nuget pack %cd%\PirateX.Protocol.V1\PirateX.Protocol.V1.csproj -Prop Configuration=Release
nuget pack %cd%\PirateX.Protocol.V2\PirateX.Protocol.V2.csproj -Prop Configuration=Release

pause