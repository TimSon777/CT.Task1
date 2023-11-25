import boto3
from PIL import Image
import io
import uuid
import json
import os


def proccess_message(message):
    session = boto3.session.Session()
    storage = session.client(service_name='s3')
    print(message)

    image = storage.get_object(
        Bucket=message['source_bucket_id'],
        Key=message['source_image_id']
    )['Body']

    print('image', image)
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

    print((left, top, right, bottom))
    face_image = image.crop((left, top, right, bottom))

    face_image_bytes = io.BytesIO()
    face_image.save(face_image_bytes, format='JPEG')
    face_image_bytes.seek(0)
    key = str(uuid.uuid4()) + '.jpg'

    storage.put_object(
        Bucket=os.environ['FACES_BUCKET_ID'],
        Body=face_image_bytes,
        Key=key
    )

def handler(event, context):
    for message in event['messages']:
        json_body = message['details']['message']['body']
        print(json_body)
        message = json.loads(json_body)
        proccess_message(message)
