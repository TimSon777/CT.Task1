variable "cloud_id" {
  type        = string
  description = "Идентификатор облака"
}

variable "folder_id" {
  type        = string
  description = "Идентификатор каталога"
}

variable "photo_bucket_name" {
  type        = string
  description = "Названия бакета для фоток"
}

variable "account_number" {
  type        = number
  description = "Номер аккаунта, используемый в префиксе: например, в vvot14 это 14"
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

variable "photo_face_database_name" {
  type        = string
  description = "Название базы данных"
}

variable "photo_face_database_user" {
  type        = string
  description = "Пользователь для базы данных"
}

variable "photo_face_database_password" {
  type        = string
  description = "Пароль для базы данных"
}

variable "photo_face_database_faces_table_name" {
  type        = string
  description = "Название таблицы с лицами"
}
