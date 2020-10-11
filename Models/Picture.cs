using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace S.Models
{
    public class Picture
    {
        public int imageId { get; set; }
        public string imageName { get; set; }
        public double Z { get; set; }
        public double H { get; set; }
        public double W { get; set; }
        public HttpPostedFileWrapper imageFile { get; set; }

    }
}