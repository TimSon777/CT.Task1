data "http" "webhook" {
  url = "https://api.telegram.org/bot${var.telegram_token}/setWebhook?url=https://functions.yandexcloud.net/${yandex_function.telegram_bot.id}"
}
