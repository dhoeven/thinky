using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.AuthorizationServer.OAuth2;

namespace Thinktecture.AuthorizationServer.Test
{

    [TestClass]
    public class AuthorizeRequest_Validation_Context
    {

        IAuthorizationServerConfiguration _testConfig;

        [TestInitialize]
        public void Init()
        {
            DataProtectection.Instance = new NoProtection();

            _testConfig = new TestAuthorizationServerConfiguration();
        }

        [TestMethod]
        public void NeedsGranted()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "implicitclient",
                response_type = "token",
                scope = "needs",
                redirect_uri = "https://test2.local",
                context = "11234"
            };
            var memberships = new List<IdentityMembership>();
            memberships.Add(new IdentityMembership(){CanAccessNeeds = true, MembershipID = 11234});

            var result = validator.Validate(app, new List<IdentityMembership>(), request);
            Assert.AreEqual("11234", result.context);
        }

        [TestMethod]
        public void NeedsDenied()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "implicitclient",
                response_type = "token",
                scope = "needs",
                redirect_uri = "https://test2.local",
                context = "11234"
            };
            var memberships = new List<IdentityMembership>();

            var result = validator.Validate(app, new List<IdentityMembership>(), request);
            Assert.AreEqual("11234", result.context);
            Assert.AreEqual(0, result.Scopes.Count);
        }
        
        [TestMethod]
        public void MembershipDenied()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "implicitclient",
                response_type = "token",
                scope = "membership",
                redirect_uri = "https://test2.local",
                context = "11234"
            };
            var memberships = new List<IdentityMembership>();

            var result = validator.Validate(app, new List<IdentityMembership>(), request);
            Assert.AreEqual("11234", result.context);
            Assert.AreEqual(0, result.Scopes.Count);
        }
    }
}
