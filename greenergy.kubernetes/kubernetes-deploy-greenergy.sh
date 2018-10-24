#!/bin/bash

MONGODB_ROOT_PASSWORD="!dev123"
helm install --name green-mongo-dev --set mongodbRootPassword="$MONGODB_ROOT_PASSWORD" stable/mongodb

kubectl create configmap greenergy.api.server.appsettings --from-file=appsettings.json=./CM-greenergy.api.server.json
kubectl apply -f kubernetes-greenergy.api.server.yaml


#kubectl create configmap greenergy.syncdata.energinetdk-kubemongo --from-file=appsettings.json=./CM-greenergy.syncdata.energinetdk.json

# Create or update the kubernetes deployment
# kubectl apply -f kubernetes-greenergy-deployment.yaml

read -p "Press any key to continue... " -n1 -s
