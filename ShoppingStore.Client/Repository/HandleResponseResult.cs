using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingStore.Model.Dtos;
using System.Net.Http;
using System.Text.Json;

namespace ShoppingStore.Client.Repository
{
    public static class HandleResponseResult
    {
        public static async Task<T> DeserializeData<T>(Stream contentStream)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return await System.Text.Json.JsonSerializer.DeserializeAsync<T>(contentStream, options);
        }

        public static async Task<IActionResult> HandleResult<T>(HttpResponseMessage httpResponseMessage, 
            Func<Stream, T, IActionResult> callbackSuccess,
            Func<string, IActionResult> callbackFail)
        {
            if (httpResponseMessage.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response => don't need try catch.
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var responseData = await DeserializeData<T>(contentStream);
                return callbackSuccess(contentStream, responseData);
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(httpResponseMessage.Content.ReadAsStringAsync().Result) ?? "Error Occurred";
                return callbackFail(errMsg);
            }
        }

        public static async Task<IViewComponentResult?> HandleResult<T>(HttpResponseMessage httpResponseMessage,
            Func<Stream, T, IViewComponentResult?> callbackSuccess,
            Func<string, IViewComponentResult?> callbackFail)
        {
            if (httpResponseMessage.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response => don't need try catch.
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var responseData = await DeserializeData<T>(contentStream);
                return callbackSuccess(contentStream, responseData);
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(httpResponseMessage.Content.ReadAsStringAsync().Result) ?? "Error Occurred";
                return callbackFail(errMsg);
            }
        }

        public static async Task<IViewComponentResult?> HandleResultTuple<T>(HttpResponseMessage httpResponseMessage,
            Func<Stream, T, IViewComponentResult?> callbackSuccess,
            Func<string, IViewComponentResult?> callbackFail)
        {
            if (httpResponseMessage.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response => don't need try catch.
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                //var responseData = await DeserializeData<T>(contentStream);
                var responseData = JsonConvert.DeserializeObject<T> (httpResponseMessage.Content.ReadAsStringAsync().Result);
                return callbackSuccess(contentStream, responseData);
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(httpResponseMessage.Content.ReadAsStringAsync().Result) ?? "Error Occurred";
                return callbackFail(errMsg);
            }
        }

        public static async Task<T1?> HandleResult<T, T1>(HttpResponseMessage httpResponseMessage,
            Func<Stream, T, T1?> callbackSuccess,
            Func<string, T1?> callbackFail)
        {
            if (httpResponseMessage.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response => don't need try catch.
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var responseData = await DeserializeData<T>(contentStream);
                return callbackSuccess(contentStream, responseData);
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(httpResponseMessage.Content.ReadAsStringAsync().Result) ?? "Error Occurred";
                return callbackFail(errMsg);
            }
        }

        //public static async Task<dynamic> HandleResult<T>(HttpResponseMessage httpResponseMessage,
        //         Func<Stream, T, dynamic> callbackSuccess,
        //         Func<string, dynamic> callbackFail)
        //{
        //    if (httpResponseMessage.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response => don't need try catch.
        //    {
        //        using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
        //        var responseData = await DeserializeData<T>(contentStream);
        //        return callbackSuccess(contentStream, responseData);
        //    }
        //    else
        //    {
        //        var errMsg = JsonConvert.DeserializeObject<dynamic>(httpResponseMessage.Content.ReadAsStringAsync().Result) ?? "Error Occurred";
        //        return callbackFail(errMsg);
        //    }
        //}

        public static async Task<IActionResult> HandleResultWithNoResponseData(HttpResponseMessage httpResponseMessage,
            Func<Stream, IActionResult> callbackSuccess,
            Func<string, IActionResult> callbackFail)
        {
            if (httpResponseMessage.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response => don't need try catch.
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                return callbackSuccess(contentStream);
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(httpResponseMessage.Content.ReadAsStringAsync().Result) ?? "Error Occurred";
                return callbackFail(errMsg);
            }
        }
    }
}
