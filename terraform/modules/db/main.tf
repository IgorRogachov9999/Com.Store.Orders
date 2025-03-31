resource "aws_db_subnet_group" "orders_db_subnet_group" {
  name       = "orders-db-subnet-group"
  subnet_ids = [var.db_subnet_a.id, var.db_subnet_b.id]
}

resource "aws_db_instance" "orders_db" {
  identifier           = "orders"
  engine              = "postgres"
  engine_version      = "14.7"
  instance_class      = "db.t3.micro"
  allocated_storage   = 20
  max_allocated_storage = 100
  storage_type        = "gp2"
  db_name             = "orders"
  username           = "postgres"
  password           = var.db_password
  port               = 5432

  db_subnet_group_name   = aws_db_subnet_group.orders_db_subnet_group.name
  vpc_security_group_ids = [var.db_sg.id]

  backup_retention_period = 7
  backup_window          = "03:00-04:00"
  maintenance_window     = "Mon:04:00-Mon:05:00"

  skip_final_snapshot    = true
  publicly_accessible    = false
  multi_az              = false
  copy_tags_to_snapshot = true
}