using ShoppingStore.Model.Dtos;
using System.Net;

namespace ShoppingStore.Client.Repository
{
	public static class Utilities
	{
		public static IEnumerable<ProductDto> GetRelatedProduct(int skipPage, IEnumerable<ProductDto> resultData, ProductDto productsById)
		{
			return resultData
					.Where(p => p.Id != productsById.Id)
					.Skip(skipPage*3).Take(3).ToList();
		}

		public static bool URLExists(string url)
		{
			bool result = true;

			WebRequest webRequest = WebRequest.Create(url);
			webRequest.Timeout = 1200; // miliseconds
			webRequest.Method = "HEAD";

			try
			{
				webRequest.GetResponse();
			}
			catch
			{
				result = false;
			}

			return result;
		}
	}
}
