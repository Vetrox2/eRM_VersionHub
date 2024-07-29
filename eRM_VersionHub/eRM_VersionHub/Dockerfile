##See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
# Use the ASP.NET runtime image as a base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["eRM-VersionHub/eRM_VersionHub/eRM_VersionHub.csproj", "eRM_VersionHub/"]
RUN dotnet restore "eRM_VersionHub/eRM_VersionHub.csproj"

# Copy the rest of the application code and build the application
COPY eRM-VersionHub/eRM_VersionHub .
WORKDIR "/src/eRM_VersionHub"
RUN dotnet build "eRM_VersionHub.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "eRM_VersionHub.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Create the final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER app
ENTRYPOINT ["dotnet", "eRM_VersionHub.dll"]
