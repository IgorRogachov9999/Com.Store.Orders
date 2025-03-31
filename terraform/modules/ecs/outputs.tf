output "ecs_cluster" {
  value = aws_ecs_cluster.orders
}

output "ecs_service" {
  value = aws_ecs_service.orders
}