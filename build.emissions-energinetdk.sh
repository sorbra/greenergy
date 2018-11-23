#!/bin/bash

export DOCKER_TAG_ENERGINETDK="sorbra/emissions-energinetdk:0.2.0"

# Build the docker container and tag it.
docker build -t $DOCKER_TAG_ENERGINETDK -f ./emissions-energinetdk/Dockerfile .

# Push container to Docker Hub
docker login -u sorbra
docker push "$DOCKER_TAG_ENERGINETDK"
