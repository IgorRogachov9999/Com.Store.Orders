output "ecr_orders_repostiory" {
  value = aws_ecr_repository.orders_ecr.repository_url
}

output "ecr_orders_lambda_repostiory" {
  value = aws_ecr_repository.orders_lambda_ecr.repository_url
}