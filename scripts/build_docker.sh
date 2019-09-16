#!/bin/bash

docker login -u=$DOCKER_USERNAME -p=$DOCKER_PASSWORD 
docker build -t sych474/$SERVICE_NAME:latest ./
docker push sych474/$SERVICE_NAME:latest
      