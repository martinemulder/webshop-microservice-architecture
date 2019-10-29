using HalfWerk.BffWebshop.Controllers;
using HalfWerk.BffWebshop.DataMapper;
using HalfWerk.BffWebshop.Helpers;
using HalfWerk.BffWebshop.Test.Entities;
using HalfWerk.BffWebshop.Test.Entities.Bestellingen;
using HalfWerk.CommonModels;
using HalfWerk.CommonModels.BffWebshop.BestellingService;
using HalfWerk.CommonModels.CommonExceptions;
using HalfWerk.CommonModels.DsBestelService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HalfWerk.BffWebshop.Entities;
using Bestelling = HalfWerk.CommonModels.BffWebshop.BestellingService.Bestelling;
using BestelRegel = HalfWerk.CommonModels.BffWebshop.BestellingService.BestelRegel;
using BestelStatus = HalfWerk.CommonModels.BffWebshop.BestellingService.BestelStatus;
using BevestigdeBestelling = HalfWerk.CommonModels.BffWebshop.BestellingService.BevestigdeBestelling;
using ICommandPublisher = Minor.Nijn.WebScale.Commands.ICommandPublisher;
using InvalidUpdateException = HalfWerk.CommonModels.CommonExceptions.InvalidUpdateException;

namespace HalfWerk.BffWebshop.Test.Controllers
{
    [TestClass]
    public class BestellingControllerTest
    {
        private Mock<ICommandPublisher> _commandPublisherMock;
        private Mock<IBestellingDataMapper> _bestellingDataMapperMock;
        private Mock<IKlantDataMapper> _klantDataMapperMock;
        private Mock<IMagazijnSessionDataMapper> _magazijnSessionDataMapperMock;
        private Mock<IJwtHelper> _jwtHelperMock;

        private BestellingController _target;

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            ConfigAutoMapper.Initialize();
        }

        [TestInitialize]
        public void BeforeEach()
        {
            _commandPublisherMock = new Mock<ICommandPublisher>(MockBehavior.Strict);
            _bestellingDataMapperMock = new Mock<IBestellingDataMapper>(MockBehavior.Strict);
            _klantDataMapperMock = new Mock<IKlantDataMapper>(MockBehavior.Strict);
            _magazijnSessionDataMapperMock = new Mock<IMagazijnSessionDataMapper>(MockBehavior.Strict);
            _jwtHelperMock = new Mock<IJwtHelper>(MockBehavior.Strict);

            _target = new BestellingController(
                _commandPublisherMock.Object, 
                _bestellingDataMapperMock.Object,
                _klantDataMapperMock.Object,
                _magazijnSessionDataMapperMock.Object,
                _jwtHelperMock.Object,
                new LoggerFactory()
            );
        }

        [TestMethod]
        public async Task CreateBestelling_ShouldReturn201StatusCode()
        {
            // Arrange
            var klant = new KlantBuilder().SetDummy().SetId(10).SetEmail("test@test.nl").Create();

            var bestelling = new Bestelling()
            {
                ContactInfo = new ContactInfo(),
                Afleveradres = new Afleveradres(),
                BestelRegels = new List<BestelRegel>()
            };

            _jwtHelperMock.Setup(h => h.GetEmail(It.IsAny<HttpContext>())).Returns(klant.Email);
            _klantDataMapperMock.Setup(k => k.GetByEmail(klant.Email)).Returns(klant);

            _commandPublisherMock.Setup(repo => repo.Publish<long>(It.Is<PlaatsBestellingCommand>(c =>
                c.RoutingKey == NameConstants.BestelServicePlaatsBestellingCommandQueue
                && c.Bestelling.Klantnummer == klant.Id
            ))).ReturnsAsync(42);

            // Act
            var result = await _target.CreateBestelling(bestelling);

            // Assert
            _commandPublisherMock.VerifyAll();
            _klantDataMapperMock.VerifyAll();

            var createdResult = result as CreatedResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.IsInstanceOfType(createdResult.Value, typeof(long));
            Assert.AreEqual(42L, createdResult.Value);
        }

        [TestMethod]
        public async Task CreateBestelling_WithBestelRegels_ShouldReturn201StatusCode()
        {
            // Arrange
            var klant = new KlantBuilder().SetDummy().SetId(10).SetEmail("test@test.nl").Create();

            var bestelling = new Bestelling()
            {
                ContactInfo = new ContactInfo(),
                Afleveradres = new Afleveradres(),
                BestelRegels = new List<BestelRegel>()
                {
                    new BestelRegel()
                    {
                        Aantal = 1,
                        Artikelnummer = 1254
                    }
                }
            };

            _jwtHelperMock.Setup(h => h.GetEmail(It.IsAny<HttpContext>())).Returns(klant.Email);
            _klantDataMapperMock.Setup(k => k.GetByEmail(klant.Email)).Returns(klant);

            _commandPublisherMock.Setup(repo => repo.Publish<long>(It.Is<PlaatsBestellingCommand>(c => 
                c.Bestelling.BestelRegels.Any(r => r.Aantal == 1 && r.Artikelnummer == 1254)
            ))).ReturnsAsync(10);
          
            // Act
            var result = await _target.CreateBestelling(bestelling);

            // Assert
            _commandPublisherMock.VerifyAll();

            var createdResult = result as CreatedResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.AreEqual((long)10, createdResult.Value);
        }

        [TestMethod]
        public async Task CreateBestelling_ShouldReturn400StatusCode_WhenBestellingIsInvalid()
        {
            // Arrange
            var klant = new KlantBuilder().SetDummy().SetId(10).SetEmail("test@test.nl").Create();

            var bestelling = new Bestelling
            {
                ContactInfo = new ContactInfo(),
                Afleveradres = new Afleveradres(),
                BestelRegels = new List<BestelRegel>()
            };

            _jwtHelperMock.Setup(h => h.GetEmail(It.IsAny<HttpContext>())).Returns(klant.Email);
            _klantDataMapperMock.Setup(k => k.GetByEmail(klant.Email)).Returns(klant);

            _commandPublisherMock.Setup(repo => repo.Publish<long>(It.IsAny<PlaatsBestellingCommand>()))
                .ThrowsAsync(new DatabaseException("error"));

            // Act
            var result = await _target.CreateBestelling(bestelling);

            // Assert
            _commandPublisherMock.VerifyAll();

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Bestelling bevat incorrecte data", badRequestResult.Value);
        }

        [TestMethod]
        public async Task CreateBestelling_ShouldReturn408StatusCode_OnCommandRequestTimeout_InBestellingCommand()
        {
            // Arrange
            var klant = new KlantBuilder().SetDummy().SetId(10).SetEmail("test@test.nl").Create();

            var bestelling = new Bestelling
            {
                ContactInfo = new ContactInfo(),
                Afleveradres = new Afleveradres(),
                BestelRegels = new List<BestelRegel>()
            };

            _jwtHelperMock.Setup(h => h.GetEmail(It.IsAny<HttpContext>())).Returns(klant.Email);
            _klantDataMapperMock.Setup(k => k.GetByEmail(klant.Email)).Returns(klant);

            _commandPublisherMock.Setup(repo => repo.Publish<long>(It.IsAny<PlaatsBestellingCommand>()))
                .ThrowsAsync(new TimeoutException());

            // Act
            var result = await _target.CreateBestelling(bestelling);

            // Assert
            _commandPublisherMock.VerifyAll();

            var requestResult = result as ObjectResult;
            Assert.IsNotNull(requestResult);
            Assert.AreEqual(408, requestResult.StatusCode);
            Assert.AreEqual("Aanvraag kon niet worden verwerkt", requestResult.Value);
        }

        [TestMethod]
        public async Task CreateBestelling_ShouldReturn500StatusCode_WhenUnknownExceptionOccured_InBestellingCommand()
        {
            // Arrange
            var klant = new KlantBuilder().SetDummy().SetId(10).SetEmail("test@test.nl").Create();

            var bestelling = new Bestelling
            {
                ContactInfo = new ContactInfo(),
                Afleveradres = new Afleveradres(),
                BestelRegels = new List<BestelRegel>()
            };

            _jwtHelperMock.Setup(h => h.GetEmail(It.IsAny<HttpContext>())).Returns(klant.Email);
            _klantDataMapperMock.Setup(k => k.GetByEmail(klant.Email)).Returns(klant);

            _commandPublisherMock.Setup(repo => repo.Publish<long>(It.IsAny<PlaatsBestellingCommand>()))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _target.CreateBestelling(bestelling);

            // Assert
            _commandPublisherMock.VerifyAll();

            var requestResult = result as StatusCodeResult;
            Assert.IsNotNull(requestResult);
            Assert.AreEqual(500, requestResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateBestelling_ShouldSendAdres_WhenAdresIsFilled()
        {
            // Arrange
            var klant = new KlantBuilder().SetDummy().SetId(10).SetEmail("test@test.nl").Create();

            var bestelling = new Bestelling
            {
                ContactInfo = new ContactInfo(),
                Afleveradres = new Afleveradres(),
                BestelRegels = new List<BestelRegel>()
            };

            _jwtHelperMock.Setup(h => h.GetEmail(It.IsAny<HttpContext>())).Returns(klant.Email);
            _klantDataMapperMock.Setup(k => k.GetByEmail(klant.Email)).Returns(klant);

            _commandPublisherMock.Setup(repo => repo.Publish<long>(It.IsAny<PlaatsBestellingCommand>()))
                .ReturnsAsync(1);

            // Act
            var result = await _target.CreateBestelling(bestelling);

            // Assert
            _commandPublisherMock.VerifyAll();

            var createdResult = result as CreatedResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.AreEqual((long)1, createdResult.Value);
        }

        [TestMethod]
        public void GetBestelling_ShouldReturnBestellingen()
        {
            // Arrange
            BevestigdeBestellingBuilder builder = new BevestigdeBestellingBuilder();
            var bestellingen = new List<BevestigdeBestelling>
            {
                builder.SetDummy().Create(),
                builder.SetDummy().SetId(2).Create()
            };

            _bestellingDataMapperMock.Setup(mapper => mapper.GetAll()).Returns(bestellingen);

            // Act
            var result = _target.GetBestellingen().Value.ToList();

            // Assert
            _bestellingDataMapperMock.VerifyAll();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result[1].Id);
        }

        [TestMethod]
        public void GetBestelling_WithoutBestellingen_ShouldReturnEmptyList()
        {
            // Arrange
            _bestellingDataMapperMock.Setup(mapper => mapper.GetAll()).Returns(new List<BevestigdeBestelling>());

            // Act
            var result = _target.GetBestellingen().Value.ToList();

            // Assert
            _bestellingDataMapperMock.VerifyAll();
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetBestelling_WithoutBestellingen_ShouldReturnListOfBestellingenWithProvidedStatus()
        {
            // Arrange
            BevestigdeBestellingBuilder builder = new BevestigdeBestellingBuilder();
            var bestellingen = new List<BevestigdeBestelling>
            {
                builder.SetDummy().SetBestelStatus(BestelStatus.Goedgekeurd).Create(),
                builder.SetDummy().SetBestelStatus(BestelStatus.Geplaatst).Create(),
                builder.SetDummy().SetBestelStatus(BestelStatus.Afgekeurd).Create(),
                builder.SetDummy().SetBestelStatus(BestelStatus.Goedgekeurd).Create(),
            };

            _bestellingDataMapperMock.Setup(mapper => mapper.Find(It.IsAny<Expression<Func<BevestigdeBestelling, bool>>>()))
                .Returns<Expression<Func<BevestigdeBestelling, bool>>>(expr =>
                {
                    var query = expr.Compile();
                    return bestellingen.Where(query);
                });

            // Act
            var result = _target.GetBestellingen("Goedgekeurd").Value.ToList();

            // Assert
            _bestellingDataMapperMock.VerifyAll();
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task GetNextBestelling_ShouldCallBestellingDataMapperWithExpression()
        {
            // Arrange
            BevestigdeBestellingBuilder builder = new BevestigdeBestellingBuilder();
            var bestellingen = new List<BevestigdeBestelling>
            {
                builder.SetDummy()
                    .SetFactuurnummer(1)
                    .SetBesteldatum(new DateTime(2018, 11, 10))
                    .Create(),

                builder.SetDummy()
                    .SetFactuurnummer(2)
                    .SetBesteldatum(new DateTime(2018, 11, 11))
                    .Create(),
            };

            _jwtHelperMock.Setup(j => j.GetEmail(It.IsAny<HttpContext>())).Returns("email");
            _magazijnSessionDataMapperMock.Setup(m => m.Find(It.IsAny<Expression<Func<MagazijnSessionEntity, bool>>>()))
                .Returns(new List<MagazijnSessionEntity>());

            _magazijnSessionDataMapperMock.Setup(m => m.Insert(It.Is<MagazijnSessionEntity>(e =>
                e.MedewerkerEmail == "email" && e.Factuurnummer == 1
            ))).Returns(new MagazijnSessionEntity());

            _bestellingDataMapperMock.Setup(d => d.Find(It.IsAny<Expression<Func<BevestigdeBestelling, bool>>>()))
                .Returns(bestellingen);

            _commandPublisherMock.Setup(publisher => publisher.Publish<long>(It.IsAny<UpdateBestelStatusCommand>()))
                .ReturnsAsync(1);

            // Act
            var result = await _target.GetNextBestelling();

            // Assert
            _jwtHelperMock.VerifyAll();
            _magazijnSessionDataMapperMock.VerifyAll();
            _bestellingDataMapperMock.VerifyAll();
            _commandPublisherMock.VerifyAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(bestellingen.First(), result.Value);
        }

        [TestMethod]
        public async Task GetNextBestelling_WhenStatusUpdateFails_ShouldInternalServerErrorWhenBestelStatusUpdatedFailed()
        {
            // Arrange
            BevestigdeBestellingBuilder builder = new BevestigdeBestellingBuilder();
            var bestelling = builder.SetDummy().Create();

            _jwtHelperMock.Setup(j => j.GetEmail(It.IsAny<HttpContext>())).Returns("email");
            _magazijnSessionDataMapperMock.Setup(m => m.Find(It.IsAny<Expression<Func<MagazijnSessionEntity, bool>>>()))
                .Returns(new List<MagazijnSessionEntity>());

            _bestellingDataMapperMock.Setup(mapper => mapper.Find(It.IsAny<Expression<Func<BevestigdeBestelling, bool>>>()))
                .Returns(new List<BevestigdeBestelling>{ bestelling });

            _commandPublisherMock.Setup(publisher => publisher.Publish<long>(It.IsAny<UpdateBestelStatusCommand>()))
                .ThrowsAsync(new Exception());

            // Act
            ActionResult<BevestigdeBestelling> result = await _target.GetNextBestelling();

            // Assert
            _jwtHelperMock.VerifyAll();
            _magazijnSessionDataMapperMock.VerifyAll();
            _bestellingDataMapperMock.Verify(mapper => mapper.Find(It.IsAny<Expression<Func<BevestigdeBestelling, bool>>>()), Times.Once());
            _bestellingDataMapperMock.VerifyAll();

            Assert.IsNotNull(result);
            var res = (ObjectResult) result.Result;
            Assert.AreEqual(500, res.StatusCode);
        }

        [TestMethod]
        public async Task GetNextBestelling_WithoutBestellingen_ShouldReturnStatusCode204()
        {
            // Arrange
            _jwtHelperMock.Setup(j => j.GetEmail(It.IsAny<HttpContext>())).Returns("email");
            _magazijnSessionDataMapperMock.Setup(m => m.Find(It.IsAny<Expression<Func<MagazijnSessionEntity, bool>>>()))
                .Returns(new List<MagazijnSessionEntity>());

            _bestellingDataMapperMock.Setup(mapper => mapper.Find(It.IsAny<Expression<Func<BevestigdeBestelling, bool>>>()))
                .Returns(new List<BevestigdeBestelling>());

            // Act
            ActionResult<BevestigdeBestelling> result = await _target.GetNextBestelling();

            // Assert
            _jwtHelperMock.VerifyAll();
            _magazijnSessionDataMapperMock.VerifyAll();
            _bestellingDataMapperMock.VerifyAll();

            Assert.IsNotNull(result);
            var res = (NoContentResult)result.Result;
            Assert.AreEqual(204, res.StatusCode);
        }

        [TestMethod]
        public async Task GetNextBestelling_ShouldReturnStatusCode408_WhenUpdateBestelStatusCommandTimedOut()
        {
            // Arrange
            BevestigdeBestellingBuilder builder = new BevestigdeBestellingBuilder();
            var bestelling = builder.SetDummy().SetBestelStatus(BestelStatus.Geplaatst).Create();

            _jwtHelperMock.Setup(j => j.GetEmail(It.IsAny<HttpContext>())).Returns("email");
            _magazijnSessionDataMapperMock.Setup(m => m.Find(It.IsAny<Expression<Func<MagazijnSessionEntity, bool>>>()))
                .Returns(new List<MagazijnSessionEntity>());

            _bestellingDataMapperMock.Setup(mapper => mapper.Find(It.IsAny<Expression<Func<BevestigdeBestelling, bool>>>()))
                .Returns(new List<BevestigdeBestelling>() { bestelling });

            _commandPublisherMock.Setup(publisher => publisher.Publish<long>(It.IsAny<UpdateBestelStatusCommand>()))
                .ThrowsAsync(new TimeoutException());

            // Act
            ActionResult<BevestigdeBestelling> result = await _target.GetNextBestelling();

            // Assert
            _bestellingDataMapperMock.VerifyAll();

            Assert.IsNotNull(result);
            var res = (ObjectResult)result.Result;
            Assert.AreEqual(408, res.StatusCode);
        }


        [TestMethod]
        public async Task GetNextBestelling_ShouldReturnExistingBestelling_WhenSessionExists()
        {
            // Arrange
            BevestigdeBestellingBuilder builder = new BevestigdeBestellingBuilder();
            var bestellingen = new List<BevestigdeBestelling>
            {
                builder.SetDummy()
                    .SetFactuurnummer(1)
                    .SetBesteldatum(new DateTime(2018, 11, 10))
                    .Create(),

                builder.SetDummy()
                    .SetFactuurnummer(2)
                    .SetBesteldatum(new DateTime(2018, 11, 11))
                    .Create(),
            };

            var session = new MagazijnSessionEntity { MedewerkerEmail = "email", Factuurnummer = 1 };

            _jwtHelperMock.Setup(j => j.GetEmail(It.IsAny<HttpContext>())).Returns("email");
            _magazijnSessionDataMapperMock.Setup(m => m.Find(It.IsAny<Expression<Func<MagazijnSessionEntity, bool>>>()))
                .Returns(new List<MagazijnSessionEntity> { session });

            _bestellingDataMapperMock.Setup(d => d.GetByFactuurnummer(session.Factuurnummer))
                .Returns(bestellingen[0]);

            // Act
            var result = await _target.GetNextBestelling();

            // Assert
            _jwtHelperMock.VerifyAll();
            _magazijnSessionDataMapperMock.VerifyAll();
            _bestellingDataMapperMock.VerifyAll();
            _commandPublisherMock.Verify(publisher => publisher.Publish<long>(It.IsAny<UpdateBestelStatusCommand>()), Times.Never());

            Assert.IsNotNull(result);
            Assert.AreEqual(bestellingen[0], result.Value);
        }

        [TestMethod]
        public async Task UpdateBestelStatus_ShouldReturnStatusCode200()
        {
            // Arrange
            var updateBestelStatus = new UpdateBestelStatus()
            {
                Status = BestelStatus.Goedgekeurd,
            };

            _commandPublisherMock.Setup(publisher => publisher.Publish<long>(It.Is<UpdateBestelStatusCommand>
                (x => x.Factuurnummer == 1)))
                .ReturnsAsync(1);

            // Act
            var result = await _target.UpdateBestelStatus(1, updateBestelStatus);

            // Assert
            _commandPublisherMock.VerifyAll();

            var requestResult = result as StatusCodeResult;
            Assert.IsNotNull(requestResult);
            Assert.AreEqual(200, requestResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateBestelStatus_DatabaseException_ShouldReturnFactuurnummerError()
        {
            // Arrange
            var updateBestelStatus = new UpdateBestelStatus()
            {
                Status = BestelStatus.Goedgekeurd,
            };

            _commandPublisherMock.Setup(publisher => publisher.Publish<long>(It.Is<UpdateBestelStatusCommand>
                (x => x.Factuurnummer == 1)))
                .ThrowsAsync(new DatabaseException("error"));

            // Act
            var result = await _target.UpdateBestelStatus(1, updateBestelStatus);

            // Assert
            _commandPublisherMock.VerifyAll();

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Factuurnummer is incorrect", badRequestResult.Value);
        }

        [TestMethod]
        public async Task UpdateBestelStatus_TimeoutException()
        {
            // Arrange
            var updateBestelStatus = new UpdateBestelStatus()
            {
                Status = BestelStatus.Goedgekeurd,
            };

            _commandPublisherMock.Setup(publisher => publisher.Publish<long>(It.Is<UpdateBestelStatusCommand>
                (x => x.Factuurnummer == 1)))
                .ThrowsAsync(new TimeoutException());

            // Act
            var result = await _target.UpdateBestelStatus(1, updateBestelStatus);

            // Assert
            _commandPublisherMock.VerifyAll();

            var requestResult = result as ObjectResult;
            Assert.IsNotNull(requestResult);
            Assert.AreEqual(408, requestResult.StatusCode);
            Assert.AreEqual("Aanvraag kon niet worden verwerkt", requestResult.Value);
        }

        [TestMethod]
        public async Task UpdateBestelStatus_Exception()
        {
            // Arrange
            var updateBestelStatus = new UpdateBestelStatus()
            {
                Status = BestelStatus.Goedgekeurd,
            };

            _commandPublisherMock.Setup(publisher => publisher.Publish<long>(It.Is<UpdateBestelStatusCommand>
                (x => x.Factuurnummer == 1)))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _target.UpdateBestelStatus(1, updateBestelStatus);

            // Assert
            _commandPublisherMock.VerifyAll();

            var requestResult = result as StatusCodeResult;
            Assert.IsNotNull(requestResult);
            Assert.AreEqual(500, requestResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateBestelStatus_InvalidUpdateException()
        {
            // Arrange
            var updateBestelStatus = new UpdateBestelStatus()
            {
                Status = BestelStatus.Goedgekeurd,
            };

            _commandPublisherMock.Setup(publisher => publisher.Publish<long>(It.Is<UpdateBestelStatusCommand>
                    (x => x.Factuurnummer == 1)))
                .ThrowsAsync(new InvalidUpdateException());

            // Act
            var result = await _target.UpdateBestelStatus(1, updateBestelStatus);

            // Assert
            _commandPublisherMock.VerifyAll();

            var requestResult = result as ObjectResult;
            Assert.IsNotNull(requestResult);
            Assert.AreEqual(400, requestResult.StatusCode);
            Assert.AreEqual($"Update van bestelstatus naar: Goedgekeurd is niet toegestaan", requestResult.Value);
        }

        [TestMethod]
        public async Task UpdateBestelStatus_ShouldRemoveBestellingFromSessionsWhenStatusIsVerzonden()
        {
            // Arrange
            var updateBestelStatus = new UpdateBestelStatus()
            {
                Status = BestelStatus.Verzonden,
            };

            var session = new MagazijnSessionEntity { Factuurnummer = 2 };

            _commandPublisherMock.Setup(publisher => publisher.Publish<long>(It.Is<UpdateBestelStatusCommand>
                    (x => x.Factuurnummer == 1)))
                .ReturnsAsync(1);

            _magazijnSessionDataMapperMock.Setup(d => d.GetByFactuurnummer(1)).Returns(session);
            _magazijnSessionDataMapperMock.Setup(d => d.Delete(It.Is<MagazijnSessionEntity>(e =>
                e.Factuurnummer == 2 && e.Id == session.GetKeyValue()
            )));

            // Act
            var result = await _target.UpdateBestelStatus(1, updateBestelStatus);

            // Assert
            _commandPublisherMock.VerifyAll();
            _magazijnSessionDataMapperMock.VerifyAll();

            var requestResult = result as StatusCodeResult;
            Assert.IsNotNull(requestResult);
            Assert.AreEqual(200, requestResult.StatusCode);
        }
    }
}
