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

namespace Microsoft.Identity.Core
{
    /// <summary>
    /// Factory for exceptions for Adal and Msal. Use the <see cref="Instance"/>
    /// singleton to access an actual implementation which will have been injected.
    /// </summary>
    internal interface ICoreExceptionFactory
    {
        Exception GetClientException(
            string errorCode,
            string errorMessage,
            Exception innerException = null);

        Exception GetServiceException(
            string errorCode,
            string errorMessage);

        Exception GetServiceException(
           string errorCode,
           string errorMessage,
           ExceptionDetail exceptionDetail = null);

        Exception GetServiceException(
           string errorCode,
           string errorMessage,
           Exception innerException = null,
           ExceptionDetail exceptionDetail = null);

        Exception GetUiRequiredException(
           string errorCode,
           string errorMessage,
           Exception innerException = null,
           ExceptionDetail exceptionDetail = null);

        string GetPiiScrubbedDetails(Exception exception);
    }

    // TODO: see if we can fully get rid of this, but we have to initialize this conditionally,
    // and can't pass it as a parameter to public classes.  Need to figure this out.
    internal static class InternalCoreExceptionFactory
    {
        private static ICoreExceptionFactory _coreExceptionFactory;

        public static void InitializeCoreExceptionFactory(ICoreExceptionFactory coreExceptionFactory)
        {
            _coreExceptionFactory = coreExceptionFactory;
        }

        public static ICoreExceptionFactory GetCoreExceptionFactory()
        {
            return _coreExceptionFactory;
        }
    }
}
