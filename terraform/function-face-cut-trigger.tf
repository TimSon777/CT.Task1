resource "yandex_function_trigger" "face_cut_func_trigger" {
  name = "${var.prefix}-task"
  message_queue {
    queue_id           = yandex_message_queue.task_queue.arn
    service_account_id = yandex_iam_service_account.sa_default.id
    batch_cutoff       = "0"
    batch_size         = "1"
  }
  function {
    id                 = yandex_function.face_cut_func.id
    service_account_id = yandex_iam_service_account.sa_default.id
  }
}
