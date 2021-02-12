# EasyStego

## Simple and easy steganography for C#

<p align="center">
<img src="https://github.com/gabrielareia/Logos/blob/main/EasyStegoLogo.png?sanitize=true" alt="EasyStego" title="Interesting image..." width="512"/>
</p>

<!--This is a temporary README, I am still writing it.-->

# WARNING

This tool was created for **fun** and **demonstration** purposes only. Please **DO NOT** use this tool to commit any crimes, neither to hide anything sensitive, nor to hide any private information, since it could easily be detected by anyone with some knowledge on the subject, and the hidden message (text or image) would be easily extracted from the public image. EasyStego **don't** use any advanced techiniques to hide the message inside the image, therefore use this tool with resposability and caution, for learning, exploration and fun or just to watermark your images.
<br/>
<br/>

### Download an executable and test it for yourself

I just published a version I was using for testing, it is a very basic and simple application but it's enough to show what EasyStego can do, without coding or cloning anything.

You can **download** it here: [EasyStego_App](https://github.com/gabrielareia/EasyStego_AppExample/releases/tag/v1.0)<br/>
Just download the "EasyStego_App.zip" file, extract it to a folder and open the "EasyStego_App.exe" file.

The source code to the application you can find it here: [Source](https://github.com/gabrielareia/EasyStego_AppExample)
<br/>

### What is EasyStego?

EasyStego is a library that allows you to create simple steganography. You can hide text or even another image inside an image. Please note that I don't intend for this to be a professional library, I created this library mainly for fun and exploration, I saw a video on the subject and decided to give it a try, so I didn't use any fancy techniques, yet.

### How does it work?

EasyStego currently uses one of the most simple ways of steganography, two least significant bits. EasyStego converts the public image (the image everyone will see) to an array of bits, and does the same with the message (text or image that will be hidden from the public), then it takes two bits of the message and replace the 2 first bits of the public image, then two more bits of the message and replace two bits of the next byte of the image, and so on.

In this [ComputerPhile video](https://www.youtube.com/watch?v=TWEXCYQKyDc) they explain better than I ever could, so if you don't know how this technique work and want to learn, this video is a good way to start.

### Things you should know

When I started the project I decided to leave the alpha channels untouched by the message, that way it would be less obvious there was a hidden message inside, because if anyone looked at the alpha channel of the image they would notice that it wasn't full opaque where it should be, but this leads to a bottleneck for the message size, you could only add a message that was way less than the image size. So I decided to allow for alpha channel as well, and let the user decide, since we are using only the two least significant bits of the colors of the image it is almost unnoticeable, but sometimes it is still noticeable. 

I found out that this works best with more complex images, and maybe even noisy, when I tried with an image of the sky that was composed of just one single blue color,and it was very smooth, I could clearly see where the message was hidden inside the image, it generates a noise pattern on the top of the image, so keep that in mind.

Below there is more information on the size of the image and the message it can carry.

### Some important notes

We can hide two types of message inside an image using EasyStego, a text or another image. They are interpreted in similar but different ways:

There was no way for decrypting the message if EasyStego didn't know the size of the message beforehand, so the message size needs to be encrypted along with the message.
I chose to use the first bytes of the image to hold the message size information. 

For the text message we just want to know the string size, and this can be represented as an integer.<br/>
For the image message we need the width and the height, so we need 2 integers for that.

Since an int has 32 bits (4 bytes) we need to reserve a place at the beginning of the image to save this information. Each byte of the public image can hold 2 bits of information, therefore 32 bits divided by 2 = 16 bytes of the original image, but that's only if we are using the alpha channel to hold bits as well, if not we need to skip the alpha channel for every pixel, and we would use 21 bytes of the public image to hold 4 bytes of message, but I decided to round it to 24 bytes even though we don't use this 3 last bytes, because this way we complete a full pixel (each pixel has 4 bytes, 21 bytes = 5 pixels + 1 red channel. 24 bytes = 6 whole pixels).
The same thing happens for the image message but with 2 ints, then we need 48 bytes of the public image to hold the information.

Now with all this in mind, let's have a look at the sizes of the images and the message it can carry:

If an public image is 1024px by 1024px, this image has 1.048.576 pixels total, each pixel has 4 bytes, and if we were to hide information in all the bytes (without skipping the alpha) we could hold 262.144 pixels, 25% of the public image, this would be the same as a 512px by 512px. But if we skip the alpha channel we could only carry 196.608 pixels, approximately 18% of the image, and this image would be 443px by 443px.

If we are hiding a text however, each character in the text is 1 byte long, so if we don't skip the alpha we can carry as much characters as there are pixels on the image (minus the first 24 bytes for the size of the image). In the first example, 1024px * 1024px we could carry a text with more or less 1.048.576 characters. But if we skip the alpha we can only carry 75% of that, 786.432 characters in total.

The formula I use for this is: 

**For image message,<br/>without skipping the alpha channel:** | **For image message,<br/>skipping the alpha channel:**
-|-
nPixelsPublicImage / 4 = maxPixelsAllowedForMessage | ((nPixelsPublicImage * 3) / 4) / 4 = maxPixelsAllowedForMessage

**For text message,<br/>without skipping the alpha channel:** | **For text message,<br/>skipping the alpha channel:**
-|-
nPixelsPublicImage = maxChractersAllowedForMessage | (nPixelsPublicImage * 3) / 4 = maxChractersAllowedForMessage

<br/>
Below there is a table with some values for more clarification, if you didn't get it yet try to do some calculations on your own and I think you'll understand where these numbers came from:

### For images as messages:

Public image size | Pixels | Bytes | Bits |  Max message pixel size using alpha | Max message pixel size skipping alpha
-----------|-------------|-|--------------|-|-
1024px * 1024px | 1.048.576 | 4.194.304 | 33.554.432 | ~262.144 (512px * 512px) | ~196.608 (443px * 443px)
2048px * 2048px | 4.194.304 | 16.777.216 | 134.217.728 | ~1.048.576 (1024px * 1024px) | ~786.432 (886px * 886px)

### For text as messages:

Public image size | Pixels | Bytes | Bits |  Max message char size using alpha | Max message char size skipping alpha
-----------|-------------|-|--------------|-|-
1024px * 1024px | 1.048.576 | 4.194.304 | 33.554.432 | ~1.048.576 (1mb) | ~786.432 (768kb)
2048px * 2048px | 4.194.304 | 16.777.216 | 134.217.728 | ~4.194.304 (4mb) | ~3.145.728 (3mb)


I think that sums it up.

Feel free to mess with the code to improve the tool if you want to, and send a pull request.

I did a very simple .exe that implements this library, I'll upload it as well so you can use the library for yourself and test it very easily.

Hope you have fun using it to share with friends and maybe even learn something new. Enjoy!

