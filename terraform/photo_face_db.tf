resource "yandex_ydb_database_serverless" "photo_face" {
  name = "${var.prefix}-db-photo-face"
  serverless_database {
    storage_size_limit = 5
  }
}

resource "yandex_ydb_table" "faces" {
  path              = local.photo_faces_database_faces_path
  connection_string = yandex_ydb_database_serverless.photo_face.ydb_full_endpoint
  primary_key       = ["name"]

  column {
    name = "user_defined_name"
    type = "Utf8"
  }

  column {
    name = "original_photo_id"
    type = "Utf8"
  }

  column {
    name     = "name"
    type     = "Utf8"
    not_null = true
  }

  column {
    name = "telegram_file_id"
    type = "Utf8"
  }
}

locals {
  photo_face_db_endpoint = "grpcs://${yandex_ydb_database_serverless.photo_face.ydb_api_endpoint}"
}
