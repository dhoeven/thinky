﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Tests.Data;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.AuthorizationServer.OAuth2;

namespace Thinktecture.AuthorizationServer.Test
{
    [TestClass]
    public class AuthorizeRequest_Validation_General
    {
        IAuthorizationServerConfiguration _testConfig;
        TestSmiIdentityConfiguration _testSmi;

        [TestInitialize]
        public void Init()
        {
            DataProtectection.Instance = new NoProtection();

            _testConfig = new TestAuthorizationServerConfiguration();
            _testSmi = new TestSmiIdentityConfiguration();
        }

        [TestMethod]
        public void UnknownApplication()
        {
            var controller = new AuthorizeController(null, _testConfig, null, _testSmi);
            var result = controller.Index("unknown", null);

            Assert.IsTrue(result is HttpNotFoundResult);
        }

        [TestMethod]
        public void NoParameters()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");

            try
            {
                var result = validator.Validate(app, new List<IdentityMembership>(), null);
            }
            catch (AuthorizeRequestResourceOwnerException ex)
            {
                // todo: inspect exception
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void MissingRedirectUri()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "codeclient",
                response_type = "code",
                scope = "read"
            };

            try
            {
                var result = validator.Validate(app, new List<IdentityMembership>(), request);
            }
            catch (AuthorizeRequestResourceOwnerException ex)
            {
                // todo: check error code
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void MalformedRedirectUri1()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "codeclient",
                response_type = "code",
                scope = "read",
                redirect_uri = "https:/prod.local"
            };

            try
            {
                var result = validator.Validate(app, new List<IdentityMembership>(), request);
            }
            catch (AuthorizeRequestResourceOwnerException ex)
            {
                // todo: check error code
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void MalformedRedirectUri2()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "codeclient",
                response_type = "code",
                scope = "read",
                redirect_uri = "malformed"
            };

            try
            {
                var result = validator.Validate(app, new List<IdentityMembership>(), request);
            }
            catch (AuthorizeRequestResourceOwnerException ex)
            {
                // todo: check error code
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void MissingResponseType()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "codeclient",
                scope = "read",
                redirect_uri = "https://prod.local"
            };

            try
            {
                var result = validator.Validate(app, new List<IdentityMembership>(), request);
            }
            catch (AuthorizeRequestClientException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidRequest, ex.Error);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void UnsupportedResponseType()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "codeclient",
                response_type = "unsupported",
                scope = "read",
                redirect_uri = "https://prod.local"
            };

            try
            {
                var result = validator.Validate(app, new List<IdentityMembership>(), request);
            }
            catch (AuthorizeRequestClientException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.UnsupportedResponseType, ex.Error);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void MissingClientId()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                response_type = "code",
                scope = "read",
                redirect_uri = "https://prod.local"
            };

            try
            {
                var result = validator.Validate(app, new List<IdentityMembership>(), request);
            }
            catch (AuthorizeRequestResourceOwnerException ex)
            {
                // todo: check error code
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void UnknownClientId()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "unknown",
                response_type = "code",
                scope = "read",
                redirect_uri = "https://prod.local"
            };

            try
            {
                var result = validator.Validate(app, new List<IdentityMembership>(), request);
            }
            catch (AuthorizeRequestResourceOwnerException ex)
            {
                // todo: check error code
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void MissingScope()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "codeclient",
                response_type = "code",
                redirect_uri = "https://prod.local"
            };

            try
            {
                var result = validator.Validate(app, new List<IdentityMembership>(), request);
            }
            catch (AuthorizeRequestClientException ex)
            {
                Assert.IsTrue(ex.Error == OAuthConstants.Errors.InvalidScope);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void NonSslRedirectUri()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "codeclient",
                response_type = "code",
                scope = "read",
                redirect_uri = "http://prod.local"
            };

            try
            {
                var result = validator.Validate(app, new List<IdentityMembership>(), request);
            }
            catch (AuthorizeRequestClientException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidRequest, ex.Error);
                return;
            }

            Assert.Fail("No exception thrown.");
        }
    }
}
