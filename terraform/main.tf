terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "5.0"
    }
  }
}

# Configure the AWS Provider
provider "aws" {
  region = var.region
  default_tags {
    tags = {
      Environment = "dev"
      Terraform   = "true"
      Name        = "orders"
      Project     = "orders"
      Billing     = "orders"
    }
  }
}

module "secrets_manager" {
  source = "./modules/secrets-manager"
}

module "vpc" {
  source = "./modules/vpc"
}

module "elb" {
  source                 = "./modules/elb"
  hosted_zone_id         = var.hosted_zone_id
  load_balancer_sg       = module.vpc.load_balancer_sg
  load_balancer_subnet_a = module.vpc.load_balancer_subnet_a
  load_balancer_subnet_b = module.vpc.load_balancer_subnet_b
  vpc                    = module.vpc.vpc
}

module "iam" {
  source        = "./modules/iam"
  elb           = module.elb.elb
  orders_secret = module.secrets_manager.orders_secret
}

module "ecr" {
  source = "./modules/ecr"
}

module "ecs" {
  source                = "./modules/ecs"
  ecs_role              = module.iam.ecs_role
  ecs_sg                = module.vpc.ecs_sg
  ecs_subnet_a          = module.vpc.ecs_subnet_a
  ecs_subnet_b          = module.vpc.ecs_subnet_b
  ecs_target_group      = module.elb.ecs_target_group
  ecr_orders_repostiory = module.ecr.ecr_orders_repostiory
  redis_sg              = module.vpc.redis_sg
  orders_secret         = module.secrets_manager.orders_secret
}

module "auto_scaling" {
  source      = "./modules/auto-scaling"
  ecs_cluster = module.ecs.ecs_cluster
  ecs_service = module.ecs.ecs_service
}

module "sqs" {
  source = "./modules/sqs"
}

module "lambda" {
  source                         = "./modules/lambda"
  sqs_order_status_updated_queue = module.sqs.sqs_order_status_updated_queue
  ecr_orders_lambda_repostiory   = module.ecr.ecr_orders_lambda_repostiory
  lambda_subnet_a                = module.vpc.lambda_subnet_a
  lambda_subnet_b                = module.vpc.lambda_subnet_b
  lambda_sg                      = module.vpc.lambda_sg
}

module "db" {
  source      = "./modules/db"
  db_subnet_a = module.vpc.db_subnet_a
  db_subnet_b = module.vpc.db_subnet_b
  db_sg       = module.vpc.db_sg
  db_password = var.db_password
}

module "elasticache" {
  source         = "./modules/elasticache"
  redis_subnet_a = module.vpc.redis_subnet_a
  redis_subnet_b = module.vpc.redis_subnet_b
  redis_sg       = module.vpc.redis_sg
}