using HalfWerk.BffWebshop.Controllers;
using HalfWerk.BffWebshop.DataMapper;
using HalfWerk.BffWebshop.Helpers;
using HalfWerk.CommonModels.AuthenticationService;
using HalfWerk.CommonModels.AuthenticationService.Models;
using HalfWerk.CommonModels.BffWebshop;
using HalfWerk.CommonModels.BffWebshop.KlantBeheer;
using HalfWerk.CommonModels.DsKlantBeheer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn.WebScale.Commands;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HalfWerk.BffWebshop.Test.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        private AccountController _target;
        private Mock<ICommandPublisher> _commandPublisherMock;
        private Mock<IJwtHelper> _jwtHelperMock;
        private Mock<IKlantDataMapper> _klantDataMapperMock;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            ConfigAutoMapper.Initialize();
        }

        [TestInitialize]
        public void BeforeEach()
        {
            _jwtHelperMock = new Mock<IJwtHelper>(MockBehavior.Strict);
            _commandPublisherMock = new Mock<ICommandPublisher>(MockBehavior.Strict);
            _klantDataMapperMock = new Mock<IKlantDataMapper>(MockBehavior.Strict);
            _target = new AccountController(_commandPublisherMock.Object, new LoggerFactory(), _jwtHelperMock.Object, _klantDataMapperMock.Object);
        }

        [TestMethod]
        public void Index_ShouldReturnKlant()
        {
            // Arrange
            _jwtHelperMock.Setup(j => j.GetEmail(null)).Returns("klant");
            _klantDataMapperMock.Setup(k => k.GetByEmail("klant"))
                                .Returns(
                                    new Klant()
                                    {
                                        Id = 1,
                                        Achternaam = "de Koning",
                                        Email = "Kees@dekoning.nl",
                                        Telefoonnummer = "0685458565",
                                        Voornaam = "Kees",
                                        Adres = new Adres()
                                        {
                                            Id = 1,
                                            Huisnummer = "3a",
                                            Land = "Nederland",
                                            Plaats = "Everdingen",
                                            Postcode = "4121 EV",
                                            Straatnaam = "Oranjestraat"
                                        }
                                    });

            // Act
            var result = _target.Index();
            
            // Assert
            _commandPublisherMock.VerifyAll();
            Assert.IsNotNull(result);
            var klant = result.Value as Klant;

            Assert.AreEqual(1, klant.Id);
            Assert.AreEqual("de Koning", klant.Achternaam);
            Assert.AreEqual("Kees@dekoning.nl", klant.Email);
            Assert.AreEqual("0685458565", klant.Telefoonnummer);
            Assert.AreEqual("Kees", klant.Voornaam);

            Assert.AreEqual(1, klant.Adres.Id);
            Assert.AreEqual("3a", klant.Adres.Huisnummer);
            Assert.AreEqual("Nederland", klant.Adres.Land);
            Assert.AreEqual("Everdingen", klant.Adres.Plaats);
            Assert.AreEqual("4121 EV", klant.Adres.Postcode);
            Assert.AreEqual("Oranjestraat", klant.Adres.Straatnaam);
        }

        [TestMethod]
        public void AllRoles_ShouldReturnListWithAllRoles()
        {
            // Act
            var result = _target.AllRoles();

            // Assert
            _commandPublisherMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            IEnumerable<Roles> list1 = (Roles[])Enum.GetValues(typeof(Roles));
            var list2 = (IEnumerable<string>)((OkObjectResult)result.Result).Value;

            list1 = list1.OrderBy(x => x.ToString());
            list2 = list2.OrderBy(x => x);

            Assert.AreEqual(list1.Count(), list2.Count());

            for(int i = 0; i < list1.Count(); ++i)
            {
                Assert.AreEqual(list1.ElementAt(i).ToString(), list2.ElementAt(i));
            }
        }

        [TestMethod]
        public void Register_ShouldReturnBadRequest_When_InvalidPostParametersGiven()
        {
            // Act

            var result = _target.Register(new KlantRegistration()).Result;


            // Assert
            _commandPublisherMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void Register_ShouldReturnOk()
        {
            // Arrange
            _commandPublisherMock.Setup(x => x.Publish<RegisterResult>(It.IsAny<RegisterCommand>())).Returns(Task.FromResult(RegisterResult.Ok));
            _commandPublisherMock.Setup(x => x.Publish<long>(It.IsAny<VoegKlantToeCommand>())).Returns(Task.FromResult<long>(1));

            // Act
            var result = _target.Register(new KlantRegistration() { Email = "a@gmail.com", Password = "321" }).Result;


            // Assert
            _commandPublisherMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void Register_ShouldThrowTimeoutException_WhenKlantCommandThrowsTimeout()
        {
            // Arrange
            _commandPublisherMock.Setup(x => x.Publish<RegisterResult>(It.IsAny<RegisterCommand>())).Returns(Task.FromResult(RegisterResult.Ok));
            _commandPublisherMock.Setup(x => x.Publish<long>(It.IsAny<VoegKlantToeCommand>())).ThrowsAsync(new TimeoutException());

            // Act
            var result = _target.Register(new KlantRegistration() { Email = "a@gmail.com", Password = "321" }).Result;

            // Assert
            _commandPublisherMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(408, objectResult.StatusCode);
        }

        [TestMethod]
        public void Register_ShouldThrowException_WhenKlantCommandThrowsUnknownException()
        {
            // Arrange
            _commandPublisherMock.Setup(x => x.Publish<RegisterResult>(It.IsAny<RegisterCommand>())).Returns(Task.FromResult(RegisterResult.Ok));
            _commandPublisherMock.Setup(x => x.Publish<long>(It.IsAny<VoegKlantToeCommand>())).Throws<Exception>();

            // Act
            var result = _target.Register(new KlantRegistration() { Email = "a@gmail.com", Password = "321" }).Result;

            // Assert
            _commandPublisherMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(400, objectResult.StatusCode);
        }

        [TestMethod]
        public void Register_ShouldReturnConflict_When_RegisterResultNotOk()
        {
            // Arrange
            _commandPublisherMock.Setup(x => x.Publish<RegisterResult>(It.IsAny<RegisterCommand>())).Returns(Task.FromResult(RegisterResult.Unknown));

            // Act
            var result = _target.Register(new KlantRegistration() { Email = "a@gmail.com", Password = "321" }).Result;

            // Assert
            _commandPublisherMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ConflictObjectResult));
        }

        [TestMethod]
        public void Register_ShouldReturnBadRequest_On_Exception()
        {
            // Arrange
            _commandPublisherMock.Setup(x => x.Publish<RegisterResult>(It.IsAny<RegisterCommand>())).Throws<Exception>();

            // Act
            var result = _target.Register(new KlantRegistration() { Email = "a@gmail.com", Password = "321" }).Result;

            // Assert
            _commandPublisherMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void Register_ShouldReturnTimeout_When_PublishThrowsTimeoutException()
        {
            // Arrange
            _commandPublisherMock.Setup(x => x.Publish<RegisterResult>(It.IsAny<RegisterCommand>())).Throws<TimeoutException>();

            // Act
            var result = _target.Register(new KlantRegistration() { Email = "a@gmail.com", Password = "321" }).Result;

            // Assert
            _commandPublisherMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var requestResult = result as ObjectResult;
            Assert.IsNotNull(requestResult);
            Assert.AreEqual(408, requestResult.StatusCode);
        }

        [TestMethod]
        public void Login_ShouldReturnBadRequest_When_InvalidPostParametersGiven()
        {
            // Act
            var result = _target.Login(new Credential()).Result;

            // Assert
            _commandPublisherMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void Login_ShouldReturnUnauthorized_When_NoJWTReceivedAsCommandResult()
        {
            // Arrange
            _commandPublisherMock.Setup(x => x.Publish<string>(It.IsAny<LoginCommand>())).Returns(Task.FromResult(""));

            // Act
            var result = _target.Login(new Credential() { Email = "a@mail.com", Password = "123" }).Result;

            // Assert
            _commandPublisherMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void Login_ShouldReturnOk_When_TokenReceived()
        {
            // Arrange
            _commandPublisherMock.Setup(x => x.Publish<string>(It.IsAny<LoginCommand>())).Returns(Task.FromResult("Token"));

            // Act
            var result = _target.Login(new Credential() { Email = "a@mail.com", Password = "123" }).Result;

            // Assert
            _commandPublisherMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void Login_ShouldReturnBadRequest_When_PublishThrowsException()
        {
            // Arrange
            _commandPublisherMock.Setup(x => x.Publish<string>(It.IsAny<LoginCommand>())).Throws<Exception>();

            // Act
            var result = _target.Login(new Credential() { Email = "a@mail.com", Password = "123" }).Result;
            
            // Assert
            _commandPublisherMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void Login_ShouldReturnTimeout_When_PublishThrowsTimeoutException()
        {
            // Arrange
            _commandPublisherMock.Setup(x => x.Publish<string>(It.IsAny<LoginCommand>())).Throws<TimeoutException>();

            // Act
            var result = _target.Login(new Credential() { Email = "a@mail.com", Password = "123" }).Result;

            // Assert
            _commandPublisherMock.VerifyAll();
            var requestResult = result.Result as ObjectResult;
            Assert.IsNotNull(requestResult);
            Assert.AreEqual(408, requestResult.StatusCode);
        }

        [TestMethod]
        public void Validate_ShouldReturnOk()
        {
            // Arrange
            _commandPublisherMock.Setup(x => x.Publish<bool>(It.IsAny<ValidateCommand>())).Returns(Task.FromResult(true));

            // Act
            var result = _target.Validate(new JwtToken() { Token = "Token" }).Result;

            // Assert
            _commandPublisherMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void Validate_ShouldReturnUnauthorized_When_NoValidTokenProvided()
        {
            // Act
            var result = _target.Validate(null).Result;

            // Assert
            _commandPublisherMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void Validate_ShouldReturnUnauthorized_When_PublisherReturnsFalse()
        {
            // Arrange
            _commandPublisherMock.Setup(x => x.Publish<bool>(It.IsAny<ValidateCommand>())).Returns(Task.FromResult(false));

            // Act
            var result = _target.Validate(new JwtToken() { Token = "Token" }).Result;

            // Assert
            _commandPublisherMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void Validate_ShouldReturnTimeout_When_PublishThrowTimeoutException()
        {
            // Arrange
            _commandPublisherMock.Setup(x => x.Publish<bool>(It.IsAny<ValidateCommand>())).Throws<TimeoutException>();

            // Act
            var result = _target.Validate(new JwtToken() { Token = "Token" }).Result;

            // Assert
            _commandPublisherMock.VerifyAll();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var requestResult = result as ObjectResult;
            Assert.IsNotNull(requestResult);
            Assert.AreEqual(408, requestResult.StatusCode);
        }
    }
}
