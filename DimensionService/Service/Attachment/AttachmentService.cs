using DimensionService.Common;
using DimensionService.Dao.UserInfo;
using DimensionService.Models.DimensionModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

namespace DimensionService.Service.Attachment
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IUserInfoDAO _userInfoDAO;

        public AttachmentService(IUserInfoDAO userInfoDAO)
        {
            _userInfoDAO = userInfoDAO;
        }

        public bool UploadAttachment(IFormFile file, string fileName)
        {
            using Stream stream = file.OpenReadStream();
            if (file.FileName.ToLower(ClassHelper.cultureInfo).Contains(".gif", StringComparison.CurrentCulture))
            {
                using Image image = Image.Load(stream);
                if (image.GetType() == typeof(Image<Rgba32>))
                {
                    foreach (ImageFrame<Rgba32> item in image.Frames)
                    {
                        for (int y = 0; y < item.Height; y++)
                        {
                            Span<Rgba32> rgba32s = item.GetPixelRowSpan(y);
                            for (int x = 0; x < item.Width; x++)
                            {
                                if (rgba32s[x].A == 0)
                                {
                                    rgba32s[x] = new Rgba32(255, 255, 255);
                                }
                            }
                        }
                    }
                }
                image.SaveAsGif(Path.Combine(ClassHelper.attachmentsPath, $"{fileName}"));
            }
            else
            {
                ClassHelper.WriteFile(stream, Path.Combine(ClassHelper.attachmentsPath, fileName));
            }
            stream.Close();
            return true;
        }

        public FileResult GetAttachments(string fileName, int height)
        {
            if (fileName == null)
            {
                fileName = "404.png";
            }
            string filePath = Path.Combine(ClassHelper.attachmentsPath, fileName);
            if (!File.Exists(filePath))
            {
                filePath = Path.Combine(ClassHelper.attachmentsPath, "404.png");
            }
            FileInfo fileInfo = new(filePath);
            string ext = fileInfo.Extension.ToLower(ClassHelper.cultureInfo);
            Stream ms = null;
            if (ext.Contains("jpg") || ext.Contains("png"))
            {
                if (height > 0)
                {
                    ms = new MemoryStream();
                    Image image = ClassHelper.CompressPictures(filePath, height);
                    image.Save(ms, ext.Contains("png") ? new PngEncoder() : new JpegEncoder());
                    image.Dispose();
                }
            }
            if (ms == null)
            {
                ms = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            new FileExtensionContentTypeProvider().Mappings.TryGetValue(ext, out string contentType);
            return ms.GetType() == typeof(MemoryStream)
                ? new FileContentResult(((MemoryStream)ms).ToArray(), contentType ?? "application/octet-stream")
                : new FileStreamResult(ms, contentType ?? "application/octet-stream");
        }

        public FileResult GetHeadPortraits(string userID, int height)
        {
            string fileName = _userInfoDAO.UserInfoFindForUserID(userID) is UserInfoModel userInfo ? userInfo.HeadPortrait : "404.png";
            return GetAttachments(fileName, height);
        }
    }
}
