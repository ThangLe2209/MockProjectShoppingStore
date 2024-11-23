using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.DataProtection;

namespace ShoppingStore.API.Services
{
	public class CloudinaryImageUploadService : IImageUploadService
	{
		private readonly Cloudinary _cloudinary;
		private readonly IConfiguration Configuration;

		public CloudinaryImageUploadService(IConfiguration configuration)
		{
			Configuration = configuration;
			var account = new Account(Configuration["CloudinaryConnection:Cloud"], 
				Configuration["CloudinaryConnection:ApiKey"], Configuration["CloudinaryConnection:ApiSecret"]);
			_cloudinary = new Cloudinary(account);
			_cloudinary.Api.Secure = true;
		}

		public async Task<List<string>> Upload(ICollection<IFormFile> files)
		{
			var imageUrls = new List<string>();

			foreach (var file in files)
			{
				using var memoryStream = new MemoryStream();
				await file.CopyToAsync(memoryStream);
				memoryStream.Position = 0;

				var uploadparams = new ImageUploadParams
				{
					File = new FileDescription(file.FileName, memoryStream),
				};

				var result = _cloudinary.Upload(uploadparams);

				if (result.Error != null)
				{
					throw new Exception($"Cloudinary error occured: {result.Error.Message}");
				}

				imageUrls.Add(result.SecureUrl.ToString());
			}

			return imageUrls;
		}

		public async Task<string> Upload(IFormFile file)
		{
			using var memoryStream = new MemoryStream();
			await file.CopyToAsync(memoryStream);
			memoryStream.Position = 0;

			var uploadparams = new ImageUploadParams
			{
				File = new FileDescription(file.FileName, memoryStream),
			};

			var result = _cloudinary.Upload(uploadparams);

			if (result.Error != null)
			{
				throw new Exception($"Cloudinary error occured: {result.Error.Message}");
			}
			return result.SecureUrl.ToString();
		}
	}
}
