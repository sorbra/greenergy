#!/bin/bash

export DOCKER_TAG_CHATBOT="sorbra/emissions-chatbot:0.2.0"

# Build the docker container and tag it.
docker build -t $DOCKER_TAG_CHATBOT -f ./emissions-chatbot/emissions-chatbot.fulfillment/Dockerfile .

# Push container to Docker Hub
docker login -u sorbra
docker push "$DOCKER_TAG_CHATBOT"