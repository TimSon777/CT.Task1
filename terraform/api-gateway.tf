resource "yandex_api_gateway" "default" {
  name        = "${var.prefix}-apigw"
  description = "API-Gateway, доступный из интернета"
  spec        = <<-EOT
      openapi: "3.0.0"
      info:
        version: 1.0.0
        title: API
      paths:
        /faces/{face}:
          get:
            parameters:
              - name: face
                in: path
                description: User name to appear in greetings
                required: true
                schema:
                  type: string
            x-yc-apigateway-integration:
              type: object_storage
              bucket: ${yandex_storage_bucket.faces.id}
              object: '{face}'
              error_object: error.html
              service_account_id: ${yandex_iam_service_account.sa_default.id}
        /photo/{photo}:
          get:
            parameters:
              - name: photo
                in: path
                description: User name to appear in greetings
                required: true
                schema:
                  type: string
            x-yc-apigateway-integration:
              type: object_storage
              bucket: ${yandex_storage_bucket.photo.id}
              object: '{photo}'
              error_object: error.html
              service_account_id: ${yandex_iam_service_account.sa_default.id}
  EOT
}
