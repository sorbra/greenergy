#!/bin/bash

export MONGODB_ROOT_PASSWORD=$(kubectl get secret --namespace default green-mongo-dev-mongodb -o jsonpath="{.data.mongodb-root-password}" | base64 --decode)
export NODE_IP=$(kubectl get nodes --namespace default -o jsonpath="{.items[0].status.addresses[0].address}")
export NODE_PORT=$(kubectl get --namespace default -o jsonpath="{.spec.ports[0].nodePort}" services green-mongo-dev-mongodb)

echo ConnectionString="mongodb://root:$MONGODB_ROOT_PASSWORD@:$NODE_IP:$NODE_PORT"

read -p "Press any key to continue... " -n1 -s

