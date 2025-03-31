variable "db_password" {
  description = "Password for the database"
  type        = string
  sensitive   = true
}

variable "db_sg" {}

variable "db_subnet_a" {}

variable "db_subnet_b" {}
