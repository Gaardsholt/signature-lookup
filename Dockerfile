FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src

COPY ["*.csproj", ""]
RUN dotnet restore

COPY . ./
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -r linux-musl-x64 -p:PublishTrimmed=True -p:TrimMode=Link -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "signature-lookup.dll"]