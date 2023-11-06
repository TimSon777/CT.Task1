terraform {
  required_providers {
    yandex = {
      source = "yandex-cloud/yandex"
    }
  }
  required_version = ">= 0.13"
}

locals {
  zone                     = "ru-central1-a"
  service_account_key_file = "key.json"
  prefix                   = "vvot14"
}

provider "yandex" {
  service_account_key_file = local.service_account_key_file
  cloud_id                 = var.cloud_id
  folder_id                = var.folder_id
  zone                     = local.zone
}
