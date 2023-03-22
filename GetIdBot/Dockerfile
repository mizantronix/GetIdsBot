FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /App
COPY . ./

RUN dotnet restore && dotnet build -c Release -o out
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /App
COPY --from=build /App/out App/

ENTRYPOINT ["dotnet", "./App/GetIdBot.dll"]