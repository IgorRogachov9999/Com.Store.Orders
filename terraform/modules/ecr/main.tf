resource "aws_ecr_repository" "orders_ecr" {
  name                 = "orders-api"

  image_scanning_configuration {
    scan_on_push = true
  }
}

resource "aws_ecr_repository" "orders_lambda_ecr" {
  name                 = "orders-lambda"

  image_scanning_configuration {
    scan_on_push = true
  }
}