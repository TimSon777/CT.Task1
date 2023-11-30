resource "yandex_storage_bucket" "faces" {
  bucket     = "${var.prefix}-faces"
  access_key = yandex_iam_service_account_static_access_key.sa_default_keys.access_key
  secret_key = yandex_iam_service_account_static_access_key.sa_default_keys.secret_key
}
