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
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs

# Copy .NET project files
COPY Prema.ShuleOne.Web.Server/Prema.ShuleOne.Web.Server.csproj Prema.ShuleOne.Web.Server/
COPY Prema.ShuleOne.Web.ServiceDefaults/Prema.ShuleOne.Web.ServiceDefaults.csproj Prema.ShuleOne.Web.ServiceDefaults/

# Ensure directory exists and copy the file to the runtime image
WORKDIR /app
RUN mkdir -p /app/Database/LocationData
COPY /src/Prema.ShuleOne.Web.Server/Database/LocationData/kenya-location-data.csv /app/Database/LocationData/


# Restore .NET dependencies
RUN dotnet restore Prema.ShuleOne.Web.Server/Prema.ShuleOne.Web.Server.csproj

# Build Angular project (Node.js Stage)
FROM node:18 AS angular_build
WORKDIR /src/prema.shuleone.web.client

# Copy Angular project files
COPY prema.shuleone.web.client/package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the Angular source files
COPY prema.shuleone.web.client/ .

# Build the Angular project
RUN npm run build --prod

# Build .NET project
FROM build AS dotnet_build
WORKDIR /src
COPY . .
WORKDIR /src/Prema.ShuleOne.Web.Server
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Publish .NET app
FROM dotnet_build AS publish
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app

# Copy published .NET files
COPY --from=publish /app/publish .



# Copy Angular build output to .NET's wwwroot
COPY --from=angular_build /src/prema.shuleone.web.client/dist/prema.shuleone.web.client wwwroot/


# Set the entry point
ENTRYPOINT ["dotnet", "Prema.ShuleOne.Web.Server.dll"]
