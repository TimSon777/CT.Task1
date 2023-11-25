resource "yandex_message_queue" "task_queue" {
  name       = "${local.prefix}-task"
  fifo_queue = false
  access_key = yandex_iam_service_account_static_access_key.sa_default_keys.access_key
  secret_key = yandex_iam_service_account_static_access_key.sa_default_keys.secret_key
}
