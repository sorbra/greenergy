#!/bin/bash

source  $(dirname "$0")/set-docker-tags.sh

echo Tag: $DOCKER_TAG_ENERGINETDK

# Build the docker container and tag it.
docker build -t $DOCKER_TAG_ENERGINETDK -f ./greenergy.syncdata.energinetdk/Dockerfile .

# Push container to Docker Hub
docker login -u sorbra
docker push "$DOCKER_TAG_ENERGINETDK"
