using System;
using HalfWerk.CommonModels;
using HalfWerk.CommonModels.DsKlantBeheer;
using HalfWerk.DsKlantBeheer.Controllers;
using HalfWerk.DsKlantBeheer.DAL;
using HalfWerk.DsKlantBeheer.DAL.DataMappers;
using HalfWerk.DsKlantBeheer.Test.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minor.Nijn.TestBus;
using Minor.Nijn.WebScale.Events;
using System.Collections.Generic;
using System.Linq;
using HalfWerk.CommonModels.DsKlantBeheer.Models;
using HalfWerk.CommonModels.CommonExceptions;

namespace HalfWerk.DsKlantBeheer.Test.Controllers
{
    [TestClass]
    public class KlantControllerIntegrationTest
    {
        private KlantBuilder _klantBuilder;
        private SqliteConnection _connection;
        private KlantContext _dbContext;
        private ITestBusContext _nijnContext;

        private KlantController _target;

        [TestInitialize]
        public void BeforeEach()
        {
            _klantBuilder = new KlantBuilder();

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<KlantContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging()
                .Options;

            _dbContext = new KlantContext(options);
            _dbContext.Database.EnsureCreated();
            SeedDatabase();

            _nijnContext = new TestBusContextBuilder().CreateTestContext();

            var klantDataMapper = new KlantDataMapper(_dbContext);
            var eventPublisher = new EventPublisher(_nijnContext);

            _target = new KlantController(klantDataMapper, eventPublisher, new LoggerFactory());
        }

        [TestCleanup]
        public void AfterEach()
        {
            _dbContext.Dispose();
            _connection.Dispose();
        }


        private void SeedDatabase()
        {
            _dbContext.Klanten.AddRange(new List<Klant>
            {
                _klantBuilder.SetDummy()
                    .SetId(1)
                    .SetEmail("test1@test.nl")
                    .Create(),

                _klantBuilder.SetDummy()
                    .SetId(2)
                    .SetEmail("test2@test.nl")
                    .Create(),
            });

            _dbContext.SaveChanges();
        }

        [TestMethod]
        public void HandleVoegKlantToe_ShouldInsertKlantIntoTheDatabase()
        {
            var eventQueueName = "TestEventQueue";
            var klant = _klantBuilder.SetDummy().SetId(3).SetEmail("email@domain.com").Create();
            var command = new VoegKlantToeCommand(klant, NameConstants.KlantBeheerVoegKlantToeCommand);

            _nijnContext.EventBus.DeclareQueue(eventQueueName, new List<string> { NameConstants.KlantBeheerKlantToegevoegdEvent });

            var result = _target.HandleVoegKlantToe(command);

            var eventQueue = _nijnContext.EventBus.Queues[eventQueueName];
            Assert.AreEqual(1, eventQueue.MessageQueueLength);

            var dbResult = _dbContext.Klanten.Include(k => k.Adres).SingleOrDefault(k => k.Id == 3);
            Assert.IsNotNull(dbResult);
            Assert.AreEqual(klant.Email, dbResult.Email);
        }

        [TestMethod]
        public void HandleVoegKlantToe_ShouldReturnExistingKlantIdWhenEmailExists()
        {
            var eventQueueName = "TestEventQueue";
            var klant = _klantBuilder.SetDummy().SetEmail("test1@test.nl").Create();
            var command = new VoegKlantToeCommand(klant, NameConstants.KlantBeheerVoegKlantToeCommand);

            _nijnContext.EventBus.DeclareQueue(eventQueueName, new List<string> { NameConstants.KlantBeheerKlantToegevoegdEvent });

            Action action = () => _target.HandleVoegKlantToe(command);

            var eventQueue = _nijnContext.EventBus.Queues[eventQueueName];
            Assert.AreEqual(0, eventQueue.MessageQueueLength);
            Assert.ThrowsException<EmailAllreadyExistsException>(action);
        }
    }
}