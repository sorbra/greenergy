#!/bin/bash

# Create configmaps greenergy
kubectl create configmap greenergy-appsettings-kubemongo --from-file=appsettings.json=./greenergy-appsettings.kubemongo.json

# Create or update the kubernetes deployment
kubectl apply -f kubernetes-greenergy-deployment.yaml

read -p "Press any key to continue... " -n1 -s
