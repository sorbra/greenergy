#!/bin/bash

source  $(dirname "$0")/set-docker-tags.sh

echo Tag: $DOCKER_TAG_API

# Build the docker container and tag it.
docker build -t $DOCKER_TAG_API -f ./greenergy.api.server/Dockerfile .

# Push container to Docker Hub
docker login -u sorbra
docker push "$DOCKER_TAG_API"

cd ..

read -p "Press any key to continue... " -n1 -s
