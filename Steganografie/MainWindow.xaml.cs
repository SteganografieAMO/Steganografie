using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.IO;
using System.Collections;

namespace Steganografie
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported images|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
            }
            bitTextBox.IsEnabled = true;
            encryptButton.IsEnabled = true;

            var bmp = imgPhoto.Source as BitmapImage;

            int height = bmp.PixelHeight;
            int width = bmp.PixelWidth;
            int stride = width * ((bmp.Format.BitsPerPixel + 7) / 8);

            byte[] Imgbytes = new byte[height * stride];
            bmp.CopyPixels(Imgbytes, stride, 0);

            foreach (byte bits in Imgbytes)
            {
                string bitString = Convert.ToString(bits, 2);
                char[] bit = bitString.ToCharArray();

            }
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
                byte[] inputArray = Convert.FromBase64String(input);
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

        private void SaveImgButton_Click(object sender, RoutedEventArgs e)
        {
            String filePath = @"C:\Image\" + saveNameTextBox.Text + ".jpg";
            string path = @"C:\Image";

            if (imgPhoto.Source == null)
            {
                MessageBox.Show("maak eerst een foto");
            }

            else if (saveNameTextBox.Text == "")
            {
                MessageBox.Show("u moet eerst nog een naam geven in de textbox boven de knop");
            }

            else if (Directory.Exists(path))
            {

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imgPhoto.Source));
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    encoder.Save(stream);

                MessageBox.Show("uw foto is nu opgeslagen");

            }
            else
            {
                DirectoryInfo di = Directory.CreateDirectory(path);


           }
        }
        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void encryptButton_Click(object sender, RoutedEventArgs e)
        {
            

            if (bitTextBox.Text != null)
            {
                var bmp = imgPhoto.Source as BitmapImage;

                int height = bmp.PixelHeight;
                int width = bmp.PixelWidth;
                int stride = width * ((bmp.Format.BitsPerPixel + 7) / 8);

                int imglength = height * stride;
                byte[] IMGbytes = new byte[imglength];
                bmp.CopyPixels(IMGbytes, stride, 0);

                foreach (byte bits in IMGbytes)
                {
                    string bitString = Convert.ToString(bits, 2);
                    char[] imgbit = bitString.ToCharArray();

                }

                saveImgButton.IsEnabled = true;
                saveNameTextBox.IsEnabled = true;

                EncryptText.Text = EncryptDecrypt.Encrypt(bitTextBox.Text, "sblw-3hn8-sqoy19");
                string text = bitTextBox.Text;
                byte[] bytes = Encoding.UTF8.GetBytes(text);

                foreach (byte bit in bytes)
                {

                    string bitString = Convert.ToString(bit, 2);
                    char[] bits = bitString.ToCharArray();





                    foreach (char thisBit in bits)
                    {
                        foreach (byte PhotoByte in IMGbytes)
                        {
                            byte photobyte = PhotoByte;
                            byte result = HideBit(thisBit, photobyte);
                        }

                    }
                }
                MessageBox.Show("tekst succesvol encrypted in foto");

                
               



                //nodig voor het decrypten straks
                //string someString = Encoding.ASCII.GetString(bytes);

                //saveNameTextBox.Text = EncryptDecrypt.Decrypt(someString, "sblw-3hn8-sqoy19");
            }
            else if (string.IsNullOrWhiteSpace(bitTextBox.Text))
            {
                MessageBox.Show("Je moet tekst invullen in het tekst vak");   
            }
            
        }

        public static byte HideBit(char bits, byte destinationByte)
        {
            int result = 0;
            if (bits == '1')
            {
                result = destinationByte | 0b0000_0001;
            }
            else if (bits == '0')
            {
                result = destinationByte & 0b1111_1110;
            }
            return (byte)result;
        }

        private void decryptImage_Click(object sender, RoutedEventArgs e)
        {
            DecryptWindow decryp = new DecryptWindow();
            decryp.Show();
            this.Close();
        }


    }
}

