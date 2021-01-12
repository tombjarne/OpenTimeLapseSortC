using System;
using System.Collections.Generic;
using OpenTimelapseSort.Contexts;
using System.Linq;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace OpenTimelapseSort.DataServices
{
    class DbService
    {
        public async System.Threading.Tasks.Task<List<SImport>> ReturnImportsAsync()
        {
            await using var context = new ImportContext();
            var imports = await context.Imports
                .ToListAsync();

            return imports;
        }


        /**
         * ReturnCurrentImport
         * 
         * Returns the latest Import instance including its directories
         * 
         */

        public async System.Threading.Tasks.Task<SImport> GetImportAsync()
        {
            await using var context = new ImportContext();
            
            var import = await context.Imports
                .SingleAsync(i => i.timestamp == DateTime.Today);

            Debug.WriteLine(import);
            return import;
        }

        public async System.Threading.Tasks.Task<bool> ImportExistsAsync()
        {
            try
            {
                await GetImportAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async System.Threading.Tasks.Task SaveImageAsync(SImage image)
        {
            await using var database = new ImportContext();
            
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

        public async System.Threading.Tasks.Task SaveImageDirectoryAsync(SDirectory directory)
        {
            await using var database = new ImportContext();
            
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

        public async System.Threading.Tasks.Task SaveImportAsync(SImport import)
        {
            await using var database = new ImportContext();
            
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

        public async System.Threading.Tasks.Task UpdateImportAsync(SImport import)
        {
            await using var database = new ImportContext();
            try
            {
                var entity = database.Imports
                    .Single(i => i.timestamp == DateTime.Today);

                entity.directories = import.directories;
                entity.length = import.length;
                
                await database.SaveChangesAsync();
            }
            catch (Exception)
            {
                // handle exception
            }
        }

        public void UpdateImport(SImport import)
        {
            using var context = new ImportContext();

            context.Update(import);
        }

        public async System.Threading.Tasks.Task<List<SImage>> GetImagesAsync(string id)
        {
            await using var context = new ImportContext();

            var directory = await context.ImageDirectories
                .SingleAsync(d => d.id.Equals(id));

            var images = await context.Images
                .Where(i => i.directoryId == directory.id)
                .ToListAsync();
            
            return images;
        }

        // TODO: should return all directories

        public async System.Threading.Tasks.Task<List<SDirectory>> GetDirectoriesAsync()
        {
            await using var context = new ImportContext();
            
            var directories = await context.ImageDirectories
                .ToListAsync();

            return directories;
        }
    }
}
