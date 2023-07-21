using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services.SvgToPdfConverter.Interfaces
{
    /// <summary>
    /// Конвертация свг в пдф
    /// </summary>
    public interface ISvgToPdfConverter
    {
        /// <summary>
        /// Сам метод конвертации
        /// </summary>
        /// <param name="svgList"></param>
        /// <returns></returns>
        public Task<byte[]> ConvertSvgToPdfAsync(List<byte[]> svgList);
    }
}
