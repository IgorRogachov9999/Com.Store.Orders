resource "aws_appautoscaling_target" "orders_target" {
  max_capacity = 5
  min_capacity = 2
  resource_id = "service/${var.ecs_cluster.name}/${var.ecs_service.name}"
  scalable_dimension = "ecs:service:DesiredCount"
  service_namespace = "ecs"
}

resource "aws_appautoscaling_policy" "orders_memory" {
  name               = "orders-memory"
  policy_type        = "TargetTrackingScaling"
  resource_id        = aws_appautoscaling_target.orders_target.resource_id
  scalable_dimension = aws_appautoscaling_target.orders_target.scalable_dimension
  service_namespace  = aws_appautoscaling_target.orders_target.service_namespace

  target_tracking_scaling_policy_configuration {
    predefined_metric_specification {
      predefined_metric_type = "ECSServiceAverageMemoryUtilization"
    }

    target_value       = 80
  }
}

resource "aws_appautoscaling_policy" "orders_cpu" {
  name = "orders-cpu"
  policy_type = "TargetTrackingScaling"
  resource_id = aws_appautoscaling_target.orders_target.resource_id
  scalable_dimension = aws_appautoscaling_target.orders_target.scalable_dimension
  service_namespace = aws_appautoscaling_target.orders_target.service_namespace

  target_tracking_scaling_policy_configuration {
    predefined_metric_specification {
      predefined_metric_type = "ECSServiceAverageCPUUtilization"
    }

    target_value = 60
  }
}