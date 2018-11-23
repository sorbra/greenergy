#!/bin/bash

pwd
cd "$(dirname "$0")"
pwd

export DOCKER_TAG_API="sorbra/emissions-api:0.2.0"

# Build the docker container and tag it.
docker build -t $DOCKER_TAG_API -f ./emissions-api.server/Dockerfile .

# Push container to Docker Hub
docker login -u sorbra
docker push "$DOCKER_TAG_API"
