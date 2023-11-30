import boto3
from PIL import Image
import io
import json
import os
import ydb
import random
import string

def generate_random_string(length):
    characters = string.ascii_letters + string.digits
    random_string = ''.join(random.choices(characters, k=length))
    return random_string

def proccess_message_db(message, name, session):
  query = f'''
    DECLARE $name AS Utf8;
    DECLARE $original_photo_id AS Utf8;

    UPSERT INTO `{os.getenv('YDB_TABLE')}` (`name`, `original_photo_id`)
    VALUES (
        $name,
        $original_photo_id
    )
    '''

  params = {
    '$name': name,
    '$original_photo_id': message['source_image_id']
  }

  query = session.prepare(query)
  return session.transaction().execute(
    query,
    params,
    commit_tx=True,
    settings=ydb.BaseRequestSettings().with_timeout(3).with_operation_timeout(2)
  )

def proccess_message(message):
    session = boto3.session.Session()
    storage = session.client(service_name='s3')

    image = storage.get_object(
        Bucket=message['source_bucket_id'],
        Key=message['source_image_id']
    )['Body']

    image = Image.open(io.BytesIO(image.read()))

    rectangle = message['rectangle']
    left = rectangle[0]['x']
    top = rectangle[0]['y']
    right = rectangle[0]['x']
    bottom = rectangle[0]['y']

    for coordinate in rectangle:
        if coordinate['x'] > right:
            right = coordinate['x']
        if coordinate['x'] < left:
            left = coordinate['x']

        if coordinate['y'] > bottom:
            bottom = coordinate['y']
        if coordinate['y'] < top:
            top = coordinate['y']

    face_image = image.crop((left, top, right, bottom))

    face_image_bytes = io.BytesIO()
    face_image.save(face_image_bytes, format='JPEG')
    face_image_bytes.seek(0)
    key = generate_random_string(10) + '.jpg'

    storage.put_object(
        Bucket=os.environ['FACES_BUCKET_ID'],
        Body=face_image_bytes,
        Key=key
    )

    driver = ydb.Driver(
        endpoint=os.getenv('YDB_ENDPOINT'),
        database=os.getenv('YDB_DATABASE'),
        credentials=ydb.iam.MetadataUrlCredentials(),
    )

    driver.wait(fail_fast=True, timeout=5)

    pool = ydb.SessionPool(driver)

    pool.retry_operation_sync(lambda session: proccess_message_db(message, key, session))

def handler(event, context):
    for message in event['messages']:
        json_body = message['details']['message']['body']
        message = json.loads(json_body)
        proccess_message(message)
