# syntax=docker/dockerfile:1

################################################################################
# Build Stage
################################################################################
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /source

# Copy csproj first for caching restore
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the source
COPY . ./

# Publish app (architecture x64)
RUN dotnet publish -c Release -a x64 --self-contained false -o /app

################################################################################
# Final Stage
################################################################################
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Install ICU for globalization
RUN apk add --no-cache icu-libs bash

# Enable full globalization
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_URLS=http://+:5000

# Copy published app from build stage
COPY --from=build /app .

# Expose port
EXPOSE 5000

# Run the app
ENTRYPOINT ["dotnet", "Dotventure.dll"]
