﻿using ConvertApiDotNet;
using Logic.Services;
using Logic.Services.MuseScoreConnectionService;
using Logic.Services.MuseScoreConnectionService.Interfaces;
using Logic.Services.SequenceSvgLoaderService;
using Logic.Services.SequenceSvgLoaderService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Logic
{
    /// <summary>
    /// Добавление логики в di
    /// </summary>
    public static class LogicForStartup
    {
        public static IServiceCollection AddLogic(this IServiceCollection collection)
        {
            collection.AddTransient<IMuseScoreConnectionService, MuseScoreConnectionService>();
            collection.AddTransient<ISequenceSvgLoaderService, SequenceSvgLoaderService>();
            
            return collection;
        }
    }
}
