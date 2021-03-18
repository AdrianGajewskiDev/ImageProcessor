# Image Processor

*A wpf application that converts colors in image based on the highest value from pixel color (RGB):*

## Formula:
  * highest number is R, then new color is #FF0000
  * highest number is G, then new color is #00FF00
  * highest number is B, then new color is #0000FF
    
## Preview
![Paste Image](https://github.com/AdrianGajewskiDev/ImageProcessor/blob/main/assets/preview1.PNG)

## Usage:
1. Select image from disk
2. Click RunSync to run synchronous version or click RunAsync to run asynchronous version
3. Once the converting is done you will be asked to select where you want to save the newly created image
4. Then program will display the converted image as well as the time needed to complete the conversion


Select Image           |  Result
:-------------------------:|:-------------------------:
![](https://github.com/AdrianGajewskiDev/ImageProcessor/blob/main/assets/preview2.PNG)|![](https://github.com/AdrianGajewskiDev/ImageProcessor/blob/main/assets/preview2c.PNG)
