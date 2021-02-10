using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Easy_Stego.Stego
{
    /// <summary>
    /// Sets the width and height of the given type, 
    /// that will be used for saving information in the image and later used for decryption.
    /// </summary>
    public enum ImageType
    {
        /// <summary>
        /// It will not set any width or height variable. It is used when getting the image you are going to decrypt.
        /// </summary>
        None,
        /// <summary>
        /// The image everyone will see.
        /// </summary>
        Original,
        /// <summary>
        /// The image that will be hidden.
        /// </summary>
        Message
    }

    public enum SizeInfo
    {
        //CHANGE THE AMOUNT OF BYTES HERE IF YOU WANT MORE SPACE TO HOLD INFORMATION AT THE BEGINNING OF THE IMAGE.
        Image = 48 * 8, //48 bytes * 8 = 384 bits
        Text = 24 * 8 //24 bytes * 8 = 192 bits
    }

    public static class Steganography
    {
        //ATTENTION!
        //IN THE 'README' FILE THERE IS A SHORT EXPLANATION ON HOW THIS TOOL WORKS, IF YOU ARE WANDERING 
        //WHAT ARE SOME NUMBERS AND ARE NOT UNDERSTANDING HOW THE TOOL WORKS PLEASE READ THE 'README' FILE
        //FOR CLARIFICATION.

        /// <summary>
        /// The width of the image that will hold the message. The image everyone will see.
        /// </summary>
        public static int originalWidth;

        /// <summary>
        /// The height of the image that will hold the message. The image everyone will see.
        /// </summary>
        public static int originalHeight;

        /// <summary>
        /// The width of the image that will be hidden.
        /// </summary>
        public static int embeddedWidth;

        /// <summary>
        /// The height of the image that will be hidden.
        /// </summary>
        public static int embeddedHeight;


        /// <summary>
        /// Converts a string message to byte array.
        /// </summary>
        /// <param name="message">The message you want to hide in the image.</param>
        /// <returns></returns>
        private static byte[] StringToByteArray(string message)
        {
            //Transform a string into a byte array
            byte[] result = Encoding.UTF8.GetBytes(message);
            return result;
        }

        /// <summary>
        /// Gets the text file that is going to be hidden in the image.
        /// </summary>
        /// <param name="path">The path of the text file.</param>
        /// <returns>An array of bytes with the text converted to bytes.</returns>
        public static byte[] GetText(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception("The file doesn't exist.");
            }

            string message = File.ReadAllText(path);

            return StringToByteArray(message);
        }


        /// <summary>
        /// Converts an image to byte array
        /// </summary>
        /// <param name="path">The path to the image</param>
        /// <param name="type">Is the image public or hidden?</param>
        /// <returns>An array of bytes containing the image.</returns>
        public static byte[] GetImage(string path, ImageType type)
        {
            if (!File.Exists(path))
            {
                throw new Exception("File doesn't exist.");
            }

            using (Bitmap image = new Bitmap(path))
            {
                List<byte> list = new List<byte>();

                switch (type)
                {
                    case ImageType.Original:
                        originalWidth = image.Width;
                        originalHeight = image.Height;
                        break;
                    case ImageType.Message:
                        embeddedWidth = image.Width;
                        embeddedHeight = image.Height;
                        break;
                }

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        var pixel = image.GetPixel(x, y);

                        list.Add(pixel.R);
                        list.Add(pixel.G);
                        list.Add(pixel.B);
                        list.Add(pixel.A);
                    }
                }

                return list.ToArray();
            }
        }

        /// <summary>
        /// Converts a byte array to an image if possible, and saves it to the given path.
        /// </summary>
        /// <param name="bytes">The byte array to convert to image.</param>
        /// <param name="path">The path where the image is going to be saved.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        public static void SaveImage(byte[] bytes, string path, int width, int height)
        {
            using (Bitmap image = new Bitmap(width, height))
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = (y * width + x) * 4;

                        System.Drawing.Color color =
                            System.Drawing.Color.FromArgb(
                                bytes[index + 3],
                                bytes[index + 0],
                                bytes[index + 1],
                                bytes[index + 2]);

                        image.SetPixel(x, y, color);
                    }
                }

                image.Save(path, ImageFormat.Png);
            }
        }

        /// <summary>
        /// Saves a text file to the given path.
        /// </summary>
        /// <param name="path">The path where the file will be created</param>
        /// <param name="message">The text that is going to be saved.</param>
        /// <param name="encoding">The text encoding.</param>
        public static void SaveTextFile(string path, byte[] message, Encoding encoding)
        {
            string text = encoding.GetString(message);

            using (StreamWriter writer = new StreamWriter(path + @".txt"))
            {
                writer.Write(text);
            }
        }        
    }
}
