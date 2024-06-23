#!/bin/bash

# #########################################################
# #                    HOW TO EXECUTE                     #
# #########################################################
# 
# 1) Ensure you have RabbitMQ running on your local machine.
# 
# 2) Open a terminal
# 
# 3) Navigate to the "bankSystem" folder
# 
# 4) Make the script executable by running the following command:
#    chmod +x rabbitMqScript.sh
# 
# 5) Execute the script by running:
#    ./rabbitMqScript.sh
# 
# This script will configure RabbitMQ with the following:
# - Create a direct exchange named 'customer_exchange'
# - Create two queues: 
#       'credit_proposal_queue' and 'credit_card_queue'
# - Bind these queues to the exchange with routing keys:
#       'credit_proposal' and 'credit_card' respectively
# 
# #########################################################


echo "###############################################"
echo "########## Starting RabbitMQ Script ###########"
echo "###############################################"


RABBITMQ_HOST="localhost"
RABBITMQ_PORT="15672"
USERNAME="guest"
PASSWORD="guest"
EXCHANGE_NAME="customer_exchange"
QUEUES=("credit_proposal_queue" "credit_card_queue" "new_customer_queue")
ROUTING_KEYS=("credit_proposal" "credit_card" "new_customer")


echo "### Installing necessary packages"
apt-get update && sleep 10 && apt-get install -y curl && sleep 5


create_exchange() {
  curl -v -i -u $USERNAME:$PASSWORD -H "content-type:application/json" \
    -X PUT \
    -d'{"type":"direct","durable":true}' \
    http://$RABBITMQ_HOST:$RABBITMQ_PORT/api/exchanges/%2f/$EXCHANGE_NAME
}

create_queue() {
  local queue_name=$1
  curl -v -i -u $USERNAME:$PASSWORD -H "content-type:application/json" \
    -X PUT \
    -d'{"durable":true}' \
    http://$RABBITMQ_HOST:$RABBITMQ_PORT/api/queues/%2f/$queue_name
}

bind_queue() {
  local queue_name=$1
  local routing_key=$2
  curl -v -i -u $USERNAME:$PASSWORD -H "content-type:application/json" \
    -X POST \
    -d'{"routing_key":"'$routing_key'"}' \
    http://$RABBITMQ_HOST:$RABBITMQ_PORT/api/bindings/%2f/e/$EXCHANGE_NAME/q/$queue_name
}

# Creating exchange
create_exchange

# Creating queues and sync to exchange
for i in "${!QUEUES[@]}"; do
  create_queue "${QUEUES[$i]}"
  bind_queue "${QUEUES[$i]}" "${ROUTING_KEYS[$i]}"
done


echo "### Starting RabbitMQ Management and wait until it is ready"
rabbitmq-plugins enable rabbitmq_management && sleep 10


echo "###############################################"
echo "###############################################"
echo "########## Finished RabbitMQ Setup ############"
echo "###############################################"
echo "###############################################"

# Keep RabbitMQ Up
tail -f /dev/null