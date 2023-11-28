terraform {
  required_providers {
    yandex = {
      source = "yandex-cloud/yandex"
    }
  }
  required_version = ">= 0.13"
}

locals {
  zone                            = "ru-central1-a"
  service_account_key_file        = "key.json"
  prefix                          = "vvot${var.account_number}"
  photo_faces_database_faces_path = "faces"
}

provider "yandex" {
  service_account_key_file = local.service_account_key_file
  cloud_id                 = var.cloud_id
  folder_id                = var.folder_id
  zone                     = local.zone
}
