#!/bin/bash

cd "$(dirname "$0")"

nswag swagger2csclient /input:http://localhost:5000/swagger/v0.2/swagger.json /output:emissions-api.testclient/EmissionsClient.cs /namespace:Greenergy.Emissions.API

read -p "Press any key to continue... " -n1 -s
