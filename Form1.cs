

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;


namespace IAProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static Image<Gray, Byte> img;
       

        
        private void openImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == DialogResult.OK)
            {
                img = new Image<Gray, Byte>(open.FileName);
                imageBox1.Image = img;
                imageBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            //MessageBox.Show("Image loaded", null, MessageBoxButtons.OK);
        }

       

        

        public static void ThreadProc()
        {
            Application.Run(new Form2());
            Form2 f = new Form2();
           
        }

        private void operationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this.Close();

           
           
            
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(f);

            
            //downsampling the image
            //Image<Gray,Byte> dimage=new Image<Gray,Byte>(img.Width/2,img.Height/2);

            //CvInvoke.cvPyrDown(img,dimage,Emgu.CV.CvEnum.FILTER_TYPE.CV_GAUSSIAN_5x5);

            //Image<Gray, Byte> dimage2= new Image<Gray, Byte>(img.Width / 4, img.Height / 4);
            //CvInvoke.cvPyrDown(dimage, dimage2, Emgu.CV.CvEnum.FILTER_TYPE.CV_GAUSSIAN_5x5);

            //Increasing the consrast
            int ht = img.Height;
            int wd = img.Width;

            

           

            Image<Gray, Byte> dimage3 = new Image<Gray, Byte>(wd, ht);
            CvInvoke.cvEqualizeHist(img, dimage3);

            //Mathematical Morphological operations

            //opening and then closing
            Image<Gray, Byte> dimage4 = new Image<Gray, Byte>(wd, ht);
            Image<Gray, Byte> dimage5 = new Image<Gray, Byte>(wd, ht);
            Image<Gray, Byte> dimage6 = new Image<Gray, Byte>(wd, ht);
            Image<Gray, Byte> dimage7 = new Image<Gray, Byte>(wd, ht);

            StructuringElementEx SE = new StructuringElementEx(3, 3, 3 / 2, 3 / 2, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);


            CvInvoke.cvMorphologyEx(dimage3, dimage4, dimage5, SE, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_CLOSE, 1);
            CvInvoke.cvMorphologyEx(dimage4, dimage6, dimage7, SE, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_OPEN, 1);


            //closing and the opening
            Image<Gray, Byte> dimage8 = new Image<Gray, Byte>(wd, ht);
            Image<Gray, Byte> dimage9 = new Image<Gray, Byte>(wd, ht);
            Image<Gray, Byte> dimage10 = new Image<Gray, Byte>(wd, ht);
            Image<Gray, Byte> dimage11 = new Image<Gray, Byte>(wd, ht);

            StructuringElementEx S2E = new StructuringElementEx(3, 3, 3 / 2, 3 / 2, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);


            CvInvoke.cvMorphologyEx(dimage3, dimage8, dimage9, S2E, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_OPEN, 1);
            CvInvoke.cvMorphologyEx(dimage8, dimage10, dimage11, S2E, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_CLOSE, 1);

            for (int i = 0; i < dimage11.Height; i++)
            {
                for (int j = 0; j < dimage11.Width; j++)
                {

                    dimage11.Data[i, j, 0] = (byte)((dimage10.Data[i, j, 0] + dimage6.Data[i, j, 0]) / 2);

                }

            }
            //dilating-eroding
            Image<Gray, Byte> dimage12 = new Image<Gray, Byte>(wd, ht);
            Image<Gray, Byte> dimage13 = new Image<Gray, Byte>(wd, ht);
            Image<Gray, Byte> dimage14 = new Image<Gray, Byte>(wd, ht);

            CvInvoke.cvDilate(dimage11, dimage12, SE, 1);
            CvInvoke.cvErode(dimage11, dimage13, SE, 1);
            for (int i = 0; i < dimage11.Height; i++)
            {
                for (int j = 0; j < dimage11.Width; j++)
                {
                    if ((dimage12.Data[i, j, 0] - dimage13.Data[i, j, 0]) > 0)
                        dimage14.Data[i, j, 0] = (byte)((dimage12.Data[i, j, 0] - dimage13.Data[i, j, 0]));
                    else
                        dimage14.Data[i, j, 0] = 0;
                }

            }

            //Binarization
            float[,] mat = new float[1, 3];
            mat[0, 0] = -1;
            mat[0, 1] = 0;
            mat[0, 2] = 1;
            ConvolutionKernelF g1 = new ConvolutionKernelF(mat);
            Image<Gray, float> dimag15 = dimage14 * g1;
            for (int i = 0; i < dimag15.Height; i++)
            {
                for (int j = 0; j < dimag15.Width; j++)
                {
                    dimag15.Data[i, j, 0] = Math.Abs(dimag15.Data[i, j, 0]);
                }
            }

            float[,] mat2 = new float[3, 1];
            mat2[0, 0] = -1;
            mat2[1, 0] = 0;
            mat2[2, 0] = 1;
            ConvolutionKernelF g2 = new ConvolutionKernelF(mat2);
            Image<Gray, float> dimag16 = dimage14 * g2;
            for (int i = 0; i < dimag16.Height; i++)
            {
                for (int j = 0; j < dimag16.Width; j++)
                {
                    dimag16.Data[i, j, 0] = Math.Abs(dimag16.Data[i, j, 0]);
                }
            }

            //taking maximum of two convolved images
            Image<Gray, float> dimag17 = new Image<Gray, float>(dimag16.Size);
            for (int i = 0; i < dimag17.Height; i++)
            {
                for (int j = 0; j < dimag17.Width; j++)
                {
                    dimag17.Data[i, j, 0] = Math.Max(dimag15.Data[i, j, 0], dimag16.Data[i, j, 0]);
                }
            }

            //Calculating threshold
            float gamma1 = 0;
            float gamma2 = 0;
            float gamma = 0;
            float max = -1;
            for (int i = 0; i < dimag17.Height; i++)
            {
                for (int j = 0; j < dimag17.Width; j++)
                {
                    if (max < dimage14.Data[i, j, 0])
                    {
                        max = dimage14.Data[i, j, 0];
                    }
                    gamma1 = gamma1 + (dimage14.Data[i, j, 0] * dimag17.Data[i, j, 0]);
                    gamma2 = gamma2 + dimag17.Data[i, j, 0];
                }
            }
            gamma = gamma1 / gamma2;
            float thres = gamma * 255 / max;
            Console.WriteLine(thres);

            Image<Gray, Byte> binImage = new Image<Gray, Byte>(dimage14.Size);
            for (int i = 0; i < dimage14.Height; i++)
            {
                for (int j = 0; j < dimage14.Width; j++)
                {
                    if (dimage14.Data[i, j, 0] <= thres)
                    {
                        binImage.Data[i, j, 0] = 0;
                    }
                    else
                        binImage.Data[i, j, 0] = 255;
                }
            }
           // Image<Gray, Byte> dimag18 = new Image<Gray, Byte>(wd, ht);
            Image<Gray, Byte> dimag18 = new Image<Gray, Byte>(wd, ht);
            Image<Gray, Byte> dimag19 = new Image<Gray, Byte>(wd, ht);
            Image<Gray, Byte> dimag20 = new Image<Gray, Byte>(wd, ht);

            //closing
            CvInvoke.cvMorphologyEx(binImage, dimag18, dimag19, S2E, Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_CLOSE, 1);

            // this is the thresholed image
            //imageBox2.Image = dimag18;
            
           
            dimag18.Save(@"imgmid17.jpg");

           

            
            // show the image after preprocessing !


            imageBox2.Image = dimag18;


            imageBox2.SizeMode = PictureBoxSizeMode.StretchImage;
           
            
           // imageBox2.SizeMode = PictureBoxSizeMode.StretchImage;

           
            
            
            // to get the connected components in this image


            Image<Gray, Byte> mask = new Image<Gray, Byte>(img.Size);
            
            MemStorage storage = new MemStorage();
            Contour<Point> contours = dimag18.FindContours(
                 Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
                 Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST,
                 storage);

            Rgb r = new Rgb(0, 0, 0);
            Rgb r1 = new Rgb(255, 255, 255);



            MCvScalar color1 = r.MCvScalar;
            MCvScalar color2 = r1.MCvScalar;


            Point p = new Point(0, 0);

            List<double> heightslist = new List<double>();
            List<Rectangle> bboxlist = new List<Rectangle>();
            List<Rectangle> bboxs = new List<Rectangle>();
            int font_size = font.Value;



            Image<Gray, Byte> thresh;


            //Image<Gray, Byte> mask = new Image<Gray, byte>(img.Size);

            mask.SetValue(0.0);


            

            //int font_size = 26; 

            //Console.Write("font size =" + font_size);
            //MessageBox.Show(""+font_size);

            double center_h_r1, center_h_r2;

            double ratio_wh_r;

            double height_r1, height_r2, height_r3=0;

            double width_r;

            double areaRatio_r1, areaRatio_r2;

            if (font_size >= 20 && font_size < 30)
            {

                //MessageBox.Show("Font 20 to 30");
               /* center_h_r1 = 0.2;
                center_h_r2 = 0.5;
                ratio_wh_r = 6;
                width_r = 0.3;
                height_r1 = 0.2;
                height_r2 = 8;
                areaRatio_r1 = 0.05;
                areaRatio_r2 = 0.95;
                hw.Value = 6;
                het.Value = 8;
                are.Value = 0;
                thresh = img.ThresholdBinary(new Gray(80), new Gray(255));*/

                //MessageBox.Show("Font 30 to 40");
                center_h_r1 = 0.25;
                center_h_r2 = 0.75;
                ratio_wh_r = 3;
                width_r = 0.9;
                height_r1 = 0.7;
                height_r2 = 25;
                height_r3 = 80;
                areaRatio_r1 = 0.1;
                areaRatio_r2 = 0.95;
                hw.Value = 10;
                het.Value = 10;
                are.Value = 0;
                thresh = img.ThresholdBinary(new Gray(100), new Gray(255));


            }
            else if (font_size >= 30 && font_size < 40)
            {
                //MessageBox.Show("Font 30 to 40");
                center_h_r1 = 0.25;
                center_h_r2 = 0.75;
                ratio_wh_r = 3;
                width_r = 0.9;
                height_r1 = 0.7;
                height_r2 = 25;
                height_r3 = 80;
                areaRatio_r1 = 0.1;
                areaRatio_r2 = 0.95;
                hw.Value = 10;
                het.Value = 10;
                are.Value = 0;
                thresh = img.ThresholdBinary(new Gray(100), new Gray(255));


            }
            else
            {
               // MessageBox.Show("Font 40 to 50");
                center_h_r1 = 0.25;
                center_h_r2 = 0.75;
                ratio_wh_r = 3;
                width_r = 0.9;
                height_r1 = 0.7;
                height_r2 = 75;
                height_r3 = 290;
                areaRatio_r1 = 0.1;
                areaRatio_r2 = 0.95;
                hw.Value = 15;
                het.Value = 12;
                are.Value = 0;
                //thresh = img.ThresholdBinary(new Gray(255), new Gray(255));



            }


            Image<Gray, Byte> mask3 = new Image<Gray, Byte>(mask.Size);


           

            for (; contours != null; contours = contours.HNext)
            {
                Contour<Point> approxContour = contours.ApproxPoly(contours.Perimeter * 0.01, contours.Storage);

                double area = approxContour.Area;
                Rectangle boundbox = approxContour.BoundingRectangle;
                double height = boundbox.Height;
                double width = boundbox.Width;

                double Ratio_wh = height/width;
                double center_w = (boundbox.X + width / 2) / dimag18.Cols;
                double center_h = (boundbox.Y + height / 2) / dimag18.Rows;
                MCvBox2D bbox = approxContour.GetMinAreaRect();
                SizeF s = bbox.size;
                double barea = s.Height * s.Width;
                double areaRatio = area / barea;

                bool ok = false;

                

                //determine if the contours are good based on some of our observations
                if ((center_h > center_h_r1) && (center_h < center_h_r2))
                {

                    if ((Ratio_wh < ratio_wh_r) && (width < width_r * dimag18.Cols))
                    {
                        if ((height < height_r1 * dimag18.Rows) && (height > height_r2) && height<height_r3)
                        {
                            if ((areaRatio > areaRatio_r1) && (areaRatio < areaRatio_r2))
                            {
                                ok = true;
                            }
                        }
                    }
                }

                if (ok)
                {


                    CvInvoke.cvDrawContours(mask, contours, color2, color1, 0, -1, Emgu.CV.CvEnum.LINE_TYPE.EIGHT_CONNECTED, p);
                    //CvInvoke.cvDrawContours(dimag18, contours, color2, color1, 0, 30, Emgu.CV.CvEnum.LINE_TYPE.EIGHT_CONNECTED,p);
                    Rectangle rect = contours.BoundingRectangle;
                    //Draw(rect, new Gray(255.0), -1);
                    bboxlist.Add(boundbox);
                    heightslist.Add(height);


                }



                /*else
                {
                    Rectangle rect = contours.BoundingRectangle;
                    dimag20.Draw(rect, new Gray(0.0), -1);

                }*/



                //Image<Gray, Byte> plate = dimag18.Copy(bbox);
                //     Rectangle rect = contours.BoundingRectangle;
                //     dimag18.Draw(rect, new Gray(255.0), -1);



            }


           

            mask.Save(@"before_threshold.jpg");
            
            // Thresholding the image to seperate foreground (characters) from background (scene)

            //thresh.SetValue(0, mask);

            //mask.SetValue(0, thresh);

            imageBox3.Image = mask;
            imageBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            

            mask.Save(@"resultimage1.jpg");

            mask3 = mask;

            if (font_size >= 20 && font_size < 30)
            {



                Image<Gray, byte> mask2 = new Image<Gray, byte>(mask.Width * 2, mask.Height * 2);
                CvInvoke.cvResize(mask, mask2, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);




               

                mask2.Save(@"resultimage2.jpg");


               
                for (int i = 0; i < mask3.Height; i++)
                {
                    for (int j = 0; j < mask3.Width; j++)
                    {
                        mask3.Data[i, j, 0] = mask2.Data[((int)(mask2.Height / 4)) + i, ((int)(mask2.Width / 4)) + j, 0];
                    }
                }
                imageBox3.Image = mask3;
                imageBox3.SizeMode = PictureBoxSizeMode.StretchImage;



            }

            imageBox3.Image = mask;
            imageBox3.SizeMode = PictureBoxSizeMode.StretchImage;



           // f.showimages(img,dimag18,mask);
           


            
            
            
            
            
            
            
            
            
            // now recognition 
            
            
            
            //Image<Gray, Byte> mask = new Image<Gray, byte>(img.Size);

          /*  Image<Gray, Byte> mask1 = new Image<Gray, Byte>(img.Width + 2, img.Height + 2);

            for (int j = 0; j < mask1.Height; j++)
            {
                for (int k = 0; k < mask1.Width; k++)
                {
                    mask1.Data[j, k, 0] = 0;

                }
            }*/


            // now filling the text for recognition using OCR

         /*   MCvConnectedComp comp = new MCvConnectedComp();

            for (int i = 0; i < bboxlist.Count; i++)
            {

                Rectangle bb = bboxlist[i];


                CvInvoke.cvFloodFill(mask.Ptr, bb.Location, new MCvScalar(255), new MCvScalar(1), new MCvScalar(20), out comp, Emgu.CV.CvEnum.CONNECTIVITY.EIGHT_CONNECTED, Emgu.CV.CvEnum.FLOODFILL_FLAG.DEFAULT, mask1.Ptr);


            }



            // I wanted white foreground, black background


            for (int i = 0; i < mask.Height; i++)
            {
                for (int j = 0; j < mask.Width; j++)
                {


                    mask.Data[i, j, 0] = (byte)(255 - mask.Data[i, j, 0]);
                }
            }


            // dilating the image for darker (more profound) characters

            Image<Gray, Byte> DilImg = new Image<Gray, Byte>(wd, ht);
            StructuringElementEx S3E = new StructuringElementEx(3, 6, 3 / 2, 6 / 2, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE);
            CvInvoke.cvDilate(mask, DilImg, S3E, 1);

            DilImg.Save(@"jatinzshowroom.jpg");
            imageBox2.Image = DilImg;*/






            // OCR engine starts here !

             Bitmap image1 = mask3.ToBitmap();

          
            
             tessnet2.Tesseract ocr = new tessnet2.Tesseract();

             //tessnet2.Tesseract.OcrDoneHandler t = ocr.OcrDone;

             //Console.Out.WriteLine(t + "hubababa");


             ocr.Init(@"C:\Users\Jatin\Downloads\bin\Release32\tessdata", "eng", false);

             Console.Out.WriteLine("tes");
             //ocr.SetVariable("tessedit_char_whitelist", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"); // If digit only

            
             //ocr.Init(@"ullu123", "GOVTOFDELHI", true); // To use correct tessdata

            

             List<tessnet2.Word> result = ocr.DoOCR(image1, mask.ROI);

            


             Console.Out.WriteLine("tes123");

             ocr.Dispose();
            
             //Console.Out.WriteLine(result.ElementAt(0));
             Console.Out.WriteLine("Hello");

            // writing results to a text file

             System.IO.StreamWriter file = new System.IO.StreamWriter(@"togoogle.txt");

             String input="";
             
             foreach (tessnet2.Word word in result)
             {
                 //MessageBox.Show(word.Text);

                 input = input + word.Text + " ";
                 file.Write(word.Text+" ");

                 //Console.Out.WriteLine(word.Text);

             }



             file.Close();


            // String translatedtext = TranslateText(input, "en|fra", System.Text.Encoding.UTF7);


            // System.IO.StreamWriter file2 = new System.IO.StreamWriter(@"togoog.txt");

            // file2.Write(translatedtext);

            // file2.Close();
            
            MessageBox.Show("The extracted text written to file...The text extracted: "+ input);

            // connecting with Google Translate !


            
            
            //dimag18.Save(@"check12.png");

            //MCvMat heightsMat(heightslist);    
            //MCvScalar meanS;
            //MCvScalar stdS;

            // Image<Gray, Byte> thresh = img.ThresholdBinary(new Gray(120), new Gray(0));


            // double mean =heightslist.Average();

            //Rectangle dr = new Rectangle();

            //meanStdDev(heightsMat, meanS, stdS);
            //double mean = meanS[0];

            //  Image<Gray, Byte> mask = new Image<Gray, byte>(img.Size);

            // mask.SetValue(0.0);
            /*for (int i = 0; i<heightslist.Count; i++)
            {
                Rectangle bbox = bboxlist[i];
                //heights within the bounds
                if(heightslist[i]>=mean*0.6 && heightslist[i]<=mean*1.8)
                {
                    bboxs.Add(bbox);

                   
                    dr.X = bbox.X;
                    dr.Y = bbox.Y;
                    dr.Height = bbox.Height-1;
                    dr.Width = bbox.Width-1;

                    //CvInvoke.cvDrawContours(dimag18, contours, color2, color1, 0, 30, Emgu.CV.CvEnum.LINE_TYPE.EIGHT_CONNECTED, p);
                    //CvInvoke.cvDrawContours(mask, contours, color2, color1, 0, 30, Emgu.CV.CvEnum.LINE_TYPE.EIGHT_CONNECTED, p);
                    dimag18.Draw(dr, new Gray(255.0), -1);
                    //mask.Draw(dr, new Gray(255.0), -1);

                   
                    
                }

               

           }*/
            //heightslist.clear();
            //bboxsRaw.clear();
            // thresh.SetValue(0, mask);

            //I1->copyTo(*I2);



            //  mask.Save(@"ullu123.png");

            //dimag18.Save(@"check123.png");
            //MessageBox.Show("" + contours);

            //System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc));
            //t.Start();





        }


        public static string TranslateText(string input, string languagePair, Encoding encoding)
    {
      string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);

      string result = String.Empty;

      using (WebClient webClient = new WebClient())
      {
        webClient.Encoding = encoding;
        result = webClient.DownloadString(url);
      }
      Match m = Regex.Match(result, "(?<=<div id=result_box dir=\"ltr\">)(.*?)(?=</div>)");

      if (m.Success)
          result = m.Value;

      return result;
    }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
