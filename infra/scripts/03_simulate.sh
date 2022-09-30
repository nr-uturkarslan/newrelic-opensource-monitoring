###########################################
### Create a pod to make internal calls ###
###########################################

# Start a test pod in the cluster:
sudo kubectl run -it --rm simulator --image=debian:stable

# After the test pod is running, you will gain access to the pod.
# Then you can run the following commands:
apt-get update -y && \
apt-get install dnsutils -y && \
apt-get install curl -y && \
apt-get install vim -y && \
apt-get install jq -y

#########

#################################################
### Run the following script to generate load ###
#################################################

#!/bin/bash

createValue() {

  local endpoint=$1

  local randomValue=$(openssl rand -base64 12)
  local randomTag=$(openssl rand -base64 12)

  echo -e "---\n"

  curl -X POST "http://${endpoint}" \
    -i \
    -H "Content-Type: application/json" \
    -d \
    '{
        "value": "'"${randomValue}"'",
        "tag": "'"${randomTag}"'"
    }'

  echo -e "\n"
  sleep $REQUEST_INTERVAL
}

####################
### SCRIPT START ###
####################

# Set variables
REQUEST_INTERVAL=2

dotnetEndpoint="dotnet-first.dotnet.svc.cluster.local:8080/dotnet/second"
javaEndpoint="java-first.java.svc.cluster.local:8080/java/second"

createValue $dotnetEndpoint
createValue $javaEndpoint

nginxEndpoint="nginx-ingress-ingress-nginx-controller.nginx.svc.cluster.local:80"
dotnetEndpoint="${nginxEndpoint}/dotnet/second"
javaEndpoint="${nginxEndpoint}/java/second"

createValue $dotnetEndpoint
createValue $javaEndpoint

# # Start making requests
# while true
# do

#   # Create value
#   for i in {1..3}
#   do
#     createValue $bravoEndpoint
#     createValue $charlieEndpoint
#   done
# done
