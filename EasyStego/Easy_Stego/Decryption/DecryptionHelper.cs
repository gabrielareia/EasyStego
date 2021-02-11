using Easy_Stego.Stego;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace Stego.Decryption
{
    public static class DecryptionHelper
    {
        /// <summary>
        /// Gets the message that is hidden inside the image.
        /// </summary>
        /// <param name="imageBits">The image bits which are holding the message.</param>
        /// <param name="messageBits">The bit array to hold the decrypted message.</param>
        /// <param name="sizeInfo">How many bits are going to be skipped until the message starts.</param>
        /// <param name="skipAlpha">Should the alpha channel remain untouched?.</param>
        /// <returns>An array of bytes that cointains the message to be converted.</returns>
        public static byte[] GetDecryptedMessage(BitArray imageBits, BitArray messageBits, SizeInfo sizeInfo, bool skipAlpha)
        {
            //Start decrypting after the first 24 bytes, before that it's only the size of the message, not the message itself.
            int offset = (int)sizeInfo;

            //For every byte that contains a message:

            //Parallel.For(0, messageBits.Count / 2, (i) => // 
            for (int i = 0; i < messageBits.Count / 2; i++)
            {
                int indexImage = i * 8 + offset;
                int indexMessage = i * 2;

                if (skipAlpha)
                {
                    //Skip alpha bytes
                    if ((indexImage + 8) % 32 == 0)
                    {
                        offset += 8;
                        i--;
                        continue;
                    }
                }

                //Assign each message bit to each corresponding bit on the message array.
                messageBits[indexMessage] = imageBits[indexImage + 0];
                messageBits[indexMessage + 1] = imageBits[indexImage + 1];
            }

            byte[] messageDecrypted = new byte[messageBits.Count / 8];

            //Transform from bits to bytes
            messageBits.CopyTo(messageDecrypted, 0);

            return messageDecrypted;
        }

        /// <summary>
        /// Gets the size of the message that is hidden inside the image.
        /// </summary>
        /// <param name="image">The image bits which are holding the message.</param>
        /// <param name="offset">How many bits should be skipped.</param>
        /// <param name="skipAlpha">Should the alpha channel remain untouched?.</param>
        /// <returns>An int with the size of the message.</returns>
        public static int GetMessageSize(BitArray image, int offset, bool skipAlpha)
        {
            //I decided that it shouldn't take more than one int size to tell the message size. (4 bytes = 32 bits)
            //This would be: 2,147,483,647 of bits in size. (Around 256MB message, your image would have to be even bigger)
            //But is very easy to change this if you need to.
            BitArray size = new BitArray(32);

            int _offset = offset;

            //For every bit in the size array, 2 at a time:
            for (int i = 0; i < size.Count / 2; i++)
            {
                int currentByte = i * 8 + _offset;
                int sizeIndex = i * 2;

                if (skipAlpha)
                {
                    //Skip alpha bytes
                    if ((currentByte + 8) % 32 == 0)
                    {
                        _offset += 8;
                        i--;
                        continue;
                    }
                }

                //Get the bits on the image that hold the size information.
                size[sizeIndex] = image[currentByte];
                size[sizeIndex + 1] = image[currentByte + 1];
            }

            byte[] sizeByte = new byte[size.Count / 8];

            size.CopyTo(sizeByte, 0);

            //Convert the byte array to an int again
            int messageSize = BitConverter.ToInt32(sizeByte, 0);

            return messageSize;
        }

    }
}
