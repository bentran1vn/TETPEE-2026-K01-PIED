using Microsoft.AspNetCore.Http;

namespace TetPee.Service.MediaService;

public interface IService
{
    public Task<string> UploadImageAsync(IFormFile file);
}