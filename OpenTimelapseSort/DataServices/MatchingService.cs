using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using OpentimelapseSort.Models;

namespace OpenTimelapseSort.DataServices
{
    internal class MatchingService
    {
        public delegate void RenderDelegate(List<SDirectory> imageDirectories);

        private readonly DbPreferencesService _dbPreferencesService = new DbPreferencesService();
        private readonly DbService _dbService = new DbService();

        private readonly ImageProcessingService _imageProcessingService = new ImageProcessingService();
        private readonly List<SDirectory> _imageDirectories = new List<SDirectory>();

        private readonly Preferences _preferences;
        private readonly double _deviationGenerosity;
        private readonly int _runs;

        private readonly List<SImage> dirList;
        private readonly List<SImage> randomDirList;
        private long _mostOccuredInterval;

        public MatchingService()
        {
            _preferences = _dbPreferencesService.FetchPreferences();
            _deviationGenerosity = _preferences.SequenceIntervalGenerosity / 100;
            _runs = _preferences.SequenceImageCount;

            dirList = new List<SImage>();
            randomDirList = new List<SImage>();
        }

        public void SortImagesAsync(List<SImage> imageList, RenderDelegate render)
        {
            dirList.Clear();
            randomDirList.Clear();

            if (!_preferences.UseAutoDetectInterval)
                SortImagesAuto(imageList, render);
            else
                SortImages(imageList, render);
        }

        private bool WithinSameShot(SImage pImage, SImage cImage)
        {
            const int colorSync = 0xffff;
            const int lumenSync = 500;

            _imageProcessingService.SetImageMetaValues(pImage);
            _imageProcessingService.SetImageMetaValues(cImage);

            var cMatrixPre = pImage.Colors;
            var cMatrixCur = cImage.Colors;
            var cLumenPre = pImage.Lumen;
            var cLumenCur = cImage.Lumen;

            return (cMatrixPre >= cMatrixCur - colorSync && cMatrixPre <= cMatrixCur + colorSync ||
                    cMatrixPre <= cMatrixCur - colorSync && cMatrixPre >= cMatrixCur + colorSync) &&
                   (cLumenPre >= cLumenCur - lumenSync && cLumenPre <= cLumenCur + lumenSync ||
                    cLumenPre <= cLumenCur - lumenSync && cLumenPre >= cLumenCur + lumenSync);
        }

        public bool WithinSameSequence(double curD, double preD)
        {
            var syncValue = preD * _deviationGenerosity;

            Debug.WriteLine(preD >= curD - syncValue && preD <= curD + syncValue ||
                            preD <= curD - syncValue && preD >= curD + syncValue);

            return preD >= curD - syncValue && preD <= curD + syncValue ||
                   preD <= curD - syncValue && preD >= curD + syncValue;
        }

        private bool BelongToEachOther(SImage pImage, SImage cImage)
        {
            return cImage.FileTime >= pImage.FileTime + _preferences.SequenceInterval;
        }

        public async void SortImagesAuto(List<SImage> imageList, RenderDelegate render)
        {
            for (var i = 0; i < imageList.Count - 1; i++)
                if (WithinSameShot(imageList[i], imageList[i + 1]) &&
                    BelongToEachOther(imageList[i], imageList[i + 1]))
                {
                    dirList.Add(imageList[i]);
                }
                else
                {
                    // TODO: tilpass til endringene i andre funksjonen
                    if (dirList.Count >= _runs)
                        await CreateDirAsync();
                    else
                        randomDirList.Add(imageList[i]);

                    dirList.Clear();
                }

            HandleLastElement(imageList);

            await CompleteDirectories(render);
        }

        private void HandleLastElement(List<SImage> imageList)
        {
            var lastIndex = imageList.Count - 1;

            if (WithinSameShot(imageList[lastIndex - 1], imageList[lastIndex]))
                dirList.Add(imageList[lastIndex]);
            else
                randomDirList.Add(imageList[lastIndex]);
        }

        private async Task CompleteDirectories(RenderDelegate render)
        {
            if (dirList.Count < _runs && dirList.Count > 0)
            {
                randomDirList.AddRange(dirList);
                await CreateRandomDirAsync();
            }
            else if (randomDirList.Count > 0 && randomDirList.Count < _runs)
            {
                await CreateRandomDirAsync();
            }

            if (dirList.Count >= _runs) await CreateDirAsync();

            render(_imageDirectories);
        }

        public async void SortImages(List<SImage> imageList, RenderDelegate render)
        {
            // TODO: match to fit seconds spec again! Fix milliseconds issue

            for (var i = 1; i < imageList.Count - 1; i++)
            {
                var preD = imageList[i].FileTime - imageList[i - 1].FileTime;
                var curD = imageList[i + 1].FileTime - imageList[i].FileTime;

                if (WithinSameSequence(curD, preD))
                {
                    dirList.Add(imageList[i]);
                }
                else
                {
                    if (WithinSameShot(imageList[i - 1], imageList[i]))
                    {
                        dirList.Add(imageList[i]);
                    }
                    else
                    {
                        if (dirList.Count >= _runs)
                        {
                            await CreateDirAsync();
                            dirList.Clear();
                        } else
                        {
                            randomDirList.Add(imageList[i]);
                        }
                    }
                }
            }

            HandleLastElement(imageList);

            await CompleteDirectories(render);
        }

        private async Task CreateRandomDirAsync()
        {
            var name = Path.GetDirectoryName(randomDirList[0].Target) + "_R";
            var directory = new SDirectory
            (
                randomDirList[0].Target,
                name
            )
            {
                Id = Guid.NewGuid().ToString(),
                ImageList = randomDirList
            };

            await SaveMatch(directory);
            _imageDirectories.Add(directory);
        }

        private async Task CreateDirAsync()
        {
            var name = Path.GetDirectoryName(dirList[0].Target);
            var directory = new SDirectory
            (
                dirList[0].Target,
                name
            )
            {
                Id = Guid.NewGuid().ToString(),
                ImageList = dirList
            };

            await SaveMatch(directory);
            _imageDirectories.Add(directory);
        }

        private async Task SaveMatch(SDirectory directory)
        {
            SImport import;
            var importExists = await _dbService.ImportExistsAsync();

            if (importExists)
            {
                import = await _dbService.GetImportAsync();

                directory.ImportId = import.Id;

                import.Directories = new List<SDirectory>();
                import.Directories.Add(directory);
                import.Length++;

                await _dbService.UpdateImportAsync(import);
            }
            else
            {
                import = new SImport
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = directory.Name,
                    ImportDate = DateTime.Today.ToShortDateString(),
                    Length = 0,
                    Target = directory.Target,
                    Directories = new List<SDirectory>()
                };

                directory.ImportId = import.Id;

                import.Directories.Add(directory);

                await _dbService.SaveImportAsync(import);
            }
        }
    }
}