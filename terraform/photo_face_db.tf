resource "yandex_ydb_database_serverless" "photo_face" {
  name = var.photo_face_database_name
  connection {
    host     = ""
    user     = var.photo_face_database_user
    password = var.photo_face_database_password
  }
}

resource "yandex_ydb_table" "faces" {
  path              = "${var.photo_face_database_name}/${var.photo_face_database_faces_table_name}"
  connection_string = yandex_ydb_database_serverless.photo_face.ydb_full_endpoint
  primary_key       = []

  column {
    name = "user_defined_name"
    type = "String"
  }

  column {
    name     = "original_photo_id"
    type     = "String"
    not_null = true
  }

  column {
    name     = "original_photo_bucket_id"
    type     = "String"
    not_null = true
  }

  column {
    name     = "id"
    type     = "String"
    not_null = true
  }
}
