resource "aws_ecs_cluster" "orders" {
  name = "orders"
  setting {
    name = "containerInsights"
    value = "enabled"
  }
}

resource "aws_ecs_task_definition" "orders" {
  family = "orders"
  container_definitions = <<TASK_DEFINITION
  [
  {
    "portMappings": [
      {
        "hostPort": 80,
        "protocol": "tcp",
        "containerPort": 80
      }
    ],
    "cpu": 512,
    "memory": 1024,
    "image": "${var.ecr_orders_repostiory}:latest",
    "essential": true,
    "name": "site",
    "secrets": [
      {
        "name": "ConnectionStrings__OrdersDbConnectionString",
        "valueFrom": "${var.orders_secret.arn}:ConnectionStrings__OrdersDbConnectionString::"
      },
      {
        "name": "ConnectionStrings__RedisConnectionString",
        "valueFrom": "${var.orders_secret.arn}:ConnectionStrings__RedisConnectionString::"
      },
      {
        "name": "AwsMessaging__Queues__OrderStatusUpdated",
        "valueFrom": "${var.orders_secret.arn}:AwsMessaging__Queues__OrderStatusUpdated::"
      },
      {
        "name": "JwtSettings__SecretKey",
        "valueFrom": "${var.orders_secret.arn}:JwtSettings__SecretKey::"
      }
    ]
  }
]
TASK_DEFINITION

  network_mode = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  memory = "1024"
  cpu = "512"
  execution_role_arn = var.ecs_role.arn
  task_role_arn = var.ecs_role.arn
}

resource "aws_ecs_service" "orders" {
  name = "orders"
  cluster = aws_ecs_cluster.orders.id
  task_definition = aws_ecs_task_definition.orders.arn
  desired_count = 1
  launch_type = "FARGATE"
  platform_version = "1.4.0"

  lifecycle {
    ignore_changes = [
      desired_count]
  }

  network_configuration {
    subnets = [
      var.ecs_subnet_a.id,
      var.ecs_subnet_b.id]
    security_groups = [var.ecs_sg.id, var.redis_sg.id]
    assign_public_ip = true
  }

  load_balancer {
    target_group_arn = var.ecs_target_group.arn
    container_name = "site"
    container_port = 80
  }
}