#!/bin/bash

export PODNAME=$1

export API_POD=$(kubectl get pods -o go-template --template '{{range .items}}{{.metadata.name}}{{"\n"}}{{end}}' | grep $PODNAME)

echo Deleting pod $API_POD...

kubectl delete pod $API_POD
# kubectl get pods

# read -p "Press any key to continue... " -n1 -s
