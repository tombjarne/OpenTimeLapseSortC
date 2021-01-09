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
			Debug.WriteLine("syncvalue :" + syncValue);
			Debug.WriteLine("syncvalue :" + generosity);
			Debug.WriteLine("syncvalue :" + preD);
			Debug.WriteLine("syncvalue :" + curD);

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

		public void SortImages(List<SImage> imageList, RenderDelegate render)
        {
			int pointer = 0; // marks end of sequence
			int seqPointer = 0; // marks begin of sequence
			double currDeviation = 0.0;
			double prevDeviation = 0.0;

			// TODO: simplify to only fetch preferences instance once
			// TODO: make algorithm detect new sequences based on date or filesize
			// TODO: consider that sequence length can be longer than minimum - do not create directory on minimum length match

			double deviationGenerosity = ((double)service.FetchPreferences().sequenceIntervalGenerosity)/100;
			int runs = service.FetchPreferences().sequenceImageCount; // pref count to make a sequence

            if (service.FetchPreferences().useAutoDetectInterval)
            {
				prevDeviation = service.FetchPreferences().sequenceInterval;
            }

			List<SImage> dirList = new List<SImage>();
			List<SImage> randomDirList = new List<SImage>();

			// TODO: match to fit seconds spec again! Fix milliseconds issue

			for (int i = 0; i < imageList.Count; i++)
			{
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
					if(dirList.Count < runs && dirList.Count > 0)
                    {
						randomDirList.AddRange(dirList);
						addToRandomDir(randomDirList);
					} else if (randomDirList.Count > 0 && randomDirList.Count < runs)
                    {
						addToRandomDir(randomDirList);
					}
					
					if (dirList.Count >= runs)
                    {
						createDir(dirList);
                    }
				}
			}
			Debug.WriteLine(imageDirectories.Count());
			Debug.WriteLine(imageDirectories[0].imageList.Count);
			render(imageDirectories);
		}

		private SImport GetImportInstance()
        {
			return dbService.ReturnCurrentImport();
        }

		private void addToRandomDir(List<SImage> dirList) // can sometimes only contain a single image
        {
			Debug.WriteLine("addToRandomDir");

			Preferences preferences = service.FetchPreferences();
			// TODO: check if there is already a random directory for the current import
			string target = dirList[0].parentInstance;

			DateTime today = new DateTime();
			int directoryId = (int)(today.Year * 1000000 + today.Month * 10000 + today.Day + today.Ticks);

			SDirectory directory = dbService.GetRandomDirInstance();

			if(directory.target == "default")
            {
				directory.target = target;
				directory.name = GetTrimmedName(target) + "Random";
				directory.id = directoryId;
				directory.imageList = dirList;
			} else
            {
				directory.imageList.AddRange(dirList);
			}

			dbService.UpdateImageDirectory(directory);

			/*
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
			*/

			// TODO: save changes to db

			imageDirectories.Add(directory);
		}


		/**
         * GuessName
         * 
         * Returns a guess for directory name based on the image names
         * useful when user imports images other than from camera itself
         * 
         */

		private string GuessName(List<SImage> images) {
			string dirName = "";

			string prevName = "";
			string curName = "";

			HashSet<string> wordList = new HashSet<string>();
			string[] sanitizedName;

			foreach(SImage image in images)
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

		private void createDir(List<SImage> dirList)
        {
			Debug.WriteLine("createDir");

			Preferences preferences = service.FetchPreferences();
			string target = dirList[0].parentInstance;

			DateTime today = new DateTime();
			int directoryId = (int)(today.Year * 1000000 + today.Month * 10000 + today.Day + today.Ticks);

			SDirectory directory = new SDirectory
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

			SImport import = new SImport()
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

			imageDirectories.Add(directory);

			// TODO: save changes to db
        }
    }
}
