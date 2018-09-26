//----------------------------------------------------------------------
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

using System;
using Microsoft.Identity.Core;

namespace Test.Microsoft.Identity.Core.Unit.Mocks
{
    internal class TestExceptionFactory : ICoreExceptionFactory
    {
        public Exception GetClientException(string errorCode, string errorMessage, Exception innerException = null)
        {
            return new TestClientException(errorCode, errorMessage, innerException);
        }

        public string GetPiiScrubbedDetails(Exception exception)
        {
            return exception.ToString();
        }

        public Exception GetServiceException(string errorCode, string errorMessage)
        {
            return GetServiceException(errorCode, errorMessage, null);
        }

        public Exception GetServiceException(
            string errorCode,
            string errorMessage,
            ExceptionDetail exceptionDetail = null)
        {
            return GetServiceException(errorCode, errorMessage, null, null);
        }

        public Exception GetServiceException(
            string errorCode,
            string errorMessage,
            Exception innerException = null,
            ExceptionDetail exceptionDetail = null)
        {
            return new TestServiceException(errorCode, errorMessage, innerException)
            {
                Claims = exceptionDetail?.Claims,
                StatusCode = exceptionDetail != null ? exceptionDetail.StatusCode : 0,
                ResponseBody = exceptionDetail?.ResponseBody,
                IsUiRequired = false
            };
        }

        public Exception GetUiRequiredException(string errorCode, string errorMessage, Exception innerException = null, ExceptionDetail exceptionDetail = null)
        {
            return new TestServiceException(errorCode, errorMessage, innerException)
            {
                Claims = exceptionDetail?.Claims,
                StatusCode = exceptionDetail != null ? exceptionDetail.StatusCode : 0,
                ResponseBody = exceptionDetail?.ResponseBody,
                IsUiRequired = true
            };
        }
    }
}
