using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public SequenceSvgLoaderService(IMuseScoreConnectionService museScoreConnectionService)
        {
            _museScoreConnectionService = museScoreConnectionService;
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

            var api = new ConvertApi(KeyStorageService.GetKey(responseSvgList.Count == 1 ? 1 : responseSvgList.Count + 1));
            var user = await api.GetUserAsync();
            var remain = user.ConversionsTotal - user.ConversionsConsumed;
            Console.WriteLine($"Конверсий осталось: {remain - responseSvgList.Count - 1}");

            var responseList = new List<ConvertApiResponse>();
            for (var i = 0; i < responseSvgList.Count; i++)
            {
                using var ms = new MemoryStream(responseSvgList[i]);
                var param = new ConvertApiFileParam(ms, "test.svg");
                var response = await api.ConvertAsync("svg", "pdf", param);
                responseList.Add(response);
            }

            if (responseSvgList.Count == 1)
            {
                await using var s1 = await responseList.First().Files.First().FileStreamAsync();
                using var ms3 = new MemoryStream();
                await s1.CopyToAsync(ms3);
                return ms3.ToArray();
            }
            
            var paramList = responseList.Select(x => new ConvertApiFileParam(x)).ToList();
            var mergeTask = await api.ConvertAsync("pdf", "merge", paramList);
            await using var stream = await mergeTask.Files.First().FileStreamAsync();
            using var ms2 = new MemoryStream();
            await stream.CopyToAsync(ms2);
            return ms2.ToArray();
        }
    }
}
