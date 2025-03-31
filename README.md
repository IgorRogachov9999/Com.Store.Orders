## Orders service overview
Order service provides functionality to create orders, manage order status, and get order current state information.

## Table of Contents
- [Pre Requirements](#pre-requirements)
- [Setup Instructions](#setup-instructions)
- [Architecture Overview](#architecture-overview)
- [Hosting, Reliability, Scalability, And Security](#hosting-reliability-scalability-and-security)
- [Testing](#testing)

## Pre Requirements

1. **Terraform**
   - Version: 1.11.3
   - Installation: [Terraform Download Page](https://www.terraform.io/downloads.html)
   - Verify: `terraform --version`

2. **AWS CLI**
   - Version: 2.25.4
   - Installation: [AWS CLI Installation Guide](https://aws.amazon.com/cli/)
   - Verify: `aws --version`

3. **Docker**
   - Version: 27.5.1
   - Installation: [Docker Desktop](https://www.docker.com/products/docker-desktop)
   - Verify: `docker --version`

4. **.NET**
   - Version: 8.0.403
   - Installation: [Download .NET](https://dotnet.microsoft.com/en-us/download)
   - Verify: `dotnet --version`

## Setup Instructions

### 1. Configure infrusturcture

```bash
# Configure AWS credentials
aws configure
```

```bash
# Navigate to the terraform directory
cd terraform

# Initialize Terraform
terraform init

# Apply the changes
terraform apply
```
### 2. Add secrets to the Secrets Manager
```bash
ConnectionStrings__OrdersDbConnectionString
ConnectionStrings__RedisConnectionString
AwsMessaging__Queues__OrderStatusUpdated
JwtSettings__SecretKey
```

### 3. Build API

```bash
# AWS ECR Authorization
aws ecr get-login-password --region eu-central-1 | docker login --username AWS --password-stdin ${AWS_ACCOUNT_ID}.dkr.ecr.eu-central-1.amazonaws.com
```

```bash
# Navigate to the API directory
cd services/api

# Build Docker image
docker build -t orders-api:latest .

# Tag image for ECR
docker tag orders-api:latest ${AWS_ACCOUNT_ID}.dkr.ecr.eu-central-1.amazonaws.com/orders-api:latest

# Push image to ECR
docker push ${AWS_ACCOUNT_ID}.dkr.ecr.eu-central-1.amazonaws.com/orders-api:latest
```

### 4. Build Lambda
```bash
# Navigate to the Lambda directory
cd services/lambda

# Build Docker image
docker build -t orders-lambda:latest .

# Tag image for ECR
docker tag orders-lambda:latest ${AWS_ACCOUNT_ID}.dkr.ecr.eu-central-1.amazonaws.com/orders-lambda:latest

# Push image to ECR
docker push ${AWS_ACCOUNT_ID}.dkr.ecr.eu-central-1.amazonaws.com/orders-lambda:latest
```

## Architecture Overview

- AWS ECS with Fargate is used as the host to make API scalable and reliable.
- PostgreSQL is used as database for its good balance of functionality/complexity.
- ElastiCache with Redis is used as cache storage to have destributed cache to work with multiple containers.
- Orders are updated by SQS/Lambda to make this workflow scalable for the lowest price.

## Hosting, Reliability, Scalability, And Security

   AWS is the hosting provider for this app, which makes AWS responsible for managing and maintaining hardware, network, and core software. Infrastructure subnets are distributed between 2 availability zones, which makes the system reliable and up when one of the zones is down. High scalability is achieved by deploying services on serverless and container orchestration services, which scale automatically depending on load.

   Api is hosted on AWS ECS with Fargate. This approach allows to have flexibility in scaling app depending on load without needing to manage instances. The ECS automatically pulls new images from ECR.

   Order status update process is hosted on AWS Lambda with SQS triger. Lambda is scaling automaticaly when queue messages count is getting high and free when there are no messages. SQS is configured to use long poling that makes Lambda cost-effective.

   The database is hosted on RDS and can be scaled both vertically and horizontally on demand. Reliabilty is achived by regular backup.

   ElastiCache is fully managed by AWS and can be scaled both verticaly and horizontaly by demand.

   Services security is protected by authorization, SQL injections and other data issues are handled by ORM, sencitive data is stored in Secrets Manager. IAM service-to-service communcation may be added on demand.

## Testing

```bash
# Integration tests dependencies are covered by Testcontainers. 
# Run solution tests
dotnet test
```