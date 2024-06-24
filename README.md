## Project: Microservices Communication using Messaging

This repository contains a proposal for efficient communication between three microservices using messaging, based on the following requirements and specifications:

### Problem Statement

The objective is to ensure communication between three microservices when a new client is registered. Specifically, we need to:

1. **Generate a credit proposal** for the new client.
2. **Issue 1 or more credit cards** for the new client.

To handle potential communication errors during credit proposal generation or card issuance, resilient processes must be implemented, with appropriate event signaling to the client microservice.

### Microservices Involved

The three microservices involved in this solution are:

1. **Customer Registration**
2. **Credit Proposal**
3. **Credit Card Issuance**

### Technical Requirements

To implement this solution, the following technical requirements must be met:

1. **Credit Proposal**: Should be implemented using `.NET 8.0`.
2. **Messaging Service**: Should utilize one of the following technologies:
   - RabbitMQ or;
   - Azure Service Bus or;
   - Azure Event Hub.
