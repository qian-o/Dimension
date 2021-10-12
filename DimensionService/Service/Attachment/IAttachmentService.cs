using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DimensionService.Service.Attachment
{
    public interface IAttachmentService
    {
        Task<bool> UploadAttachment(IFormFile file, string fileName);
        FileResult GetAttachments(string fileName, int height);
        FileResult GetHeadPortraits(string userID, int height);
    }
}
