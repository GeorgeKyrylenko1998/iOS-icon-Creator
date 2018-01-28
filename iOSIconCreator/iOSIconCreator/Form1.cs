using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace iOSIconCreator
{
    public partial class Form1 : Form
    {
        Action<int, Rectangle> updateArray;
        Action<int> updateProgress;
        Action end;
        double more = 1;
        int heigth = 0;
        int width = 0;
        int count = 1;
        Thread thread;
        BindingList<ImagePreview> files = new BindingList<ImagePreview>();
        public void endMeth()
        {
            btnOpenFile.Enabled = true;
            btnSave.Enabled = true;
        }
        public Form1()
        {
            InitializeComponent();
            end = endMeth;
            dataGridView1.DataSource = files;
            updateArray = updateArrayMethod; 
        }

        void updateArrayMethod( int num, Rectangle rec)
        {
            files[num].image = files[num].image.Crop(rec);//new Rectangle(y, x, y1 - y, x1 - x));
        }
        void updatePROGRESSMethod(int progress)
        {
            progressBar.Value = progress;
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            foreach (string t in openFileDialog.FileNames)
            {
                files.Add(new ImagePreview { path = t, image = Image.FromFile(t) });
            }
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ChangeImage()
        {
            //    thread.Start();

            btnOpenFile.Enabled = false;
            btnSave.Enabled = false;
            thread = new Thread(saveImage);
            if(checkBox.Checked)thread = new Thread(CropImage);
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                if(comboBox1.SelectedIndex == 1)
                {
                    heigth = 24;
                    width = 24;
                }
             if(tbH.Text!="")
                {
                    more = double.Parse( tbH.Text);
                }
             thread.Start();
            //CropImage();
        }

        public void saveImage()
        {
            int num = 0;
            if (count == 2) num = files.Count();
            foreach (ImagePreview t in files)
            {
                int h;
                int w;
                if (heigth == 0)
                    h = t.image.Height;
                else
                    h = heigth;
                if (width == 0)

                    w = t.image.Width;
                else
                    w = width;
                var imgX1 = Imager.Resize(t.image,(int)( w * more), (int)(h * more),true);
                var imgX2 = Imager.Resize(t.image,(int)( w * 2 * more),(int)( h * 2 * more), true);
                var imgX3 = Imager.Resize(t.image,(int)( w * 3 * more), (int)(h * 3 * more), true);
                string[] name = t.path.Split('\\');
                string[] nameFile = name[name.Length-1].Split('.');
                imgX1.Save(Path.Combine(folderBrowserDialog.SelectedPath , nameFile[0] +"." + nameFile[1]));
                imgX2.Save( Path.Combine(folderBrowserDialog.SelectedPath , nameFile[0]+ "@2x." + nameFile[1]));
                imgX3.Save(Path.Combine(folderBrowserDialog.SelectedPath , nameFile[0]+ "@3x." + nameFile[1]));
                ++num;
                //this.Invoke(updateProgress ,(int)(((double)num / (double)(files.Count * count)) * 100));
            }

            MessageBox.Show("Compleate", "Compleate");
            Invoke(end);
        }

        public void CropImage()
        {
            int num = 0;
            foreach (ImagePreview t in files)
            {
                var bit = new Bitmap(t.image);
                var h = bit.Width;
                var w = bit.Height;
                int x = w;
                int y = h;
                int x1 = 0;
                int y1 = 0;
                for (int i = 0; i < h; ++i)
                {
                    for (int k = 0; k < w; ++k)
                    {
                        //string color = bit.GetPixel(i, k).ToString();
                       
                        if (bit.GetPixel(i, k) != Color.FromArgb(0,0,0,0))
                        {
                            x = Math.Min(x, i);
                            x1 = Math.Max(x1, i);
                            y = Math.Min(y, k);
                            y1 = Math.Max(y1, k);
                        }
                    }
                }
                this.Invoke(this.updateArray,num, new Rectangle(x, y, x1 - x, y1 - y)) ;
                //updateArrayMethod(num, new Rectangle(x, y, x1 - x, y1 - y));
                ++num;
                //this.Invoke(updateProgress, (int)(((double)num / (double)(files.Count * count)) * 100));
            }
            saveImage();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ChangeImage();
        }

        private void Exit(object sender, EventArgs e)
        {
            thread.Abort();
            this.Close();
        }
    }
}
