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
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
            }
            TextBox.IsEnabled = true;
            encryptButton.IsEnabled = true;

            var bmp = imgPhoto.Source as BitmapImage;

            int height = bmp.PixelHeight;
            int width = bmp.PixelWidth;
            int stride = width * ((bmp.Format.BitsPerPixel + 7) / 8);

            byte[] bytes = new byte[height * stride];
            bmp.CopyPixels(bytes, stride, 0);

            foreach (byte bits in bytes)
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

        public class TextToBit
        {

        }
                    

        
        private void Button_Click(object sender, RoutedEventArgs e)

        {

            



            if (TextBox.Text == null)

            {

                MessageBox.Show("je moet tekst invullen");

            }

            else if (TextBox.Text != null)

            {

                MessageBox.Show("yess");
                
                    saveImgButton.IsEnabled = true;
                    saveNameTextBox.IsEnabled = true;

                EncryptText.Text = EncryptDecrypt.Encrypt(TextBox.Text, "sblw-3hn8-sqoy19");
                string text = EncryptText.Text;
                byte[] bytes = Encoding.ASCII.GetBytes(text);

                foreach (byte bits in bytes)
                {
                    string bitString = Convert.ToString(bits, 2);
                    char[] bit = bitString.ToCharArray();

                }

                //nodig voor straks het decrypten
                //string someString = Encoding.ASCII.GetString(bytes);

                //saveNameTextBox.Text = EncryptDecrypt.Decrypt(someString, "sblw-3hn8-sqoy19");


            }

        }

        private void SaveImgButton_Click(object sender, RoutedEventArgs e)
        {
            String filePath = @"C:\image\"+saveNameTextBox.Text+".png";
            string path = @"C:\image";

            if(imgPhoto.Source == null)
            {
                MessageBox.Show("maak eerst een foto");
            }

            else if(saveNameTextBox.Text == "")
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

    }
}

