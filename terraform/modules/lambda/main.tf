variable "lambda_function_name" {
  default = "order-status-updated-handler"
}

resource "aws_iam_role" "order_status_updated_lambda_role" {
  name = "order-status-updated-lambda-role"
  assume_role_policy   = jsonencode({
    Statement = [{
      Action = "sts:AssumeRole"
      Effect = "Allow"
      Principal = {
        Service = "lambda.amazonaws.com"
      }
    }]
  })
}

resource "aws_lambda_function" "order_status_updated_lambda" {
  function_name = var.lambda_function_name
  role = aws_iam_role.order_status_updated_lambda_role.arn
  package_type = "Image"
  image_uri = "${var.ecr_orders_lambda_repostiory}:latest"
  vpc_config {
    subnet_ids = [var.lambda_subnet_a.id, var.lambda_subnet_b.id]
    security_group_ids = [var.lambda_sg.id]
  }

  depends_on = [
    aws_iam_role_policy_attachment.lambda_logs,
    aws_cloudwatch_log_group.lambda_log_group
  ]
}

# This is to optionally manage the CloudWatch Log Group for the Lambda Function.
# If skipping this resource configuration, also add "logs:CreateLogGroup" to the IAM policy below.
resource "aws_cloudwatch_log_group" "lambda_log_group" {
  name              = "/aws/lambda/${var.lambda_function_name}"
  retention_in_days = 14
}

# See also the following AWS managed policy: AWSLambdaBasicExecutionRole
data "aws_iam_policy_document" "lambda_logging" {
  statement {
    effect = "Allow"

    actions = [
      "logs:CreateLogGroup",
      "logs:CreateLogStream",
      "logs:PutLogEvents",
    ]

    resources = ["arn:aws:logs:*:*:*"]
  }
}

resource "aws_iam_policy" "lambda_logging" {
  name        = "lambda_logging"
  path        = "/"
  description = "IAM policy for logging from a lambda"
  policy      = data.aws_iam_policy_document.lambda_logging.json
}

resource "aws_iam_role_policy_attachment" "lambda_logs" {
  role       = aws_iam_role.order_status_updated_lambda_role.name
  policy_arn = aws_iam_policy.lambda_logging.arn
}

#Lambda SQS Trigger
resource "aws_lambda_event_source_mapping" "lambda_order_status_updated_sqs_trigger" {
  event_source_arn = var.sqs_order_status_updated_queue.arn
  function_name    = aws_lambda_function.order_status_updated_lambda.arn
  depends_on       = [var.sqs_order_status_updated_queue]
}