// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

#if WINRT || WINDOWS_UWP
using Windows.Storage;
#endif

namespace Microsoft.Toolkit.Services.MicrosoftGraph
{
    /// <summary>
    /// Authentication Helper Using Azure Active Directory v2.0 app Model
    /// </summary>
    internal class MicrosoftGraphAuthenticationHelper : IAuthenticationHelper
    {
        /// <summary>
        /// Base Url for service.
        /// </summary>
        private const string Authority = "https://login.microsoftonline.com/{0}/";

        /// <summary>
        /// Default Logout Url for V2
        /// </summary>
        private const string LogoutUrlV2Model = "https://login.microsoftonline.com/{0}/oauth2/v2.0/logout";

#if WINRT || WINDOWS_UWP

        private const string LogoutUrl = "https://login.microsoftonline.com/{0}/oauth2/logout";

        /// <summary>
        /// Storage key name for user name.
        /// </summary>
        private static readonly string STORAGEKEYUSER = "user";

#endif

        private readonly IAuthenticationHelper _authHelperImplementation;

#if WINRT || WINDOWS_UWP
        public MicrosoftGraphAuthenticationHelper(MicrosoftGraphEnums.AuthenticationModel authenticationModel, string clientId, string tenantId = "common")
        {
            ClientId = clientId;

            if (authenticationModel == MicrosoftGraphEnums.AuthenticationModel.V1)
            {
                _authHelperImplementation = new AuthenticationHelperV1(clientId, string.Format(Authority, tenantId), string.Format(LogoutUrl, tenantId));
            }
            else
            {
                _authHelperImplementation = new AuthenticationHelperV2(clientId, string.Format(Authority, tenantId), string.Format(LogoutUrlV2Model, tenantId));
            }
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftGraphAuthenticationHelper"/> class.
        /// </summary>
        public MicrosoftGraphAuthenticationHelper(string clientId, string tenantId = "common")
        {
            ClientId = clientId;
            _authHelperImplementation = new AuthenticationHelperV2(clientId, string.Format(Authority, tenantId), string.Format(LogoutUrlV2Model, tenantId));
        }

        /// <summary>
        /// Gets ClientId for the application
        /// </summary>
        public string ClientId { get; }

        public Task<string> AquireTokenAsync(IEnumerable<string> scopes, UIParent uiParent = null, string redirectUri = null, string loginHint = null)
        {
            return _authHelperImplementation.AquireTokenAsync(scopes, uiParent, redirectUri, loginHint);
        }

        /// <summary>
        /// Logout the user
        /// </summary>
        /// <returns>Success or failure</returns>
        public async Task<bool> LogoutAsync()
        {
#if WINRT || WINDOWS_UWP
            ApplicationData.Current.LocalSettings.Values[STORAGEKEYUSER] = null;
#endif

            return await _authHelperImplementation.LogoutAsync().ConfigureAwait(false);
        }

        public Task<string> AquireTokenAsync(string resourceId, UIParent uiParent = null, string redirectUri = null, string loginHint = null)
        {
            return AquireTokenAsync(new[] { resourceId }, uiParent, redirectUri, loginHint);
        }
    }
}
