# Use the official .NET 8 SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

# Install basic tools
RUN apt-get update && apt-get install -y \
    git \
    curl \
    wget \
    htop \
    nano \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Set the working directory
WORKDIR /workspace

# Copy all files from the current directory to the container's workspace
COPY . .

# # Restore dependencies
# RUN dotnet restore

# # Build the project
# RUN dotnet build

# Expose ports for the .NET application
EXPOSE 5000
EXPOSE 5001