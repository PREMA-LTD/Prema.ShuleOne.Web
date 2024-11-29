#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Prema.ShuleOne.Web.Server.csproj", "."]
RUN dotnet restore "./Prema.ShuleOne.Web.Server.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./Prema.ShuleOne.Web.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Prema.ShuleOne.Web.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Copy the csv file to the image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Ensure the directory exists before copying the file
RUN mkdir -p ./Database/LocationData
COPY ["Database/LocationData/kenya-location-data.csv", "./Database/LocationData/"]

ENTRYPOINT ["dotnet", "Prema.ShuleOne.Web.Server.dll"]