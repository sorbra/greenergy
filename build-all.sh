#!/bin/bash

./emissions-api/build.emissions-api.sh
./build.emissions-energinetdk.sh
./build-chatbot.sh

read -p "Press any key to continue... " -n1 -s
