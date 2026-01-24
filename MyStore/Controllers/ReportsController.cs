using AutoMapper;
using DevExpress.Xpo;
using DevExpress.XtraCharts;
using Infrastructure.Data;
using Kafaa.API.Extention;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyStore.Reports;
using System.Drawing;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace MyStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly StoreDbContext _context;
        private readonly IMapper _mapper;

        public ReportsController(StoreDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpPost("ExportAllUsers")]
        public async Task<IResult> ExportAllUsers()
        {

            var data =  _context.Users.ToList();

            try
            {
                UsersReport usersReport = new UsersReport();
                usersReport.xrTable1.Fill(data.Select(x => new List<string>
                 {
                 x.Username,
                  x.Email,
                     x.NameAR,


                 }).ToList());

                var pdfContent = usersReport.GenerateReportPdf();
                return Results.File(
                    pdfContent,
                    "application/pdf",
                    "TestReport.pdf");
            }
            catch (Exception ex)
            {

                throw new Exception($"Error: {ex.Message} - {ex.InnerException?.Message}");
            }

        }

    }
}
