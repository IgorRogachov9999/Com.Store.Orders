resource "aws_elasticache_subnet_group" "redis_subnet_group" {
  name       = "redis-subnet-group"
  subnet_ids = [var.redis_subnet_a.id, var.redis_subnet_b.id]
}

resource "aws_elasticache_cluster" "redis_orders" {
  cluster_id = "redis-orders"
  engine                = "redis"
  node_type            = "cache.t3.micro"
  num_cache_nodes      = 1
  parameter_group_name = "default.redis3.2"
  port                 = 6379
  subnet_group_name    = aws_elasticache_subnet_group.redis_subnet_group.name
  security_group_ids  = [var.redis_sg.id]
}