FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Shared/Shared.csproj Shared/
COPY InventoryService/InventoryService.csproj InventoryService/

RUN dotnet restore "InventoryService/InventoryService.csproj"

COPY . .
WORKDIR /src/InventoryService
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.dll"]
