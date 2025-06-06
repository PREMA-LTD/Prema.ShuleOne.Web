# Base image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Create directories with proper permissions BEFORE switching to app user
RUN mkdir -p /app/Files && \
    mkdir -p /app/Database/LocationData && \
    chown -R app:app /app

# Now switch to app user
USER app
EXPOSE 8080
EXPOSE 8081

# Build stage for .NET and Angular
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Install Node.js for Angular builds
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs

# Copy .NET project files
COPY Prema.ShuleOne.Web.Server/Prema.ShuleOne.Web.Server.csproj Prema.ShuleOne.Web.Server/
COPY Prema.ShuleOne.Web.ServiceDefaults/Prema.ShuleOne.Web.ServiceDefaults.csproj Prema.ShuleOne.Web.ServiceDefaults/

# Ensure directory exists and copy the file to the runtime image
RUN mkdir -p /app/Database/LocationData
COPY Prema.ShuleOne.Web.Server/Database/LocationData/kenya-location-data.csv /app/Database/LocationData/

# Restore .NET dependencies
RUN dotnet restore Prema.ShuleOne.Web.Server/Prema.ShuleOne.Web.Server.csproj

# Build .NET project
FROM build AS dotnet_build
WORKDIR /src
COPY Prema.ShuleOne.Web.ServiceDefaults/. Prema.ShuleOne.Web.ServiceDefaults/
COPY Prema.ShuleOne.Web.Server/. Prema.ShuleOne.Web.Server/
WORKDIR /src/Prema.ShuleOne.Web.Server
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Publish .NET app
FROM dotnet_build AS publish
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app

# Copy published .NET files with proper ownership
COPY --from=publish --chown=app:app /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "Prema.ShuleOne.Web.Server.dll"]