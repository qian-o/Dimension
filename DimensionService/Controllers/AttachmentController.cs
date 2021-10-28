using DimensionService.Common;
using DimensionService.Filter.Authorized;
using DimensionService.Models;
using DimensionService.Service.Attachment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DimensionService.Controllers
{
    [ApiVersionNeutral]
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizedActionFilter]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService _attachment;

        public AttachmentController(IAttachmentService attachment)
        {
            _attachment = attachment;
        }

        /// <summary>
        /// 上传附件
        /// </summary>
        /// <param name="file">附件</param>
        /// <returns></returns>
        [Route("UploadAttachment")]
        [HttpPost]
        public async Task<WebResultModel> UploadAttachment(IFormFile file)
        {
            string fileName = $"{ClassHelper.GetRandomString(10)}_{file.FileName}";
            WebResultModel webResult = new()
            {
                State = await _attachment.UploadAttachment(file, fileName),
                Data = fileName,
                Message = "上传成功"
            };
            return webResult;
        }

        /// <summary>
        /// 获取附件
        /// </summary>
        /// <param name="fileName">附件名</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        [Route("GetAttachments/{fileName?}")]
        [HttpGet]
        public FileResult GetAttachments(string fileName, int height)
        {
            return _attachment.GetAttachments(fileName, height);
        }

        /// <summary>
        /// 根据用户ID获取头像
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        [Route("GetHeadPortraits/{userID?}")]
        [HttpGet]
        public FileResult GetHeadPortraits(string userID, int height)
        {
            return _attachment.GetHeadPortraits(userID, height);
        }
    }
}
