using Easy_Stego.Stego;
using System.Text;

namespace Easy_Stego
{
    public static class EasyStego
    {
        //PLEASE READ THE WARNING STATEMENT BELOW!

        #region WARNING!
        //WARNING

        //THIS TOOL IS JUST FOR FUN AND DEMONSTRATION PURPOSES ONLY. PLEASE **DO NOT** USE THIS TOOL TO COMMIT ANY CRIMES,
        //TO HIDE ANYTHING SENSITIVE, OR TO HIDE ANY PRIVATE INFORMATION, SINCE IT COULD EASILY BE DETECTED
        //AND THE HIDDEN MESSAGE EASILY EXTRACTED FROM THE IMAGE, EASYSTEGO DON'T USE
        //ANY ADVANCED TECHNIQUES TO HIDE THE MESSAGE IN THE IMAGE, JUST THE SIMPLEST FORM OF STEGANOGRAPHY THERE IS,
        //THEREFORE USE THIS TOOL WITH CAUTION AND ONLY FOR EXPLORATION AND FUN OR TO WATERMARK YOUR IMAGES.
        #endregion


        /// <summary>
        /// Encrypts an image inside of another image and saves it to the given path.
        /// </summary>
        /// <param name="publicImagePath">The path to the image everyone will see.</param>
        /// <param name="hiddenImagePath">The path to the image that will be hidden.</param>
        /// <param name="outputPath">The path to the final encrypted image.</param>
        /// <param name="skipAlpha">Should the alpha channel remain untouched?.</param>
        public static void EncryptImageWithImage(string publicImagePath, string hiddenImagePath, 
            string outputPath, bool skipAlpha = false)
        {
            byte[] publicImage = Steganography.GetImage(publicImagePath, ImageType.Original);
            byte[] messageImage = Steganography.GetImage(hiddenImagePath, ImageType.Message);
            byte[] encryptedImage = Encrypt.EncryptMessage(EncryptionType.Image, publicImage, 
                messageImage, skipAlpha);

            Steganography.SaveImage(encryptedImage, outputPath, Steganography.originalWidth, 
                Steganography.originalHeight);
        }

        /// <summary>
        /// Encrypts a text inside of an image and saves it to the given path.
        /// </summary>
        /// <param name="publicImagePath">The path to the image everyone will see.</param>
        /// <param name="hiddenTextPath">The path to the text that will be hidden.</param>
        /// <param name="outputPath">The path to the final encrypted image.</param>
        /// <param name="skipAlpha">Should the alpha channel remain untouched?.</param>
        public static void EncryptImageWithText(string publicImagePath, string hiddenTextPath, 
            string outputPath, bool skipAlpha = false)
        {
            byte[] publicImage = Steganography.GetImage(publicImagePath, ImageType.Original);
            byte[] messageString = Steganography.GetText(hiddenTextPath);
            byte[] encryptedImage = Encrypt.EncryptMessage(EncryptionType.Text, publicImage,
                messageString, skipAlpha);

            Steganography.SaveImage(encryptedImage, outputPath, Steganography.originalWidth, 
                Steganography.originalHeight);
        }

        /// <summary>
        /// Decrypts the image that is inside another image and saves it to the given path.
        /// </summary>
        /// <param name="imagePath">The path to the image containing the message.</param>
        /// <param name="outputPath">The path to the final decrypted image.</param>
        /// <param name="skipAlpha">Should the alpha channel remain untouched?.</param>
        public static void DecryptImageWithImage(string imagePath, string outputPath, bool skipAlpha = false)
        {
            byte[] imageToDecrypt = Steganography.GetImage(imagePath, ImageType.Original);
            byte[] decryptedImage = Decrypt.DecryptMessage(DecryptionType.Image, imageToDecrypt, skipAlpha);
            Steganography.SaveImage(decryptedImage, outputPath, Steganography.embeddedWidth, 
                Steganography.embeddedHeight);
        }

        /// <summary>
        /// Decrypts a text that is inside of an image and saves it to the given path with the given encoding.
        /// </summary>
        /// <param name="imagePath">The path to the image containing the message.</param>
        /// <param name="outputPath">The path to the final decrypted text.</param>
        /// <param name="encoding">The encoding of the text.</param>
        /// <param name="skipAlpha">Should the alpha channel remain untouched?.</param>
        public static void DecryptImageWithText(string imagePath, string outputPath, 
            Encoding encoding, bool skipAlpha = false)
        {
            byte[] imageToDecrypt = Steganography.GetImage(imagePath, ImageType.Original);
            byte[] decryptedText = Decrypt.DecryptMessage(DecryptionType.Text, imageToDecrypt, skipAlpha);

            Steganography.SaveTextFile(outputPath, decryptedText, encoding);
        }
    }
}
