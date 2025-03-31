output "vpc" {
  value = aws_vpc.vpc
}

output "load_balancer_subnet_a" {
  value = aws_subnet.elb_a
}

output "load_balancer_subnet_b" {
  value = aws_subnet.elb_b
}

output "ecs_subnet_a" {
  value = aws_subnet.ecs_a
}

output "ecs_subnet_b" {
  value = aws_subnet.ecs_b
}

output "lambda_subnet_a" {
  value = aws_subnet.lambda_a
}

output "lambda_subnet_b" {
  value = aws_subnet.lambda_b
}

output "db_subnet_a" {
  value = aws_subnet.db_a
}

output "db_subnet_b" {
  value = aws_subnet.db_b
}

output "redis_subnet_a" {
  value = aws_subnet.redis_a
}

output "redis_subnet_b" {
  value = aws_subnet.redis_b
}

output "load_balancer_sg" {
  value = aws_security_group.load_balancer
}

output "ecs_sg" {
  value = aws_security_group.ecs_task
}

output "lambda_sg" {
  value = aws_security_group.lambda
}

output "db_sg" {
  value = aws_security_group.db
}

output "redis_sg" {
  value = aws_security_group.redis
}
