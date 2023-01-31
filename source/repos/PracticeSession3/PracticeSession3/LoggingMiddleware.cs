using Microsoft.IO;
using System.Diagnostics;

namespace PracticeSession3
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logFactory)
        {
            _next = next;

            _logger = logFactory;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Stopwatch st = new Stopwatch();
            st.Start();

            await LogRequest(httpContext);

            //  await _next(httpContext); // calling next middleware

            await LogResponse(httpContext);

            st.Stop();

            _logger.LogInformation($"\nURL:{httpContext.Request.Method}     http://{httpContext.Request.Host}/{httpContext.Request.Path} " +
                $"\nTime taken in API call is :{st.ElapsedMilliseconds} ms");
        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();
            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            _logger.LogInformation($"\n\nHttp Request Information:\n" +
                                   $"Schema:{context.Request.Scheme} \n" +
                                   $"Host: {context.Request.Host} \n" +
                                   $"Path: {context.Request.Path} \n" +
                                   $"QueryString: {context.Request.QueryString} \n" +
                                   $"Request method: {context.Request.Method}\n" +
                                   $"Request content: {context.Request.ContentType}\n" +
                                   $"Request Body: {ReadStreamInChunks(requestStream)}\n\n");
            context.Request.Body.Position = 0;
        }
        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;
            do
            {
                readChunkLength = reader.ReadBlock(readChunk,
                                                   0,
                                                   readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);
            return textWriter.ToString();
        }

        private async Task LogResponse(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;
            await _next(context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            _logger.LogInformation($"\n\nHttp Response Information:\n" +
                                   $"Schema:{context.Request.Scheme} \n" +
                                   $"Host: {context.Request.Host} \n" +
                                   $"Path: {context.Request.Path} \n" +
                                   $"response content type: {context.Response.ContentType} \n" +
                                   $"Response status code: {context.Response.StatusCode} \n" +
                                   $"Response Headers: {context.Response.Headers} \n" +
                                   $"QueryString: {context.Request.QueryString} \n" +
                                   $"Response Body: {text}\n");
            await responseBody.CopyToAsync(originalBodyStream);
        }

    }
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MyMiddlewareExtensions
    {
        public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
