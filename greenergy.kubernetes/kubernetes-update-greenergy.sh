#!/bin/bash

kubectl apply -f kubernetes-greenergy.api.server.yaml
kubectl apply -f kubernetes-greenergy.syncdata.energinetdk.yaml
kubectl apply -f kubernetes-greenergy.chatbot.yaml

read -p "Press any key to continue... " -n1 -s
