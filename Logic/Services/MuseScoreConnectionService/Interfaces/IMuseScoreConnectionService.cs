using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Enums;
using Logic.Models;

namespace Logic.Services.MuseScoreConnectionService.Interfaces
{
    /// <summary>
    /// Сервис-обёртка для взаимодействия с api museScore
    /// </summary>
    public interface IMuseScoreConnectionService
    {
        /// <summary>
        /// Загрузить сырой файл
        /// </summary>
        public Task<Stream> LoadRawFileAsync(MuseScoreConnectionModel model);

        /// <summary>
        /// Загрузить файл, как массив байт
        /// </summary>
        public Task<byte[]> LoadFileAsByteArrayAsync(MuseScoreConnectionModel model);
    }
}
