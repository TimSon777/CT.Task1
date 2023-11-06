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
