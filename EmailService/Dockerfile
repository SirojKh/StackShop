﻿FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY EmailService/EmailService.csproj EmailService/
COPY Shared/Shared.csproj Shared/
RUN dotnet restore "EmailService/EmailService.csproj"

COPY . .
WORKDIR /src/EmailService
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmailService.dll"]
