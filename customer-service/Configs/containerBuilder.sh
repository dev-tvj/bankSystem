#!/bin/bash

########################################################
#                    HOW TO EXECUTE                    #
########################################################
# 
# 1) This script will run automatically inside the Docker container.
# 
# 2) Ensure the necessary environment is set up correctly.
# 
# 3) Ensure PostgreSQL is up before run this script.
# 
# 4) This script assumes dotnet is installed and available.
#
#
#   To RUN this script manually:
#
#   a) Make the script executable by running the following command:
#    chmod +x containerBuilder.sh
# 
#   b) Execute the script by running:
#    ./containerBuilder.sh
#
# 
# ########################################################


echo "###############################################"
echo "####### Starting Docker Container Setup #######"
echo "###############################################"


# Ensure HTTPS certificates are cleaned and trusted for secure connections
echo "### Ensuring HTTPS certificates are trusted"
dotnet dev-certs https --clean && dotnet dev-certs https --trust


# Restores dependencies and compiles the project
#echo "### Restoring dependencies..."
#dotnet restore

echo "### Building the project..."
dotnet build

echo "### Starting the application..."
dotnet run --project customer-service.csproj &


# Run database configuration script
#echo "### Running database configuration"
#chmod +x ./Configs/databaseStartConfig.sh && ./Configs/databaseStartConfig.sh


echo "###############################################"
echo "####### Docker Container Setup Finished #######"
echo "###############################################"

#Keeps the container running (if necessary)
#tail -f /dev/null