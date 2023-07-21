using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Models;
using Logic.Services.MuseScoreConnectionService.Interfaces;
using Logic.Services.SequenceSvgLoaderService.Interfaces;
using Logic.Services.SvgToPdfConverter.Interfaces;

namespace Logic.Services.SequenceSvgLoaderService
{
    ///<inheritdoc cref="ISequenceSvgLoaderService"/>
    internal class SequenceSvgLoaderService : ISequenceSvgLoaderService
    {
        private readonly IMuseScoreConnectionService _museScoreConnectionService;
        private readonly ISvgToPdfConverter _converter;

        public SequenceSvgLoaderService(IMuseScoreConnectionService museScoreConnectionService, ISvgToPdfConverter converter)
        {
            _museScoreConnectionService = museScoreConnectionService;
            _converter = converter;
        }

        ///<inheritdoc />
        public async Task<byte[]> LoadPdfDataAsync(MuseScoreConnectionModel model)
        {
            var responseSvgList = new List<byte[]>();
            var isContinue = true;
            do
            {
                try
                {
                    var pageResponse = await _museScoreConnectionService.LoadFileAsByteArrayAsync(model);
                    responseSvgList.Add(pageResponse);
                }
                catch (FileNotFoundException) when (model.Index == "0")
                {
                    throw;
                }
                catch (FileNotFoundException)
                {
                    isContinue = false;
                }

                model.IncrementIndex();
            } while (isContinue);

            var pdf = await _converter.ConvertSvgToPdfAsync(responseSvgList);
            return pdf;
        }
    }
}
