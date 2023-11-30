import json
import requests
import os
import ydb
import boto3
import uuid
import string
import random

driver = ydb.Driver(
  endpoint=os.getenv('YDB_ENDPOINT'),
  database=os.getenv('YDB_DATABASE'),
  credentials=ydb.iam.MetadataUrlCredentials(),
)

driver.wait(fail_fast=True, timeout=5)
pool = ydb.SessionPool(driver)

def process_find_name_command_db(user_defined_name, session):
  query = f'''
    SELECT * FROM `{os.getenv('YDB_TABLE')}`
    WHERE `user_defined_name` = '{user_defined_name}'
    LIMIT 10
    '''

  query = session.prepare(query)
  return session.transaction().execute(
    query,
    commit_tx=True,
    settings=ydb.BaseRequestSettings().with_timeout(3).with_operation_timeout(2)
  )

def process_get_face_command_db(session):
  query = f'''
    SELECT * FROM `{os.getenv('YDB_TABLE')}`
    WHERE `user_defined_name` IS NULL
    LIMIT 1
    '''

  query = session.prepare(query)
  return session.transaction().execute(
    query,
    commit_tx=True,
    settings=ydb.BaseRequestSettings().with_timeout(3).with_operation_timeout(2)
  )

def process_send_photo_command_db(telegram_file_id, name, session):
  query = f'''
    UPDATE `{os.getenv('YDB_TABLE')}`
    SET `telegram_file_id` = '{telegram_file_id}'
    WHERE `name` = '{name}'
    '''

  query = session.prepare(query)
  return session.transaction().execute(
    query,
    commit_tx=True,
    settings=ydb.BaseRequestSettings().with_timeout(3).with_operation_timeout(2)
  )

def process_reply_to_command_db(telegram_file_id, user_defined_name, session):
  query = f'''
    UPDATE `{os.getenv('YDB_TABLE')}`
    SET `user_defined_name` = '{user_defined_name}'
    WHERE `telegram_file_id` = '{telegram_file_id}'
    '''

  query = session.prepare(query)
  return session.transaction().execute(
    query,
    commit_tx=True,
    settings=ydb.BaseRequestSettings().with_timeout(3).with_operation_timeout(2)
  )

def generate_random_string(length):
    characters = string.ascii_letters + string.digits
    random_string = ''.join(random.choices(characters, k=length))
    return random_string

def handler(event, context):
  tgkey = os.environ["TG_KEY"]
  update = json.loads(event["body"])
  message = update["message"]
  message_id = message["message_id"]
  chat_id = message["chat"]["id"]

  if "text" in message and message["text"] == '/getface':
    result = pool.retry_operation_sync(process_get_face_command_db)[0]
    if len(result.rows) == 0:
      requests.get(
        url=f'https://api.telegram.org/bot{tgkey}/sendMessage',
        params={
          "chat_id": chat_id,
          "text": "Нет доступных лиц",
          "reply_to_message_id": message_id
        }
      )
    else:
      name = result.rows[0].name
      response = requests.post(
        url=f'https://api.telegram.org/bot{tgkey}/sendPhoto',
        data={
          "chat_id": chat_id,
          "photo": f'{os.getenv("FACE_URI")}/faces/{name}',
          "reply_to_message_id": message_id
        }
      )
      file_id = json.loads(response.text)['result']['photo'][0]['file_id']
      pool.retry_operation_sync(lambda session: process_send_photo_command_db(file_id, name, session))
  elif "text" in message and message["text"].startswith('/find '):
    name = message["text"].split(" ", 1)[1]
    result = pool.retry_operation_sync(lambda session: process_find_name_command_db(name, session))
    print(result[0].rows)
    if len(result[0].rows) == 0:
      requests.get(
        url=f'https://api.telegram.org/bot{tgkey}/sendMessage',
        params={
          "chat_id": chat_id,
          "text": "Ничего не найдено." ,
          "reply_to_message_id": message_id
        }
      )
    else:
      links = [f'{os.getenv("PHOTO_URI")}/photo/{row.original_photo_id}' for row in result[0].rows]
      media_group = json.dumps(
        [{"type": "photo", "media": link} for link in links]
      )
      requests.post(
        url=f'https://api.telegram.org/bot{tgkey}/sendMediaGroup',
        data={
          "chat_id": chat_id,
          "media": media_group,
          "reply_to_message_id": message_id
        }
      )
  elif 'reply_to_message' in message and len(message['reply_to_message']["photo"]) > 0:
    file_id = message['reply_to_message']['photo'][0]['file_id']
    user_defined_name = message['text']
    pool.retry_operation_sync(lambda session: process_reply_to_command_db(file_id, user_defined_name, session))
    requests.get(
      url=f'https://api.telegram.org/bot{tgkey}/sendMessage',
      params={
        "chat_id": chat_id,
        "text": "Отличное название!" ,
        "reply_to_message_id": message_id
      }
    )
  elif "photo" in message and len(message["photo"]) > 0:
    photos = list(filter(lambda x: x['file_size'] <= 1024 ** 2, message['photo']))
    if len(photos) == 0:
      requests.get(
        url=f'https://api.telegram.org/bot{tgkey}/sendMessage',
        params={
          "chat_id": chat_id,
          "text": "Слишком большой размер файла." ,
          "reply_to_message_id": message_id
        }
      )
    else:
      photo = max(photos, key=lambda x: x['file_size'])

      file_path_response = requests.get(f'https://api.telegram.org/bot{tgkey}/getFile?file_id={photo["file_id"]}')
      file_path = file_path_response.json()['result']['file_path']

      image_response = requests.get(f'https://api.telegram.org/file/bot{tgkey}/{file_path}')

      session = boto3.session.Session()
      storage = session.client(service_name='s3')

      storage.put_object(
        Bucket=os.getenv('PHOTO_BUCKET'),
        Key=generate_random_string(10) + ".jpg",
        Body=image_response.content
      )

      requests.get(
        url=f'https://api.telegram.org/bot{tgkey}/sendMessage',
        params={
          "chat_id": chat_id,
          "text": "Спасибо, что поделились вашей фоткой." ,
          "reply_to_message_id": message_id
        }
      )
  else:
    requests.get(
      url=f'https://api.telegram.org/bot{tgkey}/sendMessage',
      params={
        "chat_id": chat_id,
        "text": "Ошибка." ,
        "reply_to_message_id": message_id
      }
    )

  return {
    "statusCode": 200
  }
