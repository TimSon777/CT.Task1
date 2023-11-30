data "archive_file" "face_cut_func_zip" {
  output_path = "face_cut_func.zip"
  type        = "zip"
  source_dir  = "../src/Functions/FaceCut"
}

resource "yandex_function" "face_cut_func" {
  name               = "${var.prefix}-face-cut"
  description        = "Функция, обрабатывающая лица"
  user_hash          = data.archive_file.face_cut_func_zip.output_base64sha256
  runtime            = "python312"
  entrypoint         = "index.handler"
  memory             = "128"
  execution_timeout  = "60"
  service_account_id = yandex_iam_service_account.sa_default.id
  environment = {
    "AWS_ACCESS_KEY_ID" : yandex_iam_service_account_static_access_key.sa_default_keys.access_key,
    "AWS_SECRET_ACCESS_KEY" : yandex_iam_service_account_static_access_key.sa_default_keys.secret_key,
    "AWS_ENDPOINT_URL_S3" : var.storage_api_uri,
    "FACES_BUCKET_ID" : yandex_storage_bucket.faces.id,
    "YDB_ENDPOINT" : local.photo_face_db_endpoint,
    "YDB_DATABASE" : yandex_ydb_database_serverless.photo_face.database_path,
    "YDB_TABLE" : local.photo_faces_database_faces_path
  }
  content {
    zip_filename = data.archive_file.face_cut_func_zip.output_path
  }
}
