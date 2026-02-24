FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY src/Aliare.Weather.Api/Aliare.Weather.Api.csproj src/Aliare.Weather.Api/
RUN dotnet restore src/Aliare.Weather.Api/Aliare.Weather.Api.csproj

COPY src/ src/
RUN dotnet publish src/Aliare.Weather.Api/Aliare.Weather.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Aliare.Weather.Api.dll"]
