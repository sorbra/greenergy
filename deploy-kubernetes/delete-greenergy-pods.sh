#!/bin/bash

./deletepod.sh emissions-api
./deletepod.sh chatbot
./deletepod.sh emissions-energinetdk

read -p "Press any key to continue... " -n1 -s
