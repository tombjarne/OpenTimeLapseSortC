using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OpenTimelapseSort.Models;

namespace OpenTimelapseSort.DataServices
{
    internal class MatchingService
    {
        /// <summary>
        /// services
        /// </summary>
        private readonly DbPreferencesService _dbPreferencesService = new DbPreferencesService();
        private readonly ImageProcessingService _imageProcessingService = new ImageProcessingService();
        private readonly DbService _dbService = new DbService();

        /// <summary>
        /// <see cref="_imageDirectories"/> holds list of all matched directories
        /// <see cref="_dirList"/> holds list of regular directories
        /// <see cref="_randomDirList"/> holds list of random directories
        /// </summary>
        private readonly List<SDirectory> _imageDirectories = new List<SDirectory>();
        private List<SImage> _dirList;
        private List<SImage> _randomDirList;

        /// <summary>
        /// preferences values to be used during the matching process
        /// </summary>
        private Preferences _preferences;
        private int _runs;
        private int _sequence;
        private int _deviationGenerosity;

        public MatchingService()
        {
            _dirList = new List<SImage>();
            _randomDirList = new List<SImage>();
        }

        /// <summary>
        ///     MatchImages()
        ///     kicks off the sorting process depending on the choices in <see cref="_preferences" />
        ///     resets all crucial lists, pointers and values
        /// </summary>
        /// <param name="imageList"></param>
        /// <returns>
        ///     <see cref="_imageDirectories" />
        /// </returns>
        public List<SDirectory> MatchImages(List<SImage> imageList)
        {
            _preferences = _dbPreferencesService.FetchPreferences();
            _deviationGenerosity = _preferences.SequenceIntervalGenerosity / 100;
            _runs = _preferences.SequenceImageCount;

            _dirList = new List<SImage>();
            _randomDirList = new List<SImage>();
            _imageDirectories.Clear();
            _sequence = 0;

            return !_preferences.UseManualSettings ? SortImagesAuto(imageList) : SortImages(imageList);
        }

        /// <summary>
        ///     SortImagesAuto()
        ///     sorts images according to their luminance and color
        ///     calls <see cref="WithinSameShot" /> to determine visual data and comparison values
        ///     calls <see cref="CreateDirAsync" /> to create a regular directory
        ///     calls <see cref="HandleLastElement" /> to add the lists last element to a directory
        ///     calls <see cref="CompleteDirectories" /> to actually write all sorted directories
        /// </summary>
        /// <param name="imageList"></param>
        /// <returns></returns>
        private List<SDirectory> SortImagesAuto(List<SImage> imageList)
        {
            for (var i = 1; i < imageList.Count; i++)
            {
                var lastItem = i < imageList.Count - 1 ? imageList[i + 1] : imageList[i];
                if (WithinSameShot(imageList[i], lastItem) &&
                    BelongToEachOther(imageList[i], lastItem))
                {
                    _dirList.Add(imageList[i - 1]);
                }
                else
                {
                    if (_dirList.Count >= _runs)
                    {
                        _ = CreateDirAsync();
                        // for some reason List.Clear() did not work properly
                        _dirList = new List<SImage>();
                    }

                    _randomDirList.Add(imageList[i - 1]);
                }
            }

            HandleLastElement(imageList);
            _ = CompleteDirectories();

            return _imageDirectories;
        }

        /// <summary>
        ///     SortImages()
        ///     sorts images according to their interval and deviation
        ///     calls <see cref="WithinSameSequence" /> to determine visual data and comparison values
        ///     calls <see cref="CreateDirAsync" /> to create a regular directory
        ///     calls <see cref="HandleLastElement" /> to add the lists last element to a directory
        ///     calls <see cref="CompleteDirectories" /> to actually write all sorted directories
        /// </summary>
        /// <param name="imageList"></param>
        /// <returns></returns>
        public List<SDirectory> SortImages(List<SImage> imageList)
        {
            for (var i = 1; i < imageList.Count; i++)
            {
                var preD = imageList[i].FileTime - imageList[i - 1].FileTime;
                var curD = i < imageList.Count - 1 ? imageList[i + 1].FileTime - imageList[i].FileTime : preD;

                if (WithinSameSequence(curD, preD))
                {
                    _dirList.Add(imageList[i - 1]);
                }
                else
                {
                    if (_dirList.Count >= _runs)
                    {
                        _ = CreateDirAsync();
                        // for some reason List.Clear() did not work properly
                        _dirList = new List<SImage>();
                    }

                    _randomDirList.Add(imageList[i - 1]);
                }
            }

            HandleLastElement(imageList);
            _ = CompleteDirectories();

            return _imageDirectories;
        }

        /// <summary>
        ///     WithinSameShot
        ///     determines whether two provided images are within the same shot
        ///     by calling <see cref="_imageProcessingService" /> to retrieve luminance and color data
        /// </summary>
        /// <param name="pImage"></param>
        /// <param name="cImage"></param>
        /// <returns>bool</returns>
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

        /// <summary>
        ///     WithinSameSequence()
        ///     determines whether two provided images are within the same sequence
        ///     based on their time interval deviation
        /// </summary>
        /// <param name="curD"></param>
        /// <param name="preD"></param>
        /// <returns></returns>
        public bool WithinSameSequence(double curD, double preD)
        {
            var syncValue = preD * _deviationGenerosity;

            return preD >= curD - syncValue && preD <= curD + syncValue ||
                   preD <= curD - syncValue && preD >= curD + syncValue;
        }

        /// <summary>
        ///     BelongToEachOther()
        ///     determines whether two provided images were taken after another
        ///     this is done by checking if <see cref="cImage" /> was taken after <see cref="pImage" />
        /// </summary>
        /// <param name="pImage"></param>
        /// <param name="cImage"></param>
        /// <returns></returns>
        private bool BelongToEachOther(SImage pImage, SImage cImage)
        {
            return cImage.FileTime >= pImage.FileTime + _preferences.SequenceInterval;
        }

        /// <summary>
        ///     HandleLastElement()
        ///     adds the last element of the provided list in <see cref="imageList" />
        ///     to a corresponding directory
        /// </summary>
        /// <param name="imageList"></param>
        private void HandleLastElement(List<SImage> imageList)
        {
            var lastIndex = imageList.Count - 1;

            if (WithinSameShot(imageList[lastIndex - 1], imageList[lastIndex]))
                _dirList.Add(imageList[lastIndex]);
            else
                _randomDirList.Add(imageList[lastIndex]);
        }

        /// <summary>
        ///     CompleteDirectories()
        ///     calls <see cref="CreateRandomDirAsync" /> to create a random directory
        ///     calls <see cref="CreateDirAsync" /> to create a regular directory
        /// </summary>
        /// <returns></returns>
        private async Task CompleteDirectories()
        {
            if (_dirList.Count < _runs && _dirList.Count > 0)
            {
                _randomDirList.AddRange(_dirList);
                await CreateRandomDirAsync();
                // for some reason List.Clear() did not work properly
                _randomDirList = new List<SImage>();
            }
            else if (_randomDirList.Count > 0)
            {
                await CreateRandomDirAsync();
                // for some reason List.Clear() did not work properly
                _randomDirList = new List<SImage>();
            }

            if (_dirList.Count >= _runs)
            {
                await CreateDirAsync();
                // for some reason List.Clear() did not work properly
                _dirList = new List<SImage>();
            }
        }

        /// <summary>
        ///     CreateRandomDirAsync()
        ///     creates a random directory and calls <see cref="SaveMatch" /> to save it
        /// </summary>
        /// <returns></returns>
        private async Task CreateRandomDirAsync()
        {
            var name = Path.GetFileName(Path.GetDirectoryName(_randomDirList[0].Origin)) ?? "Default";
            var uniqueIdentifier = Guid.NewGuid().ToString();
            var sanitizedName = name.Length > 15
                ? name.Substring(0, 9) + _sequence + "RND-"
                : name + _sequence + "RND-";

            sanitizedName += uniqueIdentifier.Substring(0, 4);

            var directory = new SDirectory
            (
                _randomDirList[0].Origin,
                sanitizedName
            )
            {
                Id = uniqueIdentifier,
                ImageList = _randomDirList
            };

            await SaveMatch(directory);
            _sequence++;
        }

        /// <summary>
        ///     CreateDirAsync()
        ///     creates a regular directory and calls <see cref="SaveMatch" /> to save it
        /// </summary>
        /// <returns></returns>
        private async Task CreateDirAsync()
        {
            var name = Path.GetFileName(Path.GetDirectoryName(_randomDirList[0].Origin)) ?? "Default";
            var uniqueIdentifier = Guid.NewGuid().ToString();
            var sanitizedName = name.Length > 15
                ? name.Substring(0, 10) + _sequence
                : name + _sequence;

            sanitizedName += uniqueIdentifier.Substring(0, 4);

            var directory = new SDirectory
            (
                _dirList[0].Origin,
                sanitizedName
            )
            {
                Id = uniqueIdentifier,
                ImageList = _dirList
            };

            await SaveMatch(directory);
            _sequence++;
        }


        /// <summary>
        ///     SaveMatch()
        ///     saves provided directory in <see cref="directory" /> to the database
        ///     calls <see cref="_dbService" /> to perform the corresponding action
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private async Task SaveMatch(SDirectory directory)
        {
            SImport import;
            var importExists = await _dbService.ImportExistsAsync();

            if (importExists)
            {
                import = await _dbService.GetImportAsync();

                directory.ImportId = import.Id;
                directory.ParentImport = import;

                import.Directories = new List<SDirectory>
                {
                    directory
                };

                await _dbService.UpdateCurrentImportAsync(import);
            }
            else // create a new import if none exists
            {
                import = new SImport
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = directory.Name,
                    ImportDate = DateTime.Today.ToShortDateString(),
                    Origin = directory.Origin,
                    Directories = new List<SDirectory>()
                };

                directory.ImportId = import.Id;
                directory.ParentImport = import;

                import.Directories = new List<SDirectory>
                {
                    directory
                };

                await _dbService.SaveImportAsync(import);
            }

            _imageDirectories.Insert(0, directory);
        }
    }
}