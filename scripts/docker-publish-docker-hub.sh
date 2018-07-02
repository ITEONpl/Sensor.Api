#!/bin/bash
BRANCH=$(git symbolic-ref --short HEAD)

DOCKER_ENV=''
DOCKER_TAG=''
case "$BRANCH" in
  "master")
    DOCKER_ENV=production
    DOCKER_TAG=latest
    ;;
  "develop")
    DOCKER_ENV=development
    DOCKER_TAG=dev
    ;;    
esac

docker login -u $DOCKER_USERNAME -p $DOCKER_PASSWORD
docker build -f ./src/Sensor.Api/Dockerfile.$DOCKER_ENV -t sensor.api:$DOCKER_TAG ./src/Sensor.Api
docker tag Sensor.api:$DOCKER_TAG $DOCKER_USERNAME/sensor.api:$DOCKER_TAG
docker push $DOCKER_USERNAME/sensor.api:$DOCKER_TAG