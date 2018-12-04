#!/bin/bash

# Requirements:
# 1. Azure CLI: see https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-macos?view=azure-cli-latest
# 2. Helm: 
#
# TO DO: 
# X. Do more to secure Helm/Tiller.

export AZ_SUBSCRIPTION="OoT SOB"
export AZ_CLUSTER="K8S-AZURE-SOB"
export AZ_RESOURCEGROUP="K8S-AZURE-SOB-RG"
export KUBERNETES_VERSION="1.11.5"
export KUBE_CONFIG="C:\minikube\.kube\config"

az login

# Set default subscription for az commands:
az account set --subscription "$AZ_SUBSCRIPTION"

# Create Azure resource group
az group create --name "$AZ_RESOURCEGROUP" --location westeurope

# Create AKS cluster
az aks create --resource-group "$AZ_RESOURCEGROUP" --name "$AZ_CLUSTER" --kubernetes-version "$KUBERNETES_VERSION" --node-count 3 --enable-addons monitoring --generate-ssh-keys

# Configure kubectl to connect to new Kubernetes cluster in AKS:
az aks get-credentials --resource-group "$AZ_RESOURCEGROUP" --name "$AZ_CLUSTER" --file "$KUBE_CONFIG"

# Setup user account that is allowed to access the Kubernetes AKS dashboard:
kubectl create serviceaccount dashboard -n default
kubectl create clusterrolebinding dashboard-admin -n default --clusterrole=cluster-admin --serviceaccount=default:dashboard

# create Service Account for Helm/Tiller:
kubectl create -f helm-rbac.yaml

# install helm/tiller to cluster
helm init --service-account tiller
