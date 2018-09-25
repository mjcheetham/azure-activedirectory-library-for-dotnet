//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.Identity.Core.Http
{
    // TODO: need to make this internal
    public interface IHttpClientFactory
    {
        HttpClient GetHttpClient();
    }

    internal class HttpClientFactory : IHttpClientFactory
    {
        // as per guidelines HttpClient should be a singeton instance in an application.
        private static HttpClient _client;
        private static readonly object LockObj = new object();
        private readonly bool _returnHttpClientForMocks;
        public const long MaxResponseContentBufferSizeInBytes = 1024*1024;

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient(HttpMessageHandlerFactory.GetMessageHandler(_returnHttpClientForMocks))
            {
                MaxResponseContentBufferSize = MaxResponseContentBufferSizeInBytes
            };

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }

        public HttpClientFactory(bool returnHttpClientForMocks = false)
        {
            _returnHttpClientForMocks = returnHttpClientForMocks;
        }

        public HttpClient GetHttpClient()
        {
            // we return a new instanceof httpclient beacause there
            // is no way to provide new http request message handler
            // for each request made and it makes mocking of network calls 
            // impossible. So to circumvent, we simply return new instance for
            // for mocking purposes.

            // TODO: since we're now abstracting httpclient out under IHttpManager, can we just get rid of this?
            if (_returnHttpClientForMocks)
            {
                return CreateHttpClient();
            }

            if (_client == null)
            {
                lock (LockObj)
                {
                    if (_client == null)
                    {
                        _client = CreateHttpClient();
                    }
                }
            }

            return _client;
        }
    }
}