using BlogCK.Entity.DTOs.Images;
using BlogCK.Entity.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCK.Service.Helpers.Images
{
    public interface IImageHelper
    {
        Task<UploadedImageDto> Upload(string name, IFormFile imageFile, ImageType imageType, string folderName = null);
        void Delete(string imageName);
    }
}
