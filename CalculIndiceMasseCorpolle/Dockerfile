FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CalculIndiceMasseCorpolle.csproj", "."]
RUN dotnet restore "CalculIndiceMasseCorpolle.csproj"
COPY . .
RUN dotnet build "CalculIndiceMasseCorpolle.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "CalculIndiceMasseCorpolle.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CalculIndiceMasseCorpolle.dll"]