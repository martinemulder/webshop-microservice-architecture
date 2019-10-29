using HalfWerk.BffWebshop.DataMapper;
using HalfWerk.BffWebshop.Entities;
using HalfWerk.CommonModels.BffWebshop;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Net;
using Microsoft.CodeAnalysis.Operations;

namespace HalfWerk.BffWebshop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtikelController : ControllerBase
    {
        private readonly IArtikelDataMapper _artikelDataMapper;

        public ArtikelController(IArtikelDataMapper artikelDataMapper)
        {
            _artikelDataMapper = artikelDataMapper;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lijst van artikelen",
            OperationId = "GetArtikelen"
        )]
        [SwaggerResponse((int) HttpStatusCode.OK)]
        public ActionResult<IEnumerable<Artikel>> GetArtikelen()
        {
            var artikelEntities = _artikelDataMapper.GetAll();
            var resultList = new List<Artikel>(); 
            foreach(var artikelEntity in artikelEntities)
            {
                resultList.Add(artikelEntity.ToArtikel());
            }

            return resultList;
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Enkel artikel op basis van het ID",
            OperationId = "GetArtikelById"
        )]
        [SwaggerResponse((int) HttpStatusCode.OK)]
        public ActionResult<Artikel> GetArtikelById(int id)
        {
            return _artikelDataMapper.GetById(id)?.ToArtikel();
        }
    }
}