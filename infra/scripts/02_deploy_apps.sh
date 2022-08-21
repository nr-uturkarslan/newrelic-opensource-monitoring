#!/bin/bash

##################
### Apps Setup ###
##################

### Set variables

# Otel Collector
declare -A otelcollector
otelcollector["name"]="otelcollector"
otelcollector["namespace"]="monitoring"
otelcollector["port"]=4318

# Fluent Bit
declare -A fluentbit
fluentbit["name"]="fluentbit"
fluentbit["namespace"]="monitoring"
fluentbit["port"]=8006

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
  --set newRelicLicenseKey=$NEWRELIC_LICENSE_KEY \
  --set name=${otelcollector[name]} \
  --set namespace=${otelcollector[namespace]} \
  --set port=${otelcollector[port]} \
  "../charts/newrelic-otelcollector"
#########

##################
### Fluent Bit ###
##################
helm upgrade ${fluentbit[name]} \
  --install \
  --wait \
  --debug \
  --create-namespace \
  --namespace ${fluentbit[namespace]} \
  --set namespace=${fluentbit[namespace]} \
  "../charts/fluentbit"
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
