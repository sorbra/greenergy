#!/bin/bash

helm delete green-mongo-dev --purge
read -p "Press any key to continue... " -n1 -s