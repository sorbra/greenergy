#!/bin/bash

./deletepod.sh greenergy-api
./deletepod.sh greenergy-chatbot
./deletepod.sh greenergy-syncdata-energinetdk

read -p "Press any key to continue... " -n1 -s
