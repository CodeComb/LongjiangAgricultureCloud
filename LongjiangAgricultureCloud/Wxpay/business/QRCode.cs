using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtWorks.QRCode.Codec;

namespace WxPayAPI
{
    public class QRCode
    {
        public static string Create(string s)
        {

            //初始化二维码生成工具
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeScale = 4;

            //将字符串生成二维码图片
            Bitmap image = qrCodeEncoder.Encode(s, Encoding.Default);

            //保存为PNG到内存流  
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            return "data:image/jpeg;base64," + Convert.ToBase64String(ms.GetBuffer());
        }
    }
}
