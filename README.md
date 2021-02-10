# EasyStego

### Simple and easy steganography for C#

<p align="center">
<img src="https://github.com/gabrielareia/Logos/blob/main/EasyStegoLogo.png?sanitize=true" alt="EasyStego" title="Interesting image..." width="512"/>
</p>

This is a temporary README, I am still writing it.

<!-- Due to the way this tool is currently constructed, an image can hold a message that is
in average 18% the size of the image holding it. So for example if you have a message that is 450kb in size
you will need an image of more or less 2.5mb
Feel free to mess with the code to improve this if you want to, I decided that only the 2 least important bits
of the bytes would carry a bit of the message each, and the alpha channel wouldn't carry any message because it could
be easily detected by looking into the alpha channel values of the image.


<!-- NOTE: I decided to use the first 24 or 48 bytes of the image to hold some message information,
like the size of the string message or the width and height of the image message. This will be used later when
decrypting.
For the string message size I decided to use only 24 bytes to hold the information, 
which is enough to hold one int.MaxValue.
For the image message I will be using 48 bytes, to hold the width and height.
