#!/bin/bash

./build-api.server.sh
./build-syncdata.energinetdk.sh
./build-chatbot.sh

read -p "Press any key to continue... " -n1 -s
