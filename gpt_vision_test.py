from openai import OpenAI
import base64
import re


def image_to_base64(image_path):
    with open(image_path, "rb") as image_file:
        return base64.b64encode(image_file.read()).decode('utf-8')


def gpt_analyze_image(source_img_url,
                      prompt="Please analyze the uploaded image and determine whether it contains any type of food or beverage. \
                        1. If the image does not contain any food or beverage, respond with: ‘The image does not contain any food or beverage and is not viable for analysis.’ \
                        2. If the image contains food or beverage, list each identifiable item in the image and provide the estimated amount of calories, fiber, protein and fat for each item. \
                    Format your response as the following example: \
                        • Apple: xyz calories, xxx fiber, yyy protein, zzz fats \
                        • Plain Omelette: abc calories, aaa fiber, bbb protein, ccc fats \
                        • Orange Juice: lmn calories, lll fiber, mmm protein, nnn fats \
                        Total amount of calories: ... \
                        Total amount of fiber: ... \
                        Total amount of protein: ... \
                        Total  amount of fats: ..."
                    ):
    # Initialize the OpenAI client with your API key
    # Load and encode the image

    enc_im = image_to_base64(source_img_url)

    client = OpenAI(api_key="sk-proj-VEIoyafXNeKNgh2v4CfTT3BlbkFJ7pJQ9gtLPfZZid2s5gpR")
                    # organization="org-ApWwCbdvTUruFjmOZLm8vkJ9",
                    # project="proj_f59VIHf6wqpOzKZ926VCb0Ec")

    response = client.chat.completions.create(
        model="gpt-4o-2024-08-06",
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
                            "url": f"data:image/jpeg;base64,{enc_im}"
                        }
                        # "detail": "high"
                    },
                ],
            }
        ],
        response_format={
            "type": "json_schema",
            "json_schema": {
                "name": "calories_count",
                "strict": True,
                "schema": {
                    "type": "object",
                    "properties": {
                        "steps": {
                            "type": "array",
                            "items": {
                                "type": "object",
                                "properties": {
                                    "explanation": {
                                        "type": "string"
                                    },
                                    "output": {
                                        "type": "string"
                                    }
                                },
                                "required": ["explanation", "output"],
                                "additionalProperties": False
                            }
                        },
                        "final_answer": {
                            "type": "object",
                            "properties": {
                                "calories": {
                                    "type": "number"
                                },
                                "fiber": {
                                    "type": "object",
                                    "properties": {
                                        "value":{
                                            "type": "number"
                                        },
                                        "units":{
                                            "type": "string"
                                        }
                                    },
                                    "required": ["value", "units"],
                                    "additionalProperties": False
                                },
                                "protein": {
                                    "type": "object",
                                    "properties": {
                                        "value":{
                                            "type": "number"
                                        },
                                        "units":{
                                            "type": "string"
                                        }
                                    },
                                    "required": ["value", "units"],
                                    "additionalProperties": False
                                },
                                "fat": {
                                    "type": "object",
                                    "properties": {
                                        "value":{
                                            "type": "number"
                                        },
                                        "units":{
                                            "type": "string"
                                        }
                                    },
                                    "required": ["value", "units"],
                                    "additionalProperties": False
                                },
                            },
                            "required": ["calories", "fiber", "protein", "fat"],
                            "additionalProperties": False
                        }
                    },
                    "required": ["steps", "final_answer"],
                    "additionalProperties": False
                }
            }
        },
        max_tokens=1000
    )

    return response.choices[0].message.content


if __name__ == '__main__':
    im_path = "/Users/gilsheinbaum/Desktop/Screenshot 2024-07-29.png"
    # test_prompt = """
    #                 Analyze the image and determine whether it contains any type of food or beverage.
    #                     1. If the image does not contain any food or beverage, respond with: ‘The image does not
    #                     contain any food or beverage and is not viable for analysis.’
    #                     2. If the image contains food or beverage, list each identifiable item in the image and provide
    #                     the estimated amount of calories, fiber, protein and fats for each item. If an item repeats itself, count the amount of repetitions and provide the breakdown accordingly.
    #
    #                 Format your response as the following example:
    #                     • Apple: xyz calories, xxx fiber, yyy protein, zzz fats
    #                     • Plain Omelette: abc calories, aaa fiber, bbb protein, ccc fats
    #                     • Orange Juice: lmn calories, lll fiber, mmm protein, nnn fats
    #
    #                     Total amount of calories: ...
    #                 """

    res = gpt_analyze_image(source_img_url=im_path)
    print(res)
