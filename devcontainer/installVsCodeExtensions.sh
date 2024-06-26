#!/bin/bash

# ############################################################
# #                       HOW TO EXECUTE                     #
# ############################################################
# 
# 1) Open a terminal.
# 
# 2) Navigate to the directory where this script is located.
# 
# 3) Make the script executable by running the following command:
#    chmod +x installVsCodeExtensions.sh
# 
# 4) Execute the script by running:
#    ./installVsCodeExtensions.sh
#
# ############################################################

extensions=(
    "ms-dotnettools.csharp"
    "ms-dotnettools.vscode-dotnet-runtime"
    "ms-dotnettools.csdevkit"
    "visualstudioexptteam.vscodeintellicode"
    "ms-azuretools.vscode-docker"
    "ms-vscode.vscode-typescript-next"
    "PKief.material-icon-theme"
)


echo "Installing extensions..."

for extension in "${extensions[@]}"
do
    code --install-extension $extension
done


echo "###############################################"
echo "###############################################"
echo "########## Finished VS Code Setup #############"
echo "###############################################"
echo "###############################################"