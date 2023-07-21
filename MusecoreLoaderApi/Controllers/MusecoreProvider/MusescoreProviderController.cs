using System.ComponentModel.DataAnnotations;
using Api.Filters;
using Logic.Enums;
using Logic.Models;
using Logic.Services.MuseScoreConnectionService.Interfaces;
using Logic.Services.SequenceSvgLoaderService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.MusescoreProvider
{
    /// <summary>
    /// Общение с музкором
    /// </summary>
    [ApiController]
    [Route("api/musescore-provider")]
    public class MusescoreProviderController : Controller
    {
        private readonly IMuseScoreConnectionService _connectionService;

        public MusescoreProviderController(IMuseScoreConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        /// <summary>
        /// Загрузка mp3 версии
        /// </summary>
        [HttpGet("get-mp3-notes/{id}")]
        [OutputFileNameFilter]
        public async Task<IActionResult> GetMp3NotesAsync([FromRoute, Required]string id, [FromQuery]string outFileName)
        {
            var model = new MuseScoreConnectionModel(id, MuseScoreContentType.Mp3);
            var byteFile = await _connectionService.LoadRawFileAsync(model);
            return File(byteFile, "application/mp3", $"{outFileName}.mp3");
        }

        /// <summary>
        /// Загрузка midi версии
        /// </summary>
        [HttpGet("get-midi-notes/{id}")]
        [OutputFileNameFilter]
        public async Task<IActionResult> GetMidiNotesAsync([FromRoute, Required] string id, [FromQuery] string outFileName)
        {
            var model = new MuseScoreConnectionModel(id, MuseScoreContentType.Midi);
            var byteFile = await _connectionService.LoadRawFileAsync(model);
            return File(byteFile, "application/midi", $"{outFileName}.midi");
        }

        /// <summary>
        /// Загрузка pdf версии
        /// </summary>
        [HttpGet("get-pdf-notes/{id}")]
        [OutputFileNameFilter]
        public async Task<IActionResult> GetPdfNotesAsync([FromRoute, Required] string id,
            [FromQuery] string outFileName, [FromServices]ISequenceSvgLoaderService sequenceSvgLoaderService)
        {
            var model = new MuseScoreConnectionModel(id, MuseScoreContentType.Svg);
            var pdf = await sequenceSvgLoaderService.LoadPdfDataAsync(model);
            return File(pdf, "application/pdf", $"{outFileName}.pdf");
        }
    }
}
