data "archive_file" "telegram_bot_func_zip" {
  output_path = "telegram_bot_func.zip"
  type        = "zip"
  source_dir  = "../src/Functions/Bot"
}

resource "yandex_function" "telegram_bot" {
  name               = "${var.prefix}-boot"
  description        = "Обработчик комманд telegram"
  user_hash          = data.archive_file.telegram_bot_func_zip.output_base64sha256
  runtime            = "python312"
  entrypoint         = "index.handler"
  memory             = "128"
  execution_timeout  = "60"
  service_account_id = yandex_iam_service_account.sa_default.id
  environment = {
    "AWS_ACCESS_KEY_ID" : yandex_iam_service_account_static_access_key.sa_default_keys.access_key,
    "AWS_SECRET_ACCESS_KEY" : yandex_iam_service_account_static_access_key.sa_default_keys.secret_key,
    "AWS_ENDPOINT_URL_S3" : var.storage_api_uri,
    "TG_KEY" : var.telegram_token,
    "YDB_ENDPOINT" : local.photo_face_db_endpoint,
    "YDB_DATABASE" : yandex_ydb_database_serverless.photo_face.database_path,
    "YDB_TABLE" : local.photo_faces_database_faces_path,
    "FACE_URI" : "https://${yandex_api_gateway.default.id}.apigw.yandexcloud.net",
    "PHOTO_URI" : "https://${yandex_api_gateway.default.id}.apigw.yandexcloud.net",
    "PHOTO_BUCKET" : yandex_storage_bucket.photo.id
  }
  content {
    zip_filename = data.archive_file.telegram_bot_func_zip.output_path
  }
}

resource "yandex_function_iam_binding" "telegram_bot_iam" {
  function_id = yandex_function.telegram_bot.id
  role        = "serverless.functions.invoker"

  members = [
    "system:allUsers",
  ]
}
