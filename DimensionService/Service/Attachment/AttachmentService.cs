using DimensionService.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace DimensionService.Service.Attachment
{
    public class AttachmentService : IAttachmentService
    {
        public async Task<bool> UploadAttachment(IFormFile file, string fileName)
        {
            using Stream stream = file.OpenReadStream();
            await ClassHelper.WriteFileAsync(stream, Path.Combine(ClassHelper.attachmentsPath, fileName));
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
                    Bitmap bitmap = ClassHelper.ResizeImage(new Bitmap(filePath), height);
                    bitmap.Save(ms, ext.Contains("png") ? ImageFormat.Png : ImageFormat.Jpeg);
                    bitmap.Dispose();
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
    }
}
