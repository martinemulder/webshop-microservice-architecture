using System;
using System.Collections.Generic;
using System.Data.Common;
using HalfWerk.CommonModels.DsKlantBeheer;
using HalfWerk.CommonModels.DsKlantBeheer.Models;
using HalfWerk.DsKlantBeheer.Controllers;
using HalfWerk.DsKlantBeheer.DAL;
using HalfWerk.DsKlantBeheer.DAL.DataMappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Minor.Nijn.WebScale.Events;
using Moq;
using TechTalk.SpecFlow;

namespace HalfWerk.DsKlantBeheer.Spec
{
    [Binding]
    public class KlantToevoegenSteps
    {
        private readonly DbConnection _connection;
        private readonly KlantContext _context;

        public KlantToevoegenSteps()
        {
            SpecFlowTestLock.Lock();

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<KlantContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new KlantContext(options);
            _context.Database.EnsureCreated();
        }

        ~KlantToevoegenSteps()
        {
            _context.Dispose();
            _connection.Dispose();

            SpecFlowTestLock.Unlock();
        }

        private List<Tuple<string, string, string, string, Adres>> _klanten = new List<Tuple<string, string, string, string, Adres>>();

        [Given(@"er is een klant binnen gekomen met de volgende gegevens:")]
        public void GivenErIsEenKlantBinnenGekomenMetDeVolgendeGegevens(Table table)
        {
            Assert.IsTrue(table.Rows.Count > 0);

            foreach (var row in table.Rows)
            {
                Assert.IsTrue(row.TryGetValue("Voornaam", out string voornaam));
                Assert.IsTrue(row.TryGetValue("Achternaam", out string achternaam));
                Assert.IsTrue(row.TryGetValue("Telefoonnummer", out string telefoonnummer));
                Assert.IsTrue(row.TryGetValue("Email", out string email));

                _klanten.Add(new Tuple<string, string, string, string, Adres>(voornaam, achternaam, telefoonnummer,
                    email, new Adres()));
            }

            Assert.AreEqual(table.Rows.Count, _klanten.Count);
        }
        
        [Given(@"de klant heeft het adres:")]
        public void GivenDeKlantHeeftHetAdres(Table table)
        {
            Assert.IsTrue(table.Rows.Count > 0);
            Assert.IsTrue(_klanten.Count == table.Rows.Count);
            
            for (int i = 0; i < table.Rows.Count; i++)
            {
                Assert.IsTrue(table.Rows[i].TryGetValue("Straatnaam", out string straatnaam));
                _klanten[i].Item5.Straatnaam = straatnaam;

                Assert.IsTrue(table.Rows[i].TryGetValue("Postcode", out string postcode));
                _klanten[i].Item5.Postcode = postcode;

                Assert.IsTrue(table.Rows[i].TryGetValue("Huisnummer", out string huisnummer));
                _klanten[i].Item5.Huisnummer = huisnummer;

                Assert.IsTrue(table.Rows[i].TryGetValue("Plaats", out string plaats));
                _klanten[i].Item5.Plaats = plaats;

                Assert.IsTrue(table.Rows[i].TryGetValue("Land", out string land));
                _klanten[i].Item5.Land = land;
            }
        }
        
        [Then(@"voeg een klant toe aan het systeem met voornaam, achternaam, telefoonnummer, email en adres")]
        public void ThenVoegEenKlantToeAanHetSysteemMetVoornaamAchternaamTelefoonnummerEmailEnAdres()
        {
            var dataMapper = new KlantDataMapper(_context);
            var eventPublisherMock = new Mock<IEventPublisher>(MockBehavior.Strict);
            eventPublisherMock.Setup(p => p.Publish(It.IsAny<DomainEvent>()));
            var controller = new KlantController(dataMapper, eventPublisherMock.Object, new LoggerFactory());

            List<long> results = new List<long>();

            for (int i = 0; i < _klanten.Count; i++)
            {
                Klant nieuweKlant = new Klant()
                {
                    Voornaam = _klanten[i].Item1,
                    Achternaam = _klanten[i].Item2,
                    Telefoonnummer = _klanten[i].Item3,
                    Email = _klanten[i].Item4,
                    Adres = _klanten[i].Item5
                };

                results.Add(controller.HandleVoegKlantToe(new VoegKlantToeCommand(nieuweKlant, "")));
            }

            eventPublisherMock.Verify(e => e.Publish(It.IsAny<DomainEvent>()), Times.Exactly(2));
            eventPublisherMock.VerifyAll();
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(2, results[1]);
        }
        
        [Then(@"genereer een klantnummer")]
        public void ThenGenereerEenKlantnummer()
        {
            var dataMapper = new KlantDataMapper(_context);
            List<Klant> results = new List<Klant>();
            results.Add(dataMapper.GetById(1));
            results.Add(dataMapper.GetById(2));

            Assert.AreEqual(1, results[0].Id);
            Assert.AreEqual("Hans", results[0].Voornaam);
            Assert.AreEqual(1, results[0].Adres.Id);
            Assert.AreEqual("St. Jacobsstraat", results[0].Adres.Straatnaam);

            Assert.AreEqual(2, results[1].Id);
            Assert.AreEqual("Hannah", results[1].Voornaam);
            Assert.AreEqual(2, results[1].Adres.Id);
            Assert.AreEqual("Longest Place Name Avenue", results[1].Adres.Straatnaam);
        }
    }
}
