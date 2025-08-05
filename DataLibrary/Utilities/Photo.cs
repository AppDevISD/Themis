using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class Photo
    {
        private static Photo _Photo;

        public static Photo Instance
        {
            get
            {
                if (_Photo == null)
                {
                    _Photo = new Photo();
                }
                return _Photo;
            }
        }
        public string Base64ImgSrc(string fileNameandPath)
        {
            try
            {
                byte[] byteArray = System.IO.File.ReadAllBytes(fileNameandPath);
                string base64 = Convert.ToBase64String(byteArray);

                return string.Format("data:image/gif;base64,{0}", base64);
            }
            catch
            {
                return "./assets/images/misc/NoProfilePhoto.png";
            }
        }
    }
}
