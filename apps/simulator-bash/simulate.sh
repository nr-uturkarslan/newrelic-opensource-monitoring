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
REQUEST_INTERVAL=1

nginxEndpoint="nginx-ingress-ingress-nginx-controller.nginx.svc.cluster.local:80"
dotnetEndpoint="${nginxEndpoint}/dotnet/dotnet/second"
javaEndpoint="${nginxEndpoint}/java/second"
errorEndpoint="${nginxEndpoint}/error"

# Start making requests
loopCount=$(echo $(( $RANDOM % 2 + 1 )))
dotnetCount=$(echo $(( $RANDOM % 3 + 1 )))
javaCount=$(echo $(( $RANDOM % 3 + 1 )))
errorCount=$(echo $(( $RANDOM % 2 )))

for i in {1..$loopCount}
do
  for i in {1..$dotnetCount}
  do
    createValue $dotnetEndpoint
  done

  for i in {1..$javaCount}
  do
    createValue $javaEndpoint
  done

  for i in {1..$errorCount}
  do
    createValue $errorEndpoint
  done
done

exit 0
