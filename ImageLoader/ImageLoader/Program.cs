using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
namespace ImageLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap b = new Bitmap(Image.FromFile(Directory.GetCurrentDirectory() + "//piece.png"));
            StreamWriter f = new StreamWriter(Directory.GetCurrentDirectory() + "//piece.txt",false,Encoding.UTF8);
            f.Write("{");
           for(int i = 0; i < b.Height; i++)
            {
                for(int j = 0; j < b.Width; j++)
                {
                    Color c = b.GetPixel(j, i);
                    if (c.ToArgb() == 0)
                    {

                        f.Write(Color.Transparent.ToArgb().ToString() + ",");
                    }
                    else
                    {
                        f.Write(c.ToArgb().ToString() + ",");
                    }
                    
                }
            }

            f.Write("}");
            f.Close();
        }
    }
}
