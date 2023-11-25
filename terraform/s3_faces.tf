resource "yandex_storage_bucket" "faces" {
  bucket     = "${local.prefix}-faces"
  access_key = yandex_iam_service_account_static_access_key.sa_default_keys.access_key
  secret_key = yandex_iam_service_account_static_access_key.sa_default_keys.secret_key
  max_size   = 1073741824
  anonymous_access_flags {
    read = false
    list = false
  }
}
