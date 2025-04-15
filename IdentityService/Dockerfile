FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 7005

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY IdentityService/IdentityService.csproj IdentityService/
COPY Shared/Shared.csproj Shared/
RUN dotnet restore "IdentityService/IdentityService.csproj"

COPY . .
WORKDIR /src/IdentityService
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.dll"]
