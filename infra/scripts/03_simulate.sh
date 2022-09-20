###########################################
### Create a pod to make internal calls ###
###########################################

# Start a test pod in the cluster:
sudo kubectl run -it --rm simulator --image=debian:stable

# After the test pod is running, you will gain access to the pod.
# Then you can run the following commands:
apt-get update -y
apt-get install dnsutils -y
apt-get install curl -y
apt-get install vim -y
apt-get install jq -y

# After the packages are installed, test the connectivity to the application pod:
curl -Iv http://http://java-first.java.svc.cluster.local:8080/java/second
#########

#################################################
### Run the following script to generate load ###
#################################################
#!/bin/bash

makeRestCall() {

  local randomValue=$(openssl rand -base64 12)

  echo -e "---\n"

  curl -X POST "http://java-first.java.svc.cluster.local:8080/java/second" \
    -i \
    -H "Content-Type: application/json" \
    -d \
    '{
        "message": "'"${randomValue}"'"
    }'

  echo -e "\n"
  sleep 2
}

while true
do
  makeRestCall
done
#########
