#!/bin/bash

cd ./kni_d6
docker-compose pull
docker-compose stop
docker-compose rm -f
docker-compose up -d
