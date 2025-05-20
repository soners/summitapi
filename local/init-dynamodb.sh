#!/bin/bash

awslocal configure set region eu-central-1

awslocal dynamodb create-table \
  --table-name all-students \
  --attribute-definitions AttributeName=StudentId,AttributeType=S \
  --key-schema AttributeName=StudentId,KeyType=HASH \
  --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5 \
  --region eu-central-1

awslocal dynamodb put-item \
  --table-name all-students \
  --item '{"StudentId": {"S": "68b3nak"}, "Name": {"S": "James Brown"}, "Email": {"S": "james.brown@test.com"}}' \
  --region eu-central-1
