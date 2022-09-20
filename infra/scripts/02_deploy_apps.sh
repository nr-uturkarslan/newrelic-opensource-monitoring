#!/bin/bash

# Get commandline arguments
while (( "$#" )); do
  case "$1" in
    --cluster-name)
      clusterName="$2"
      shift
      ;;
    *)
      shift
      ;;
  esac
done

if [[ $clusterName == "" ]]; then
  echo -e "Cluster name is not given. (--cluster-name)\n"
  exit 1
fi

##################
### Apps Setup ###
##################

### Set variables
newRelicOtlpGrpcEndpoint="https://otlp.eu01.nr-data.net:4317"

# Otel Collector
declare -A otelcollector
otelcollector["name"]="otelcollector"
otelcollector["namespace"]="monitoring"
otelcollector["grpcPort"]=4317
otelcollector["httpPort"]=4318
otelcollector["fluentPort"]=8006
otelcollector["grpcEndpoint"]="http://${otelcollector[name]}.${otelcollector[namespace]}.svc.cluster.local:${otelcollector[grpcPort]}"

# Fluent Bit
declare -A fluentbit
fluentbit["name"]="fluentbit"
fluentbit["namespace"]="monitoring"

# Prometheus
declare -A prometheus
prometheus["name"]="prometheus"
prometheus["namespace"]="monitoring"

### Java ###

# First
declare -A javafirst
javafirst["name"]="java-first"
javafirst["namespace"]="java"
javafirst["port"]=8080

# Second
declare -A javasecond
javasecond["name"]="java-second"
javasecond["namespace"]="java"
javasecond["port"]=8080

### Simulator ###
declare -A simulator
simulator["name"]="simulator"
simulator["namespace"]="simulator"
simulator["port"]=8080
#########

####################
### Build & Push ###
####################

### Java ###

# First
docker build \
  --build-arg otelExporterOtlpEndpoint=${otelcollector[grpcEndpoint]} \
  --tag "${DOCKERHUB_NAME}/${javafirst[name]}" \
  "../../apps/java-first/."
docker push "${DOCKERHUB_NAME}/${javafirst[name]}"

# Second
docker build \
  --build-arg otelExporterOtlpEndpoint=${otelcollector[grpcEndpoint]} \
  --tag "${DOCKERHUB_NAME}/${javasecond[name]}" \
  "../../apps/java-second/."
docker push "${DOCKERHUB_NAME}/${javasecond[name]}"

# ### Simulator ###
# docker build \
#   --tag "${DOCKERHUB_NAME}/${simulator[name]}" \
#   "../../apps/simulator/."
# docker push "${DOCKERHUB_NAME}/${simulator[name]}"
# #######

#############
### Pixie ###
#############
helm repo add newrelic https://helm-charts.newrelic.com && helm repo update && \
kubectl create namespace "monitoring" ; helm upgrade newrelic-bundle newrelic/nri-bundle \
  --install \
  --wait \
  --debug \
  --set global.licenseKey=$NEWRELIC_LICENSE_KEY \
  --set global.cluster=$clusterName \
  --namespace="monitoring" \
  --set newrelic-infrastructure.privileged=true \
  --set global.lowDataMode=true \
  --set ksm.enabled=true \
  --set kubeEvents.enabled=true \
  --set newrelic-pixie.enabled=true \
  --set newrelic-pixie.apiKey=$PIXIE_API_KEY \
  --set pixie-chart.enabled=true \
  --set pixie-chart.deployKey=$PIXIE_DEPLOY_KEY \
  --set pixie-chart.clusterName=$clusterName 
#########

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
  --set grpcPort=${otelcollector[grpcPort]} \
  --set httpPort=${otelcollector[httpPort]} \
  --set fluentPort=${otelcollector[fluentPort]} \
  --set newRelicOtlpGrpcEndpoint=$newRelicOtlpGrpcEndpoint \
  "../charts/otelcollector"
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

##################
### Prometheus ###
##################
helm dependency update "../charts/prometheus"
helm upgrade prometheus \
  --install \
  --wait \
  --debug \
  --create-namespace \
  --namespace ${prometheus[namespace]} \
  --set namespace=${prometheus[namespace]} \
  --set server.remoteWrite[0].url="https://metric-api.eu.newrelic.com/prometheus/v1/write?prometheus_server=${prometheus[name]}" \
  --set server.remoteWrite[0].bearer_token=$NEWRELIC_LICENSE_KEY \
  --set server.remoteWrite[0].write_relabel_configs[0].source_labels[0]="namespace" \
  --set server.remoteWrite[0].write_relabel_configs[0].regex=${javafirst[namespace]} \
  --set server.remoteWrite[0].write_relabel_configs[0].action="keep" \
  "../charts/prometheus"

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

# Second
helm upgrade ${javasecond[name]} \
  --install \
  --wait \
  --debug \
  --create-namespace \
  --namespace ${javasecond[namespace]} \
  --set dockerhubName=$DOCKERHUB_NAME \
  --set name=${javasecond[name]} \
  --set namespace=${javasecond[namespace]} \
  --set port=${javasecond[port]} \
  "../charts/java-second"
#########

# #################
# ### Simulator ###
# #################

# # Simulator
# helm upgrade ${simulator[name]} \
#   --install \
#   --wait \
#   --debug \
#   --create-namespace \
#   --namespace ${simulator[namespace]} \
#   --set dockerhubName=$DOCKERHUB_NAME \
#   --set name=${simulator[name]} \
#   --set imageName=${simulator[name]} \
#   --set namespace=${simulator[namespace]} \
#   --set port=${simulator[port]} \
#   "../charts/simulator"
# #########