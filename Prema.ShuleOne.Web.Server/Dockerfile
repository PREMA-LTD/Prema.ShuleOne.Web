# Base image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage for .NET and Angular
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Install Node.js for Angular builds
RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash - && \
    apt-get install -y nodejs

# Copy project files
COPY Prema.ShuleOne.Web.Server/Prema.ShuleOne.Web.Server.csproj Prema.ShuleOne.Web.Server/
COPY prema.shuleone.web.client/prema.shuleone.web.client.esproj prema.shuleone.web.client/
COPY Prema.ShuleOne.Web.ServiceDefaults/Prema.ShuleOne.Web.ServiceDefaults.csproj Prema.ShuleOne.Web.ServiceDefaults/

# Restore .NET dependencies
RUN dotnet restore Prema.ShuleOne.Web.Server/Prema.ShuleOne.Web.Server.csproj

# Copy all source files
COPY . .

# Build Angular project
# Use a base image with Node.js 18 or later
FROM node:22 AS build
WORKDIR /src/prema.shuleone.web.client
COPY ./package*.json ./
RUN npm install
RUN npm run build --prod

# Build .NET project
WORKDIR /src/Prema.ShuleOne.Web.Server
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Publish stage for .NET
FROM build AS publish
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Prema.ShuleOne.Web.Server.dll"]
