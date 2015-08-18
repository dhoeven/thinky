/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.IdentityModel;

namespace Thinktecture.AuthorizationServer
{
    public class TokenService
    {
        GlobalConfiguration globalConfiguration;

        public TokenService(GlobalConfiguration globalConfiguration)
        {
            this.globalConfiguration = globalConfiguration;
        }

        public TokenService(IAuthorizationServerConfiguration authorizationServerConfiguration)
            : this(authorizationServerConfiguration.GlobalConfiguration)
        {
        }

        public virtual TokenResponse CreateTokenResponse(StoredGrant handle, IStoredGrantManager handleManager)
        {
            if (handle.Type == StoredGrantType.AuthorizationCode)
            {
                return CreateTokenResponseFromAuthorizationCode(handle, handleManager);
            }
            if (handle.Type == StoredGrantType.RefreshTokenIdentifier)
            {
                return CreateTokenResponseFromRefreshToken(handle, handleManager);
            }

            throw new ArgumentException("handle.Type");
        }

        public virtual TokenResponse CreateTokenResponseFromAuthorizationCode(StoredGrant handle, IStoredGrantManager handleManager)
        {
            var resourceOwner = Principal.Create(
                "OAuth2",
                handle.ResourceOwner.ToClaims().ToArray());

            var validatedRequest = new ValidatedRequest
            {
                Client = handle.Client,
                Application = handle.Application,
                Scopes = handle.Scopes
            };

            var response = CreateTokenResponse(validatedRequest, resourceOwner);

            if (handle.CreateRefreshToken)
            {
                var refreshTokenHandle = StoredGrant.CreateRefreshTokenHandle(
                    resourceOwner.GetSubject(),
                    handle.Client,
                    handle.Application,
                    resourceOwner.Claims,
                    handle.Scopes,
                    handle.RefreshTokenExpiration.Value);

                handleManager.Add(refreshTokenHandle);
                response.RefreshToken = refreshTokenHandle.GrantId;
            }
                
            handleManager.Delete(handle.GrantId);

            return response;
        }

        public virtual TokenResponse CreateTokenResponseFromRefreshToken(StoredGrant handle, IStoredGrantManager handleManager)
        {
            var resourceOwner = Principal.Create(
                "OAuth2",
                handle.ResourceOwner.ToClaims().ToArray());

            if (DateTime.UtcNow > handle.Expiration)
            {
                throw new InvalidOperationException("Refresh token has expired.");
            }

            var validatedRequest = new ValidatedRequest
            {
                Client = handle.Client,
                Application = handle.Application,
                Scopes = handle.Scopes,
            };

            var response = CreateTokenResponse(validatedRequest, resourceOwner);
            response.RefreshToken = handle.GrantId;
            
            return response;
        }

        public virtual TokenResponse CreateTokenResponse(ValidatedRequest request, ClaimsPrincipal resourceOwner = null)
        {
            try
            {
                var claims = CreateClaims(request, resourceOwner);
                var token = CreateToken(request, claims);

                return new TokenResponse
                {
                    AccessToken = WriteToken(token),
                    ExpiresIn = request.Application.TokenLifetime * 60,
                    TokenType = "Bearer"
                };
            }
            catch (Exception ex)
            {
                Tracing.Error(ex.ToString());
                throw;
            }
        }

        protected virtual string WriteToken(JwtSecurityToken token)
        {
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        protected virtual JwtSecurityToken CreateToken(ValidatedRequest request, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                issuer: globalConfiguration.Issuer,
                audience: request.Application.Audience,
                claims: claims,
                lifetime: new Lifetime(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(request.Application.TokenLifetime)),
                signingCredentials: request.Application.SigningCredentials);

            return token;
        }

        protected virtual IEnumerable<Claim> CreateClaims(ValidatedRequest request, ClaimsPrincipal resourceOwner = null)
        {
            var claims = new List<Claim>();

            claims.AddRange(CreateRequestClaims(request));

            if (resourceOwner != null)
            {
                claims.AddRange(CreateResourceOwnerClaims(resourceOwner));
            }

            return request.Impersonation == null ? claims : DoImpersonation(request, claims);
        }

        private List<Claim> DoImpersonation(ValidatedRequest request, List<Claim> claims)
        {
            var sub = claims.First(c => c.Type == "sub");
            ReplaceClaim("sub", request.Impersonation.impersonationId, claims);
            ReplaceClaim("email_verified", request.Impersonation.emailVerified.ToString(), claims);
            ReplaceClaim("nickname", request.Impersonation.nickname, claims);
            ReplaceClaim("is_locked_out", request.Impersonation.isLockedOut.ToString(), claims);
            ReplaceClaim("impersonator_id", sub.Value, claims);
            ReplaceClaim(ClaimTypes.Email, request.Impersonation.emailAddress, claims);

            ReplaceRoles(claims, request.Impersonation.roles);
            return claims;
        }

        private static void ReplaceRoles(List<Claim> claims, IList<string> roles)
        {
            claims.RemoveAll(c=>c.Type == ClaimTypes.Role);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        private void ReplaceClaim(string type, string newValue, List<Claim> claims)
        {
            var claim = claims.FirstOrDefault(c => c.Type == type);
            if (claim != null) claims.Remove(claim);
            claims.Add(new Claim(type, newValue));
        }

        protected virtual IEnumerable<Claim> CreateResourceOwnerClaims(ClaimsPrincipal resourceOwner)
        {
            return resourceOwner.FilterInternalClaims();
        }

        protected virtual List<Claim> CreateRequestClaims(ValidatedRequest request)
        {
            var claims = new List<Claim>
            {
                new Claim("client_id", request.Client.ClientId)
            };

            request.Scopes.ForEach(s => claims.Add(new Claim("scope", s.Name)));
            if (request.context != null)
            {
                claims.Add(new Claim("context", request.context));
            }

            claims.Add(new Claim("has_claimed_membership", request.HasClaimedMembership
                                                               ? true.ToString()
                                                               : false.ToString(), ClaimValueTypes.Boolean));

            return claims;
        }
    }
}
