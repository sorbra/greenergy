#!/bin/bash

kubectl apply -f emissions-api.yaml
kubectl apply -f emissions-energinetdk.yaml
kubectl apply -f emissions-chatbot.yaml

read -p "Press any key to continue... " -n1 -s
