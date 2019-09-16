#!/bin/bash

cd ./kni_d6
docker-compose pull
docker-compose down
docker-compose up -d
