#!/bin/bash

kubectl apply -f greenergy.api.server.yaml
kubectl apply -f greenergy.syncdata.energinetdk.yaml
kubectl apply -f greenergy.chatbot.yaml

read -p "Press any key to continue... " -n1 -s
