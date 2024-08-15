# 1st Stage: Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy all source files
COPY . ./
# Publish the project
RUN dotnet publish -c Release -o out

# 2nd Stage: Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
# Copy the published files from the build stage
COPY --from=build-env /app/out .
# Run the application
ENTRYPOINT [ "dotnet", "PlatformService.dll" ]