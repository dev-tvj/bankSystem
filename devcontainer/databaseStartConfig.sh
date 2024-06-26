#!/bin/bash

########################################################
#                    HOW TO EXECUTE                     #
########################################################
# 
# 1) Ensure you have PostgreSQL running on your local machine.
# 
# 2) Open a terminal
# 
# 3) Navigate to the project directory
# 
# 4) Make the script executable by running the following command:
#    chmod +x databaseStartConfig.sh
# 
# 5) Execute the script by running:
#    ./databaseStartConfig.sh
# 
# This script will configure your ASP.NET Core application to:
# - Install dotnet-ef global tool
# - Apply migrations to update your PostgreSQL database
# ########################################################


echo "###############################################"
echo "########## Starting Database Setup ############"
echo "###############################################"


# Ensure dotnet-ef tool is installed
echo "### Installing dotnet-ef tool"
dotnet tool install --global dotnet-ef


# Add dotnet-ef to PATH
export PATH="$PATH:$HOME/.dotnet/tools"


# Apply migrations to update the database
echo "### Applying Entity Framework Core Migrations"
dotnet ef migrations add InitialCreate
dotnet ef database update --context BankContext --verbose


echo "###############################################"
echo "########## Database Setup Complete ############"
echo "###############################################"

