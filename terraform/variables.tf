variable "cloud_id" {
  type        = string
  description = "Идентификатор облака"
}

variable "folder_id" {
  type        = string
  description = "Идентификатор каталога"
}

variable "prefix" {
  type        = string
  description = "Префикс для всех ресурсов"
}

variable "vision_api_uri" {
  type        = string
  description = "URI для доступа к API Vision"
}

variable "storage_api_uri" {
  type        = string
  description = "Service URI для доступа к API Storage"
}

variable "queue_api_uri" {
  type        = string
  description = "Service URI для доступа к API Queue"
}

variable "telegram_token" {
  type        = string
  description = "Токен telegram бота"
}
