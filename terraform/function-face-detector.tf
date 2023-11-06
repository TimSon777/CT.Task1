data "archive_file" "zip" {
  output_path = "function.zip"
  type        = "zip"
  source_dir  = "../src/Functions/FaceDetector"
  excludes    = ["bin", "obj"]
}

resource "yandex_function" "face_detector_func" {
  name               = "${local.prefix}-face-detection"
  description        = "Функция, по картинке определяющая лица"
  user_hash          = "1"
  runtime            = "dotnet8"
  entrypoint         = "FaceDetector.Handler"
  memory             = "128"
  execution_timeout  = "60"
  service_account_id = yandex_iam_service_account.sa_default.id
  content {
    zip_filename = "function.zip"
  }
}
