using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OpenTimelapseSort.Models;

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

        private List<SImage> _dirList;
        private List<SImage> _randomDirList;

        public MatchingService()
        {
            _preferences = _dbPreferencesService.FetchPreferences();
            _deviationGenerosity = _preferences.SequenceIntervalGenerosity / 100;
            _runs = _preferences.SequenceImageCount;

            _dirList = new List<SImage>();
            _randomDirList = new List<SImage>();
        }

        public List<SDirectory> MatchImages(List<SImage> imageList)
        {
            _dirList = new List<SImage>();
            _randomDirList = new List<SImage>();

            //dirList.Clear();
            //randomDirList.Clear();

            return !_preferences.UseAutoDetectInterval ? SortImagesAuto(imageList) : SortImages(imageList);
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

        private bool WithinSameShotLite(SImage pImage, SImage cImage)
        {
            const int colorSync = 0xffff;
            const int lumenSync = 500;

            _imageProcessingService.SetImageMetaValuesLite(pImage);
            _imageProcessingService.SetImageMetaValuesLite(cImage);

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

            return preD >= curD - syncValue && preD <= curD + syncValue ||
                   preD <= curD - syncValue && preD >= curD + syncValue;
        }

        private bool BelongToEachOther(SImage pImage, SImage cImage)
        {
            return cImage.FileTime >= pImage.FileTime + _preferences.SequenceInterval;
        }

        private List<SDirectory> SortImagesAuto(List<SImage> imageList)
        {
            for (var i = 0; i < imageList.Count - 1; i++)
            {
                Debug.WriteLine("Image " + i);

                if (WithinSameShot(imageList[i], imageList[i + 1]) &&
                    BelongToEachOther(imageList[i], imageList[i + 1]))
                {
                    _dirList.Add(imageList[i]);
                }
                else
                {
                    if (_dirList.Count >= _runs)
                    {
                        CreateDirAsync();
                        _dirList = new List<SImage>();
                        //dirList.Clear();
                    }
                    else
                    {
                        _randomDirList.Add(imageList[i]);
                    }
                }
                Debug.WriteLine(_imageDirectories);
            }

            HandleLastElement(imageList);
            CompleteDirectories();

            return _imageDirectories;
        }

        private void HandleLastElement(List<SImage> imageList)
        {
            var lastIndex = imageList.Count - 1;

            if (WithinSameShot(imageList[lastIndex - 1], imageList[lastIndex]))
                _dirList.Add(imageList[lastIndex]);
            else
                _randomDirList.Add(imageList[lastIndex]);
        }

        private void CompleteDirectories()
        {
            if (_dirList.Count < _runs && _dirList.Count > 0)
            {
                _randomDirList.AddRange(_dirList);
                CreateRandomDirAsync();
                _randomDirList = new List<SImage>();
                //randomDirList.Clear();
            }
            else if (_randomDirList.Count > 0)
            {
                CreateRandomDirAsync();
                _randomDirList = new List<SImage>();
                //randomDirList.Clear();
            }

            if (_dirList.Count >= _runs)
            {
                CreateDirAsync();
                _dirList = new List<SImage>();
                //dirList.Clear();
            }
            Debug.WriteLine(_imageDirectories);
        }

        public List<SDirectory> SortImages(List<SImage> imageList)
        {
            Debug.WriteLine("SortImages ");
            for (var i = 1; i < imageList.Count - 1; i++)
            {
                var preD = imageList[i].FileTime - imageList[i - 1].FileTime;
                var curD = imageList[i + 1].FileTime - imageList[i].FileTime;

                if (WithinSameSequence(curD, preD))
                {
                    _dirList.Add(imageList[i]);
                }
                else
                {
                    if (_dirList.Count >= _runs)
                    {
                        CreateDirAsync();
                        _dirList = new List<SImage>();
                        //dirList.Clear();
                    }
                    else
                    {
                        _randomDirList.Add(imageList[i]);
                    }
                }
                Debug.WriteLine(_imageDirectories);
            }

            HandleLastElement(imageList);
            CompleteDirectories();

            return _imageDirectories;
        }

        private void CreateRandomDirAsync()
        {
            Debug.WriteLine("CreateRandomDirAsync ");

            var name = Path.GetFileName(_randomDirList[0].Target) + "_R";
            var directory = new SDirectory
            (
                _randomDirList[0].Target,
                name
            )
            {
                Id = Guid.NewGuid().ToString(),
                ImageList = _randomDirList
            };

            var saveTask = SaveMatch(directory);
        }

        private void CreateDirAsync()
        {
            Debug.WriteLine("CreateDirAsync ");

            var name = Path.GetFileName(_dirList[0].Target);

            Debug.WriteLine(_dirList);

            var directory = new SDirectory
            (
                _dirList[0].Target,
                name
            )
            {
                Id = Guid.NewGuid().ToString(),
                ImageList = _dirList
            };

            var saveTask = SaveMatch(directory);
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
            _imageDirectories.Insert(0, directory);
            Debug.WriteLine(import.Directories.Last());
        }
    }
}