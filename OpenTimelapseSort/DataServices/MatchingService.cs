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

		DBPreferencesService service = new DBPreferencesService();
		DBService dbService = new DBService();

		List<SDirectory> imageDirectories = new List<SDirectory>(); // each directory will receive their images in the matching function
		List<SImport> imports = new List<SImport>(); // does it need to be a list?

		public bool UseAutoDetection()
        {
			return service.FetchPreferences().useAutoDetectInterval;
        }

		public bool UseCopy()
        {
			return service.FetchPreferences().useCopy;
        }

        public bool WithinSameSequence(double curD, double preD, double generosity)
        {

			double syncValue = preD * generosity;

			// TODO: fix statement

            if (preD >= curD - syncValue && preD <= curD + syncValue ||
				preD <= curD - syncValue && preD >= curD + syncValue)
            {
				return true;
            } else
            {
				return false;
            }
        }

		public async void SortImages(List<SImage> imageList, RenderDelegate render)
        {
            List<SImage> dirList = new List<SImage>();
            List<SImage> randomDirList = new List<SImage>();

            int pointer = 0; // marks end of sequence
			int seqPointer = 0; // marks begin of sequence
			double currDeviation = 0.0;
			double prevDeviation = 0.0;
            double deviationGenerosity = ((double)service.FetchPreferences().sequenceIntervalGenerosity)/100;
			int runs = service.FetchPreferences().sequenceImageCount; // pref count to make a sequence

            if (service.FetchPreferences().useAutoDetectInterval)
            {
				prevDeviation = service.FetchPreferences().sequenceInterval;
            }

            // TODO: match to fit seconds spec again! Fix milliseconds issue

			for (int i = 0; i < imageList.Count; i++)
            {
                imageList[i].id = System.Guid.NewGuid().ToString();

                if (i > 0)
				{
					prevDeviation = Math.Abs((imageList[i].fileTime - imageList[i - 1].fileTime).Milliseconds);
					Debug.WriteLine(prevDeviation);

					if (i < imageList.Count - 1)
					{
						currDeviation = Math.Abs((imageList[i + 1].fileTime - imageList[i].fileTime).Milliseconds);
						Debug.WriteLine(currDeviation);
					}
					else
					{
						currDeviation = prevDeviation;
					}
				}

				if (WithinSameSequence(currDeviation, prevDeviation, deviationGenerosity))
				{
					dirList.Add(imageList[i]);
					pointer += 1; // marks last image in current sequence that fits previous deviations
				}
				else
				{

					//if (pointer - seqPointer >= runs) // images do not have same deviation and do fill the length requirement
					if (dirList.Count >= runs) // images do not have same deviation and do fill the length requirement
					{
						createDir(dirList); // current list will be added due to matching requirements
					}
					else // images do not have same deviation and do not fill the minimum length requirement
					{
						randomDirList.Add(imageList[i]);
					}

					pointer = i;
					seqPointer = i;
					dirList = new List<SImage>(); // reinit dirList
				}
				if (i + 1 == imageList.Count)
				{
                    imageList[i].id = System.Guid.NewGuid().ToString();

                    if (dirList.Count < runs && dirList.Count > 0)
                    {
						randomDirList.AddRange(dirList);
                        await addToRandomDirAsync(randomDirList);
					} else if (randomDirList.Count > 0 && randomDirList.Count < runs)
                    {
                        await addToRandomDirAsync(randomDirList);
					}
					
					if (dirList.Count >= runs)
                    {
						createDir(dirList);
                    }
				}
			}
            render(imageDirectories);
		}

        private async System.Threading.Tasks.Task addToRandomDirAsync(List<SImage> dirList)
		{
            SDirectory directory = await dbService.ImportExistsAsync() == true ?
                await dbService.GetRandomDirInstance() : new SDirectory
                (
                    dirList[0].target,
                    dirList[0].name + "Random"
                )
                {
                    id = System.Guid.NewGuid().ToString(),
                    imageList = dirList
                };

            imageDirectories.Add(directory);
        }

		private void createDir(List<SImage> dirList)
        {
            SDirectory directory = new SDirectory
                (
                    dirList[0].target,
					dirList[0].name
                )
                {
                    id = System.Guid.NewGuid().ToString(),
                    imageList = dirList
                };

            imageDirectories.Add(directory);
		}
    }
}
