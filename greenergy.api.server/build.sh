#!/bin/bash

export DOCKER_TAG="sorbra/greenergy.api.server:0.0.1"

# Build the docker container and tag it.
docker build -t $DOCKER_TAG .

# Push container to Docker Hub
docker login -u sorbra
docker push "$DOCKER_TAG"

read -p "Press any key to continue... " -n1 -s
