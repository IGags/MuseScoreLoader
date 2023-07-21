using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Models;

namespace Logic.Services.SequenceSvgLoaderService.Interfaces
{
    /// <summary>
    /// Загрузка всех страничек нот в пдф
    /// </summary>
    public interface ISequenceSvgLoaderService
    {
        public Task<byte[]> LoadPdfDataAsync(MuseScoreConnectionModel model);
    }
}
