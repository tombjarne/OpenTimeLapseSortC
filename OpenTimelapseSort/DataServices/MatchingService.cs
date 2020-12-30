using OpentimelapseSort.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;

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

        public bool WithinSameSequence(Image previous, Image current)
        {
            // get Preferences from DB
            Preferences preferences = DBService.ReturnPreferences();
            return true;
        }

		//public void SortImages(List<Image> images, RenderNewDirectoryCallback rndc, RenderExistingDirectoryCallback redc)

		public void SortImages(List<Image> imageList, RenderDelegate render)
        {
			// TODO: implement logic

			int pointer = 0; // marks end of sequence
			int seqPointer = 0; // marks begin of sequence
			double currDeviation = 0.0;
			double prevDeviation = 0.0;

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

				if (currDeviation == prevDeviation)
				{

					dirList.Add(imageList[i]);
					if(i + 1 == imageList.Count)
                    {
						if(pointer - seqPointer >= runs)
                        {
							createDir(dirList, render);
						}
						else
                        {
							addToRandomDir(dirList, render);
                        }
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
				if (i + 1 == imageList.Count && randomDirList.Count > 0)
				{
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
			ImageDirectory directory;
			// TODO: check if there is already a random directory for the current import

			try
            {
				directory = dbService.GetRandomDirInstance();
				directory.imageList.AddRange(dirList);

				dbService.UpdateImageDirectory(directory);
			}
            catch
            {

            }

			directory = new ImageDirectory
				(
					dirList[0].parentInstance,
					"Random Directory"
				)
			{
				imageList = dirList,
			};

            try
            {
				Import import = GetImportInstance();
				import.directories.Add(directory);
				import.length++;
			}
			catch
            {
				Import import = new Import(false);
				import.directories.Add(directory);
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

		private void createDir(List<Image> dirList, RenderDelegate render)
        {
			Preferences preferences = service.FetchPreferences();
			Debug.WriteLine("Name: "+dirList[0].parentInstance);

			// TODO: fix statement
			//Debug.WriteLine(dirList[0].parentInstance.Substring(dirList[0].parentInstance.LastIndexOf(@"\"), dirList[0].parentInstance.Length - 1));

			ImageDirectory directory = new ImageDirectory
				(
					dirList[0].parentInstance,
					"lel"
				)
			{
				imageList = dirList,
			};

			//preferences.useAutoNaming == false ? GuessName(dirList) :
			//dirList[0].parentInstance.Substring(dirList[0].parentInstance.LastIndexOf(@"\"), dirList[0].parentInstance.Length - 1))

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
