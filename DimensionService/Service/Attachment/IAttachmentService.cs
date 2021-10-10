using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DimensionService.Service.Attachment
{
    public interface IAttachmentService
    {
        bool UploadAttachment(IFormFile file, string fileName);
        FileResult GetAttachments(string fileName, int height);
        FileResult GetHeadPortraits(string userID, int height);
    }
}
