FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base
WORKDIR /app

COPY *.sln ./

COPY src/AwsInspectorPoc.API/*.csproj src/AwsInspectorPoc.API/

COPY tests/AwsInspectorPoc.API.Tests/*.csproj tests/AwsInspectorPoc.API.Tests/

RUN dotnet restore AwsInspectorPoc.API.sln

COPY . .

FROM base AS publish-stage
RUN dotnet publish -c Release -o dist src/AwsInspectorPoc.API/AwsInspectorPoc.API.csproj

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=publish-stage /app/dist ./
ENTRYPOINT ["dotnet", "AwsInspectorPoc.API.dll"]