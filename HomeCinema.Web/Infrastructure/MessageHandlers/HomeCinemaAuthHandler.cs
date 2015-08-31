using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HomeCinema.Services;
using HomeCinema.Web.Infrastructure.Extensions;

namespace HomeCinema.Web.Infrastructure.MessageHandlers
{
    public class HomeCinemaAuthHandler : DelegatingHandler
    {
        private IEnumerable<string> _authHeaderValues;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                request.Headers.TryGetValues("Authorization", out _authHeaderValues);
                if (_authHeaderValues == null)
                {
                    return await base.SendAsync(request, cancellationToken);
                }

                var tokens = _authHeaderValues.FirstOrDefault();
                if (tokens == null)
                {
                    return CreateForbiddenResponse();
                }

                tokens = tokens.Replace("Basic", "").Trim();
                if (string.IsNullOrEmpty(tokens))
                {
                    return CreateForbiddenResponse();
                }

                var membershipCtx = await GetMembershipContextAsync(request, tokens);
                if (membershipCtx.User == null)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    return response;
                }

                InitializeCurrentUser(membershipCtx);
                return await base.SendAsync(request, cancellationToken);
            }

            catch
            {
                return CreateForbiddenResponse();
            }
        }

        private static void InitializeCurrentUser(MembershipContext membershipCtx)
        {
            var principal = membershipCtx.Principal;
            Thread.CurrentPrincipal = principal;
            HttpContext.Current.User = principal;
        }

        private static async Task<MembershipContext> GetMembershipContextAsync(HttpRequestMessage request, string tokens)
        {
            var data = Convert.FromBase64String(tokens);
            var decodedString = Encoding.UTF8.GetString(data);
            var tokensValues = decodedString.Split(':');
            var membershipService = request.GetMembershipService();
            var membershipCtx = await membershipService.ValidateUserAsync(tokensValues[0], tokensValues[1]);
            return membershipCtx;
        }

        private static HttpResponseMessage CreateForbiddenResponse()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            return response;
        }
    }
}