using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Constants;
using Logic.Enums;

namespace Logic.Models
{
    /// <summary>
    /// Модель для логики с подключением к muse score
    /// </summary>
    public class MuseScoreConnectionModel
    {
        /// <summary>
        /// Индекс
        /// </summary>
        private int _index = -1;

        /// <summary>
        /// Id сущности
        /// </summary>
        public string EntityId { get; }

        /// <summary>
        /// Заголовок авторизации
        /// </summary>
        public string AuthId { get; }

        /// <summary>
        /// Тип контента
        /// </summary>
        public string ContentQuery { get; }

        /// <summary>
        /// Индекс для изображений
        /// </summary>
        public string Index => _index.ToString();

        /// <summary>
        /// Версия запроса
        /// </summary>
        public string V2Flag { get; } = "1";

        public MuseScoreConnectionModel(string entityId, MuseScoreContentType contentType, int index = 0)
        {
            EntityId = entityId;
            _index = index;
            ContentQuery = ResolveContentQuery(contentType);
            AuthId = ResolveAuthHeader(contentType);
        }
        
        /// <summary>
        /// Повышает значение индекса
        /// </summary>
        public void IncrementIndex() => _index++;
        
        /// <summary>
        /// Тип квери запроса
        /// </summary>
        private string ResolveContentQuery(MuseScoreContentType contentType)
        {
            switch (contentType)
            {
                case MuseScoreContentType.Midi:
                    return "midi";
                case MuseScoreContentType.Mp3:
                    return "mp3";
                case MuseScoreContentType.Svg:
                    return "img";
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Тип авторизационного токена
        /// </summary>
        private string ResolveAuthHeader(MuseScoreContentType contentType)
        {
            switch (contentType)
            {
                case MuseScoreContentType.Midi:
                    return MuseScoreConnectionConstants.MidiAuthorization;
                case MuseScoreContentType.Mp3:
                    return MuseScoreConnectionConstants.Mp3Authorization;
                case MuseScoreContentType.Svg:
                    return MuseScoreConnectionConstants.SvgAuthorization;
                default: 
                    throw new NotImplementedException();
            }
        }
    }
}
