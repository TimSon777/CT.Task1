resource "yandex_function_trigger" "face_detector_func_trigger" {
  name        = "${var.prefix}-photo"
  description = "Триггер FaceDetector функции"
  object_storage {
    bucket_id    = yandex_storage_bucket.photo.id
    create       = true
    delete       = false
    update       = false
    suffix       = ".jpg"
    batch_cutoff = ""
  }
  function {
    id                 = yandex_function.face_detector_func.id
    service_account_id = yandex_iam_service_account.sa_default.id
  }
}
