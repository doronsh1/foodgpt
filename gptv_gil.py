from openai import OpenAI
import base64
import re


def extract_prices(gpt_response):

    # Regular expression to find prices
    price_pattern = r'\$\d+\.?\d*'

    # Split the text into sentences
    sentences = gpt_response.split('. ')

    # Initialize a dictionary to store the prices
    prices = {'bbb_price': None, 'competitor_price': None}

    for sentence in sentences:
        # Find all prices in the current sentence
        found_prices = re.findall(price_pattern, sentence)

        # Skip sentences with no prices
        if not found_prices:
            continue

        # Check which retailer the prices belong to
        if 'Bed Bath & Beyond' in sentence:
            prices['bbb_price'] = sorted(found_prices)
        elif any(retailer in sentence for retailer in ['Amazon', 'Walmart', 'Wayfair', 'Target']):
            prices['competitor_price'] = sorted(found_prices)

    return prices


def image_to_base64(image_path):
    with open(image_path, "rb") as image_file:
        return base64.b64encode(image_file.read()).decode('utf-8')


def gpt_compare_screenshots(source_img_url,
                       comp_img_url,
                       prompt="""Answer the following questions per image:
                              1. What is the retailer's name? Answer in the format of: "retailer: <your answer>"
                              2. What is the title of the product? Answer in the format of: "title: <your answer>"
                              3. What is the color of the product? Answer in the format of: "color: <your answer>"
                              4. What is the size (or dimensions) of the product? Answer in the format of: "size: <your answer>"
                              5. What is the brand of the product? Answer in the format of: "brand: <your answer>"
                              6. What is the price of the product? Answer in the format of: "current price: <your answer>, previous price: <your answer> if there was any price change. Pay close attention to the digits of the prices - don't mix them up with the $ sign."
                              
                              If you can't find an answer to any of the questions, please write: "NaN".
                              
                              Construct your answer as follows:
                              First, answer the following question: 
                              Which of the products in the images is cheaper and why? When answering this question, please provide the answer in two sentences such that the first sentence describes the first image and the second sentence describes the second image. Also include the name of the retailer that each image is taken from.
                              
                              And then:
                              First image:
                              1. "retailer: <your answer>"
                              2. "title: <your answer>"
                              3. "color: <your answer>"
                              4. "size: <your answer>"
                              5. "brand: <your answer>"
                              6. "price: <your answer>"
                              
                              Second image:
                              1. "retailer: <your answer>"
                              2. "title: <your answer>"
                              3. "color: <your answer>"
                              4. "size: <your answer>"
                              5. "brand: <your answer>"
                              6. "price: <your answer>"
                              
                              If you can't find an answer to any of the questions, please write: "NaN".

                              Based on the knowledge that you got for each image, are these two products completely identical?
                              If not, in what do they differ?
                       """):
    # Initialize the OpenAI client with your API key
    client = OpenAI(api_key="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")

    source_img = image_to_base64(image_path=source_img_url)
    comp_img = image_to_base64(image_path=comp_img_url)

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
                # "url": source_img_url
                "url": f"data:image/jpeg;base64,{source_img}"
              },
            },
            {
              "type": "image_url",
              "image_url": {
                # "url": comp_img_url
                "url": f"data:image/jpeg;base64,{comp_img}"
              },
            },
          ],
        }
      ],
      max_tokens=500
    )

    return response.choices[0].message.content


def gpt_compare_images(source_img_url,
                       comp_img_url,
                       prompt="Are the provided images identical? If not, please explain in what they differ."):

    # Initialize the OpenAI client with your API key
    client = OpenAI(api_key="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")

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
                    {
                        "type": "image_url",
                        "image_url": {
                            "url": comp_img_url
                        },
                    },
                ],
            }
        ],
        max_tokens=500
    )

    return response.choices[0].message.content


def analyze_products(source_pdp_image_path, candidate_pdp_image_path, source_image_url, candidate_image_url):
    images_content = gpt_compare_screenshots(source_pdp_image_path,
                                 candidate_pdp_image_path)
    print(images_content)

    prices = extract_prices(images_content)

    src_price = prices['bbb_price']
    cand_price = prices['competitor_price']

    if src_price:
        if len(src_price) > 1:
            print(f"The source product price was: {src_price[1]} and now it's: {src_price[0]}")
        else:
            print(f"The source product price is: {src_price[0]}")

    if cand_price:
        if len(cand_price) > 1:
            print(f"The candidate product price was: {cand_price[1]} and now it's: {cand_price[0]}")
        else:
            print(f"The candidate product price is: {cand_price[0]}")

    images_similarity = gpt_compare_images(source_image_url, candidate_image_url)
    print("\n")
    print(images_similarity)

    return


if __name__ == '__main__':

      analyze_products(source_pdp_image_path="/Users/gsheinbaumnw/Desktop/Screenshot 2024-03-13 at 16.20.02.png",
                       candidate_pdp_image_path="/Users/gsheinbaumnw/Desktop/Screenshot 2024-03-13 at 13.10.59.png",
                       source_image_url="https://m.media-amazon.com/images/I/81XcpJPb0ML._AC_SL1500_.jpg",
                       candidate_image_url="https://ak1.ostkcdn.com/images/products/is/images/direct/23334e1d4434defbdb7d63ace3bed5d7ab95a12a/Madison-Park-Mansfield-6-Piece-Reversible-Daybed-Cover-Set.jpg")
