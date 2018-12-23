#!/bin/bash

# see https://github.com/helm/charts/tree/master/incubator/kafka

helm repo add incubator http://storage.googleapis.com/kubernetes-charts-incubator
helm install --name green-kafka incubator/kafka


kubectl -n default exec testclient -- /usr/bin/kafka-topics --zookeeper green-kafka-zookeeper:2181 --topic best-future-consumption --create --partitions 1 --replication-factor 1

kubectl -n default exec -ti testclient -- /usr/bin/kafka-console-consumer --bootstrap-server green-kafka:9092 --topic best-future-consumption --from-beginning