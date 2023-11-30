data "archive_file" "face_detector_func_zip" {
  output_path = "face_detector_func.zip"
  type        = "zip"
  source_dir  = "../src/Functions/FaceDetector"
  excludes    = ["bin", "obj"]
}

resource "yandex_function" "face_detector_func" {
  name               = "${var.prefix}-face-detection"
  description        = "Функция, по картинке определяющая лица"
  user_hash          = data.archive_file.face_detector_func_zip.output_base64sha256
  runtime            = "dotnet8"
  entrypoint         = "FaceDetector.Handler"
  memory             = "128"
  execution_timeout  = "60"
  service_account_id = yandex_iam_service_account.sa_default.id
  environment = {
    "Yandex__FolderId" : var.folder_id,
    "Yandex__VisionApiUri" : var.vision_api_uri,
    "Yandex__StorageApiUri" : var.storage_api_uri,
    "Yandex__ApiKey" : yandex_iam_service_account_api_key.default.secret_key
    "Yandex__TaskQueueUri" : yandex_message_queue.task_queue.id,
    "Yandex__QueueApiUri" : var.queue_api_uri,
    "AWS_ACCESS_KEY_ID" : yandex_iam_service_account_static_access_key.sa_default_keys.access_key,
    "AWS_SECRET_ACCESS_KEY" : yandex_iam_service_account_static_access_key.sa_default_keys.secret_key,
    "Yandex__Region" : "ru-central1"
  }
  content {
    zip_filename = data.archive_file.face_detector_func_zip.output_path
  }
}
