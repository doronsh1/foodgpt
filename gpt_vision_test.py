from openai import OpenAI
import base64
import re


def image_to_base64(image_path):
    with open(image_path, "rb") as image_file:
        return base64.b64encode(image_file.read()).decode('utf-8')


def gpt_compare_images(source_img_url,
                       prompt="OUR PROMPT"):
    # Initialize the OpenAI client with your API key
    client = OpenAI(api_key="")

    response = client.chat.completions.create(
        model="gpt-4-vision-preview",
        messages=[
            {
                "role": "user",
                "content": [
                    {
                        "type": "text",
                        "text": prompt,
                    },
                    {
                        "type": "image_url",
                        "image_url": {
                            "url": source_img_url
                        },
                    },
                ],
            }
        ],
        max_tokens=500
    )

    return response.choices[0].message.content


if __name__ == '__main__':
    pass
