using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;
namespace S.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public string FindS(string Imagename,double height,double width,double Z)
        {
            string fullPath = Path.Combine(Server.MapPath("~/Images/"), Imagename);
            string result = fullPath;
            if (System.IO.File.Exists(fullPath))
            {
                var file = System.IO.File.OpenRead(fullPath);
                Image picture = Image.FromStream(file, true, true);
                Bitmap Bpicture = (Bitmap)picture;
                double S = 0;
                double a = 0;
                double b = 0;
                double c = 0;
                double a1 = 0;
                double b1 = 0;
                double p = 0;
                double p1 = 0;
                double[,] arr = new double[Bpicture.Width, Bpicture.Height];
                double H = height / (double)(Bpicture.Height - 1);
                double W = width / (double)(Bpicture.Width - 1);
                for (int i = 0; i < Bpicture.Width; i++)
                {
                    for (int j = 0; j < Bpicture.Height; j++)
                    {
                        arr[i, j] = Bpicture.GetPixel(i, j).R * Z;
                    }
                }
                for (int i = 0; i < Bpicture.Width - 1; i++)
                {
                    for (int j = 0; j < Bpicture.Height - 1; j++)
                    {
                        a = Math.Sqrt(H * H + Math.Pow((arr[i, j + 1] - arr[i, j]), 2));
                        b = Math.Sqrt(W * W + Math.Pow((arr[i + 1, j] - arr[i, j]), 2));
                        a1 = Math.Sqrt(H * H + Math.Pow((arr[i + 1, j + 1] - arr[i + 1, j]), 2));
                        b1 = Math.Sqrt(W * W + Math.Pow((arr[i, j + 1] - arr[i + 1, j + 1]), 2));
                        c = Math.Sqrt(W * W + H * H + Math.Pow((arr[i, j + 1] - arr[i + 1, j]), 2));
                        p = (a + b + c) / 2;
                        p1 = (a1 + b1 + c) / 2;
                        S += Math.Sqrt(p * (p - a) * (p - b) * (p - c)) + Math.Sqrt(p1 * (p1 - a1) * (p1 - b1) * (p1 - c));
                    }
                }
                result = S.ToString();
            }
            return result;
        }
        public void UploadImage(HttpPostedFileBase ImageFile)
        {
            var file = ImageFile;
            
            if (file != null)
            {
                file.SaveAs(Server.MapPath("/Images/" + file.FileName));
            }
        }
        public void ConvertToExcel(string name)
        {
            string fullPath = Path.Combine(Server.MapPath("~/Images/"), name);
            var file = System.IO.File.OpenRead(fullPath);
            Image picture = Image.FromStream(file, true, true);
            Bitmap Bpicture = (Bitmap)picture;
            Excel.Application ExcelApp = new Excel.Application();
            Excel.Workbook ExcelWorkBook;
            Excel.Worksheet ExcelWorkSheet;
            ExcelWorkBook = ExcelApp.Workbooks.Add(Missing.Value);
            ExcelWorkSheet = (Excel.Worksheet)ExcelWorkBook.ActiveSheet;
            int[,] ExcelArray = new int[Bpicture.Width, Bpicture.Height];
            for (int i = 0; i < Bpicture.Width; i++)
            {
                for (int j = 0; j < Bpicture.Height; j++)
                {
                    ExcelArray[i,j]= Bpicture.GetPixel(i, j).R;
                }
            }
            Excel.Range rng = ExcelWorkSheet.Cells[1, "A"];
            rng = rng.Resize[Bpicture.Width, Bpicture.Height];
            rng.Value = ExcelArray;
            string filenameExcel = name.Split('.').First() + ".xlsx";
            string fullfilename = Server.MapPath("~/ExcelFiles/" + filenameExcel);
            if (System.IO.File.Exists(fullfilename)) System.IO.File.Delete(fullfilename);
            ExcelWorkBook.SaveAs(fullfilename, Excel.XlFileFormat.xlWorkbookDefault);
            ExcelWorkBook.Close(false, Type.Missing, Type.Missing);
            ExcelApp.Workbooks.Close();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ExcelWorkBook);
        }
        
        [HttpPost]
        public string Import(HttpPostedFileBase excelFile)
        {
           if(excelFile.FileName.EndsWith("xls") || excelFile.FileName.EndsWith("xlsx"))
            {
                string path = Server.MapPath("~/ExcelFiles/" + excelFile.FileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                excelFile.SaveAs(path);
                Excel.Application app = new Excel.Application();
                Excel.Workbook workbook= app.Workbooks.Open(path);
                Excel.Worksheet worksheet = workbook.ActiveSheet;
                Excel.Range range = worksheet.UsedRange;
                var ExcelArray = (object[,])range.Value;
                int R = 0;
                Bitmap tempImage = new Bitmap(range.Rows.Count, range.Columns.Count);
                for (int i = 1; i <= tempImage.Width; i++)
                {
                    for (int j = 1; j <= tempImage.Height; j++)
                    {
                        R = Convert.ToInt32(ExcelArray[i, j]);
                        Color color = Color.FromArgb(R, R, R);
                        tempImage.SetPixel(i-1, j-1, color);
                    }
                }
                tempImage.Save(Server.MapPath("/Images/" + excelFile.FileName.Replace("xlsx","jpeg")));
                ImageConverter converter = new ImageConverter();
                workbook.Close(false, Type.Missing, Type.Missing);
                app.Workbooks.Close();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                return Convert.ToBase64String((byte[])converter.ConvertTo(tempImage, typeof(byte[])));
            }
            else
            {
                return "Неверный тип файла";
            }
        }
        [HttpGet]
        public ActionResult Download(string file)
        {
            string ExcelName = file.Split('.').First() + ".xlsx";
            string fullPath = Path.Combine(Server.MapPath("~/ExcelFiles/"), ExcelName);
            if (!System.IO.File.Exists(fullPath))
            {
                ConvertToExcel(file);
            }
            return File(fullPath, "application/vnd.ms-excel", ExcelName);
        }
    }
}