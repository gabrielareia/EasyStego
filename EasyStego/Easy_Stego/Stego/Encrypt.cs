using Stego.Encryption;
using System;
using System.Collections;

namespace Easy_Stego.Stego
{
    /// <summary>
    /// What is going to be hidden inside an image.
    /// </summary>
    public enum EncryptionType
    {
        Text,
        Image
    }

    public static class Encrypt
    {
        /// <summary>
        /// Encrypts the given message into the given image.
        /// </summary>
        /// <param name="type">The type of message you want to encrypt.</param>
        /// <param name="image">The image you are going to hide your message in.</param>
        /// <param name="message">The message (string or image) you are going to hide in the image.</param>
        /// <param name="skipAlpha">Should the alpha channel remain untouched?.</param>
        /// <returns>An array of bytes that contains the image with the message inside</returns>
        public static byte[] EncryptMessage(EncryptionType type, byte[] image, byte[] message, bool skipAlpha)
        {
            byte[] result;
            switch (type)
            {
                case EncryptionType.Text:
                    result = EncryptStringMessage(image, message, skipAlpha);
                    break;
                case EncryptionType.Image:
                    result = EncryptImageMessage(image, message, skipAlpha);
                    break;
                default:
                    throw new Exception("There was no encryption type, and the array was empty.");
            }
            return result;
        }

        /// <summary>
        /// Encrypts an image inside other image.
        /// </summary>
        /// <param name="image">The image everyone will see.</param>
        /// <param name="message">The image that will be hidden.</param>
        /// <param name="skipAlpha">Should the alpha channel remain untouched?.</param>
        /// <returns>An array of bytes that contains the image with another image hidden inside</returns>
        private static byte[] EncryptImageMessage(byte[] image, byte[] message, bool skipAlpha)
        {
            //Convert the byte arrays to bit arrays.
            BitArray imageBits = new BitArray(image);
            BitArray messageBits = new BitArray(message);

            //The image needs at least 48 bytes to hold the embedded image width and height information.
            //The message will begin at the 48th byte of the image (384 bits).
            EncryptionHelper.CheckMinimumSize(imageBits.Count, messageBits.Count, SizeInfo.Image, skipAlpha);


            //First, inside the first 48 bytes, set the size of the image we are going to hide, 
            //this will be used later when decrypting:

            //Transform the size into a bit array
            BitArray widthInBits = EncryptionHelper.IntToBitArray(Steganography.embeddedWidth);
            BitArray heightInBits = EncryptionHelper.IntToBitArray(Steganography.embeddedHeight);

            //DEFINE WIDTH          

            EncryptionHelper.SetSizeBits(imageBits, widthInBits, 0, skipAlpha);

            //DEFINE HEIGHT             

            //We skip the first 24 bytes (actually 21 but I rounded it to be a complete pixel)
            //that contains the width information.
            EncryptionHelper.SetSizeBits(imageBits, heightInBits, 24 * 8, skipAlpha);


            //DEFINE MESSAGE

            byte[] imageEncrypted = EncryptionHelper.SetMessage(imageBits, messageBits, SizeInfo.Image, skipAlpha);

            return imageEncrypted;
        }

        /// <summary>
        /// Encrypts a text message inside an image.
        /// </summary>
        /// <param name="image">The image everyone will see.</param>
        /// <param name="message">The text that will be hidden.</param>
        /// <param name="skipAlpha">Should the alpha channel remain untouched?.</param>
        /// <returns>An array of bytes that contains the image with a text hidden inside.</returns>
        private static byte[] EncryptStringMessage(byte[] image, byte[] message, bool skipAlpha)
        {
            //Convert the byte arrays to bit arrays.
            BitArray imageBits = new BitArray(image);
            BitArray messageBits = new BitArray(message);

            //The image needs at least 24 bytes to hold the message size information.
            //The message will begin at the 24th byte of the image (192 bits).

            EncryptionHelper.CheckMinimumSize(imageBits.Count, messageBits.Count, SizeInfo.Text, skipAlpha);

            //In the first 24 bytes set the size of the message we are going to hide, 
            //this will be used later when decrypting:

            //Transform the message size into a bit array
            BitArray sizeInBits = EncryptionHelper.IntToBitArray(messageBits.Count);

            //DEFINE SIZE

            EncryptionHelper.SetSizeBits(imageBits, sizeInBits, 0, skipAlpha);

            //DEFINE MESSAGE

            byte[] imageEncrypted = EncryptionHelper.SetMessage(imageBits, messageBits, SizeInfo.Text, skipAlpha);

            return imageEncrypted;
        }
    }
}
