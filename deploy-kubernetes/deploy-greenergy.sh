#!/bin/bash

#MONGODB_ROOT_PASSWORD="!dev123"
#helm install --name green-mongo-dev --set mongodbRootPassword="$MONGODB_ROOT_PASSWORD",service.type=NodePort,service.nodePort=30017 stable/mongodb

kubectl create configmap emissions-api.appsettings --from-file=appsettings.json=./CM-emissions-api.json
kubectl create configmap emissions-energinetdk.appsettings --from-file=appsettings.json=./CM-emissions-energinetdk.json
kubectl create configmap emissions-chatbot.appsettings --from-file=appsettings.json=./CM-emissions-chatbot.json

kubectl apply -f emissions-api.yaml
kubectl apply -f emissions-energinetdk.yaml
kubectl apply -f emissions-chatbot.yaml

read -p "Press any key to continue... " -n1 -s
