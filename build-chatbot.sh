#!/bin/bash

source  $(dirname "$0")/set-docker-tags.sh

echo Tag: $DOCKER_TAG_CHATBOT

# Build the docker container and tag it.
docker build -t $DOCKER_TAG_CHATBOT -f ./greenergy.chatbot/Dockerfile .

# Push container to Docker Hub
docker login -u sorbra
docker push "$DOCKER_TAG_CHATBOT"

read -p "Press any key to continue... " -n1 -s
