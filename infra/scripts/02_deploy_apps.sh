#!/bin/bash

##################
### Apps Setup ###
##################

### Set variables

# Otel Collector
declare -A otelcollector
otelcollector["name"]="otelcollector"
otelcollector["namespace"]="otel"
otelcollector["port"]=4318

### Java ###

# First
declare -A javafirst
javafirst["name"]="java-first"
javafirst["namespace"]="java"
javafirst["port"]=8080
#########

####################
### Build & Push ###
####################

### Java ###

# First
docker build \
  --tag "${DOCKERHUB_NAME}/${javafirst[name]}" \
  "../../apps/java-first/."
docker push "${DOCKERHUB_NAME}/${javafirst[name]}"
#######

######################
### Otel Collector ###
######################
helm upgrade ${otelcollector[name]} \
  --install \
  --wait \
  --debug \
  --create-namespace \
  --namespace ${otelcollector[namespace]} \
  --set dockerhubName=$DOCKERHUB_NAME \
  --set newRelicLicenseKey=$NEWRELIC_LICENSE_KEY \
  --set name=${otelcollector[name]} \
  --set namespace=${otelcollector[namespace]} \
  --set port=${otelcollector[port]} \
  "../charts/newrelic-otelcollector"
#########

#################
### Java Apps ###
#################

# First
helm upgrade ${javafirst[name]} \
  --install \
  --wait \
  --debug \
  --create-namespace \
  --namespace ${javafirst[namespace]} \
  --set dockerhubName=$DOCKERHUB_NAME \
  --set name=${javafirst[name]} \
  --set namespace=${javafirst[namespace]} \
  --set port=${javafirst[port]} \
  "../charts/java-first"
#########
