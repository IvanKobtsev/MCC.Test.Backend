FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MCC.TestTask.App/MCC.TestTask.App.csproj", "MCC.TestTask.App/"]
COPY ["MCC.TestTask.Domain/MCC.TestTask.Domain.csproj", "MCC.TestTask.Domain/"]
COPY ["MCC.TestTask.Persistence/MCC.TestTask.Persistence.csproj", "MCC.TestTask.Persistence/"]
COPY ["MCC.TestTask.Infrastructure/MCC.TestTask.Infrastructure.csproj", "MCC.TestTask.Infrastructure/"]
RUN dotnet restore "MCC.TestTask.App/MCC.TestTask.App.csproj"
COPY . .
WORKDIR "/src/MCC.TestTask.App"
RUN dotnet build "./MCC.TestTask.App.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MCC.TestTask.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MCC.TestTask.App.dll"]