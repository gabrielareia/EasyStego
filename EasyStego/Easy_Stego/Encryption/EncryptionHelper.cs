using Easy_Stego.Stego;
using System;
using System.Collections;

namespace Stego.Encryption
{
    public static class EncryptionHelper
    {
        /// <summary>
        /// Checks if it is possible to hide the information inside the image.
        /// </summary>
        /// <param name="imageBitSize">The size of the image that everyone will see, in bits.</param>
        /// <param name="messageBitSize">The size of the message that will be hidden, in bits.</param>
        /// <param name="sizeInfo">How much of the image will take to hold the message size information.</param>
        public static void CheckMinimumSize(int imageBitSize, int messageBitSize, SizeInfo sizeInfo)
        {
            // int minimumImageSize = ((imageBitSize / 4) * 3) - (int)sizeInfo;
            int minimumImageSize = imageBitSize - (int)sizeInfo;

            if (minimumImageSize <= 0)
            {
                //The image isn't big enough to hold the size information.
                throw new Exception("The image is too small to hold the message size for decryption.");
            }

            if (messageBitSize / 2 > minimumImageSize / 8)
            {
                //The message is bigger than the image size.
                throw new Exception("The message is bigger than the image.");
            }
        }

        /// <summary>
        /// Transforms an integer in a bit array.
        /// </summary>
        /// <param name="number">The number you want to transform.</param>
        /// <returns>An array of bits that contains the bits that form the given integer</returns>
        public static BitArray IntToBitArray(int number)
        {
            //Transform the int into a byte array
            byte[] numberInBytes = BitConverter.GetBytes(number);
            //transform the byte array into a bit array
            BitArray numberInBits = new BitArray(numberInBytes);

            return numberInBits;
        }

        /// <summary>
        /// Sets the message size inside the image, so it can later be used for decryption.
        /// </summary>
        /// <param name="imageBits">The image bits where the message will be hidden.</param>
        /// <param name="sizeBits">The message bits to hide.</param>
        /// <param name="offsetInBits">How many bits will be skipped to add new information without overriding 
        /// what is already there.</param>
        /// <param name="skipAlpha">Should the alpha channel remain untouched?.</param>
        public static void SetSizeBits(BitArray imageBits, BitArray sizeBits, int offsetInBits, bool skipAlpha)
        {
            int offsetSize = offsetInBits;

            //The size is represented as an int.
            //An int has 32 bits (4 bytes), each byte in the image takes 2 bits of the message,
            //so we will need 16 bytes to hold the 4 bytes that holds the message size, minus the alpha channels. 
            //(21 bits total, and later it's rounded to 24 to complete a whole pixel)
            for (int i = 0; i < 16; i++)
            {
                //When we hit an alpha channel we will skip to the next byte by increasing 8 bits each time using offset.
                int indexImage = i * 8 + offsetSize;
                int indexSize = i * 2;

                if (skipAlpha)
                {
                    //Skip alpha bytes
                    if ((indexImage + 8) % 32 == 0)
                    {
                        //Skip byte
                        offsetSize += 8;
                        //Repeat the operation so it wont skip any bit of the size array.
                        i--;
                        continue;
                    }
                }

                //C# changes the order of the bits in the array. 
                //We want to change only the least 2 significant bits in the byte, 
                //but instead of changing the 6th and 7th bit we'll change the first 2 bits.
                imageBits[indexImage] = sizeBits[indexSize];

                imageBits[indexImage + 1] = sizeBits[indexSize + 1];
            }
        }

        /// <summary>
        /// Sets the message inside the image.
        /// </summary>
        /// <param name="imageBits">The image bits where the message will be hidden.</param>
        /// <param name="messageBits">The message bits to hide.</param>
        /// <param name="sizeInfo">How many bits hold the message size information and will be skipped.</param>
        /// <param name="skipAlpha">Should the alpha channel remain untouched?.</param>
        /// <returns>An array of bytes that contains the image with the message inside</returns>
        public static byte[] SetMessage(BitArray imageBits, BitArray messageBits, SizeInfo sizeInfo, bool skipAlpha)
        {
            int offset = 0;

            //For every byte in the image:
            for (int i = 0; i < imageBits.Count / 8; i++)
            {
                //Start to load message after the size information
                int indexImage = i * 8 + (int)sizeInfo;

                //If we hit the alpha channel we will decrease the indexMessage by 2 each time,
                //so we won't loose any bit of the message.
                int indexMessage = i * 2 - offset;

                if (indexMessage >= messageBits.Count)
                {
                    //If the message is done we can proceed.
                    break;
                }

                if (skipAlpha)
                {
                    //Skip alpha bytes
                    if ((indexImage + 8) % 32 == 0)
                    {
                        offset += 2;
                        continue;
                    }
                }

                //If the bit of the image is the same as the bit of the message, do nothing.
                if (imageBits[indexImage] != messageBits[indexMessage])
                {
                    imageBits[indexImage] = messageBits[indexMessage];
                }

                if (imageBits[indexImage + 1] != messageBits[indexMessage + 1])
                {
                    imageBits[indexImage + 1] = messageBits[indexMessage + 1];
                }
            }

            byte[] imageEncrypted = new byte[imageBits.Count / 8];

            //Change back to an byte array.
            imageBits.CopyTo(imageEncrypted, 0);

            return imageEncrypted;
        }
    }
}
