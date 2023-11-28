resource "yandex_ydb_database_serverless" "photo_face" {
  name = var.photo_face_database_name
  serverless_database {
    storage_size_limit = 2
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
    name     = "original_photo_id"
    type     = "Utf8"
    not_null = true
  }

  column {
    name     = "original_photo_bucket_id"
    type     = "Utf8"
    not_null = true
  }

  column {
    name     = "name"
    type     = "Utf8"
    not_null = true
  }
}
