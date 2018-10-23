#!/bin/bash

# Create or update the kubernetes deployment
kubectl delete -f kubernetes-greenergy-deployment.yaml

# Create configmaps greenergy
kubectl delete configmap greenergy-appsettings-kubemongo

read -p "Press any key to continue... " -n1 -s
