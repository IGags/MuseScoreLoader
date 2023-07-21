using System.Runtime.ConstrainedExecution;
using Logic.Services.SvgToPdfConverter.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using Svg.Skia;

namespace Logic.Services.SvgToPdfConverter
{
    ///<inheritdoc cref="ISvgToPdfConverter"/>
    internal class SvgToPdfConverter : CriticalFinalizerObject, ISvgToPdfConverter
    {
        private class NoteDocument : IDocument
        {
            private readonly List<MemoryStream> _pageImages;

            public NoteDocument(List<MemoryStream> pageImages)
            {
                _pageImages = pageImages;
            }

            public DocumentMetadata Metadata => DocumentMetadata.Default;

            public void Compose(IDocumentContainer container)
            {
                foreach (var image in _pageImages)
                {
                    container.Page(page =>
                    {
                        var img = new SKSvg();
                        img.Load(image);
                        page.Content()
                            .Padding(0)
                            .Canvas((x,s) =>
                            {
                                x.DrawPicture(img.Picture);
                            });
                    });
                }
            }
        }

        public SvgToPdfConverter()
        {
        }

        ///<inheritdoc />
        public async Task<byte[]> ConvertSvgToPdfAsync(List<byte[]> svgList)
        {
            var streamList = new List<MemoryStream>();
            foreach (var image in svgList)
            {
                var stream = new MemoryStream();
                stream.Write(image);
                streamList.Add(stream);
                stream.Position = 0;
            }
            var noteDocument = new NoteDocument(streamList);
            var bytes = noteDocument.GeneratePdf();
            foreach (var stream in streamList)
            {
                stream.Close();
                await stream.DisposeAsync();
            }
            return bytes;
        }
    }
}
