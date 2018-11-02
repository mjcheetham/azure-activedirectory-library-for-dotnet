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

using System;
using System.Threading.Tasks;

namespace Microsoft.Identity.Core.UI.Custom
{
    public interface ICustomWebUIFactory
    {
        ICustomWebUI CreateAuthenticationDialog(object parent, string clientId);
    }

    public interface ICustomWebUI
    {
        Task<CustomWebUIAuthorizationResult> AcquireAuthorizationAsync(Uri authorizationUri, Uri redirectUri, string clientId);
    }

    public class CustomWebUIAuthorizationResult
    {
        public CustomWebUIAuthorizationResult(CustomWebUIAuthorizationStatus status, string returnedUriInput) : this(status)
        {
            ReturnedUriInput = returnedUriInput;
        }

        public CustomWebUIAuthorizationResult(CustomWebUIAuthorizationStatus status)
        {
            Status = status;
        }

        internal CustomWebUIAuthorizationStatus Status { get; }

        internal string ReturnedUriInput { get; }
    }

    public enum CustomWebUIAuthorizationStatus
    {
        Success,
        ErrorHttp,
        ProtocolError,
        UserCancel,
        UnknownError
    }

    internal class CustomWebUIFactoryAdapter : IWebUIFactory
    {
        private readonly ICustomWebUIFactory _customFactory;

        public CustomWebUIFactoryAdapter(ICustomWebUIFactory customFactory)
        {
            _customFactory = customFactory;
        }

        public IWebUI CreateAuthenticationDialog(CoreUIParent coreUIParent, RequestContext requestContext)
        {
            var customWebUI = _customFactory.CreateAuthenticationDialog(null, requestContext.ClientId);
            return new CustomWebUIAdapter(customWebUI);
        }
    }

    internal class CustomWebUIAdapter : IWebUI
    {
        private readonly ICustomWebUI _customWebUI;

        public CustomWebUIAdapter(ICustomWebUI customWebUI)
        {
            _customWebUI = customWebUI;
        }

        public async Task<AuthorizationResult> AcquireAuthorizationAsync(Uri authorizationUri, Uri redirectUri, RequestContext requestContext)
        {
            var customResult = await _customWebUI.AcquireAuthorizationAsync(authorizationUri, redirectUri, requestContext.ClientId).ConfigureAwait(false);

            AuthorizationStatus status;
            switch (customResult.Status)
            {
                case CustomWebUIAuthorizationStatus.Success:
                    status = AuthorizationStatus.Success;
                    break;
                case CustomWebUIAuthorizationStatus.ErrorHttp:
                    status = AuthorizationStatus.ErrorHttp;
                    break;
                case CustomWebUIAuthorizationStatus.ProtocolError:
                    status = AuthorizationStatus.ProtocolError;
                    break;
                case CustomWebUIAuthorizationStatus.UserCancel:
                    status = AuthorizationStatus.UserCancel;
                    break;
                case CustomWebUIAuthorizationStatus.UnknownError:
                    status = AuthorizationStatus.UnknownError;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return new AuthorizationResult(status, customResult.ReturnedUriInput);
        }
    }
}
