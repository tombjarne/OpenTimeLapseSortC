using System;
using System.Collections.Generic;
using OpenTimelapseSort.Contexts;
using OpentimelapseSort.Models;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;

namespace OpenTimelapseSort.DataServices
{

    class DBService
    {

        public DBService()
        {
            // init a delegate that contains values ( HashSet ) of Directory table
            // init a delegate that contains values ( HashSet ) of Import table
            // return delegate to MainViewModel ( so that it can render HashSet of Imports ( which includes HashSet of Directories ) ) 
        }

        public void InitializeDBService()
        {
            /*
            using (var preferencesContext = new PreferencesContext())
            {
                if (!preferencesContext.Any())
                {

                }
            }
            */
        }

        public static Preferences ReturnPreferences()
        {
            // fetch from DB
            // save into Object
            // return Object

            return new Preferences(true, true, 2.0, 50, 1); // TODO: remove, test purpose only
        }

        // returns a list of Import Objects fetched from db

        public List<SImport> ReturnImports()
        {
            // fetch from DB
            // save into Object
            // return Object

            using (var context = new ImportContext())
            {
                List<SImport> imports = new List<SImport>();
                List<SDirectory> directories = new List<SDirectory>(); // images are fetched on click to reduce overhead

                foreach (SDirectory directory in context.ImageDirectories)
                {
                    SDirectory newDirectory = new SDirectory(
                        directory.name,
                        directory.target
                    );
                    directories.Add(newDirectory);
                }

                foreach (SImport import in context.Imports)
                {
                    SImport newImport = new SImport(true);
                    newImport.initImportList(directories);
                    newImport.importDate = import.importDate;
                    newImport.timestamp = import.timestamp; //convert string to date 
                    imports.Add(newImport);
                }

                Console.WriteLine(imports);
                return imports;
            }
        }


        /**
         * ReturnCurrentImport
         * 
         * Returns the latest Import instance including its directories
         * 
         */

        public SImport ReturnCurrentImport()
        {
            using (var context = new ImportContext())
            {
                var import = context.Imports
                    .Select(import => new
                    {
                        fetch = true,
                        directories = import.directories,
                        timestamp = import.timestamp,
                        importDate = import.importDate,
                        id = import.id
                    }).Where(import => import.timestamp == System.DateTime.Today); // might need to implement fuzzy logic if it is midnight
                // TODO: fix casting issue!
                return (SImport)import;
            }
        }

        public void SeedDatabase()
        {
            // init db for test purposes

            using (var context = new ImportContext())
            {

                SImport import = new SImport(false)
                {
                    name = "Urlaub",
                    length = 1,
                    directories = new List<SDirectory>(0)
                };
                context.Add(import);
                context.SaveChanges();

                SDirectory directory = new SDirectory("/", "Urlaub 1")
                {
                    imageList = new List<SImage>(0),
                };
                context.Add(directory);
                context.SaveChanges();
            }
        }

        public async System.Threading.Tasks.Task SaveImageAsync(SImage image)
        {
            using (var database = new ImportContext())
            { 
                try
                {
                    await database.AddAsync(image);
                    await database.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.StackTrace);
                    Debug.WriteLine(e.Message);
                }
            }
        }

        public async System.Threading.Tasks.Task SaveImageDirectoryAsync(SDirectory directory)
        {
            using (var database = new ImportContext())
            {
                try
                {
                    await database.AddAsync(directory);
                    await database.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.StackTrace);
                }
            }
        }

        public async System.Threading.Tasks.Task SaveImportAsync(SImport import)
        {
            using (var database = new ImportContext())
            {
                try
                {
                    await database.AddAsync(import);
                    await database.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.StackTrace);
                }
            }
        }

        public SDirectory GetRandomDirInstance()
        {
            using (var context = new ImportContext())
            {
                try
                {
                    var directory = context.ImageDirectories
                        .Single(d => d.name.Contains("Random") && d.timestamp == DateTime.Today);
                    return directory;

                } catch (Exception e)
                {
                    Debug.WriteLine(e.StackTrace);

                    var directory = new SDirectory("default", "Random Directory");
                    return directory;
                }
            }
        }

        public void UpdateImageDirectory(SDirectory directory)
        {
            using (var database = new ImportContext())
            {
                try
                {
                    var entity = database.ImageDirectories
                        .Single(d => d.name.Contains("Random") && d.timestamp == DateTime.Today);

                    database.Entry(entity).CurrentValues.SetValues(directory);
                    database.SaveChanges();

                } catch (Exception e)
                {
                    // TODO: fix insertion statement
                    Debug.WriteLine(e.StackTrace);
                    //database.ImageDirectories.Add(directory);
                    //database.SaveChanges();
                }
            }
        }

        public void UpdateImport(SImport import)
        {
            using (var context = new ImportContext())
            {
                context.Update(import);
            }
        }

         public async System.Threading.Tasks.Task<List<SImage>> GetImagesAsync(int id)
        {
            using (var context = new ImportContext())
            {
                try
                {
                    var directory = await context.ImageDirectories
                        .SingleAsync(d => d.id == id);

                    Debug.WriteLine(directory);

                    SImage image = new SImage("test", "test", "test/test/wohoo");
                    directory.imageList.Add(image);

                    return directory.imageList;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.StackTrace);
                    Debug.WriteLine(e.InnerException);

                    List<SImage> errorList = new List<SImage>();
                    return errorList;
                }
            }
        }

        /*
        private HashSet<ImageDirectory> GetDirectories(HashSet<ImageDirectory> directories) // gets all directories related to a passed import
        {
            // loop over all entries and create new objects
            // save objects to HashSet
            // repeat for each directory
            
        }

        private HashSet<Import> GetImports() // gets all imports from database
        {
            // loop over all entries and create new objects
            // save objects to HashSet
            // repeat for each directory

            Imports imports = new Imports(GetDirectories);
            return imports;
        }
        */

        //use Entity Framework here to fetch and store

    }
}
