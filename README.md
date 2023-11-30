## Задание

[Ссылка](https://docs.itiscl.ru/vvot/2023-2024/tasks/task01/task01.html)


## Мой бот

[Ссылка](https://t.me/vvot14_2023_bot)

## Как запустить в другом облаке

1. Меняем в `./terraform/terraform.tfvars`:
    `cloud_id` - если в другом облаке
    `folder_id` - всегда
    `telegram_token` - если создаете нового бота
    `prefix` - если в `itis-vvot` тестируете
2. Создать сервисный аккаунт с ролью `admin`, чтобы через него применять конфигурации terraform
3. Сгенерировать key.json. [Документация](https://cloud.yandex.ru/docs/tutorials/infrastructure-management/terraform-quickstart#get-credentials). См команду снизу. Положить в `./terraform`
4. Из `./terraform` выполнить `terraform init` и `terraform apply -auto-approve`

yc iam key create \
  --service-account-id <идентификатор_сервисного_аккаунта> \
  --folder-name <имя_каталога_с_сервисным_аккаунтом> \
  --output key.json

## Функционал бота
1. Отправить картинку
2. `/getface`
3. Ответить на сообщение с картинкой после команды `/getface` - установить название
4. `/find {name}` - поиск оригинальных картинок с названием `name`
