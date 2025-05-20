#!/bin/bash

awslocal configure set region eu-central-1

awslocal sqs create-queue \
  --queue-name teacher-student-pair-requested-queue \
  --region eu-central-1

awslocal sns subscribe \
  --topic-arn arn:aws:sns:eu-central-1:000000000000:teacher-student-pair-requested \
  --protocol sqs \
  --notification-endpoint arn:aws:sqs:eu-central-1:000000000000:teacher-student-pair-requested-queue \
  --region eu-central-1
