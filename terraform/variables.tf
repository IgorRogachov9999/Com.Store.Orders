variable "region" {
  default = "us-east-1"
  type    = string
}

variable "hosted_zone_id" {
  default = "a"
  type    = string
}

variable "db_password" {
  default = "postgres"
  type    = string
}