# ---------- 1. Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Kopieer projectbestanden en herstel afhankelijkheden
COPY *.csproj .
RUN dotnet restore

# Kopieer de rest van de code en build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ---------- 2. Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Kopieer de build-resultaten van de vorige stage
COPY --from=build /app/publish .

# Stel poort in waar ASP.NET op luistert
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "PrototypeUserService.dll"]
