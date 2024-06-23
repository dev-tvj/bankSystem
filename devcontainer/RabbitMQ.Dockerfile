# Use the official RabbitMQ image as a base
FROM rabbitmq:3-management

# Copy the RabbitMQ script to the container
COPY rabbitMqScript.sh /rabbitMqScript.sh

# Grant execute permission to the RabbitMQ script
RUN chmod +x /rabbitMqScript.sh

# Script to run the RabbitMQ script and then keep the container running
RUN /rabbitMqScript.sh