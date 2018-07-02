using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using LzmaAlone;
namespace Base64
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }
        
        
        BackgroundWorker worker1 = new BackgroundWorker();
        BackgroundWorker worker2 = new BackgroundWorker();
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
            
        }

        public static string Base64Decode(string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception ex)
            {
                return "Cannot convert.Provide proper input!";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            encodeFileToBase64Binary(textBox3.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
             OpenFileDialog dialog = new OpenFileDialog();
             dialog.Multiselect = false;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox3.Text = dialog.FileName.ToString();
            }
        }

        private void encodeFileToBase64Binary(string path)
        {
            try
            {
                string FileName = string.Empty;
                byte[] FinalByteStream = null;
                worker1.DoWork += delegate(object s, DoWorkEventArgs args)
                {
                    string[] Name = path.Split('\\');
                    FileName = Name[Name.Length - 1].Substring(0, Name[Name.Length - 1].LastIndexOf("."));
                    byte[] bytes = SevenZip.Compression.LZMA.SevenZipHelper.Compress(File.ReadAllBytes(path));
                    //string file = Convert.ToBase64String(bytes);
                     //= bytes;//Encoding.ASCII.GetBytes(file);
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetTempPath() + "\\Sagar");
                    System.IO.File.WriteAllBytes(System.IO.Path.GetTempPath() + "\\Sagar\\" + System.IO.Path.GetFileName(path), bytes);
                    FinalByteStream=SevenZip.Compression.LZMA.SevenZipHelper.Compress(File.ReadAllBytes(System.IO.Path.GetTempPath() + "\\Sagar\\" + System.IO.Path.GetFileName(path)));
                };

                string dest;
                worker1.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                {
                    pictureBox1.Visible = false;
                    pictureBox1.SendToBack();
                    FolderBrowserDialog folder = new FolderBrowserDialog();
                    if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        

                        dest = folder.SelectedPath;
                        dest = dest + "\\" + FileName + "_Base64.sagar";
                        System.IO.File.WriteAllBytes(@dest, FinalByteStream);

                        MessageBox.Show("File has been saved");
                    }
                };
                worker1.RunWorkerAsync();
                pictureBox1.Visible = true;
                pictureBox1.BringToFront();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Please select a valid file");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "Base 64 file|*.sagar;";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox4.Text = dialog.FileName.ToString();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            decodeBase64ToFile(textBox4.Text);
        }

        private void decodeBase64ToFile(string file)
        {
            try
            {
                Byte[] bytes = null;
                worker2.DoWork += delegate(object s, DoWorkEventArgs args)
                {
                    string[] Name = file.Split('\\');
                    string FileName = Name[Name.Length - 1].Substring(0, Name[Name.Length - 1].LastIndexOf("."));
                    //string FirstLayerString = Encoding.ASCII.GetString(File.ReadAllBytes(file));
                    Byte[] Compressed = File.ReadAllBytes(file);//Convert.FromBase64String(FirstLayerString);
                    bytes = SevenZip.Compression.LZMA.SevenZipHelper.Decompress(SevenZip.Compression.LZMA.SevenZipHelper.Decompress(Compressed));

                };
                worker2.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
               {
                   pictureBox1.Visible = false;
                   pictureBox1.SendToBack();
                   FolderBrowserDialog folder = new FolderBrowserDialog();
                   if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                   {
                       string FileNameWithExt = folder.SelectedPath + "\\Output." + ExtensionFinder(Convert.ToBase64String(bytes), bytes);
                       File.WriteAllBytes(FileNameWithExt, bytes);
                       MessageBox.Show("File has been saved");
                   }
               };
                worker2.RunWorkerAsync();
                pictureBox1.Visible = true;
                pictureBox1.BringToFront();
            }

            catch (Exception ex)
            {
                MessageBox.Show("Please select a valid file");
            }
        }

        private string ExtensionFinder(string Base64, Byte[] bytes)
        {

            string Extension = Base64.Substring(0, 5);

            if (Base64.Substring(0, 2).ToUpper() == "QK")
                return "bmp";

            switch (Extension.ToUpper())
            {
                case "IVBOR":
                    return "png";
                case "/9J/4":
                    return "jpg";
                case "AAAAF":
                    return "mp4";
                case "JVBER":
                    return "pdf";
                case "AAABA":
                    return "ico";
                case "UMFYI":
                    return "rar";
                case "E1XYD":
                    return "rtf";
                case "TVQQA":
                    return "exe";
                case "UESDB":
                    return NewExtensionIdentidfier(Base64, bytes);
                case "PD94B":
                    return "xml";
                case "77U/P":
                    return "xml";
                case "N3Q8R":
                    return "7z";
                case "T2ZMB":
                    return "tar";
                case "AAEAA":
                    return "accdb";
                case "R0LGO":
                    return "gif";               
                case "0M8R4":
                    return NewExtensionIdentidfier(Base64, bytes);//return "doc";
                default:
                    return string.Empty;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (DateTime.Today.Year != 2018)
                Application.Exit();
        }



        private string NewExtensionIdentidfier(string Base64, Byte[] bytes)
        {
            string Extension = string.Empty;

            if (Base64.Contains("0M8R4KGxGuEAAAAAAAAAAAAAAAAAAAAAPgADAP7"))
            {
                if (Base64.Substring(82,1)=="v")
                    Extension = "xls";
                else
                    Extension = "doc";
            }

            else if (Base64.Substring(0, 9).ToUpper()== "UESDBBQAA")
            {
                if (Base64.Substring(0, 11).ToUpper()=="UESDBBQAAAG")
                    Extension = "odt";
                else
                    Extension = "zip";
            }
            else if (Base64.Substring(0, 9).ToUpper() == "UESDBBQAB")
            {
                File.WriteAllBytes(System.IO.Path.GetTempPath() + "\\Output_Temp.zip",bytes);

                string zipPath = System.IO.Path.GetTempPath() + "\\Output_Temp.zip";
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.Contains("xl/"))
                        {
                            Extension = "xlsx";
                            break;
                        }
                        else if (entry.FullName.Contains("word/"))
                        {
                            Extension = "docx";
                            break;
                        }
                        else if (entry.FullName.Contains("ppt/"))
                        {
                            Extension = "pptx";
                            break;
                        }
                    }
                }
                File.Delete(zipPath);
            }
            
            return Extension;
        }

       
    }
}
