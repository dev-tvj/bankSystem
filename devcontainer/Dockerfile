# Use the official .NET 8 SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

# Install basic tools
RUN apt-get update && apt-get install -y \
    git \
    ssh \
    curl \
    wget \
    htop \
    nano \
    lsof \
    net-tools \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*


RUN echo "Ensure HTTPS certificates are cleaned and trusted for secure connections"
RUN dotnet dev-certs https --clean && dotnet dev-certs https --trust

# Set the working directory
WORKDIR /workspace

# Copy all files from the current directory to the container's workspace
COPY . .

# Expose ports for the .NET application
# EXPOSE 5000
# EXPOSE 5001