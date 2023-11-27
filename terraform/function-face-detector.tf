data "archive_file" "zip" {
  output_path = "function.zip"
  type        = "zip"
  source_dir  = "../src/Functions/FaceDetector"
  excludes    = ["bin", "obj"]
}

resource "yandex_function" "face_detector_func" {
  name               = "${local.prefix}-face-detection"
  description        = "Функция, по картинке определяющая лица"
  user_hash          = data.archive_file.zip.output_sha256
  runtime            = "dotnet8"
  entrypoint         = "FaceDetector.Handler"
  memory             = "128"
  execution_timeout  = "60"
  service_account_id = yandex_iam_service_account.sa_default.id
  environment = {
    "Yandex__FolderId" : var.folder_id,
    "Yandex__VisionApiUri" : var.vision_api_uri,
    "Yandex__StorageApiUri" : var.storage_api_uri,
    "Yandex__AccessKey" : yandex_iam_service_account_static_access_key.sa_default_keys.access_key,
    "Yandex__SecretKey" : yandex_iam_service_account_static_access_key.sa_default_keys.secret_key,
    "Yandex__ApiKey" : yandex_iam_service_account_api_key.default.secret_key
  }
  content {
    zip_filename = "function.zip"
  }
}
