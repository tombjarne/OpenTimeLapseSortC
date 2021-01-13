using Microsoft.EntityFrameworkCore;
using OpenTimelapseSort.Contexts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OpenTimelapseSort.DataServices
{
    internal class DbService
    {
        public async Task<List<SImport>> ReturnImportsAsync()
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

        public async Task<SImport> GetImportAsync()
        {
            await using var context = new ImportContext();

            var import = await context.Imports
                .SingleAsync(i => i.timestamp == DateTime.Today);

            Debug.WriteLine(import);
            return import;
        }

        public async Task<bool> ImportExistsAsync()
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

        public async Task SaveImageAsync(SImage image)
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

        public async Task SaveImageDirectoryAsync(SDirectory directory)
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

        public async Task SaveImportAsync(SImport import)
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

        public async Task UpdateImportAsync(SImport import)
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

        public async Task<List<SImage>> GetImagesAsync(string id)
        {
            await using var context = new ImportContext();

            var images = await context.Images
                .Where(i => i.DirectoryId == id)
                .ToListAsync();

            return images;
        }

        // TODO: should return all directories

        public async Task<List<SDirectory>> GetDirectoriesAsync()
        {
            await using var context = new ImportContext();

            var directories = await context.ImageDirectories
                .ToListAsync();

            foreach (var directory in directories)
            {
                directory.ImageList = await context.Images
                    .Where(i => i.DirectoryId == directory.Id)
                    .ToListAsync();
            }

            Debug.WriteLine(directories);

            return directories;
        }
    }
}
