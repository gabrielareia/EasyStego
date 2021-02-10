using Stego.Decryption;
using System;
using System.Collections;

namespace Easy_Stego.Stego
{
    /// <summary>
    /// What we are trying to retrieve from the image.
    /// </summary>
    public enum DecryptionType
    {
        Text,
        Image
    }

    public static class Decrypt
    {
        /// <summary>
        /// Decrypts the message hidden in the image.
        /// </summary>
        /// <param name="type">The type of the decryption.</param>
        /// <param name="image">The bytes of the image where the message is hidden.</param>
        /// <param name="skipAlpha">Should the alpha channel remain untouched?.</param>
        /// <returns>An array of bytes with the message that was hidden.</returns>
        public static byte[] DecryptMessage(DecryptionType type, byte[] image, bool skipAlpha)
        {
            byte[] result;
            switch (type)
            {
                case DecryptionType.Text:
                    result = DecryptStringMessage(image, skipAlpha);
                    break;
                case DecryptionType.Image:
                    result = DecryptImageMessage(image, skipAlpha);
                    break;
                default:
                    throw new Exception("There was no encryption type, and the array was empty.");
            }
            return result;
        }

        /// <summary>
        /// Decrypts an image hidden inside the image.
        /// </summary>
        /// <param name="image">The bytes of the image where the message is hidden.</param>
        /// <param name="skipAlpha">Should the alpha channel remain untouched?.</param>
        /// <returns>An array of bytes with the image that was hidden.</returns>
        private static byte[] DecryptImageMessage(byte[] image, bool skipAlpha)
        {
            //Transform bytes to bits.
            BitArray imageBits = new BitArray(image);

            //Get the size of the message we'll have to decrypt, at the beginning of the image.
            Steganography.embeddedWidth = DecryptionHelper.GetMessageSize(imageBits, 0, skipAlpha);
            Steganography.embeddedHeight = DecryptionHelper.GetMessageSize(imageBits, 24 * 8, skipAlpha);

            //Empty array of bits the size of the message size we just got.
            BitArray messageBits = new BitArray(Steganography.embeddedWidth * Steganography.embeddedHeight * 32);

            //Decrypts the message.

            byte[] messageDecrypted = DecryptionHelper.GetDecryptedMessage(imageBits, messageBits, 
                SizeInfo.Image, skipAlpha);

            return messageDecrypted;
        }

        /// <summary>
        /// Decrypts an text hidden inside the image.
        /// </summary>
        /// <param name="image">The bytes of the image where the message is hidden.</param>
        /// <param name="skipAlpha">Should the alpha channel remain untouched?.</param>
        /// <returns>An array of bytes with the text that was hidden.</returns>
        private static byte[] DecryptStringMessage(byte[] image, bool skipAlpha)
        {
            //Transfome bytes to bits.
            BitArray imageBits = new BitArray(image);

            //Get the size of the message we'll have to decrypt, at the beginning of the image.
            int messageSize = DecryptionHelper.GetMessageSize(imageBits, 0, skipAlpha);

            //Empty array of bits the size of the message size we just got.
            BitArray messageBits = new BitArray(messageSize);

            //Decrypts the message.

            byte[] messageDecrypted = DecryptionHelper.GetDecryptedMessage(imageBits, messageBits, 
                SizeInfo.Text, skipAlpha);

            return messageDecrypted;
        }
    }
}
