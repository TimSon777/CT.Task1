resource "yandex_iam_service_account" "sa_storage" {
  folder_id   = var.folder_id
  name        = "sa-aws-compatible"
  description = "Сервисный аккаунт для управления `storage`"
}

resource "yandex_resourcemanager_folder_iam_member" "sa_storage_grant_storage" {
  folder_id = var.folder_id
  role      = "storage.editor"
  member    = "serviceAccount:${yandex_iam_service_account.sa_storage.id}"
}

resource "yandex_iam_service_account_static_access_key" "sa_storage_keys" {
  service_account_id = yandex_iam_service_account.sa_storage.id
  description        = "Ключи для управления `storage`"
}
