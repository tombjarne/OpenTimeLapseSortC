using OpentimelapseSort.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTimelapseSort.DataServices
{
    class MatchingService
    {
        public bool WithinSameSequence(Image previous, Image current)
        {
            // get Preferences from DB
            Preferences preferences = DBService.ReturnPreferences();
            return true;
        }
		/*
		int pointer = 0; // marks end of sequence
		int seqPointer = 0; // marks begin of sequence
		int runs = preferences.count; // pref count to make a sequence
		bool seqEnded = false;
		List<Image> dirList = new List<Image>();
		List<Image> randomDirList = new List<Image>();

		for(int i = 0; i<imageList.Size(); i++ ) {
			if(currDevation == prevDeviation ) {
				if (pointer - seqPointer >= runs && seqEnded) { 
					createDir(dirList); // puts files into sequence
					dirList = newList<Image>();	
				}
				pointer += 1; // equals i 
				dirList.Add(imageList[i]);

			} else {

				if (pointer - seqPointer >= runs) {
					seqEnded = true;
				} else {
					addToRandomDir(dirList);
				}

				pointer = i;
				seqPointer = i;
			}
		}
		*/
    }
}
