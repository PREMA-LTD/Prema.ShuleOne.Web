# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["Prema.Services.ShuleOneDbManager/Prema.Services.ShuleOneDbManager.csproj", "Prema.Services.ShuleOneDbManager/"]
COPY ["Prema.ShuleOne.Web.Server/Prema.ShuleOne.Web.Server.csproj", "Prema.ShuleOne.Web.Server/"]
COPY ["Prema.ShuleOne.Web.ServiceDefaults/Prema.ShuleOne.Web.ServiceDefaults.csproj", "Prema.ShuleOne.Web.ServiceDefaults/"]

RUN dotnet restore "./Prema.Services.ShuleOneDbManager/Prema.Services.ShuleOneDbManager.csproj"

# Copy all files, then exclude appsettings
COPY . .
RUN find . -name "appsettings*.json" -delete

WORKDIR "/src/Prema.Services.ShuleOneDbManager"
RUN dotnet build "./Prema.Services.ShuleOneDbManager.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Prema.Services.ShuleOneDbManager.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Prema.Services.ShuleOneDbManager.dll"]
