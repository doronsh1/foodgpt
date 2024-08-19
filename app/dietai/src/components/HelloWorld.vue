
 
<template>
  <h1>{{ msg }}</h1>

  <div class="card">
    <!-- <button type="button" @click="count++">count is {{ count }}</button> -->
    <button @click="takePhoto">Take a Photo</button>
    <div v-if="photo">
      <img :src="photo" alt="Photo" />
      <button @click="uploadPhoto">Upload Photo</button>
    </div>
    <div v-if="response">
      <h2>Response</h2>
      <p>{{ response }}</p>
    </div>
  </div>

  
  
</template>

<script setup lang="ts">
import { ref } from 'vue';
// import axios from 'axios';
import { Camera, CameraResultType, CameraSource } from '@capacitor/camera';
import OpenAI from 'openai';
// import { analyzeImage } from '../services/vision.ts';

defineProps<{ msg: string }>()

const api_key: string  ="sk-proj-VEIoyafXNeKNgh2v4CfTT3BlbkFJ7pJQ9gtLPfZZid2s5gpR";

const response = ref<string | null>(null);

// const count = ref(0)
const photo = ref<string | null>(null);

const takePhoto = async () => {
  try {
    const image = await Camera.getPhoto({
      resultType: CameraResultType.DataUrl,
      source: CameraSource.Camera,
      quality: 90
    });
    photo.value = image.dataUrl || null;
    console.log('Photo taken:', image);
  } catch (error) {
    console.error('Error taking photo:', error);
  }
}; 

const uploadPhoto = async () => {
  if (!photo.value) return;
  console.log("we have photo!")
  console.log(photo.value)
   const prompt =  `How many calories are are in this meal?`;
  
  try {
    const openai = new OpenAI({
      organization: "org-ApWwCbdvTUruFjmOZLm8vkJ9",
      project: "proj_f59VIHf6wqpOzKZ926VCb0Ec",
      apiKey: api_key,
      dangerouslyAllowBrowser: true
    });

    console.log("created client!")
    const result = await openai.chat.completions.create({
      model: 'gpt-4o',
      messages: [
        { role: 'system', content: 'You are a helpful assistant.' },
        { role: 'user', 
          content: [
          { 
            type: "text",
            text:  prompt    
          },
          {
            type: "image_url",
            image_url: {url: `${photo.value}`}
          }          
         ]  
         },
      ],
    });
    response.value = result.choices[0].message.content;
    console.log('Response from API:', result);
  } catch (error) {
    console.error('Error uploading photo:', error);
  }
};
</script>


<style scoped>
.read-the-docs {
  color: #888;
}
#home {
  text-align: center;
}
img {
  max-width: 100%;
  height: auto;
  margin-top: 20px;
}

</style>

<!-- 00008110-001A31E811F1801E -->