#!/bin/bash

MONGODB_ROOT_PASSWORD="!dev123"
helm install --name green-mongo-dev --set mongodbRootPassword="$MONGODB_ROOT_PASSWORD" stable/mongodb

kubectl create configmap greenergy.api.server.appsettings --from-file=appsettings.json=./CM-greenergy.api.server.json
kubectl create configmap greenergy.syncdata.energinetdk.appsettings --from-file=appsettings.json=./CM-greenergy.syncdata.energinetdk.json
kubectl create configmap greenergy.chatbot.appsettings --from-file=appsettings.json=./CM-greenergy.chatbot.json

kubectl apply -f kubernetes-greenergy.api.server.yaml
kubectl apply -f kubernetes-greenergy.syncdata.energinetdk.yaml
kubectl apply -f kubernetes-greenergy.chatbot.yaml

read -p "Press any key to continue... " -n1 -s
