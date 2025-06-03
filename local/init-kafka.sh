#!/bin/bash
echo "Waiting for Kafka to be ready..."
sleep 2

kafka-topics --create --topic pair-request-processed --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1 --if-not-exists
