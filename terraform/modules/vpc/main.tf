data "aws_availability_zones" "available" {}

resource "aws_vpc" "vpc" {
  cidr_block = "192.0.0.0/16"
  enable_dns_support = true
  enable_dns_hostnames = true
}

resource "aws_internet_gateway" "internal_gateway" {
  vpc_id = aws_vpc.vpc.id
}

resource "aws_route_table" "route_table" {
  vpc_id = aws_vpc.vpc.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.internal_gateway.id
  }
}

resource "aws_subnet" "elb_a" {
  vpc_id = aws_vpc.vpc.id
  cidr_block = "192.0.0.0/24"
  availability_zone = data.aws_availability_zones.available.names[0]
  map_public_ip_on_launch = true
  tags = {
    Name = "elb-a"
  }
}

resource "aws_subnet" "elb_b" {
  vpc_id = aws_vpc.vpc.id
  cidr_block = "192.0.1.0/24"
  availability_zone = data.aws_availability_zones.available.names[1]
  map_public_ip_on_launch = true
  tags = {
    Name = "elb-b"
  }
}

resource "aws_subnet" "ecs_a" {
  vpc_id = aws_vpc.vpc.id
  cidr_block = "192.0.2.0/24"
  availability_zone = data.aws_availability_zones.available.names[0]
  map_public_ip_on_launch = true
}

resource "aws_subnet" "ecs_b" {
  vpc_id = aws_vpc.vpc.id
  cidr_block = "192.0.3.0/24"
  availability_zone = data.aws_availability_zones.available.names[1]
  map_public_ip_on_launch = true
}

resource "aws_subnet" "lambda_a" {
  vpc_id = aws_vpc.vpc.id
  cidr_block = "192.0.4.0/24"
  availability_zone = data.aws_availability_zones.available.names[0]
  map_public_ip_on_launch = true
}

resource "aws_subnet" "lambda_b" {
  vpc_id = aws_vpc.vpc.id
  cidr_block = "192.0.5.0/24"
  availability_zone = data.aws_availability_zones.available.names[1]
  map_public_ip_on_launch = true
}

resource "aws_subnet" "db_a" {
  vpc_id = aws_vpc.vpc.id
  cidr_block = "192.0.6.0/24"
  availability_zone = data.aws_availability_zones.available.names[0]
  map_public_ip_on_launch = true
}

resource "aws_subnet" "db_b" {
  vpc_id = aws_vpc.vpc.id
  cidr_block = "192.0.7.0/24"
  availability_zone = data.aws_availability_zones.available.names[1]
  map_public_ip_on_launch = true
}

resource "aws_subnet" "redis_a" {
  vpc_id = aws_vpc.vpc.id
  cidr_block = "192.0.8.0/24"
  availability_zone = data.aws_availability_zones.available.names[0]
  map_public_ip_on_launch = true
}

resource "aws_subnet" "redis_b" {
  vpc_id = aws_vpc.vpc.id
  cidr_block = "192.0.9.0/24"
  availability_zone = data.aws_availability_zones.available.names[1]
  map_public_ip_on_launch = true
}

resource "aws_route_table_association" "elb_a" {
  subnet_id = aws_subnet.elb_a.id
  route_table_id = aws_route_table.route_table.id
}

resource "aws_route_table_association" "elb_b" {
  subnet_id = aws_subnet.elb_b.id
  route_table_id = aws_route_table.route_table.id
}

resource "aws_security_group" "load_balancer" {
  vpc_id = aws_vpc.vpc.id
  tags = {
    Name = "load-balancer"
  }
}

resource "aws_security_group" "ecs_task" {
  vpc_id = aws_vpc.vpc.id
  tags = {
    Name = "ecs-task"
  }
}

resource "aws_security_group" "lambda" {
  vpc_id = aws_vpc.vpc.id
  tags = {
    Name = "lambda"
  }
}

resource "aws_security_group" "db" {
  vpc_id = aws_vpc.vpc.id
  tags = {
    Name = "db"
  }
}

resource "aws_security_group" "redis" {
  vpc_id = aws_vpc.vpc.id
  tags = {
    Name = "redis"
  }
}
resource "aws_security_group_rule" "ingress_load_balancer_http" {
  from_port = 80
  protocol = "tcp"
  security_group_id = aws_security_group.load_balancer.id
  to_port = 80
  cidr_blocks = [
    "0.0.0.0/0"]
  type = "ingress"
}

resource "aws_security_group_rule" "ingress_load_balancer_https" {
  from_port = 443
  protocol = "tcp"
  security_group_id = aws_security_group.load_balancer.id
  to_port = 443
  cidr_blocks = [
    "0.0.0.0/0"]
  type = "ingress"
}

resource "aws_security_group_rule" "ingress_ecs_task_elb" {
  from_port = 80
  protocol = "tcp"
  security_group_id = aws_security_group.ecs_task.id
  to_port = 80
  source_security_group_id = aws_security_group.load_balancer.id
  type = "ingress"
}

resource "aws_security_group_rule" "egress_load_balancer" {
  type = "egress"
  from_port = 0
  to_port = 65535
  protocol = "tcp"
  cidr_blocks = [
    "0.0.0.0/0"]
  security_group_id = aws_security_group.load_balancer.id
}

resource "aws_security_group_rule" "egress_ecs_task" {
  type = "egress"
  from_port = 0
  to_port = 65535
  protocol = "tcp"
  cidr_blocks = [
    "0.0.0.0/0"]
  security_group_id = aws_security_group.ecs_task.id
}

resource "aws_network_acl" "load_balancer" {
  vpc_id = aws_vpc.vpc.id
  subnet_ids = [
    aws_subnet.elb_a.id,
    aws_subnet.elb_b.id]
}

resource "aws_network_acl" "ecs_task" {
  vpc_id = aws_vpc.vpc.id
  subnet_ids = [
    aws_subnet.ecs_a.id,
    aws_subnet.ecs_b.id]
}

resource "aws_network_acl" "lambda" {
  vpc_id = aws_vpc.vpc.id
  subnet_ids = [
    aws_subnet.lambda_a.id,
    aws_subnet.lambda_b.id]
}
resource "aws_network_acl_rule" "load_balancer_http" {
  network_acl_id = aws_network_acl.load_balancer.id
  rule_number = 100
  egress = false
  protocol = "tcp"
  rule_action = "allow"
  cidr_block = "0.0.0.0/0"
  from_port = 80
  to_port = 80
}

resource "aws_network_acl_rule" "load_balancer_https" {
  network_acl_id = aws_network_acl.load_balancer.id
  rule_number = 200
  egress = false
  protocol = "tcp"
  rule_action = "allow"
  cidr_block = "0.0.0.0/0"
  from_port = 443
  to_port = 443
}

resource "aws_network_acl_rule" "ingress_load_balancer_ephemeral" {
  network_acl_id = aws_network_acl.load_balancer.id
  rule_number = 300
  egress = false
  protocol = "tcp"
  rule_action = "allow"
  cidr_block = "0.0.0.0/0"
  from_port = 1024
  to_port = 65535
}

resource "aws_network_acl_rule" "ecs_task_ephemeral" {
  network_acl_id = aws_network_acl.ecs_task.id
  rule_number = 100
  egress = false
  protocol = "tcp"
  rule_action = "allow"
  cidr_block = "0.0.0.0/0"
  from_port = 1024
  to_port = 65535
}

resource "aws_network_acl_rule" "ecs_task_http" {
  network_acl_id = aws_network_acl.ecs_task.id
  rule_number = 200
  egress = false
  protocol = "tcp"
  rule_action = "allow"
  cidr_block = aws_vpc.vpc.cidr_block
  from_port = 80
  to_port = 80
}

resource "aws_network_acl_rule" "load_balancer_ephemeral" {
  network_acl_id = aws_network_acl.load_balancer.id
  rule_number = 100
  egress = true
  protocol = "tcp"
  rule_action = "allow"
  from_port = 0
  to_port = 65535
  cidr_block = "0.0.0.0/0"
}

resource "aws_network_acl_rule" "ecs_task_all" {
  network_acl_id = aws_network_acl.ecs_task.id
  rule_number = 100
  egress = true
  protocol = "tcp"
  rule_action = "allow"
  from_port = 0
  to_port = 65535
  cidr_block = "0.0.0.0/0"
}