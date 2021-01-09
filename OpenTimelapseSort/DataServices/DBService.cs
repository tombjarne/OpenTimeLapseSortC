using System;
using System.Collections.Generic;
using OpenTimelapseSort.Contexts;
using OpentimelapseSort.Models;
using System.Linq;
using System.Diagnostics;
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
                    SImport newImport = new SImport();
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

        public async System.Threading.Tasks.Task<SImport> GetImportAsync()
        {
            using (var context = new ImportContext())
            {
                var import = await context.Imports
                    .SingleAsync(i => i.timestamp == DateTime.Today);

                Debug.WriteLine(import);
                return import;
            }
        }

        public async System.Threading.Tasks.Task<bool> ImportExistsAsync()
        {
            using (var context = new ImportContext())
            {
                try
                {
                    await GetImportAsync();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public void SeedDatabase()
        {
            // init db for test purposes

            using (var context = new ImportContext())
            {

                SImport import = new SImport()
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
                    Debug.WriteLine(e.StackTrace);
                }
            }
        }

        public async System.Threading.Tasks.Task<SDirectory> GetRandomDirInstance()
        {
            using (var context = new ImportContext())
            {
                var directory = await context.ImageDirectories
                    .SingleAsync(d => d.name.Contains("Random") && d.timestamp == DateTime.Today);
                
                return directory;
            }
        }

        public async System.Threading.Tasks.Task<bool> RandomDirInstanceExistsAsync()
        {
            using (var context = new ImportContext())
            {
                try
                {
                    await GetRandomDirInstance();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public void UpdateRandomImageDirectory(SDirectory directory)
        {
            using (var database = new ImportContext())
            {
                try
                {
                    var entity = database.ImageDirectories
                        .Single(d => d.name.Contains("Random") && d.timestamp == DateTime.Today);

                    //database.Entry(entity).CurrentValues.SetValues(directory);
                    //database.SaveChanges();
                    database.Update(entity);
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

        public async System.Threading.Tasks.Task UpdateImportAsync(SImport import)
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

        public void UpdateImport(SImport import)
        {
            using (var context = new ImportContext())
            {
                context.Update(import);
            }
        }

         public async System.Threading.Tasks.Task<List<SImage>> GetImagesAsync(string id)
        {
            using (var context = new ImportContext())
            {
                try
                {
                    var directory = await context.ImageDirectories
                        .SingleAsync(d => d.id.Equals(id));

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
