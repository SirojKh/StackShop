﻿FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 7003

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY OrderingService/OrderingService.csproj OrderingService/
COPY Shared/Shared.csproj Shared/
RUN dotnet restore "OrderingService/OrderingService.csproj"

COPY . .
WORKDIR /src/OrderingService
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderingService.dll"]
