//import vision from '@google-cloud/vision';
import vision from '@google-cloud/vision';
const client = new vision.ImageAnnotatorClient(
{
    //keyFilename: './google-creds.json' // Path to your service account key file
   keyFilename: './google-creds.json'
});
  
  export const analyzeImage = async (image: string) => {
    const [result] = await client.labelDetection(image);
    const labels = result.labelAnnotations;
    const ingredients = labels?.map(label => label.description) || [];
    return ingredients;
  };

