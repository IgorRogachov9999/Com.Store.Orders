resource "aws_sqs_queue" "order_status_updated_queue" {
  name                      = "order-status-updated"
  delay_seconds             = 90
  max_message_size          = 2048
  message_retention_seconds = 86400
  receive_wait_time_seconds = 20
  redrive_policy = jsonencode({
    deadLetterTargetArn = aws_sqs_queue.order_status_updated_queue_deadletter.arn
    maxReceiveCount     = 4
  })
}

resource "aws_sqs_queue" "order_status_updated_queue_deadletter" {
  name = "order-status-updated-deadletter-queue"
}

resource "aws_sqs_queue_redrive_allow_policy" "order_status_updated_queue_redrive_allow_policy" {
  queue_url = aws_sqs_queue.order_status_updated_queue_deadletter.id

  redrive_allow_policy = jsonencode({
    redrivePermission = "byQueue",
    sourceQueueArns   = [aws_sqs_queue.order_status_updated_queue.arn]
  })
}