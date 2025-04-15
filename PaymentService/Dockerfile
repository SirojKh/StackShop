FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 7007

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY PaymentService/PaymentService.csproj PaymentService/
COPY Shared/Shared.csproj Shared/
RUN dotnet restore "PaymentService/PaymentService.csproj"

COPY . .
WORKDIR /src/PaymentService
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.dll"]
