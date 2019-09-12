#!/bin/bash

docker login -u=$DOCKER_USERNAME -p=$DOCKER_PASSWORD 
sed -i "s/\"AdministrationPassword\": \"qwerty\",/\"AdministrationPassword\": \"$EMAIL_PASSWORD\",/g" ./KNI_D6_web/appsettings.json
cat ./KNI_D6_web/appsettings.json
docker build -t sych474/$SERVICE_NAME:latest ./
docker push sych474/$SERVICE_NAME:latest
      