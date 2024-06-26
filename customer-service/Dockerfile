# Use the official .NET 8 SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /customer-service

# Copy project files to working directory
COPY customer-service.csproj .

# Restore as distinct layers
RUN dotnet restore "customer-service.csproj"

COPY . .

# Ensure HTTPS certificates are cleaned and trusted for secure connections
RUN dotnet dev-certs https --clean && dotnet dev-certs https --trust


# Build and publish a release
RUN dotnet publish "customer-service.csproj" -c Release -o /out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 as final
WORKDIR /App
COPY --from=build /out .
#RUN dotnet dev-certs https --trust
ENTRYPOINT ["dotnet", "customer-service.dll"]