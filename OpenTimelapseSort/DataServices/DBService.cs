using Microsoft.EntityFrameworkCore;
using OpenTimelapseSort.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTimelapseSort.Models;

namespace OpenTimelapseSort.DataServices
{
    internal class DbService
    {

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
                .SingleAsync(i => i.Timestamp == DateTime.Today);

            return import;
        }

        public async Task DeleteImportAsync(string importId)
        {
            await using var database = new ImportContext();

            var import = await database.Imports
                .SingleAsync(i => i.Id == importId);
            database.Imports.Remove(import);
            await database.SaveChangesAsync();
        }

        public async Task DeleteDirectoryAsync(string directoryId)
        {
            await using var database = new ImportContext();

            var directory = await database.ImageDirectories
                .SingleAsync(d => d.Id == directoryId);
            database.ImageDirectories.Remove(directory);
            await database.SaveChangesAsync();
        }

        public async Task DeleteImageAsync(string imageId)
        {
            await using var database = new ImportContext();

            var image = await database.Images
                .SingleAsync(i => i.Id == imageId);
            database.Images.Remove(image);
            await database.SaveChangesAsync();
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

        public async Task SaveImportAsync(SImport import)
        {
            await using var database = new ImportContext();
            try
            {
                await database.AddAsync(import);
                await database.SaveChangesAsync();
            }
            catch
            {
                if (await database.Database.EnsureCreatedAsync())
                {
                    await SaveImportAsync(import);
                }
                else
                {
                    await CreateAndMigrate();
                    await SeedImportDatabase();
                }
            }
        }


        public async Task CreateAndMigrate()
        {
            // TODO: run migration code
        }

        public async Task SeedImportDatabase()
        {
            await using var database = new PreferencesContext();

            // TODO: create demo Import and demo directory

            await database.SaveChangesAsync();
        }

        public async Task UpdateImportAsync(SImport import)
        {
            await using var database = new ImportContext();
            try
            {
                var entity = database.Imports
                    .Single(i => i.Timestamp == DateTime.Today);

                entity.Directories = import.Directories;
                entity.Length = import.Length;

                await database.SaveChangesAsync();
            }
            catch
            {
                await SaveImportAsync(import);
            }
        }

        public async Task UpdateDirectoryAsync(SDirectory directory)
        {
            await using var database = new ImportContext();
            var entity = await database.ImageDirectories
                .SingleAsync(d => d.Id == directory.Id);

            var images = await database.Images
                .Where(i => i.ParentDirectory.Id == entity.Id).ToListAsync();

            entity.ImageList = images;

            if(entity.ImageList.Count != directory.ImageList.Count)
            {
                entity.ImageList = directory.ImageList;
            }

            entity.Name = directory.Name;

            await database.SaveChangesAsync();
        }

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

                directory.ParentImport = await context.Imports
                    .SingleAsync(i => i.Id == directory.ImportId);
            }

            return directories;
        }
    }
}
