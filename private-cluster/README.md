# AKS hackathon - private cluster

This repo contains a step-by-step giude to setup a private AKS cluster. 

# Overview

![env](img/env.png)


## Prerequisites 

- Github account
- VS code with GitHub Codespaces extension
- Azure subscription with RBAC role Owner

## Getting started

### Fork and clone the repository

Log in to GitHub with your GitHub account. Fork this repo by selecting the Fork menu in the GitHub top right corner.
![fork](img/fork.png)

> **Note**<br>
> If your GitHub account is part of an organization there is a limitation that only one fork is possible in the same organization. The workaround is to clone this repo, create a new repository and then push the code from the cloned working copy similar to this:
>
>  ``` shell
>  git clone https://github.com/jonasnorlund/aks-hack
>  cd aks-hack
>  git remote set-url origin <url of your new repo>
>  git push origin master
>  ```
>

Clone the fork using git. 
```shell
git clone <your GitHub repository url>/aks-hack
cd aks-hack/private-cluster 
```

### Open the repo in Codespaces

Click on the "Remote Explorer" extension in VS Code and choose "GitHub Codespaces" in the dropdown at the top. Then click "Create Codespace". Choose your cloned repo and the main branch, set the size to "4 cores, 8 GB RAM, 32 GB storage". VS Code restarts and now run in GitHub Codespaces. 


## Step-by-step guidance

Below is the step-by-step guidance. 

## Step 1 - Log in to Azure
```shell
az login
az account set -s [your subscription]
```
Need to register the following features. This is a two step process, run the following two commands we will return to this later because it can take up to 10 minutes to be finished.
```shell
az feature register --namespace microsoft.compute --name EncryptionAtHost
az feature register --namespace "Microsoft.ContainerService" --name "EnableWorkloadIdentityPreview"
```
## Step 2 - Create resourcegroup and choose resourcename

Choose a resourcename of six letters e.g "majnor" that will be used to create a unique environment. That will also be a part of the naming convention to be used. 
Create resourcegroup
```shell 
az group create -l westeurope -n [rg-resourcename]
```
Verify that it has been created in the portal. 

## Step 3 - Get familiar with Bicep

Open aks-hack/private-cluster/bicep folder in VS Code. 

Make sure the following parameters are set in the main.bicep file. 

```shell
param deployInit bool = true
param deployAzServices bool = true
param deployAks bool = false
param deployVm bool = false
```
Run the following command. 
```shell
az deployment group create -g [rg-resourcename] -n mainDeploy -f main.bicep -p resourcename=[resourcename]
```
Verify the deployment in the portal.

## Step 4 - Deploy AKS

Run the following Azure CLI commands. This is the second step in registering new features that we did in step 1.  
```shell
az provider register --namespace microsoft.compute
az provider register --namespace Microsoft.ContainerService
```

Change the following parameter to "true" in main.bicep
```shell
param deployAks bool = true
```

Add the following parameter in the Azure CLI command. 
```shell
admingroupobjectid=[objectId of AAD group]
```
This should be a valid objectId of a group in AAD that will have cluster role admin in AKS. 


Run the following command. 
```shell
az deployment group create -g [rg-resourcename] -n mainDeploy -f main.bicep -p resourcename=[resourcename] admingroupobjectid=[objectId of AAD group]
```
Verify the deployment in the portal.

## Step 5 - Upload file to storage and deploy deploy VM

Upload the file "setup.ps1" to your storageaccount. Run the following command. 
```shell
az storage blob upload -f setup.ps1 --account-name [your storageaccount name] -c vmconfig -n setup.ps1
```
Change the following parameter to "true" in main.bicep
```shell
param deployVm bool = true
```
Identify your IP address and add the following parameters to your Azure CLI command. 
```shell
az deployment group create -g [rg-resourcename] -n mainDeploy -f main.bicep -p resourcename=[resourcename] admingroupobjectid=[objectId of AAD group] allowedhostIp=[your IP address] vmpwd=[Set a pwd of 8 chars]
```

## Step 6 - Get access to AKS

Login to the VM in Azure, open a terminal and run the following commands. 
```shell
az login -t [your tenantId]
az aks get-credentials --resource-group [rg-resourcename] --name aks-[resourcename]-dev --admin
```
Validate access
```shell
kubectl get pods
```
Use the browser to valdate the login. You should see "No resources found in default namespace."

if you want to create a shortcut for Kubectl run the following commands in the terminal. 
```shell
New-Item -Path $profile -Type File -Force 
notepad $profile
Set-Alias -Name k -Value kubectl
```
Save the file restart the terminal. 
Validate by running "k get pods" in a terminal. 

Done.


## Step 7 - Deploy private endpoints

Change the following parameters in main.bicep
```shell
param deployInit bool = true
param deployAzServices bool = true
param deployAks bool = false
param deployVm bool = false
param deployPe bool = true

```
This will disable AKS and VM deployment (to save time) and enable deployment of private endpoints for AKV and ACR. Run the following command. 
```shell
az deployment group create -g [rg-resourcename] -n mainDeploy -f main.bicep -p resourcename=[resourcename]
```
Verify the deployment in the portal.

## Step 8 - Deploy AKS workload

Login to the VM.
Create a folder under c:\ called "temp"
```shell
cd\
mkdir temp
cd .\temp
```
Clone your repo. 
```shell
git clone https://github.com/[your github account]/aks-hack.git
```
Goto the Azure portal and copy the instrumentation key for Application Insights. 
Open the file aks-hack/private-cluster/sample/distributed-calculator/deploy/open-telemetry-collector-appinsights.yaml
Paste your instrumentation key inside the quotes "<INSTRUMENTATION-KEY>"
In the terminal navigate to aks-hack/private-cluster/sample/distributed-calculator/deploy
Run the following commands. 

```shell
kubectl create ns sample 
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update
helm install redis bitnami/redis --namespace sample
kubectl apply -f . -n sample
```

Run "kubectl get svc -n sample" to get the external IP and use a browser to access the frontend of the calculator.

Use all the calculator methods and look in Application insights to view the telemetry. 


## Step 9 - Use Policies in AKS

Navigate to aks-hack/private-cluster/policy on the VM. Deploy the policy initiative. 

```shell
az deployment sub create --location westeurope --template-file aks_initiative_template.json --parameters aks_initiative_params.json
```

Validate the policy initiative in the portal. The deployment creates an assignment on subscription level, this is not what we want so delete the assignment on the subscription level using the portal, then create the assignment on the resource group level using a bicep file.  

```shell
az deployment group create -g rg-[resourcename] -f policy.bicep -p subId=[subscriptionId]
```

Validate the assignment using the portal. 

Try to deploy the busybox.yaml to the sample namespace. 

```shell
kubectl apply -f .\busybox.yaml
```


Learn more about AKS policies https://learn.microsoft.com/en-us/azure/aks/policy-reference

## Step 10 - Use workload identity in AKS

Use the VM that was created in Azure. 
Use a terminal and navigate to aks-hack/private-cluster/workloadidentity
Run the following command to create a new User Assigned Identity that will be used as a workload identity. https://learn.microsoft.com/en-us/azure/aks/workload-identity-overview

It also adds a secret to the Keyvault that was created earlier and creates a roleassignment for the new User Assigned Identity. 

```shell
az deployment group create -g rg-[resourcename] -f workloadidentity.bicep -p resourcename=[resourcename] location=westeurope
```


### Step 10.1 Grant Access to the User Assigned Identity 
Use the Azure portal and set you as the Azure Active Directory Admin for your Azure Sql Server. 
Go to the resource sqlsrv-[resourcename]-dev --> Settings --> Azure Active Directory

Then use the Query editor in the portal under the following resource, db-[resourcename]-dev to grant access for you UMI in the database. Enable your IP in the SQL ACL list and then connect to the database and run the following commands.  

```shell
CREATE USER [umi-wli-[resourcename]-dev] FROM EXTERNAL PROVIDER
ALTER ROLE db_owner ADD MEMBER [umi-wli-[resourcename]-dev]
```

### Step 10.2 Push the code to ACR 
Push the code to Azure Container Registry and build a docker image. 
Navigate to aks-hack/private-cluster/workloadidentity/src/workloadidentity and run the following commands. 

```shell
az login -t [tenantid]
az acr build --registry acr[resourcename]dev --image wli/wli-api:v1 .
```

### Step 10.3 Deploy the workload
Open wli.yaml in notepad on the VM. Replace [resourcename] with your resourcename (4 places).
Get the ClientID of the newly created User Assigned Identity and replace [CLIENTID of User Assigned Identity used for workloadidentity] (2 places). 
Run the following commands.    

```shell
kubectl create ns wli
kubectl apply -f .\wli.yaml -n wli
```

Validate the functionality by using a browser and go to these url's from your Azure VM.   
- http://10.0.3.7/workloadidentity/getsecret
- http://10.0.3.7/workloadidentity/getdata