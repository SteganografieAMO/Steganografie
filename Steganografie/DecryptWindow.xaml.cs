using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Steganografie
{
    /// <summary>
    /// Interaction logic for DecryptWindow.xaml
    /// </summary>
    public partial class DecryptWindow : Window
    {
        public DecryptWindow()
        {
            InitializeComponent();

        }
        public static string EncodeBase64(string data)
        {
            string dummyData = data.Trim().Replace(" ", "+");
            if (dummyData.Length % 4 > 0)
                dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
            byte[] byteArray = Convert.FromBase64String(dummyData);

            return dummyData;
        }
        public class EncryptDecrypt
        {
            public static string Encrypt(string input, string key)
            {
                byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
                TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
                tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
                tripleDES.Mode = CipherMode.ECB;
                tripleDES.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tripleDES.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                tripleDES.Clear();
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            public static string Decrypt(string input, string key)
            {
                byte[] inputArray = Encoding.UTF8.GetBytes(input);
                TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
                tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
                tripleDES.Mode = CipherMode.ECB;
                tripleDES.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tripleDES.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                tripleDES.Clear();
                return UTF8Encoding.UTF8.GetString(resultArray);

                
            }
        }

        private void loadEncrypImgButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog decryptImg = new OpenFileDialog();
            decryptImg.InitialDirectory = @"C:\image";
            decryptImg.Title = "Select an Encrypted picture";
            decryptImg.Filter = "All supported images|*.jpg;*.jpeg;|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg";
            if (decryptImg.ShowDialog() == true)
            {
                encryptedImg.Source = new BitmapImage(new Uri(decryptImg.FileName));
            }
            stringTextBox.IsEnabled = true;
            decryptButton.IsEnabled = true;
            

        }

        private void decryptButton_Click(object sender, RoutedEventArgs e)
        {
            var bmp = encryptedImg.Source as BitmapImage;

            int height = bmp.PixelHeight;
            int width = bmp.PixelWidth;
            int stride = width * ((bmp.Format.BitsPerPixel + 7) / 8);

            byte[] Imgbytes = new byte[height * stride];
            bmp.CopyPixels(Imgbytes, stride, 0);

            var bitmap = BitmapImage2Bitmap(bmp);


            MessageBox.Show(extractText(bitmap));

            MessageBox.Show(EncryptDecrypt.Decrypt(extractText(bitmap), "sblw-3hn8-sqoy19"));

            
        }

        private void encryptWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        public static string extractText(Bitmap bmp)
        {
            int colorIndex = 0;
            int charValue = 0;

            string extractedText = String.Empty;

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    System.Drawing.Color pixel = bmp.GetPixel(j, i);

                    for (int n = 0; n < 3; n++)
                    {
                        switch (colorIndex % 3)
                        {
                            case 0:
                                {
                                    charValue = charValue * 2 + pixel.R % 2;
                                }break;
                            case 1:
                                {
                                    charValue = charValue * 2 + pixel.G % 2;
                                }break;
                            case 2:
                                {
                                    charValue = charValue * 2 + pixel.B % 2;
                                }break;
                        }

                        colorIndex++;

                        if (colorIndex % 8 == 0)
                        {
                            charValue = reverseBits(charValue);

                            if (charValue == 0)
                            {
                                return extractedText;
                            }

                            char c = (char)charValue;

                            extractedText += c.ToString();
                        }
                    }
                }
            }
            return extractedText;
        }
        public static int reverseBits(int n)
        {
            int result = 0;

            for (int i = 0; i < 8; i++)
            {
                result = result * 2 + n % 2;

                n /= 2;
            }

            return result;
        }
        public BitmapImage ConvertBmp(System.Drawing.Image img)
        {
            using (var memory = new MemoryStream())
            {
                img.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
    }
}
