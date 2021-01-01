using OpentimelapseSort.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenTimelapseSort.DataServices
{
    class MatchingService
    {
		public delegate void RenderDelegate(object obj);

		DBPreferencesService service = new DBPreferencesService();
		DBService dbService = new DBService();

		List<ImageDirectory> directories = new List<ImageDirectory>(); // each directory will receive their images in the matching function
		List<Import> imports = new List<Import>(); // does it need to be a list?

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

            if (preD >= curD - syncValue && preD <= curD + syncValue ||
				preD >= curD + syncValue && preD <= curD - syncValue)
            {
				return true;
            } else
            {
				return false;
            }

		}

		public void SortImages(List<Image> imageList, RenderDelegate render)
        {
			int pointer = 0; // marks end of sequence
			int seqPointer = 0; // marks begin of sequence
			double currDeviation = 0.0;
			double prevDeviation = 0.0;

			// TODO: simplify to only fetch preferences instance once
			// TODO: make algorithm detect new sequences based on date or filesize

			double deviationGenerosity = (service.FetchPreferences().sequenceIntervalGenerosity)/100;
			int runs = service.FetchPreferences().sequenceImageCount; // pref count to make a sequence

            if (service.FetchPreferences().useAutoDetectInterval)
            {
				prevDeviation = service.FetchPreferences().sequenceInterval;
            }

			List<Image> dirList = new List<Image>();
			List<Image> randomDirList = new List<Image>();

			for (int i = 0; i < imageList.Count; i++)
			{
				if (i > 0)
				{
					prevDeviation = (imageList[i].fileTime - imageList[i - 1].fileTime).TotalSeconds;

					if (i < imageList.Count - 1)
					{
						currDeviation = (imageList[i + 1].fileTime - imageList[i].fileTime).TotalSeconds;
					}
					else
					{
						currDeviation = prevDeviation;
					}
				}

				if (WithinSameSequence(currDeviation, prevDeviation, deviationGenerosity))
				{
					Debug.WriteLine(WithinSameSequence(currDeviation, prevDeviation, deviationGenerosity));
					dirList.Add(imageList[i]);

					Debug.WriteLine(pointer - seqPointer);
					Debug.WriteLine(dirList.Count);
					if(pointer - seqPointer >= runs && dirList.Count >= runs)
                    {
						createDir(dirList, render);
					}
					
					pointer += 1; // marks last image in current sequence that fits previous deviations

				}
				else
				{

					if (pointer - seqPointer >= runs) // images do not have same deviation and do fill the length requirement
					{
						createDir(dirList, render); // current list will be added due to matching requirements
					}
					else // images do not have same deviation and do not fill the minimum length requirement
					{
						randomDirList.Add(imageList[i]);
					}

					pointer = i;
					seqPointer = i;
					dirList = new List<Image>(); // reinit dirList
				}
				if (i + 1 == imageList.Count && randomDirList.Count > 0 || dirList.Count > 0)
				{
					if(dirList.Count < runs && dirList.Count > 0)
                    {
						randomDirList.AddRange(dirList);
					}

					addToRandomDir(randomDirList, render);
				}
			}
		}

		private Import GetImportInstance()
        {
			return dbService.ReturnCurrentImport();
        }

		private void addToRandomDir(List<Image> dirList, RenderDelegate render) // can sometimes only contain a single image
        {
			Preferences preferences = service.FetchPreferences();
			// TODO: check if there is already a random directory for the current import
			string target = dirList[0].parentInstance;

			DateTime today = new DateTime();
			int directoryId = (int)(today.Year * 1000000 + today.Month * 10000 + today.Day + today.Ticks);

			ImageDirectory directory;

			try
            {
				directory = dbService.GetRandomDirInstance();
				directory.imageList.AddRange(dirList);

				dbService.UpdateImageDirectory(directory);
			}
            catch
            {
				directory = new ImageDirectory
				(
					target,
					GetTrimmedName(target)
				)
				{
					imageList = dirList,
					id = directoryId
				};
			}

            try
            {
				Import import = GetImportInstance();
				import.directories.Add(directory);
				import.length++;
			}
			catch
            {
				Import import = new Import(false);
				import.tryPush(directory);
				import.length++;
			}

			// TODO: save changes to db

			render(directory);
		}


		/**
         * GuessName
         * 
         * Returns a guess for directory name based on the image names
         * useful when user imports images other than from camera itself
         * 
         */

		private string GuessName(List<Image> images) {
			string dirName = "";

			string prevName = "";
			string curName = "";

			HashSet<string> wordList = new HashSet<string>();
			string[] sanitizedName;

			foreach(Image image in images)
            {
				sanitizedName = image.name.Substring(0, image.name.LastIndexOf('.')).Split(new char[] {'-', '_'});

                if (sanitizedName.Any(word => image.name.Contains(word)))
                {
					wordList.Add(image.name);
                }
            }
			
			for(int i = 0; i < wordList.Count; i++)
            {
				//dirName = wordList.
            }

			return dirName;
		}

		private string GetTrimmedName(string target)
        {
			return target.Substring(target.LastIndexOf(@"\"), (target.Length) - target.LastIndexOf(@"\")).Replace(@"\", "");
		}

		private void createDir(List<Image> dirList, RenderDelegate render)
        {
			Preferences preferences = service.FetchPreferences();
			string target = dirList[0].parentInstance;

			DateTime today = new DateTime();
			int directoryId = (int)(today.Year * 1000000 + today.Month * 10000 + today.Day + today.Ticks);

			ImageDirectory directory = new ImageDirectory
				(
					target,
					GetTrimmedName(target)
				)
			{
				imageList = dirList,
				id = directoryId
			};

			//preferences.useAutoNaming == false ? GuessName(dirList) :

			Debug.WriteLine(directory.name);

			Import import = new Import(false)
			{
				length = 1
			};
			import.tryPush(directory);

			/*
			try
			{
				Import import = GetImportInstance();
				import.directories.Add(directory);
				import.length++;
			}
			catch
			{
				Import import = new Import(false)
				{
					length = 1
				};
				import.tryPush(directory);
			}
			*/

			render(directory);

			// TODO: save changes to db
        }
    }
}
