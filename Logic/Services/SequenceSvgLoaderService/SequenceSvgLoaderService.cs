using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvertApiDotNet;
using ConvertApiDotNet.Model;
using Logic.Models;
using Logic.Services.MuseScoreConnectionService.Interfaces;
using Logic.Services.SequenceSvgLoaderService.Interfaces;

namespace Logic.Services.SequenceSvgLoaderService
{
    ///<inheritdoc cref="ISequenceSvgLoaderService"/>
    internal class SequenceSvgLoaderService : ISequenceSvgLoaderService
    {
        private readonly IMuseScoreConnectionService _museScoreConnectionService;
        private readonly ConvertApi _convertApi;

        public SequenceSvgLoaderService(IMuseScoreConnectionService museScoreConnectionService, ConvertApi convertApi)
        {
            _museScoreConnectionService = museScoreConnectionService;
            _convertApi = convertApi;
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

            var user = await _convertApi.GetUserAsync();
            var remain = user.ConversionsTotal - user.ConversionsConsumed;
            if (remain - 1 < responseSvgList.Count)
            {
                throw new Exception("Не хватает конверсий");
            }
            else
            {
                Console.WriteLine($"Конверсий осталось: {remain - responseSvgList.Count - 1}");
            }
            
            
            var responseList = new List<ConvertApiResponse>();
            for (var i = 0; i < responseSvgList.Count; i++)
            {
                using var ms = new MemoryStream(responseSvgList[i]);
                var param = new ConvertApiFileParam(ms, "test.svg");
                var response = await _convertApi.ConvertAsync("svg", "pdf", param);
                responseList.Add(response);
            }

            var paramList = responseList.Select(x => new ConvertApiFileParam(x)).ToList();
            var mergeTask = await _convertApi.ConvertAsync("pdf", "merge", paramList);
            await using var stream = await mergeTask.Files.First().FileStreamAsync();
            using var ms2 = new MemoryStream();
            await stream.CopyToAsync(ms2);
            return ms2.ToArray();
        }
    }
}
