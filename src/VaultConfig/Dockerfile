FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /VaultConfig
COPY ["VaultConfig/VaultConfig.csproj", "VaultConfig/"]
RUN dotnet restore "VaultConfig/VaultConfig.csproj"
COPY . .
WORKDIR "/VaultConfig/VaultConfig"
RUN dotnet build "VaultConfig.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "VaultConfig.csproj" -c $configuration -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VaultConfig.dll"]
