#!/bin/bash

MONGODB_ROOT_PASSWORD="!dev123"

helm install --name green-mongo-dev --set mongodbRootPassword="$MONGODB_ROOT_PASSWORD",service.type=NodePort,service.nodePort=30017 stable/mongodb

# To connect to database:
#    kubectl run --namespace default green-mongo-dev-mongodb-client --rm --tty -i --image bitnami/mongodb --command -- mongo admin --host green-mongo-dev-mongodb -u root -p $MONGODB_ROOT_PASSWORD

read -p "Press any key to continue... " -n1 -s
