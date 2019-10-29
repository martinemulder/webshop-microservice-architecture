using HalfWerk.BffWebshop.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace HalfWerk.BffWebshop.Test.Helpers
{
    [TestClass]
    public class JwtHelperTest
    {
        private delegate void TryGetValueCallback(string s, out StringValues sv);
        private const string jwtStringWithRolesGebruikertAndKlantje = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJoYW5zQGdtYWlsLmNvbSIsImp0aSI6ImZmYzU3MGRhLTc4OTItNGRiZS04OWE3LTJhOTM1MGE5YzU2OCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiMSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6WyJHZWJydWlrZXJ0IiwiS2xhbnRqZSJdLCJleHAiOjE1NDc4MTg4OTksImlzcyI6Imlzc3Vlci5vcmciLCJhdWQiOiJpc3N1ZXIub3JnIn0.-j_GCxxtqwgt6A5hnal7IOgZ0IwAWkgnB149Nr9eocc";

        [TestMethod]
        public void GetEmail_ShouldSucceed()
        {
            // Arrange
            StringValues headerDictionaryResult;
            var headerDictionaryMock = new Mock<IHeaderDictionary>(MockBehavior.Strict);
            headerDictionaryMock.Setup(x => x.TryGetValue("Authorization", out headerDictionaryResult)).Returns(true)
                .Callback(new TryGetValueCallback((string s, out StringValues sv) => sv = new StringValues("Bearer " + jwtStringWithRolesGebruikertAndKlantje)));

            var httpRequestMock = new Mock<HttpRequest>(MockBehavior.Strict);
            httpRequestMock.Setup(x => x.Headers).Returns(headerDictionaryMock.Object);

            var httpContextMock = new Mock<HttpContext>(MockBehavior.Strict);
            httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);
            
            // Act
            string email = new JwtHelper().GetEmail(httpContextMock.Object);

            // Assert
            headerDictionaryMock.VerifyAll();
            httpRequestMock.VerifyAll();
            httpContextMock.VerifyAll();
            Assert.IsNotNull(email);
            Assert.AreEqual("hans@gmail.com", email);
        }

        [TestMethod]
        public void GetEmail_ShouldReturnNull_OnEmptyInput()
        {
            // Arrange
            // Act
            string email = new JwtHelper().GetEmail(null);

            // Assert
            Assert.IsNull(email);
        }

        [TestMethod]
        public void GetEmail_ShouldThrowSecurityTokenException_When_InvalidToken()
        {
            // Arrange
            StringValues headerDictionaryResult;
            var headerDictionaryMock = new Mock<IHeaderDictionary>(MockBehavior.Strict);
            headerDictionaryMock.Setup(x => x.TryGetValue("Authorization", out headerDictionaryResult)).Returns(true)
                .Callback(new TryGetValueCallback((string s, out StringValues sv) => sv = new StringValues("Bearer " + "IVO HEEFT DE TOKEN OPGEGETEN")));

            var httpRequestMock = new Mock<HttpRequest>(MockBehavior.Strict);
            httpRequestMock.Setup(x => x.Headers).Returns(headerDictionaryMock.Object);

            var httpContextMock = new Mock<HttpContext>(MockBehavior.Strict);
            httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);

            // Act
            Action act = () =>
            {
                new JwtHelper().GetEmail(httpContextMock.Object);
            };

            // Assert 
            var exception = Assert.ThrowsException<SecurityTokenException>(act);
            Assert.AreEqual("Failed to retrieve JwtToken from HttpContext", exception.Message);
            headerDictionaryMock.VerifyAll();
            httpRequestMock.VerifyAll();
            httpContextMock.VerifyAll();
        }

        [TestMethod]
        public void GetEmail_ShouldThrowSecurityTokenException_When_NoAuthorization()
        {
            // Arrange
            var headerDictionaryMock = new Mock<IHeaderDictionary>(MockBehavior.Strict);

            var httpRequestMock = new Mock<HttpRequest>(MockBehavior.Strict);
            httpRequestMock.Setup(x => x.Headers).Returns(headerDictionaryMock.Object);

            var httpContextMock = new Mock<HttpContext>(MockBehavior.Strict);
            httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);

            // Act
            Action act = () =>
            {
                new JwtHelper().GetEmail(httpContextMock.Object);
            };

            // Assert
            var exception = Assert.ThrowsException<SecurityTokenException>(act);
            Assert.AreEqual("Failed to retrieve JwtToken from HttpContext", exception.Message);
            headerDictionaryMock.VerifyAll();
            httpRequestMock.VerifyAll();
            httpContextMock.VerifyAll();
        }
    }
}
