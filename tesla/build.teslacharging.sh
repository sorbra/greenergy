#!/bin/bash

cd "$(dirname "$0")"

pwd

export DOCKER_TAG_TESLA="sorbra/teslacharging:0.1.0"

# Build the docker container and tag it.
docker build -t $DOCKER_TAG_TESLA -f ./teslacharging/Dockerfile .

# Push container to Docker Hub
# docker login -u sorbra
docker push "$DOCKER_TAG_TESLA"

