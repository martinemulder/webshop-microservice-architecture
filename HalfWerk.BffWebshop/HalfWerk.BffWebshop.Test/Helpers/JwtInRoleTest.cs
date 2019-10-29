using HalfWerk.BffWebshop.Helpers;
using HalfWerk.CommonModels;
using HalfWerk.CommonModels.AuthenticationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn.WebScale.Commands;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HalfWerk.BffWebshop.Test.Helpers
{
    [TestClass]
    public class JwtInRoleTest
    {
        private delegate void TryGetValueCallback(string s, out StringValues sv);
        private const string jwtStringWithRolesGebruikertAndKlantje = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJoYW5zQGdtYWlsLmNvbSIsImp0aSI6ImZmYzU3MGRhLTc4OTItNGRiZS04OWE3LTJhOTM1MGE5YzU2OCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiMSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6WyJHZWJydWlrZXJ0IiwiS2xhbnRqZSJdLCJleHAiOjE1NDc4MTg4OTksImlzcyI6Imlzc3Vlci5vcmciLCJhdWQiOiJpc3N1ZXIub3JnIn0.-j_GCxxtqwgt6A5hnal7IOgZ0IwAWkgnB149Nr9eocc";

        [TestInitialize]
        public void Init()
        {

        }

        [TestMethod]
        public void OnAuthorization_ShouldSucceed()
        {
            // Arrange
            ValidateCommand resultCommand = null;

            var commandPublisherMock = new Mock<ICommandPublisher>(MockBehavior.Strict);
            commandPublisherMock.Setup(x => x.Publish<bool>(It.IsAny<ValidateCommand>())).Returns(Task.FromResult(true))
                .Callback<ValidateCommand>(cmd =>
                {
                    resultCommand = cmd;
                });

            var serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(y => y == typeof(ICommandPublisher)))).Returns(commandPublisherMock.Object);
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(y => y == typeof(ILoggerFactory)))).Returns(new LoggerFactory());

            StringValues headerDictionaryResult;
            var headerDictionaryMock = new Mock<IHeaderDictionary>(MockBehavior.Strict);
            headerDictionaryMock.Setup(x => x.TryGetValue("Authorization", out headerDictionaryResult)).Returns(true)
                .Callback(new TryGetValueCallback((string s, out StringValues sv) => sv = new StringValues("Bearer " + jwtStringWithRolesGebruikertAndKlantje)));

            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(x => x.Headers).Returns(headerDictionaryMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.RequestServices).Returns(serviceProviderMock.Object);
            httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);

            // HttpContext isn't virtual; Can't mock AuthrizationFilterContex...
            var filterContext = new AuthorizationFilterContext(
                new ActionContext(
                    httpContextMock.Object,
                    new Microsoft.AspNetCore.Routing.RouteData(),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>()
            );

            var jwtInRole = new JwtInRole("Gebruikert", "Klantje");

            // Act
            jwtInRole.OnAuthorization(filterContext);

            // Assert
            commandPublisherMock.VerifyAll();
            serviceProviderMock.VerifyAll();
            headerDictionaryMock.VerifyAll();
            Assert.IsNotNull(resultCommand);
            Assert.AreEqual(jwtStringWithRolesGebruikertAndKlantje, resultCommand.JwtToken);
            Assert.AreEqual(NameConstants.AuthenticationServiceValidateCommand, resultCommand.RoutingKey);
        }

        [TestMethod]
        public void OnAuthorization_ShouldFail_When_NotHavingRequiredRoles()
        {
            // Arrange
            ValidateCommand resultCommand = null;

            var commandPublisherMock = new Mock<ICommandPublisher>(MockBehavior.Strict);
            var serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(y => y == typeof(ICommandPublisher)))).Returns(commandPublisherMock.Object);
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(y => y == typeof(ILoggerFactory)))).Returns(new LoggerFactory());

            StringValues headerDictionaryResult;
            var headerDictionaryMock = new Mock<IHeaderDictionary>(MockBehavior.Strict);
            headerDictionaryMock.Setup(x => x.TryGetValue("Authorization", out headerDictionaryResult)).Returns(true)
                .Callback(new TryGetValueCallback((string s, out StringValues sv) => sv = new StringValues("Bearer " + jwtStringWithRolesGebruikertAndKlantje)));

            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(x => x.Headers).Returns(headerDictionaryMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.RequestServices).Returns(serviceProviderMock.Object);
            httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);

            // HttpContext isn't virtual; Can't mock AuthrizationFilterContex...
            var filterContext = new AuthorizationFilterContext(
                new ActionContext(
                    httpContextMock.Object,
                    new Microsoft.AspNetCore.Routing.RouteData(),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>()
            );

            var jwtInRole = new JwtInRole("Patatboer", "Frietmeneer");

            // Act
            jwtInRole.OnAuthorization(filterContext);

            // Assert
            commandPublisherMock.VerifyAll();
            serviceProviderMock.VerifyAll();
            headerDictionaryMock.VerifyAll();
            Assert.IsNull(resultCommand);
            Assert.IsNotNull(filterContext.Result);
            Assert.IsInstanceOfType(filterContext.Result, typeof(StatusCodeResult));
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, ((StatusCodeResult)filterContext.Result).StatusCode);
        }

        [TestMethod]
        public void OnAuthorization_ShouldFail_When_MultipleAuthorizationHeaders()
        {
            // Arrange
            var commandPublisherMock = new Mock<ICommandPublisher>(MockBehavior.Strict);
            var serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(y => y == typeof(ICommandPublisher)))).Returns(commandPublisherMock.Object);
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(y => y == typeof(ILoggerFactory)))).Returns(new LoggerFactory());

            var strings = new string[2];
            strings[0] = "Val1";
            strings[1] = "Val1";

            StringValues headerDictionaryResult;
            var headerDictionaryMock = new Mock<IHeaderDictionary>(MockBehavior.Strict);
            headerDictionaryMock.Setup(x => x.TryGetValue("Authorization", out headerDictionaryResult)).Returns(true)
                .Callback(new TryGetValueCallback((string s, out StringValues sv) => sv = new StringValues(strings)));

            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(x => x.Headers).Returns(headerDictionaryMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.RequestServices).Returns(serviceProviderMock.Object);
            httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);

            // HttpContext isn't virtual; Can't mock AuthrizationFilterContex...
            var filterContext = new AuthorizationFilterContext(
                new ActionContext(
                    httpContextMock.Object,
                    new Microsoft.AspNetCore.Routing.RouteData(),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>()
            );

            var jwtInRole = new JwtInRole("Gebruikert", "Klantje");

            // Act
            jwtInRole.OnAuthorization(filterContext);

            // Assert
            commandPublisherMock.VerifyAll();
            serviceProviderMock.VerifyAll();
            headerDictionaryMock.VerifyAll();
            Assert.IsNotNull(filterContext.Result);
            Assert.IsInstanceOfType(filterContext.Result, typeof(StatusCodeResult));
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, ((StatusCodeResult)filterContext.Result).StatusCode);
        }

        [TestMethod]
        public void OnAuthorization_ShouldFail_When_CommandPublisherIsNull()
        {
            // Arrange
            var serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(y => y == typeof(ICommandPublisher)))).Returns(null);
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(y => y == typeof(ILoggerFactory)))).Returns(new LoggerFactory());

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.RequestServices).Returns(serviceProviderMock.Object);

            // HttpContext isn't virtual; Can't mock AuthrizationFilterContex...
            var filterContext = new AuthorizationFilterContext(
                new ActionContext(
                    httpContextMock.Object,
                    new Microsoft.AspNetCore.Routing.RouteData(),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>()
            );

            var jwtInRole = new JwtInRole("Gebruikert", "Klantje");

            // Act
            jwtInRole.OnAuthorization(filterContext);

            // Assert
            serviceProviderMock.VerifyAll();
            Assert.IsNotNull(filterContext.Result);
            Assert.IsInstanceOfType(filterContext.Result, typeof(StatusCodeResult));
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, ((StatusCodeResult)filterContext.Result).StatusCode);
        }

        [TestMethod]
        public void OnAuthorization_ShouldFail_When_CommandPublisherReturnsFalse()
        {
            // Arrange
            var commandPublisherMock = new Mock<ICommandPublisher>(MockBehavior.Strict);
            commandPublisherMock.Setup(x => x.Publish<bool>(It.IsAny<ValidateCommand>())).Returns(Task.FromResult(false));

            var serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(y => y == typeof(ICommandPublisher)))).Returns(commandPublisherMock.Object);
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(y => y == typeof(ILoggerFactory)))).Returns(new LoggerFactory());

            StringValues headerDictionaryResult;
            var headerDictionaryMock = new Mock<IHeaderDictionary>(MockBehavior.Strict);
            headerDictionaryMock.Setup(x => x.TryGetValue("Authorization", out headerDictionaryResult)).Returns(true)
                .Callback(new TryGetValueCallback((string s, out StringValues sv) => sv = new StringValues("Bearer " + jwtStringWithRolesGebruikertAndKlantje)));

            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(x => x.Headers).Returns(headerDictionaryMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.RequestServices).Returns(serviceProviderMock.Object);
            httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);

            // HttpContext isn't virtual; Can't mock AuthrizationFilterContex...
            var filterContext = new AuthorizationFilterContext(
                new ActionContext(
                    httpContextMock.Object,
                    new Microsoft.AspNetCore.Routing.RouteData(),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>()
            );

            var jwtInRole = new JwtInRole("Gebruikert", "Klantje");

            // Act
            jwtInRole.OnAuthorization(filterContext);

            // Assert
            commandPublisherMock.VerifyAll();
            serviceProviderMock.VerifyAll();
            headerDictionaryMock.VerifyAll();
            Assert.IsNotNull(filterContext.Result);
            Assert.IsInstanceOfType(filterContext.Result, typeof(StatusCodeResult));
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, ((StatusCodeResult)filterContext.Result).StatusCode);
        }

        [TestMethod]
        public void OnAuthorization_ShouldFail_When_CommandPublisherThrowsTimeoutException()
        {
            // Arrange
            var commandPublisherMock = new Mock<ICommandPublisher>(MockBehavior.Strict);
            commandPublisherMock.Setup(x => x.Publish<bool>(It.IsAny<ValidateCommand>())).Throws<TimeoutException>();

            var serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(y => y == typeof(ICommandPublisher)))).Returns(commandPublisherMock.Object);
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(y => y == typeof(ILoggerFactory)))).Returns(new LoggerFactory());

            StringValues headerDictionaryResult;
            var headerDictionaryMock = new Mock<IHeaderDictionary>(MockBehavior.Strict);
            headerDictionaryMock.Setup(x => x.TryGetValue("Authorization", out headerDictionaryResult)).Returns(true)
                .Callback(new TryGetValueCallback((string s, out StringValues sv) => sv = new StringValues("Bearer " + jwtStringWithRolesGebruikertAndKlantje)));

            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(x => x.Headers).Returns(headerDictionaryMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.RequestServices).Returns(serviceProviderMock.Object);
            httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);

            // HttpContext isn't virtual; Can't mock AuthrizationFilterContex...
            var filterContext = new AuthorizationFilterContext(
                new ActionContext(
                    httpContextMock.Object,
                    new Microsoft.AspNetCore.Routing.RouteData(),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>()
            );

            var jwtInRole = new JwtInRole("Gebruikert", "Klantje");

            // Act
            jwtInRole.OnAuthorization(filterContext);

            // Assert
            commandPublisherMock.VerifyAll();
            serviceProviderMock.VerifyAll();
            headerDictionaryMock.VerifyAll();
            Assert.IsNotNull(filterContext.Result);
            Assert.IsInstanceOfType(filterContext.Result, typeof(StatusCodeResult));
            Assert.AreEqual((int)HttpStatusCode.RequestTimeout, ((StatusCodeResult)filterContext.Result).StatusCode);
        }
    }
}
