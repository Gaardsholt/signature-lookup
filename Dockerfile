FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim AS build
WORKDIR /src

COPY ["*.csproj", ""]
RUN dotnet restore

COPY . ./
RUN dotnet publish -r linux-x64 -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:7.0-bullseye-slim
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS http://*:5000

RUN groupadd -g 2000 AppGroup && \
    useradd -m -u 2000 -g AppGroup AppUser && \
    chown -R AppUser /app
USER AppUser

ENTRYPOINT ["dotnet", "signature-lookup.dll"]