using System;
using System.Collections.Generic;

namespace OpenTimelapseSort.DataServices
{
    internal class MatchingService
    {
        public delegate void RenderDelegate(List<SDirectory> imageDirectories);

        private readonly DBPreferencesService _dbPreferencesService = new DBPreferencesService();
        private readonly DbService _dbService = new DbService();
        private readonly ImageProcessingService _imageProcessingService = new ImageProcessingService();
        private readonly List<SDirectory> _imageDirectories = new List<SDirectory>();

        private bool WithinSameLocation(double mLocPre, double mLocCur)
        {
            return true;
        }

        private bool WithinSameShot(SImage pImage, SImage cImage)
        {
            const int colorSync = 0xffff;
            const int lumenSync = 100;

            _imageProcessingService.SetImageMetaValues(pImage);
            _imageProcessingService.SetImageMetaValues(cImage);

            var cMatrixPre = pImage.Colors;
            var cMatrixCur = cImage.Colors;
            var cLumenPre = pImage.Lumen;
            var cLumenCur = cImage.Lumen;

            return ((cMatrixPre >= cMatrixCur - colorSync && cMatrixPre <= cMatrixCur + colorSync ||
                    cMatrixPre <= cMatrixCur - colorSync && cMatrixPre >= cMatrixCur + colorSync) &&
                   (cLumenPre >= cLumenCur - lumenSync && cLumenPre <= cLumenCur + lumenSync ||
                    cLumenPre <= cLumenCur - lumenSync && cLumenPre >= cLumenCur + lumenSync));
        }

        public bool WithinSameSequence(double curD, double preD, double generosity)
        {
            var syncValue = preD * generosity;

            return (preD >= curD - syncValue && preD <= curD + syncValue ||
                    preD <= curD - syncValue && preD >= curD + syncValue);
        }

        public async void SortImages(List<SImage> imageList, RenderDelegate render)
        {
            var dirList = new List<SImage>();
            var randomDirList = new List<SImage>();

            var curD = 0.0;
            var preD = 0.0;
            var lastIndex = imageList.Count - 1;
            var preI = imageList[lastIndex];

            var preferences = _dbPreferencesService.FetchPreferences();
            var deviationGenerosity = preferences.sequenceIntervalGenerosity / 100;
            var runs = preferences.sequenceImageCount;

            if (preferences.useAutoDetectInterval)
            {
                preD = preferences.sequenceInterval;
            }

            // TODO: match to fit seconds spec again! Fix milliseconds issue

            for (var i = 0; i < imageList.Count; i++)
            {
                if (i > 0)
                {
                    preD = Math.Abs((imageList[i].fileTime - imageList[i - 1].fileTime).Milliseconds);
                    preI = imageList[i - 1];

                    if (i < imageList.Count - 1)
                    {
                        curD = Math.Abs((imageList[i + 1].fileTime - imageList[i].fileTime).Milliseconds);
                    }
                    else
                    {
                        curD = preD;
                    }
                }

                if (WithinSameSequence(curD, preD, deviationGenerosity))
                {
                    dirList.Add(imageList[i]);
                }
                else
                {
                    if (WithinSameShot(preI, imageList[i]))
                    {
                        dirList.Add(imageList[i]);
                    }
                    else
                    {
                        if (dirList.Count >= runs)
                        {
                            await CreateDirAsync(dirList);
                        }
                        else
                        {
                            randomDirList.Add(imageList[i]);
                        }

                        dirList = new List<SImage>();
                    }
                }
            }

            if (dirList.Count < runs && dirList.Count > 0)
            {
                randomDirList.AddRange(dirList);
                await CreateRandomDirAsync(randomDirList);
            }
            else if (randomDirList.Count > 0 && randomDirList.Count < runs)
            {
                await CreateRandomDirAsync(randomDirList);
            }

            if (dirList.Count >= runs)
            {
                await CreateDirAsync(dirList);
            }

            render(_imageDirectories);
        }

        private async System.Threading.Tasks.Task CreateRandomDirAsync(List<SImage> dirList)
        {

            var directory = new SDirectory
            (
                dirList[0].target,
                dirList[0].name + "Random"
            )
            {
                id = Guid.NewGuid().ToString(),
                imageList = dirList
            };

            await SaveMatch(directory);
            _imageDirectories.Add(directory);
        }

        private async System.Threading.Tasks.Task CreateDirAsync(List<SImage> dirList)
        {
            var directory = new SDirectory
                (
                    dirList[0].target,
                    dirList[0].name
                )
            {
                id = Guid.NewGuid().ToString(),
                imageList = dirList
            };

            await SaveMatch(directory);
            _imageDirectories.Add(directory);
        }

        private async System.Threading.Tasks.Task SaveMatch(SDirectory directory)
        {
            SImport import;
            var importExists = await _dbService.ImportExistsAsync();

            if (importExists)
            {
                import = await _dbService.GetImportAsync();

                directory.importId = import.id;

                import.directories = new List<SDirectory>();
                import.directories.Add(directory);
                import.length++;

                await _dbService.UpdateImportAsync(import);
            }
            else
            {
                import = new SImport()
                {
                    id = Guid.NewGuid().ToString(),
                    name = directory.name,
                    importDate = DateTime.Today.ToShortDateString(),
                    length = 0,
                    target = directory.target,
                    directories = new List<SDirectory>()
                };

                directory.importId = import.id;
                import.directories.Add(directory);

                await _dbService.SaveImportAsync(import);
            }
        }
    }
}
