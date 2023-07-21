using Logic.Services.MuseScoreConnectionService;
using Logic.Services.MuseScoreConnectionService.Interfaces;
using Logic.Services.SequenceSvgLoaderService;
using Logic.Services.SequenceSvgLoaderService.Interfaces;
using Logic.Services.SvgToPdfConverter;
using Logic.Services.SvgToPdfConverter.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Logic
{
    /// <summary>
    /// Добавление логики в di
    /// </summary>
    public static class LogicForStarup
    {
        public static IServiceCollection AddLogic(this IServiceCollection collection)
        {
            collection.AddTransient<IMuseScoreConnectionService, MuseScoreConnectionService>();
            collection.AddTransient<ISvgToPdfConverter, SvgToPdfConverter>();
            collection.AddTransient<ISequenceSvgLoaderService, SequenceSvgLoaderService>();

            return collection;
        }
    }
}
