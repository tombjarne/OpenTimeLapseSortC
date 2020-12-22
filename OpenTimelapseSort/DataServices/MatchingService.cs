using OpentimelapseSort.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTimelapseSort.DataServices
{
    class MatchingService
    {
		DBPreferencesService service = new DBPreferencesService();

		public bool UseAutoDetection()
        {
			return service.FetchPreferences().useAutoDetectInterval;
        }

		public bool UseCopy()
        {
			return service.FetchPreferences().useCopy;
        }

        public bool WithinSameSequence(Image previous, Image current)
        {
            // get Preferences from DB
            Preferences preferences = DBService.ReturnPreferences();
            return true;
        }

		public void SortImages(List<Image> imageList)
        {
			// TODO: implement logic

			int pointer = 0; // marks end of sequence
			int seqPointer = 0; // marks begin of sequence
			double currDeviation = 0.0;
			double prevDeviation = 0.0;

			int runs = service.FetchPreferences().sequenceImageCount; // pref count to make a sequence
			bool seqEnded = false;

            if (service.FetchPreferences().useAutoDetectInterval)
            {
				prevDeviation = service.FetchPreferences().sequenceInterval;
            }

			List<Image> dirList = new List<Image>();
			List<Image> randomDirList = new List<Image>();

			for (int i = 0; i < imageList.Count; i++)
			{

                // TODO: find way to calculate deviation - also support users input
                if (i > 0)
                {
					//prevDeviation = imageList[i].fileTime - imageList[i - 1].fileTime;

					if (i < imageList.Count)
                    {
						//currDeviation = imageList[i + 1].fileTime - imageList[i].fileTime;
					}
					else
                    {
						currDeviation = prevDeviation;
                    }
				}

				if (currDeviation == prevDeviation)
				{
				
					dirList.Add(imageList[i]);
					pointer += 1; // marks last image in current sequence that fits previous deviations

				}
				else
				{

					if (pointer - seqPointer >= runs) // images do not have same deviation and do fill the length requirement
					{
						seqEnded = true;
						createDir(dirList); // current list will be added due to matching requirements
					}
					else // images do not have same deviation and do not fill the minimum length requirement
					{
						addToRandomDir(dirList);
					}

					pointer = i;
					seqPointer = i;
				}
			}
		}

		private void addToRandomDir(List<Image> dirList)
        {

        }

		private void createDir(List<Image> dirList)
        {

        }
    }
}
