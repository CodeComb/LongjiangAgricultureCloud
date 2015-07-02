using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LongjiangAgricultureCloud.Helpers
{
    public static class Video
    {
        #region //运行FFMpeg的视频解码，(这里是绝对路径)
        /// <summary>
        /// 视频文件转换（.flv）
        /// </summary>
        /// <param name="fileName">转换视频文件的路径（原文件）</param>
        /// <param name="playFile">转换后的文件的路径(.flv)</param>
        /// <param name="widthSize">视频帧宽</param>
        /// <param name="heightSize">视频帧高</param>
        /// <returns></returns>
        public static bool ChangeFilePhy(string fileName, string playFile, string widthSize, string heightSize)
        {
            //ffmpeg.exe flv转换工具
            string ffmpeg = MvcApplication.Path + "Shell\\ffmpeg.exe";
            //判断转换工具，转换文件是否存在
            if (!System.IO.File.Exists(ffmpeg))
            {
                throw new Exception("没有找到ffmpeg从位置：" + ffmpeg);
            }
            if (!System.IO.File.Exists(fileName))
            {
                throw new Exception("没有找到视频从位置：" + fileName);
            }
            //获得(.mp4)文件
            string flv_file = System.IO.Path.ChangeExtension(playFile, ".mp4");

            System.Diagnostics.ProcessStartInfo FilestartInfo = new System.Diagnostics.ProcessStartInfo(ffmpeg);
            //隐藏cmd命令窗口
            FilestartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //执行命令参数
            FilestartInfo.Arguments = " -i " + fileName + " -ab 56 -ar 22050 -b 500 -r 15 -s " + widthSize + "x" + heightSize + " " + flv_file;

            try
            {
                //执行文件转换
                System.Diagnostics.Process.Start(FilestartInfo);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}