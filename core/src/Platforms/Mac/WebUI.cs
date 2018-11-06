// ------------------------------------------------------------------------------
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
// ------------------------------------------------------------------------------

using AppKit;
using Foundation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Identity.Core.UI
{
    internal class WebUI : IWebUI
    {
        private SemaphoreSlim returnedUriReady;
        private AuthorizationResult authorizationResult;

        public CoreUIParent CoreUIParent { get; set; }
        public RequestContext RequestContext { get; set; }

        public async Task<AuthorizationResult> AcquireAuthorizationAsync(Uri authorizationUri, Uri redirectUri, RequestContext requestContext)
        {
            returnedUriReady = new SemaphoreSlim(0);

            Authenticate(authorizationUri, redirectUri, requestContext);

            await returnedUriReady.WaitAsync().ConfigureAwait(false);

            return authorizationResult;
        }

        public void SetAuthorizationResult(AuthorizationResult authorizationResultInput)
        {
            authorizationResult = authorizationResultInput;
            returnedUriReady.Release();
        }

        public void Authenticate(Uri authorizationUri, Uri redirectUri, RequestContext requestContext)
        {
            try
            {
                // Ensure we create the NSViewController on the main thread.
                // Consumers of our library must ensure they do not block the main thread
                // or else they will cause a deadlock.
                // For example calling `AcquireTokenAsync(...).Result` from the main thread
                // would result in this delegate never executing.
                NSApplication.SharedApplication.BeginInvokeOnMainThread(() =>
                {
                    var windowController = new AuthenticationAgentNSWindowController(authorizationUri.AbsoluteUri, redirectUri.OriginalString, SetAuthorizationResult);
                    windowController.Run(CoreUIParent.CallerWindow);
                });
            }
            catch (Exception ex)
            {
                throw CoreExceptionFactory.Instance.GetClientException(
                    CoreErrorCodes.AuthenticationUiFailed,
                    "See inner exception for details",
                    ex);
            }
        }
    }
}
