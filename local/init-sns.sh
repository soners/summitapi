#!/bin/bash

awslocal configure set region eu-central-1

awslocal sns create-topic \
  --name teacher-student-pair-requested \
  --region eu-central-1
