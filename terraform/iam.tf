resource "yandex_iam_service_account" "sa_default" {
  folder_id   = var.folder_id
  name        = "sa-default"
  description = "Сервисный аккаунт для управления различными сервисами"
}

resource "yandex_resourcemanager_folder_iam_member" "sa_default_grant" {
  folder_id = var.folder_id
  role      = "admin"
  member    = "serviceAccount:${yandex_iam_service_account.sa_default.id}"
}

resource "yandex_iam_service_account_static_access_key" "sa_default_keys" {
  service_account_id = yandex_iam_service_account.sa_default.id
  description        = "Ключи для управления различными сервисами"
}

resource "yandex_iam_service_account_api_key" "default" {
  service_account_id = yandex_iam_service_account.sa_default.id
  description        = "API ключ для доступа к различным сервисам"
}
