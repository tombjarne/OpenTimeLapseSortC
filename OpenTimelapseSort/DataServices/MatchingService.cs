using OpentimelapseSort.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;

namespace OpenTimelapseSort.DataServices
{
    class MatchingService
    {
		public delegate void RenderDelegate(List<SDirectory> imageDirectories);

        private readonly DBPreferencesService _dbPreferencesService = new DBPreferencesService();
        private readonly DbService _dbService = new DbService();
        private readonly List<SDirectory> _imageDirectories = new List<SDirectory>();

		public bool UseAutoDetection()
        {
			return _dbPreferencesService.FetchPreferences().useAutoDetectInterval;
        }

		public bool UseCopy()
        {
			return _dbPreferencesService.FetchPreferences().useCopy;
        }

        private bool WithinSameLocation(double mLocPre, double mLocCur)
        {
            return true;
        }

        private static List<Color> GetPixels(Bitmap imgBitmap)
        {
            var pixels = new List<Color>();

            for (var j = 0; j < imgBitmap.Height; j++)
            {
                if (j % 150 == 0)
                {
                    for (var i = 0; i < imgBitmap.Width; i++)
                    {
                        if (i % 150 == 0)
                        {
                            pixels.Add(imgBitmap.GetPixel(i, j));
                        }
                    }
                }
            }

            return pixels;
        }

        private bool WithinSameShot(SImage pImage, SImage cImage)
        {
            const int colorSync = 0xffff;
            const int lumenSync = 100;

            long cMatrixPre = 0x00;
            long cMatrixCur = 0x00;
            double cLumenPre = 0;
            double cLumenCur = 0;

            var pImageBitmap = (Bitmap) Image.FromFile(pImage.target);
            var cImageBitmap = (Bitmap) Image.FromFile(cImage.target);

            // TODO: fix location matching
            //var mBitPre = new BitmapMetadata(pImage.target);
            //var mBitCur = new BitmapMetadata(cImage.target);

            //var mLocPre = double.Parse(mBitPre.Location);
            //var mLocCur = double.Parse(mBitCur.Location);

            //if (!WithinSameLocation(mLocPre, mLocCur))
            //return false;

            var pixelsPre = GetPixels(pImageBitmap);
            var pixelsCur = GetPixels(cImageBitmap);

            foreach (var pixel in pixelsPre)
            {
                cMatrixPre += pixel.G;
                cLumenPre += 0.2126 * pixel.R + 0.7152 * pixel.G + 0.0722 * pixel.B;
            }

            foreach (var pixel in pixelsCur)
            {
                cMatrixCur += pixel.G;
                cLumenCur += 0.2126 * pixel.R + 0.7152 * pixel.G + 0.0722 * pixel.B;
            }

            GC.AddMemoryPressure(cMatrixPre);
            GC.AddMemoryPressure(cMatrixCur);

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
            var deviationGenerosity = preferences.sequenceIntervalGenerosity/100;
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
                id = System.Guid.NewGuid().ToString(),
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
                    id = System.Guid.NewGuid().ToString(),
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
                    id = System.Guid.NewGuid().ToString(),
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
