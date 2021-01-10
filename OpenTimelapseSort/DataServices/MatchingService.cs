using OpentimelapseSort.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenTimelapseSort.DataServices
{
    class MatchingService
    {
		public delegate void RenderDelegate(List<SDirectory> imageDirectories);

        private readonly DBPreferencesService _dbPreferencesService = new DBPreferencesService();
        private readonly DbService _dbService = new DbService();
        private readonly List<SDirectory> _imageDirectories = new List<SDirectory>(); // each directory will receive their images in the matching function

		public bool UseAutoDetection()
        {
			return _dbPreferencesService.FetchPreferences().useAutoDetectInterval;
        }

		public bool UseCopy()
        {
			return _dbPreferencesService.FetchPreferences().useCopy;
        }

        public bool WithinSameSequence(double curD, double preD, double generosity)
        {
            double syncValue = preD * generosity;

            return (preD >= curD - syncValue && preD <= curD + syncValue ||
                    preD <= curD - syncValue && preD >= curD + syncValue);
        }

		public async void SortImages(List<SImage> imageList, RenderDelegate render)
        {
            List<SImage> dirList = new List<SImage>();
            List<SImage> randomDirList = new List<SImage>();

			double currDeviation = 0.0;
			double prevDeviation = 0.0;
            double deviationGenerosity = ((double)_dbPreferencesService.FetchPreferences().sequenceIntervalGenerosity)/100;
			int runs = _dbPreferencesService.FetchPreferences().sequenceImageCount;

            if (_dbPreferencesService.FetchPreferences().useAutoDetectInterval)
            {
				prevDeviation = _dbPreferencesService.FetchPreferences().sequenceInterval;
            }

            // TODO: match to fit seconds spec again! Fix milliseconds issue

			for (int i = 0; i < imageList.Count; i++)
            {
                imageList[i].id = System.Guid.NewGuid().ToString();

                if (i > 0)
				{
					prevDeviation = Math.Abs((imageList[i].fileTime - imageList[i - 1].fileTime).Milliseconds);

					if (i < imageList.Count - 1)
					{
						currDeviation = Math.Abs((imageList[i + 1].fileTime - imageList[i].fileTime).Milliseconds);
					}
					else
					{
						currDeviation = prevDeviation;
					}
				}

				if (WithinSameSequence(currDeviation, prevDeviation, deviationGenerosity))
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

				if (i + 1 == imageList.Count)
				{

                    // TODO: simplify!

                    if (WithinSameSequence(currDeviation, prevDeviation, deviationGenerosity))
                    {
                        dirList.Add(imageList[i]);
                    }
                    else
                    {
                        randomDirList.Add(imageList[i]);
                    }

                    imageList[i].id = System.Guid.NewGuid().ToString();

                    if (dirList.Count < runs && dirList.Count > 0)
                    {
						randomDirList.AddRange(dirList);
                        await CreateRandomDirAsync(randomDirList);
					} else if (randomDirList.Count > 0 && randomDirList.Count < runs)
                    {
                        await CreateRandomDirAsync(randomDirList);
					}
					
					if (dirList.Count >= runs)
                    {
						await CreateDirAsync(dirList);
                    }
				}
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
