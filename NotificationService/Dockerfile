FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 7006

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY NotificationService/NotificationService.csproj NotificationService/
COPY Shared/Shared.csproj Shared/
RUN dotnet restore "NotificationService/NotificationService.csproj"

COPY . .
WORKDIR /src/NotificationService
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.dll"]
