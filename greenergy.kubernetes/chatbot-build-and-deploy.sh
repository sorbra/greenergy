#!/bin/bash

cd ..

# build new container using existing tag
./build-chatbot.sh

cd greenergy.kubernetes

# Force pod reload:
./deletepod.sh greenergy-chatbot

read -p "Press any key to continue... " -n1 -s
