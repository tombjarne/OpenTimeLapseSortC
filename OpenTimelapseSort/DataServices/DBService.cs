using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenTimelapseSort.Contexts;
using OpenTimelapseSort.Migrations;
using OpenTimelapseSort.Models;

namespace OpenTimelapseSort.DataServices
{
    internal class DbService
    {
        /**
         * ReturnCurrentImport
         * 
         * Returns the latest Import instance including its directories
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

            foreach (var image in directory.ImageList)
                await DeleteImageAsync(image.Id);

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
                    CreateAndMigrate();
                }
            }
        }


        public async void CreateAndMigrate()
        {
            await using var database = new ImportContext();
            await database.Database.MigrateAsync();
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            using var database = new ImportContext();
            var demoImport = new SImport();
            var demoDirectory = new SDirectory("Default", "Demo Directory");
            var demoImage = new SImage("Demo Image", "Default", demoDirectory.Name);

            demoDirectory.ImageList.Add(demoImage);
            demoImport.Directories.Add(demoDirectory);

            database.Add(demoImport);
            database.SaveChanges();
        }

        public async Task UpdateCurrentImportAsync(SImport import)
        {
            await using var database = new ImportContext();
            try
            {
                var entity = database.Imports
                    .Single(i => i.Timestamp == DateTime.Today);

                entity.Directories = import.Directories;

                await database.SaveChangesAsync();
            }
            catch
            {
                await SaveImportAsync(import);
            }
        }

        public async Task UpdateImportAfterRemovalAsync(SImport import, SDirectory directory)
        {
            await using var database = new ImportContext();
            var entity = await database.Imports
                .SingleAsync(i => i.Id == directory.ImportId);

            var directoryEntity = await database.ImageDirectories
                .SingleAsync(d => d.Id == directory.Id);

            var images = await database.Images
                .Where(i => i.DirectoryId == directoryEntity.Id)
                .ToListAsync();

            foreach (var image in images)
            {
                directoryEntity.ImageList.Remove(image);
                await DeleteImageAsync(image.Id);
            }

            entity.Directories.Remove(directoryEntity);

            await DeleteDirectoryAsync(directoryEntity.Id);
            if (entity.Directories.Count == 0) {
                await DeleteImportAsync(entity.Id);
            }
            else
            {
                await database.SaveChangesAsync();
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

            if (entity.ImageList.Count != directory.ImageList.Count) entity.ImageList = directory.ImageList;

            entity.Name = directory.Name;
            entity.Target = directory.Target;

            await database.SaveChangesAsync();
        }

        public async Task<List<SDirectory>> GetDirectoriesAsync()
        {
            try
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
            catch
            {
                CreateAndMigrate();
                return await GetDirectoriesAsync();
            }
        }
    }
}